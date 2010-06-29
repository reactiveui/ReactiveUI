using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Concurrency;
using System.Windows.Threading;

namespace ReactiveXaml
{
    public class ReactiveCommand : IReactiveCommand, IEnableLogger
    {
        public ReactiveCommand(Action<object> executed = null, IScheduler scheduler = null)
            : this((IObservable<bool>)null, executed, scheduler) { }

        public ReactiveCommand(IObservable<bool> can_execute = null, Action<object> executed = null, IScheduler scheduler = null)
        {
            can_execute = can_execute ?? Observable.Return(true).Concat(Observable.Never<bool>());
            commonCtor(executed, scheduler);
            can_execute.Subscribe(canExecuteSubject.OnNext, canExecuteSubject.OnError, canExecuteSubject.OnCompleted);
        }

        public ReactiveCommand(Func<object, bool> can_execute, Action<object> executed = null, IScheduler scheduler = null)
        {
            canExecuteExplicitFunc = can_execute;
            commonCtor(executed, scheduler);
        }

        private void commonCtor(Action<object> executed, IScheduler scheduler)
        {
            scheduler = scheduler ?? ReactiveXaml.DefaultScheduler;

            canExecuteSubject = new Subject<bool>();
            canExecuteLatest = new ObservableAsPropertyHelper<bool>(canExecuteSubject,
                b => { if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty); },
                false, scheduler);

            if (executed != null)
                this.Subscribe(executed);
        }

        Func<object, bool> canExecuteExplicitFunc;
        protected Subject<bool> canExecuteSubject;
        public IObservable<bool> CanExecuteObservable {
            get { return canExecuteSubject; }
        }

        ObservableAsPropertyHelper<bool> canExecuteLatest;
        public virtual bool CanExecute(object parameter)
        {
            if (canExecuteExplicitFunc != null)
                canExecuteSubject.OnNext(canExecuteExplicitFunc(parameter));
                
            return canExecuteLatest.Value;
        }

        public event EventHandler CanExecuteChanged;

        Subject<object> executeSubject = new Subject<object>();
        public void Execute(object parameter)
        {
            this.Log().DebugFormat("{0:X}: Executed", this.GetHashCode());
            executeSubject.OnNext(parameter);
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return executeSubject.Subscribe(observer);
        }
    }

    public class ReactiveAsyncCommand : ReactiveCommand
    {
        public ReactiveAsyncCommand(Action<object> executed = null, int maximum_concurrent = 0, IScheduler scheduler = null)
            : this((IObservable<bool>)null, executed, maximum_concurrent, scheduler) { }

        public ReactiveAsyncCommand(IObservable<bool> can_execute = null, Action<object> executed = null, int maximum_concurrent = 0, IScheduler scheduler = null)
            : base(can_execute, executed, scheduler)
        {
            commonCtor(maximum_concurrent, scheduler);
        }

        public ReactiveAsyncCommand(Func<object, bool> can_execute, Action<object> executed = null, int maximum_concurrent = 0, IScheduler scheduler = null)
            : base(can_execute, executed, scheduler)
        {
            normal_sched = scheduler;
            commonCtor(maximum_concurrent, scheduler);
        }

        void commonCtor(int maximum_concurrent, IScheduler scheduler)
        {
            AsyncCompletedNotification = new Subject<Unit>();
            normal_sched = scheduler ?? ReactiveXaml.DefaultScheduler;

            ItemsInflight = Observable.Merge(
                this.Select(_ => 1),
                AsyncCompletedNotification.Select(_ => -1)
            ).Scan0(0, (x, acc) => {
                var ret = acc + x;
                if (ret < 0)
                    throw new OverflowException("Reference count dropped below zero");
                return ret;
            });

            ItemsInflight
                .Subscribe(x => {
                    this.Log().DebugFormat("0x{0:X} - {1} items in flight", this.GetHashCode(), x);
                    tooManyItems = (x >= maximum_concurrent && maximum_concurrent > 0);
                    canExecuteSubject.OnNext(!tooManyItems);
                });
        }

        IScheduler normal_sched;

        public IObservable<int> ItemsInflight { get; protected set; }

        public Subject<Unit> AsyncCompletedNotification { get; protected set; }

        bool tooManyItems = false;
        public override bool CanExecute(object parameter)
        {
            // HACK: Normally we shouldn't need this, but due to the way that
            // ReactiveCommand.CanExecute works when you provide an explicit
            // Func<T>, it can "trump" the ItemsInFlight selector.
            if (tooManyItems)
                return false;

            return base.CanExecute(parameter);
        }

        public IObservable<TResult> RegisterAsyncFunction<TResult>(Func<object, TResult> async_func, IScheduler scheduler = null)
        {
#if SILVERLIGHT
            scheduler = scheduler ?? Scheduler.ThreadPool;
#else
            scheduler = scheduler ?? Scheduler.TaskPool;
#endif
            var rebroadcast = new Subject<TResult>(normal_sched);

            this.ObserveOn(scheduler)
                .Select<object, TResult>(async_func)
                .Do(_ => AsyncCompletedNotification.OnNext(new Unit()), _ => AsyncCompletedNotification.OnNext(new Unit()))
                .Subscribe(rebroadcast.OnNext, rebroadcast.OnError, rebroadcast.OnCompleted);

            return rebroadcast;
        }

        public IDisposable RegisterAsyncAction(Action<object> async_action)
        {
            return RegisterAsyncFunction(x => { async_action(x); return new Unit(); })
                .Subscribe();
        }

        public IObservable<TResult> RegisterMemoizedFunction<TResult>(Func<object, TResult> async_func, int cache_size = 50, Action<TResult> on_release = null, IScheduler scheduler = null)
        {
#if SILVERLIGHT
            scheduler = scheduler ?? Scheduler.ThreadPool;
#else
            scheduler = scheduler ?? Scheduler.TaskPool;
#endif
            var cache = new MemoizingMRUCache<object, TResult>(
                (param, _) => { lock (async_func) { return async_func(param); } }, 
                cache_size, on_release);

            return this.ObserveOn(scheduler)
                .Select<object, TResult>(x => cache.Get(x))
                .Do(_ => AsyncCompletedNotification.OnNext(new Unit()))
                .ObserveOn(normal_sched);
        }
    }
}