using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Concurrency;
using System.Windows.Threading;
using System.Diagnostics.Contracts;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace ReactiveXaml
{
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
            Contract.Requires(maximum_concurrent > 0);

            normal_sched = scheduler;
            commonCtor(maximum_concurrent, scheduler);
        }

        void commonCtor(int maximum_concurrent, IScheduler scheduler)
        {
            AsyncCompletedNotification = new Subject<Unit>();
            normal_sched = scheduler ?? RxApp.DeferredScheduler;

            ItemsInflight = Observable.Merge(
                this.Select(_ => 1),
                AsyncCompletedNotification.Select(_ => -1)
            ).Scan0(0, (acc, x) => {
                var ret = acc + x;
                if (ret < 0)
                    throw new OverflowException("Reference count dropped below zero");
                return ret;
            });

            ItemsInflight
                .Subscribe(x => {
                    this.Log().InfoFormat("0x{0:X} - {1} items in flight", this.GetHashCode(), x);
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
            Contract.Requires(async_func != null);

            scheduler = scheduler ?? RxApp.TaskpoolScheduler;
            var rebroadcast = new Subject<TResult>();

            this.ObserveOn(scheduler)
                .Select(async_func)
                .Do(_ => AsyncCompletedNotification.OnNext(new Unit()), _ => AsyncCompletedNotification.OnNext(new Unit()))
                .Subscribe(rebroadcast.OnNext, rebroadcast.OnError, rebroadcast.OnCompleted);

            return rebroadcast.ObserveOn(normal_sched);
        }

        public IObservable<TResult> RegisterObservableAsyncFunction<TResult>(Func<object, IObservable<TResult>> async_func)
        {
            Contract.Requires(async_func != null);

            var rebroadcast = new Subject<TResult>();

            this.SelectMany(async_func)
                .Do(_ => AsyncCompletedNotification.OnNext(new Unit()), _ => AsyncCompletedNotification.OnNext(new Unit()))
                .Subscribe(rebroadcast.OnNext, rebroadcast.OnError, rebroadcast.OnCompleted);

            return rebroadcast.ObserveOn(normal_sched);
        }

        public IDisposable RegisterAsyncAction(Action<object> async_action)
        {
            Contract.Requires(async_action != null);

            return RegisterAsyncFunction(x => { async_action(x); return new Unit(); })
                .Subscribe();
        }

        public IObservable<TResult> RegisterMemoizedFunction<TResult>(Func<object, TResult> async_func, int cache_size = 50, Action<TResult> on_release = null, IScheduler scheduler = null)
        {
            Contract.Requires(async_func != null);
            Contract.Requires(cache_size > 0);

            scheduler = scheduler ?? RxApp.TaskpoolScheduler;

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

// vim: tw=120 ts=4 sw=4 et :
