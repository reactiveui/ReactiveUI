//using System;
//using System.Collections.Generic;
//using System.Diagnostics.Contracts;
//using System.Linq;
//using System.Reactive;
//using System.Reactive.Concurrency;
//using System.Reactive.Threading.Tasks;
//using System.Reactive.Linq;
//using System.Reactive.Subjects;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using System.Linq.Expressions;
//using ReactiveUI;

//using LegacyRxCmd = ReactiveUI.Legacy.ReactiveCommand;

//namespace ReactiveUI.Legacy
//{
//    /// <summary>
//    /// ReactiveCommand is the default Command implementation in ReactiveUI, which
//    /// conforms to the spec described in IReactiveCommand. 
//    /// </summary>
//    public class ReactiveCommand : IReactiveCommand, IObservable<object>
//    {
//        IDisposable innerDisp;

//        readonly Subject<bool> inflight = new Subject<bool>();
//        readonly ScheduledSubject<Exception> exceptions;
//        readonly Subject<object> executed = new Subject<object>();
//        readonly IScheduler defaultScheduler;

//        public ReactiveCommand() : this(null, false, null) { }
//        public ReactiveCommand(IObservable<bool> canExecute, bool initialCondition = true) : this(canExecute, false, null, initialCondition) { }

//        public ReactiveCommand(IObservable<bool> canExecute, bool allowsConcurrentExecution, IScheduler scheduler, bool initialCondition = true)
//        {
//            canExecute = canExecute ?? Observable.Return(true);
//            defaultScheduler = scheduler ?? RxApp.MainThreadScheduler;
//            AllowsConcurrentExecution = allowsConcurrentExecution;

//            canExecute = canExecute.Catch<bool, Exception>(ex => {
//                exceptions.OnNext(ex);
//                return Observable.Empty<bool>();
//            });

//            ThrownExceptions = exceptions = new ScheduledSubject<Exception>(defaultScheduler, RxApp.DefaultExceptionHandler);

//            var isExecuting = inflight
//                .Scan(0, (acc, x) => acc + (x ? 1 : -1))
//                .Select(x => x > 0)
//                .Publish(false)
//                .PermaRef()
//                .DistinctUntilChanged();

//            IsExecuting = isExecuting.ObserveOn(defaultScheduler);

//            var isBusy = allowsConcurrentExecution ? Observable.Return(false) : isExecuting;
//            var canExecuteAndNotBusy = Observable.CombineLatest(canExecute, isBusy, (ce, b) => ce && !b);

//            var canExecuteObs = canExecuteAndNotBusy
//                .Publish(initialCondition)
//                .RefCount();

//            CanExecuteObservable = canExecuteObs
//                .DistinctUntilChanged()
//                .ObserveOn(defaultScheduler);

//            innerDisp = canExecuteObs.Subscribe(x => {
//                if (canExecuteLatest == x) return;

//                canExecuteLatest = x;
//                defaultScheduler.Schedule(() => this.raiseCanExecuteChanged(EventArgs.Empty));
//            }, exceptions.OnNext);
//        }

//        /// <summary>
//        /// This creates a ReactiveCommand that calls several child 
//        /// ReactiveCommands when invoked. Its CanExecute will match the
//        /// combined result of the child CanExecutes (i.e. if any child
//        /// commands cannot execute, neither can the parent)
//        /// </summary>
//        /// <param name="canExecute">An Observable that determines whether the 
//        /// parent command can execute</param>
//        /// <param name="commands">The commands to combine.</param>
//        public static LegacyRxCmd CreateCombined(IObservable<bool> canExecute, params ReactiveCommand[] commands)
//        {
//            var childrenCanExecute = commands
//                .Select(x => x.CanExecuteObservable)
//                .CombineLatest(latestCanExecute => latestCanExecute.All(x => x != false));

//            var canExecuteSum = Observable.CombineLatest(
//                canExecute.StartWith(true),
//                childrenCanExecute,
//                (parent, child) => parent && child);

//            var ret = new LegacyRxCmd(canExecuteSum);
//            ret.Subscribe(x => {
//                foreach (var cmd in commands) cmd.Execute(x);
//            });

//            return ret;
//        }

//        public static ReactiveCommand CreateCombined(params ReactiveCommand[] commands)
//        {
//            return CreateCombined(Observable.Return(true), commands);
//        }

//        /// <summary>
//        /// Registers an asynchronous method to be called whenever the command
//        /// is Executed. This method returns an IObservable representing the
//        /// asynchronous operation, and is allowed to OnError / should OnComplete.
//        /// </summary>
//        /// <returns>A filtered version of the Observable which is marshaled 
//        /// to the UI thread. This Observable should only report successes and
//        /// instead send OnError messages to the ThrownExceptions property.</returns>
//        /// <param name="asyncBlock">The asynchronous method to call.</param>
//        /// <typeparam name="T">The 1st type parameter.</typeparam>
//        public IObservable<T> RegisterAsync<T>(Func<object, IObservable<T>> asyncBlock)
//        {
//            var ret = executed.Select(x => {
//                return asyncBlock(x)
//                    .Catch<T, Exception>(ex => {
//                        exceptions.OnNext(ex);
//                        return Observable.Empty<T>();
//                    })
//                    .Finally(() => { lock (inflight) { inflight.OnNext(false); } });
//            });

//            return ret
//                .Do(_ => { lock (inflight) { inflight.OnNext(true); } })
//                .Merge()
//                .ObserveOn(defaultScheduler)
//                .Publish().RefCount();
//        }

//        /// <summary>
//        /// Gets a value indicating whether this instance is executing. This 
//        /// Observable is guaranteed to always return a value immediately (i.e.
//        /// it is backed by a BehaviorSubject), meaning it is safe to determine
//        /// the current state of the command via IsExecuting.First()
//        /// </summary>
//        /// <value>true</value>
//        /// <c>false</c>
//        public IObservable<bool> IsExecuting { get; protected set; }

//        public bool AllowsConcurrentExecution { get; protected set; }

//        /// <summary>
//        /// Fires whenever an exception would normally terminate ReactiveUI 
//        /// internal state.
//        /// </summary>
//        /// <value>The thrown exceptions.</value>
//        public IObservable<Exception> ThrownExceptions { get; protected set; }

//        public IDisposable Subscribe(IObserver<object> observer)
//        {
//            return executed.Subscribe(
//                Observer.Create<object>(
//                    x => marshalFailures(observer.OnNext, x),
//                    ex => marshalFailures(observer.OnError, ex),
//                    () => marshalFailures(observer.OnCompleted)));
//        }

//        bool canExecuteLatest;
//        public bool CanExecute(object parameter)
//        {
//            return canExecuteLatest;
//        }

//        public event EventHandler CanExecuteChanged;

//        public void Execute(object parameter)
//        {
//            lock(inflight) { inflight.OnNext(true); }
//            executed.OnNext(parameter);
//            lock(inflight) { inflight.OnNext(false); }
//        }

//        public IObservable<bool> CanExecuteObservable { get; protected set; }
    
//        public void Dispose()
//        {
//            var disp = Interlocked.Exchange(ref innerDisp, null);
//            if (disp != null) disp.Dispose();
//        }

//        void marshalFailures<T>(Action<T> block, T param)
//        {
//            try {
//                block(param);
//            } catch (Exception ex) {
//                exceptions.OnNext(ex);
//            }
//        }

//        void marshalFailures(Action block)
//        {
//            marshalFailures(_ => block(), Unit.Default);
//        }

//        protected virtual void raiseCanExecuteChanged(EventArgs e)
//        {
//            var handler = this.CanExecuteChanged;

//            if (handler != null) {
//                handler(this, e);
//            }
//        }
//    }

//    public static class ReactiveCommandMixins
//    {
//        /// <summary>
//        /// ToCommand is a convenience method for returning a new
//        /// ReactiveCommand based on an existing Observable chain.
//        /// </summary>
//        /// <param name="scheduler">The scheduler to publish events on - default
//        /// is RxApp.MainThreadScheduler.</param>
//        /// <returns>A new ReactiveCommand whose CanExecute Observable is the
//        /// current object.</returns>
//        public static ReactiveCommand ToCommand(this IObservable<bool> This, bool allowsConcurrentExecution = false, IScheduler scheduler = null)
//        {
//            return new ReactiveCommand(This, allowsConcurrentExecution, scheduler);
//        }

//        /// <summary>
//        /// A utility method that will pipe an Observable to an ICommand (i.e.
//        /// it will first call its CanExecute with the provided value, then if
//        /// the command can be executed, Execute() will be called)
//        /// </summary>
//        /// <param name="command">The command to be executed.</param>
//        /// <returns>An object that when disposes, disconnects the Observable
//        /// from the command.</returns>
//        public static IDisposable InvokeCommand<T>(this IObservable<T> This, ICommand command)
//        {
//            return This.ObserveOn(RxApp.MainThreadScheduler).Subscribe(x => {
//                if (!command.CanExecute(x)) {
//                    return;
//                }
//                command.Execute(x);
//            });
//        }

//        /// <summary>
//        /// A utility method that will pipe an Observable to an ICommand (i.e.
//        /// it will first call its CanExecute with the provided value, then if
//        /// the command can be executed, Execute() will be called)
//        /// </summary>
//        /// <param name="target">The root object which has the Command.</param>
//        /// <param name="commandProperty">The expression to reference the Command.</param>
//        /// <returns>An object that when disposes, disconnects the Observable
//        /// from the command.</returns>
//        public static IDisposable InvokeCommand<T, TTarget>(this IObservable<T> This, TTarget target, Expression<Func<TTarget, ICommand>> commandProperty)
//        {
//            return This.CombineLatest(target.WhenAnyValue(commandProperty), (val, cmd) => new { val, cmd })
//                .ObserveOn(RxApp.MainThreadScheduler)
//                .Subscribe(x => {
//                    if (!x.cmd.CanExecute(x.val)) {
//                        return;
//                    }

//                    x.cmd.Execute(x.val);
//                });
//        }

//        /// <summary>
//        /// RegisterAsyncFunction registers an asynchronous method that returns a result
//        /// to be called whenever the Command's Execute method is called.
//        /// </summary>
//        /// <param name="calculationFunc">The function to be run in the
//        /// background.</param>
//        /// <param name="scheduler"></param>
//        /// <returns>An Observable that will fire on the UI thread once per
//        /// invoecation of Execute, once the async method completes. Subscribe to
//        /// this to retrieve the result of the calculationFunc.</returns>
//        public static IObservable<TResult> RegisterAsyncFunction<TResult>(this LegacyRxCmd This,
//            Func<object, TResult> calculationFunc,
//            IScheduler scheduler = null)
//        {
//            Contract.Requires(calculationFunc != null);

//            var asyncFunc = calculationFunc.ToAsync(scheduler ?? RxApp.TaskpoolScheduler);
//            return This.RegisterAsync(asyncFunc);
//        }

//        /// <summary>
//        /// RegisterAsyncAction registers an asynchronous method that runs
//        /// whenever the Command's Execute method is called and doesn't return a
//        /// result.
//        /// </summary>
//        /// <param name="calculationFunc">The function to be run in the
//        /// background.</param>
//        public static IObservable<Unit> RegisterAsyncAction(this LegacyRxCmd This, 
//            Action<object> calculationFunc,
//            IScheduler scheduler = null)
//        {
//            Contract.Requires(calculationFunc != null);

//            // NB: This PermaRef isn't exactly correct, but the people using
//            // this method probably are Doing It Wrong, so let's let them
//            // continue to do so.
//            return This.RegisterAsyncFunction(x => { calculationFunc(x); return new Unit(); }, scheduler)
//                .Publish().PermaRef();
//        }

//        /// <summary>
//        /// RegisterAsyncTask registers an TPL/Async method that runs when a 
//        /// Command gets executed and returns the result
//        /// </summary>
//        /// <returns>An Observable that will fire on the UI thread once per
//        /// invoecation of Execute, once the async method completes. Subscribe to
//        /// this to retrieve the result of the calculationFunc.</returns>
//        public static IObservable<TResult> RegisterAsyncTask<TResult>(this LegacyRxCmd This, Func<object, Task<TResult>> calculationFunc)
//        {
//            Contract.Requires(calculationFunc != null);
//            return This.RegisterAsync(x => calculationFunc(x).ToObservable());
//        }

//        /// <summary>
//        /// RegisterAsyncTask registers an TPL/Async method that runs when a 
//        /// Command gets executed and returns no result. 
//        /// </summary>
//        /// <param name="calculationFunc">The function to be run in the
//        /// background.</param>
//        /// <returns>An Observable that signals when the Task completes, on
//        /// the UI thread.</returns>
//        public static IObservable<Unit> RegisterAsyncTask(this LegacyRxCmd This, Func<object, Task> calculationFunc)
//        {
//            Contract.Requires(calculationFunc != null);

//            // NB: This PermaRef isn't exactly correct, but the people using
//            // this method probably are Doing It Wrong, so let's let them
//            // continue to do so.
//            return This.RegisterAsync(x => calculationFunc(x).ToObservable())
//                .Publish().PermaRef();
//        }
//    }
//}
