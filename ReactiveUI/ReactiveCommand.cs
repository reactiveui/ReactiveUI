using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Threading.Tasks;
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
        public ReactiveCommand(IObservable<bool> canExecute) : this(canExecute, false, null) { }

        public ReactiveCommand(IObservable<bool> canExecute, bool allowsConcurrentExecution, IScheduler scheduler)
        {
            canExecute = canExecute ?? Observable.Return(true);
            defaultScheduler = scheduler ?? RxApp.MainThreadScheduler;
            AllowsConcurrentExecution = allowsConcurrentExecution;

            canExecute = canExecute.Catch<bool, Exception>(ex => {
                exceptions.OnNext(ex);
                return Observable.Empty<bool>();
            });

            ThrownExceptions = exceptions = new ScheduledSubject<Exception>(defaultScheduler, RxApp.DefaultExceptionHandler);

            IsExecuting = inflight
                .Scan(0, (acc, x) => acc + (x ? 1 : -1))
                .Select(x => x > 0)
                .Publish(false)
                .PermaRef()
                .DistinctUntilChanged();

            var isBusy = allowsConcurrentExecution ? Observable.Return(false) : IsExecuting;
            var canExecuteAndNotBusy = Observable.CombineLatest(canExecute, isBusy, (ce, b) => ce && !b);

            CanExecuteObservable = canExecuteAndNotBusy
                .Publish(true)
                .RefCount()
                .DistinctUntilChanged();

            innerDisp = CanExecuteObservable.Subscribe(x => {
                if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
            }, exceptions.OnNext);
        }

        public IObservable<T> RegisterAsync<T>(Func<object, IObservable<T>> asyncBlock)
        {
            var ret = executed.Select(x => {
                return asyncBlock(x)
                    .Catch<T, Exception>(ex => {
                        exceptions.OnNext(ex);
                        return Observable.Empty<T>();
                    })
                    .Finally(() => { lock (inflight) { inflight.OnNext(false); } });
            });

            return ret
                .Do(_ => { lock (inflight) { inflight.OnNext(true); } })
                .Merge()
                .ObserveOn(defaultScheduler)
                .Publish().PermaRef();
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

    public static class ReactiveCommandMixins
    {
        /// <summary>
        /// ToCommand is a convenience method for returning a new
        /// ReactiveCommand based on an existing Observable chain.
        /// </summary>
        /// <param name="scheduler">The scheduler to publish events on - default
        /// is RxApp.MainThreadScheduler.</param>
        /// <returns>A new ReactiveCommand whose CanExecute Observable is the
        /// current object.</returns>
        public static ReactiveCommand ToCommand(this IObservable<bool> This, bool allowsConcurrentExecution = false, IScheduler scheduler = null)
        {
            return new ReactiveCommand(This, allowsConcurrentExecution, scheduler);
        }

        /// <summary>
        /// A utility method that will pipe an Observable to an ICommand (i.e.
        /// it will first call its CanExecute with the provided value, then if
        /// the command can be executed, Execute() will be called)
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <returns>An object that when disposes, disconnects the Observable
        /// from the command.</returns>
        public static IDisposable InvokeCommand<T>(this IObservable<T> This, ICommand command)
        {
            return This.ObserveOn(RxApp.MainThreadScheduler).Subscribe(x => {
                if (!command.CanExecute(x)) {
                    return;
                }
                command.Execute(x);
            });
        }

        /// <summary>
        /// RegisterAsyncFunction registers an asynchronous method that returns a result
        /// to be called whenever the Command's Execute method is called.
        /// </summary>
        /// <param name="calculationFunc">The function to be run in the
        /// background.</param>
        /// <param name="scheduler"></param>
        /// <returns>An Observable that will fire on the UI thread once per
        /// invoecation of Execute, once the async method completes. Subscribe to
        /// this to retrieve the result of the calculationFunc.</returns>
        public static IObservable<TResult> RegisterAsyncFunction<TResult>(this IReactiveCommand This,
            Func<object, TResult> calculationFunc,
            IScheduler scheduler = null)
        {
            Contract.Requires(calculationFunc != null);

            var asyncFunc = calculationFunc.ToAsync(scheduler ?? RxApp.TaskpoolScheduler);
            return This.RegisterAsync(asyncFunc);
        }

        /// <summary>
        /// RegisterAsyncAction registers an asynchronous method that runs
        /// whenever the Command's Execute method is called and doesn't return a
        /// result.
        /// </summary>
        /// <param name="calculationFunc">The function to be run in the
        /// background.</param>
        public static IObservable<Unit> RegisterAsyncAction(this IReactiveCommand This, 
            Action<object> calculationFunc,
            IScheduler scheduler = null)
        {
            Contract.Requires(calculationFunc != null);
            return This.RegisterAsyncFunction(x => { calculationFunc(x); return new Unit(); }, scheduler);
        }

        /// <summary>
        /// RegisterAsyncTask registers an TPL/Async method that runs when a 
        /// Command gets executed and returns the result
        /// </summary>
        /// <returns>An Observable that will fire on the UI thread once per
        /// invoecation of Execute, once the async method completes. Subscribe to
        /// this to retrieve the result of the calculationFunc.</returns>
        public static IObservable<TResult> RegisterAsyncTask<TResult>(this IReactiveCommand This, Func<object, Task<TResult>> calculationFunc)
        {
            Contract.Requires(calculationFunc != null);
            return This.RegisterAsync(x => calculationFunc(x).ToObservable());
        }

        /// <summary>
        /// RegisterAsyncTask registers an TPL/Async method that runs when a 
        /// Command gets executed and returns no result. 
        /// </summary>
        /// <param name="calculationFunc">The function to be run in the
        /// background.</param>
        /// <returns>An Observable that signals when the Task completes, on
        /// the UI thread.</returns>
        public static IObservable<Unit> RegisterAsyncTask(this IReactiveCommand This, Func<object, Task> calculationFunc)
        {
            Contract.Requires(calculationFunc != null);
            return This.RegisterAsync(x => calculationFunc(x).ToObservable());
        }
    }
}