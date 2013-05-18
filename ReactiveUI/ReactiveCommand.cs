using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReactiveUI
{
    public interface IReactiveCommand : IHandleObservableErrors, IObservable<object>, ICommand, IDisposable, IEnableLogger
    {
        IObservable<T> RegisterAsync<T>(Func<object, IObservable<T>> asyncBlock);

        IObservable<bool> CanExecuteObservable { get; }
        IObservable<bool> IsExecuting { get; }
        bool AllowsConcurrentExecution { get; }
    }

    public class ReactiveCommand : IReactiveCommand
    {
        IDisposable innerDisp;

        readonly Subject<bool> inflight = new Subject<bool>();
        readonly ScheduledSubject<Exception> exceptions;
        readonly Subject<object> executed = new Subject<object>();
        readonly IScheduler defaultScheduler;

        public ReactiveCommand() : this(null, false, null) { }
        public ReactiveCommand(IObservable<bool> canExecute, bool allowsConcurrentExecution, IScheduler scheduler)
        {
            canExecute = canExecute ?? Observable.Return(true);
            defaultScheduler = scheduler ?? RxApp.MainThreadScheduler;
            AllowsConcurrentExecution = allowsConcurrentExecution;

            ThrownExceptions = exceptions = new ScheduledSubject<Exception>(defaultScheduler, RxApp.DefaultExceptionHandler);

            innerDisp = CanExecuteObservable.Subscribe(x => {
                if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
            }, exceptions.OnNext);

            IsExecuting = inflight
                .Scan(0, (acc, x) => acc + (x ? 1 : -1))
                .Select(x => x > 0);

            var isBusy = allowsConcurrentExecution ? Observable.Return(false) : IsExecuting;
            var canExecuteAndNotBusy = Observable.CombineLatest(canExecute, isBusy, (ce, b) => ce && !b);

            CanExecuteObservable = canExecuteAndNotBusy.Publish(true).RefCount();
        }

        public IObservable<T> RegisterAsync<T>(Func<object, IObservable<T>> asyncBlock)
        {
            var ret = executed.Select(x => {
                return asyncBlock(x)
                    .Catch<T, Exception>(ex => {
                        exceptions.OnNext(ex);
                        return Observable.Empty<T>();
                    })
                    .Multicast(new ReplaySubject<T>())
                    .RefCount();
            });

            return ret
                .Do(_ => { lock(inflight) { inflight.OnNext(true); } })
                .Merge()
                .Finally(() => { lock(inflight) { inflight.OnNext(true); } })
                .ObserveOn(defaultScheduler);
        }

        public IObservable<bool> IsExecuting { get; protected set; }

        public bool AllowsConcurrentExecution { get; protected set; }

        public IObservable<Exception> ThrownExceptions { get; protected set; }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return executed.Subscribe(
                Observer.Create<object>(
                    x => marshalFailures(observer.OnNext, x),
                    ex => marshalFailures(observer.OnError, ex),
                    () => marshalFailures(observer.OnCompleted)));
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteObservable.First();
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            lock(inflight) { inflight.OnNext(true); }
            executed.OnNext(parameter);
            lock(inflight) { inflight.OnNext(false); }
        }

        public IObservable<bool> CanExecuteObservable { get; protected set; }
    
        public void Dispose()
        {
            var disp = Interlocked.Exchange(ref innerDisp, null);
            if (disp != null) disp.Dispose();
        }

        void marshalFailures<T>(Action<T> block, T param)
        {
            try {
                block(param);
            } catch (Exception ex) {
                exceptions.OnNext(ex);
            }
        }

        void marshalFailures(Action block)
        {
            marshalFailures(_ => block(), Unit.Default);
        }
    }
}