using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Splat;
using LegacyRxCmd = ReactiveUI.Legacy.ReactiveCommand;

namespace ReactiveUI.Legacy
{
    [Obsolete("This type is obsolete and will be removed in a future version of ReactiveUI. Please switch to using ReactiveUI.ReactiveCommand instead.")]
    public interface IReactiveCommand : IHandleObservableErrors, ICommand, IDisposable, IEnableLogger
    {
        /// <summary>
        /// Gets a value indicating whether this instance can execute observable.
        /// </summary>
        /// <value><c>true</c> if this instance can execute observable; otherwise, <c>false</c>.</value>
        IObservable<bool> CanExecuteObservable { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is executing. This
        /// Observable is guaranteed to always return a value immediately (i.e.
        /// it is backed by a BehaviorSubject), meaning it is safe to determine
        /// the current state of the command via IsExecuting.First()
        /// </summary>
        /// <value><c>true</c> if this instance is executing; otherwise, <c>false</c>.</value>
        IObservable<bool> IsExecuting { get; }
    }

    /// <summary>
    /// IReactiveCommand represents an ICommand which also notifies when it is
    /// executed (i.e. when Execute is called) via IObservable. Conceptually,
    /// this represents an Event, so as a result this IObservable should never
    /// OnComplete or OnError.
    ///
    /// In previous versions of ReactiveUI, this interface was split into two
    /// separate interfaces, one to handle async methods and one for "standard"
    /// commands, but these have now been merged - every ReactiveCommand is now
    /// a ReactiveAsyncCommand.
    /// </summary>
    [Obsolete("This type is obsolete and will be removed in a future version of ReactiveUI. Please switch to using ReactiveUI.ReactiveCommand instead.")]
    public interface IReactiveCommand<T> : IObservable<T>, IReactiveCommand
    {
        IObservable<T> ExecuteAsync(object parameter = null);
    }

    [Obsolete("This type is obsolete and will be removed in a future version of ReactiveUI. Please switch to using ReactiveUI.ReactiveCommand instead.")]
    public static class ReactiveCommand
    {
        /// <summary>
        /// Creates a default ReactiveCommand that has no background action. This
        /// is probably what you want if you were calling the constructor in
        /// previous versions of ReactiveUI
        /// </summary>
        /// <param name="canExecute">An Observable that determines when the
        /// Command can Execute. WhenAny is a great way to create this!</param>
        /// <param name="scheduler">The scheduler to deliver events on.
        /// Defaults to RxApp.MainThreadScheduler.</param>
        /// <returns>A ReactiveCommand whose ExecuteAsync just returns the
        /// CommandParameter immediately. Which you should ignore!</returns>
        public static ReactiveCommand<object> Create(IObservable<bool> canExecute = null, IScheduler scheduler = null)
        {
            canExecute = canExecute ?? Observables.True;
            return new ReactiveCommand<object>(canExecute, x => Observable.Return(x), scheduler);
        }

        /// <summary>
        /// Creates a ReactiveCommand typed to the given executeAsync Observable
        /// method. Use this method if your background method returns IObservable.
        /// </summary>
        /// <param name="canExecute">An Observable that determines when the
        /// Command can Execute. WhenAny is a great way to create this!</param>
        /// <param name="executeAsync">Method to call that creates an Observable
        /// representing an operation to execute in the background. The Command's
        /// CanExecute will be false until this Observable completes. If this
        /// Observable terminates with OnError, the Exception is marshaled to
        /// ThrownExceptions.</param>
        /// <param name="scheduler">The scheduler to deliver events on.
        /// Defaults to RxApp.MainThreadScheduler.</param>
        /// <returns>A ReactiveCommand which returns all items that are created via
        /// calling executeAsync as a single stream.</returns>
        public static ReactiveCommand<T> CreateAsyncObservable<T>(IObservable<bool> canExecute, Func<object, IObservable<T>> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<T>(canExecute, executeAsync, scheduler);
        }

        /// <summary>
        /// Creates a ReactiveCommand typed to the given executeAsync Observable
        /// method. Use this method if your background method returns IObservable.
        /// </summary>
        /// <param name="executeAsync">Method to call that creates an Observable
        /// representing an operation to execute in the background. The Command's
        /// CanExecute will be false until this Observable completes. If this
        /// Observable terminates with OnError, the Exception is marshaled to
        /// ThrownExceptions.</param>
        /// <param name="scheduler">The scheduler to deliver events on.
        /// Defaults to RxApp.MainThreadScheduler.</param>
        /// <returns>A ReactiveCommand which returns all items that are created via
        /// calling executeAsync as a single stream.</returns>
        public static ReactiveCommand<T> CreateAsyncObservable<T>(Func<object, IObservable<T>> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<T>(Observables.True, executeAsync, scheduler);
        }

        /// <summary>
        /// Creates a ReactiveCommand typed to the given executeAsync Task-based
        /// method. Use this method if your background method returns Task or uses
        /// async/await.
        /// </summary>
        /// <param name="canExecute">An Observable that determines when the
        /// Command can Execute. WhenAny is a great way to create this!</param>
        /// <param name="executeAsync">Method to call that creates a Task
        /// representing an operation to execute in the background. The Command's
        /// CanExecute will be false until this Task completes. If this
        /// Task terminates with an Exception, the Exception is marshaled to
        /// ThrownExceptions.</param>
        /// <param name="scheduler">The scheduler to deliver events on.
        /// Defaults to RxApp.MainThreadScheduler.</param>
        /// <returns>A ReactiveCommand which returns all items that are created via
        /// calling executeAsync as a single stream.</returns>
        public static ReactiveCommand<T> CreateAsyncTask<T>(IObservable<bool> canExecute, Func<object, Task<T>> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<T>(canExecute, x => executeAsync(x).ToObservable(), scheduler);
        }

        /// <summary>
        /// Creates a ReactiveCommand typed to the given executeAsync Task-based
        /// method. Use this method if your background method returns Task or uses
        /// async/await.
        /// </summary>
        /// <param name="executeAsync">Method to call that creates a Task
        /// representing an operation to execute in the background. The Command's
        /// CanExecute will be false until this Task completes. If this
        /// Task terminates with an Exception, the Exception is marshaled to
        /// ThrownExceptions.</param>
        /// <param name="scheduler">The scheduler to deliver events on.
        /// Defaults to RxApp.MainThreadScheduler.</param>
        /// <returns>A ReactiveCommand which returns all items that are created via
        /// calling executeAsync as a single stream.</returns>
        public static ReactiveCommand<T> CreateAsyncTask<T>(Func<object, Task<T>> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<T>(Observables.True, x => executeAsync(x).ToObservable(), scheduler);
        }

        /// <summary>
        /// Creates a ReactiveCommand typed to the given executeAsync Task-based
        /// method. Use this method if your background method returns Task or uses
        /// async/await.
        /// </summary>
        /// <param name="executeAsync">Method to call that creates a Task
        /// representing an operation to execute in the background. The Command's
        /// CanExecute will be false until this Task completes. If this
        /// Task terminates with an Exception, the Exception is marshaled to
        /// ThrownExceptions.</param>
        /// <param name="scheduler">The scheduler to deliver events on.
        /// Defaults to RxApp.MainThreadScheduler.</param>
        /// <returns>A ReactiveCommand which returns all items that are created via
        /// calling executeAsync as a single stream.</returns>
        public static ReactiveCommand<Unit> CreateAsyncTask(Func<object, Task> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<Unit>(Observables.True, x => executeAsync(x).ToObservable(), scheduler);
        }

        /// <summary>
        /// Creates a ReactiveCommand typed to the given executeAsync Task-based
        /// method. Use this method if your background method returns Task or uses
        /// async/await.
        /// </summary>
        /// <param name="canExecute">An Observable that determines when the
        /// Command can Execute. WhenAny is a great way to create this!</param>
        /// <param name="executeAsync">Method to call that creates a Task
        /// representing an operation to execute in the background. The Command's
        /// CanExecute will be false until this Task completes. If this
        /// Task terminates with an Exception, the Exception is marshaled to
        /// ThrownExceptions.</param>
        /// <param name="scheduler">The scheduler to deliver events on.
        /// Defaults to RxApp.MainThreadScheduler.</param>
        /// <returns>A ReactiveCommand which returns all items that are created via
        /// calling executeAsync as a single stream.</returns>
        public static ReactiveCommand<Unit> CreateAsyncTask(IObservable<bool> canExecute, Func<object, Task> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<Unit>(canExecute, x => executeAsync(x).ToObservable(), scheduler);
        }

        /// <summary>
        /// Creates a ReactiveCommand typed to the given executeAsync Task-based
        /// method that supports cancellation. Use this method if your background
        /// method returns Task or uses async/await.
        /// </summary>
        /// <param name="canExecute">An Observable that determines when the
        /// Command can Execute. WhenAny is a great way to create this!</param>
        /// <param name="executeAsync">Method to call that creates a Task
        /// representing an operation to execute in the background. The Command's
        /// CanExecute will be false until this Task completes. If this
        /// Task terminates with an Exception, the Exception is marshaled to
        /// ThrownExceptions.</param>
        /// <param name="scheduler">The scheduler to deliver events on.
        /// Defaults to RxApp.MainThreadScheduler.</param>
        /// <returns>A ReactiveCommand which returns all items that are created via
        /// calling executeAsync as a single stream.</returns>
        public static ReactiveCommand<T> CreateAsyncTask<T>(IObservable<bool> canExecute, Func<object, CancellationToken, Task<T>> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<T>(canExecute, x => Observable.StartAsync(ct => executeAsync(x, ct)), scheduler);
        }

        /// <summary>
        /// Creates a ReactiveCommand typed to the given executeAsync Task-based
        /// method that supports cancellation. Use this method if your background
        /// method returns Task or uses async/await.
        /// </summary>
        /// <param name="executeAsync">Method to call that creates a Task
        /// representing an operation to execute in the background. The Command's
        /// CanExecute will be false until this Task completes. If this
        /// Task terminates with an Exception, the Exception is marshaled to
        /// ThrownExceptions.</param>
        /// <param name="scheduler">The scheduler to deliver events on.
        /// Defaults to RxApp.MainThreadScheduler.</param>
        /// <returns>A ReactiveCommand which returns all items that are created via
        /// calling executeAsync as a single stream.</returns>
        public static ReactiveCommand<T> CreateAsyncTask<T>(Func<object, CancellationToken, Task<T>> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<T>(Observables.True, x => Observable.StartAsync(ct => executeAsync(x,ct)), scheduler);
        }

        /// <summary>
        /// Creates a ReactiveCommand typed to the given executeAsync Task-based
        /// method that supports cancellation. Use this method if your background
        /// method returns Task or uses async/await.
        /// </summary>
        /// <param name="canExecute">An Observable that determines when the
        /// Command can Execute. WhenAny is a great way to create this!</param>
        /// <param name="executeAsync">Method to call that creates a Task
        /// representing an operation to execute in the background. The Command's
        /// CanExecute will be false until this Task completes. If this
        /// Task terminates with an Exception, the Exception is marshaled to
        /// ThrownExceptions.</param>
        /// <param name="scheduler">The scheduler to deliver events on.
        /// Defaults to RxApp.MainThreadScheduler.</param>
        /// <returns>A ReactiveCommand which returns all items that are created via
        /// calling executeAsync as a single stream.</returns>
        public static ReactiveCommand<Unit> CreateAsyncTask(Func<object, CancellationToken, Task> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<Unit>(Observables.True, x => Observable.StartAsync(ct => executeAsync(x,ct)), scheduler);
        }

        /// <summary>
        /// Creates a ReactiveCommand typed to the given executeAsync Task-based
        /// method that supports cancellation. Use this method if your background
        /// method returns Task or uses async/await.
        /// </summary>
        /// <param name="executeAsync">Method to call that creates a Task
        /// representing an operation to execute in the background. The Command's
        /// CanExecute will be false until this Task completes. If this
        /// Task terminates with an Exception, the Exception is marshaled to
        /// ThrownExceptions.</param>
        /// <param name="scheduler">The scheduler to deliver events on.
        /// Defaults to RxApp.MainThreadScheduler.</param>
        /// <returns>A ReactiveCommand which returns all items that are created via
        /// calling executeAsync as a single stream.</returns>
        public static ReactiveCommand<Unit> CreateAsyncTask(IObservable<bool> canExecute, Func<object, CancellationToken, Task> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<Unit>(canExecute, x => Observable.StartAsync(ct => executeAsync(x,ct)), scheduler);
        }

        /// <summary>
        /// This creates a ReactiveCommand that calls several child
        /// ReactiveCommands when invoked. Its CanExecute will match the
        /// combined result of the child CanExecutes (i.e. if any child
        /// commands cannot execute, neither can the parent)
        /// </summary>
        /// <param name="canExecute">An Observable that determines whether the
        /// parent command can execute</param>
        /// <param name="commands">The commands to combine.</param>
        public static ReactiveCommand<object> CreateCombined(IObservable<bool> canExecute, params IReactiveCommand[] commands)
        {
            var childrenCanExecute = commands
                .Select(x => x.CanExecuteObservable)
                .CombineLatest(latestCanExecute => latestCanExecute.All(x => x != false));

            var canExecuteSum = Observable.CombineLatest(
                canExecute.StartWith(true),
                childrenCanExecute,
                (parent, child) => parent && child);

            var ret = ReactiveCommand.Create(canExecuteSum);
            ret.Subscribe(x => commands.ForEach(cmd => cmd.Execute(x)));
            return ret;
        }

        /// <summary>
        /// This creates a ReactiveCommand that calls several child
        /// ReactiveCommands when invoked. Its CanExecute will match the
        /// combined result of the child CanExecutes (i.e. if any child
        /// commands cannot execute, neither can the parent)
        /// </summary>
        /// <param name="commands">The commands to combine.</param>
        public static ReactiveCommand<object> CreateCombined(params IReactiveCommand[] commands)
        {
            return CreateCombined(Observables.True, commands);
        }
    }

    /// <summary>
    /// This class represents a Command that can optionally do a background task.
    /// The results of the background task (or a signal that the Command has been
    /// invoked) are delivered by Subscribing to the command itself, since
    /// ReactiveCommand is itself an Observable. The results of individual
    /// invocations can be retrieved via the ExecuteAsync method.
    /// </summary>
    [Obsolete("This type is obsolete and will be removed in a future version of ReactiveUI. Please switch to using ReactiveUI.ReactiveCommand instead.")]
    public class ReactiveCommand<T> : IReactiveCommand<T>, IReactiveCommand
    {
#if NET_45
        public event EventHandler CanExecuteChanged;

        protected virtual void raiseCanExecuteChanged(EventArgs args)
        {
            var handler = this.CanExecuteChanged;
            if (handler != null) {
                handler(this, args);
            }
        }
#else
        public event EventHandler CanExecuteChanged
        {
            add {
                if (canExecuteDisp == null) canExecuteDisp = canExecute.Connect();
                CanExecuteChangedEventManager.AddHandler(this, value);
            }
            remove { CanExecuteChangedEventManager.RemoveHandler(this, value); }
        }

        protected virtual void raiseCanExecuteChanged(EventArgs args)
        {
            CanExecuteChangedEventManager.DeliverEvent(this, args);
        }
#endif
        readonly Subject<T> executeResults = new Subject<T>();
        readonly Subject<bool> isExecuting = new Subject<bool>();
        readonly Func<object, IObservable<T>> executeAsync;
        readonly IScheduler scheduler;
        readonly ScheduledSubject<Exception> exceptions;

        IConnectableObservable<bool> canExecute;
        bool canExecuteLatest = false;
        IDisposable canExecuteDisp;
        int inflightCount = 0;

        /// <summary>
        /// Don't use this, use ReactiveCommand.CreateXYZ instead
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecute, Func<object, IObservable<T>> executeAsync, IScheduler scheduler = null)
        {
            this.scheduler = scheduler ?? RxApp.MainThreadScheduler;
            this.executeAsync = executeAsync;

            this.canExecute = canExecute.CombineLatest(isExecuting.StartWith(false), (ce, ie) => ce && !ie)
                .Catch<bool, Exception>(ex => {
                    exceptions.OnNext(ex);
                    return Observables.False;
                })
                .Do(x => {
                    var fireCanExecuteChanged = (canExecuteLatest != x);
                    canExecuteLatest = x;

                    if (fireCanExecuteChanged) {
                        this.raiseCanExecuteChanged(EventArgs.Empty);
                    }
                })
                .Publish();

            if (ModeDetector.InUnitTestRunner()) {
                this.canExecute.Connect();
            }

            ThrownExceptions = exceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);
        }

        /// <summary>
        /// Executes a Command and returns the result asynchronously. This method
        /// makes it *much* easier to test ReactiveCommand, as well as create
        /// ReactiveCommands who invoke inferior commands and wait on their results.
        ///
        /// Note that you **must** Subscribe to the Observable returned by
        /// ExecuteAsync or else nothing will happen (i.e. ExecuteAsync is lazy)
        ///
        /// Note also that the command will be executed, irrespective of the current value
        /// of the command's canExecute observable.
        /// </summary>
        /// <returns>An Observable representing a single invocation of the Command.</returns>
        /// <param name="parameter">Don't use this.</param>
        public IObservable<T> ExecuteAsync(object parameter = null)
        {
            var ret = Observable.Create<T>(subj => {
                if (Interlocked.Increment(ref inflightCount) == 1) {
                    isExecuting.OnNext(true);
                }

                var decrement = new SerialDisposable() {
                    Disposable = Disposable.Create(() => {
                        if (Interlocked.Decrement(ref inflightCount) == 0) {
                            isExecuting.OnNext(false);
                        }
                    })
                };

                var disp = executeAsync(parameter)
                    .ObserveOn(scheduler)
                    .Do(
                        _ => { },
                        e => decrement.Disposable = Disposable.Empty,
                        () => decrement.Disposable = Disposable.Empty)
                    .Do(executeResults.OnNext, exceptions.OnNext)
                    .Subscribe(subj);

                return new CompositeDisposable(disp, decrement);
            });

            return ret.Publish().RefCount();
        }


        /// <summary>
        /// Executes a Command and returns the result as a Task. This method
        /// makes it *much* easier to test ReactiveCommand, as well as create
        /// ReactiveCommands who invoke inferior commands and wait on their results.
        /// </summary>
        /// <returns>A Task representing a single invocation of the Command.</returns>
        /// <param name="parameter">Don't use this.</param>
        /// <param name="ct">An optional token that can cancel the operation, if
        /// the operation supports it.</param>
        public Task<T> ExecuteAsyncTask(object parameter = null, CancellationToken ct = default(CancellationToken))
        {
            return ExecuteAsync(parameter).ToTask(ct);
        }

        /// <summary>
        /// Fires whenever an exception would normally terminate ReactiveUI
        /// internal state.
        /// </summary>
        /// <value>The thrown exceptions.</value>
        public IObservable<Exception> ThrownExceptions { get; protected set; }

        /// <summary>
        /// Returns a BehaviorSubject (i.e. an Observable which is guaranteed to
        /// return at least one value immediately) representing the CanExecute
        /// state.
        /// </summary>
        public IObservable<bool> CanExecuteObservable {
            get {
                var ret = canExecute.StartWith(canExecuteLatest).DistinctUntilChanged();

                if (canExecuteDisp != null) return ret;

                return Observable.Create<bool>(subj => {
                    var disp = ret.Subscribe(subj);

                    // NB: We intentionally leak the CanExecute disconnect, it's
                    // cleaned up by the global Dispose. This is kind of a
                    // "Lazy Subscription" to CanExecute by the command itself.
                    canExecuteDisp = canExecute.Connect();
                    return disp;
                });
            }
        }

        public IObservable<bool> IsExecuting {
            get { return isExecuting.StartWith(inflightCount > 0); }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return executeResults.Subscribe(observer);
        }

        public bool CanExecute(object parameter)
        {
            if (canExecuteDisp == null) canExecuteDisp = canExecute.Connect();
            return canExecuteLatest;
        }

        /// <summary>
        /// Executes a Command. Note that the command will be executed, irrespective of the current value
        /// of the command's canExecute observable.
        /// </summary>
        public void Execute(object parameter)
        {
            ExecuteAsync(parameter).Catch(Observable<T>.Empty).Subscribe();
        }

        public virtual void Dispose()
        {
            var disp = Interlocked.Exchange(ref canExecuteDisp, null);
            if (disp != null) disp.Dispose();
        }
    }

    [Obsolete("This type is obsolete and will be removed in a future version of ReactiveUI. Please switch to using ReactiveUI.ReactiveCommand instead.")]
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
        public static ReactiveCommand<object> ToCommand(this IObservable<bool> This, IScheduler scheduler = null)
        {
            return ReactiveCommand.Create(This, scheduler);
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
            return This.Throttle(x => Observable.FromEventPattern(h => command.CanExecuteChanged += h, h => command.CanExecuteChanged -= h)
                    .Select(_ => Unit.Default)
                    .StartWith(Unit.Default)
                    .Where(_ => command.CanExecute(x)))
                .Subscribe(x => {
                    command.Execute(x);
                });
        }

        /// <summary>
        /// A utility method that will pipe an Observable to an ICommand (i.e.
        /// it will first call its CanExecute with the provided value, then if
        /// the command can be executed, Execute() will be called)
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <returns>An object that when disposes, disconnects the Observable
        /// from the command.</returns>
        public static IDisposable InvokeCommand<T, TResult>(this IObservable<T> This, IReactiveCommand<TResult> command)
        {
            return This.Throttle(x => command.CanExecuteObservable.StartWith(command.CanExecute(x)).Where(b => b))
		.Select(x => command.ExecuteAsync(x).Catch(Observable<TResult>.Empty))
                .Switch()
                .Subscribe();
        }

        /// <summary>
        /// A utility method that will pipe an Observable to an ICommand (i.e.
        /// it will first call its CanExecute with the provided value, then if
        /// the command can be executed, Execute() will be called)
        /// </summary>
        /// <param name="target">The root object which has the Command.</param>
        /// <param name="commandProperty">The expression to reference the Command.</param>
        /// <returns>An object that when disposes, disconnects the Observable
        /// from the command.</returns>
        public static IDisposable InvokeCommand<T, TTarget>(this IObservable<T> This, TTarget target, Expression<Func<TTarget, ICommand>> commandProperty)
        {
            return This.CombineLatest(target.WhenAnyValue(commandProperty), (val, cmd) => new { val, cmd })
                .Throttle(x => Observable.FromEventPattern(h => x.cmd.CanExecuteChanged += h, h => x.cmd.CanExecuteChanged -= h)
                    .Select(_ => Unit.Default)
                    .StartWith(Unit.Default)
                    .Where(_ => x.cmd.CanExecute(x.val)))
                .Subscribe(x => {
                    x.cmd.Execute(x.val);
                });
        }

        /// <summary>
        /// A utility method that will pipe an Observable to an ICommand (i.e.
        /// it will first call its CanExecute with the provided value, then if
        /// the command can be executed, Execute() will be called)
        /// </summary>
        /// <param name="target">The root object which has the Command.</param>
        /// <param name="commandProperty">The expression to reference the Command.</param>
        /// <returns>An object that when disposes, disconnects the Observable
        /// from the command.</returns>
        public static IDisposable InvokeCommand<T, TResult, TTarget>(this IObservable<T> This, TTarget target, Expression<Func<TTarget, IReactiveCommand<TResult>>> commandProperty)
        {
            return This.CombineLatest(target.WhenAnyValue(commandProperty), (val, cmd) => new { val, cmd })
                .Throttle(x => x.cmd.CanExecuteObservable.StartWith(x.cmd.CanExecute(x.val)).Where(b => b))
		.Select(x => x.cmd.ExecuteAsync(x.val).Catch(Observable<TResult>.Empty))
                .Switch()
                .Subscribe();
        }

        /// <summary>
        /// A convenience method for subscribing and creating ReactiveCommands
        /// in the same call. Equivalent to Subscribing to the command, except
        /// there's no way to release your Subscription but that's probably fine.
        /// </summary>
        public static ReactiveCommand<T> OnExecuteCompleted<T>(this ReactiveCommand<T> This, Action<T> onNext, Action<Exception> onError = null)
        {
            if (onError != null) {
                This.Subscribe(onNext, onError);
                return This;
            } else {
                This.Subscribe(onNext);
                return This;
            }
        }
    }}
