using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReactiveUI
{
    /// <summary>
    /// Encapsulates a user interaction behind a reactive interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This non-generic base class defines the base behavior for all reactive commands.
    /// </para>
    /// <para>
    /// To create an instance of <c>ReactiveCommand</c>, call one of the static creation methods defined by this class.
    /// You must provide either asynchronous execution logic via a <c>Func</c> that returns an observable (or <see cref="Task"/>),
    /// or synchronous execution logic via an <c>Action</c>. Optionally, you can provide an observable that governs the
    /// availability of the command for execution, as well as a scheduler on which to surface the results of command
    /// execution.
    /// </para>
    /// <para>
    /// The <see cref="CanExecute"/> property provides an observable that can be used to determine whether the command is
    /// eligible for execution. The value of this observable is determined by both the <c>canExecute</c> observable provided
    /// during command creation, and the current execution status of the command. A command that is already executing will
    /// yield <c>false</c> from its <see cref="CanExecute"/> observable regardless of the <c>canExecute</c> observable provided
    /// during command creation.
    /// </para>
    /// <para>
    /// The <see cref="IsExecuting"/> property provides an observable whose value indicates whether the command is currently
    /// executing. This can be a useful means of triggering UI, such as displaying an activity indicator whilst a command is
    /// executing.
    /// </para>
    /// <para>
    /// As discussed above, you are under no obligation to somehow incorporate this into your <c>canExecute</c> observable
    /// because that is taken care of for you. That is, if the value of <c>IsExecuting</c> is <c>true</c>, the value of
    /// <c>CanExecute</c> will be <c>false</c>. However, if the value of <c>CanExecute</c> is <c>false</c>, that does not imply
    /// the value of <c>IsExecuting</c> is <c>true</c>.
    /// </para>
    /// <para>
    /// Any errors in your command's execution logic (including any <c>canExecute</c> observable you choose to provide) will be
    /// surfaced via the <see cref="ThrownExceptions"/> observable. This gives you the opportunity to handle the error before
    /// it triggers a default handler that tears down the application. For example, you might use this as a means of alerting
    /// the user that something has gone wrong executing the command.
    /// </para>
    /// <para>
    /// For the sake of convenience, all <c>ReactiveCommand</c> instances are also implementations of <see cref="ICommand"/>.
    /// This allows you to easily integrate instances of <c>ReactiveCommand</c> into platforms that understands <c>ICommand</c>
    /// natively (such as WPF and UWP).
    /// </para>
    /// </remarks>
    public abstract partial class ReactiveCommand
    {
        /// <summary>
        /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}"/> with synchronous execution logic.
        /// </summary>
        /// <param name="execute">
        /// The action to execute whenever the command is executed.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler that is used to surface the results of command execution.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        public static ReactiveCommand<Unit, Unit> Create(
            Action execute,
            IObservable<bool> canExecute = null,
            IScheduler scheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            return new ReactiveCommand<Unit, Unit>(
                canExecute ?? Observable.Return(true),
                _ =>
                {
                    execute();
                    return Observable.Return(Unit.Default);
                },
                scheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}"/> with synchronous execution logic that returns a value
        /// of type <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="execute">
        /// The function to execute whenever the command is executed.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler that is used to surface the results of command execution.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TResult">
        /// The type of value returned by command executions.
        /// </typeparam>
        public static ReactiveCommand<Unit, TResult> Create<TResult>(
            Func<TResult> execute,
            IObservable<bool> canExecute = null,
            IScheduler scheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            return new ReactiveCommand<Unit, TResult>(
                canExecute ?? Observable.Return(true),
                _ => Observable.Return(execute()),
                scheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with synchronous execution logic that takes a parameter of type <typeparamref name="TParam"/>.
        /// </summary>
        /// <param name="execute">
        /// The action to execute whenever the command is executed.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler that is used to surface the results of command execution.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        public static ReactiveCommand<TParam, Unit> Create<TParam>(
            Action<TParam> execute,
            IObservable<bool> canExecute = null,
            IScheduler scheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            return new ReactiveCommand<TParam, Unit>(
                canExecute ?? Observable.Return(true),
                param =>
                {
                    execute(param);
                    return Observable.Return(Unit.Default);
                },
                scheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with synchronous execution logic that takes a parameter of type <typeparamref name="TParam"/>
        /// and returns a value of type <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="execute">
        /// The function to execute whenever the command is executed.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler that is used to surface the results of command execution.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of value returned by command executions.
        /// </typeparam>
        public static ReactiveCommand<TParam, TResult> Create<TParam, TResult>(
            Func<TParam, TResult> execute,
            IObservable<bool> canExecute = null,
            IScheduler scheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            return new ReactiveCommand<TParam, TResult>(
                canExecute ?? Observable.Return(true),
                param => Observable.Return(execute(param)),
                scheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic.
        /// </summary>
        /// <param name="executeAsync">
        /// Provides an observable representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler that is used to surface the results of command execution.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static ReactiveCommand<Unit, TResult> Create<TResult>(
            Func<IObservable<TResult>> executeAsync,
            IObservable<bool> canExecute = null,
            IScheduler scheduler = null)
        {
            if (executeAsync == null)
            {
                throw new ArgumentNullException("executeAsync");
            }

            return new ReactiveCommand<Unit, TResult>(canExecute ?? Observable.Return(true), _ => executeAsync(), scheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic.
        /// </summary>
        /// <param name="executeAsync">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler that is used to surface the results of command execution.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static ReactiveCommand<Unit, TResult> CreateTask<TResult>(
            Func<Task<TResult>> executeAsync,
            IObservable<bool> canExecute = null,
            IScheduler scheduler = null)
        {
            return Create(
                () => executeAsync().ToObservable(),
                canExecute,
                scheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic that takes a parameter of type <typeparamref name="TParam"/>.
        /// </summary>
        /// <param name="executeAsync">
        /// Provides an observable representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler that is used to surface the results of command execution.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static ReactiveCommand<TParam, TResult> Create<TParam, TResult>(
                Func<TParam, IObservable<TResult>> executeAsync,
                IObservable<bool> canExecute = null,
                IScheduler scheduler = null)
        {
            return new ReactiveCommand<TParam, TResult>(canExecute ?? Observable.Return(true), executeAsync, scheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic that takes a parameter of type <typeparamref name="TParam"/>.
        /// </summary>
        /// <param name="executeAsync">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler that is used to surface the results of command execution.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static ReactiveCommand<TParam, TResult> CreateTask<TParam, TResult>(
                Func<TParam, Task<TResult>> executeAsync,
                IObservable<bool> canExecute = null,
                IScheduler scheduler = null)
        {
            return Create<TParam, TResult>(
                param => executeAsync(param).ToObservable(),
                canExecute,
                scheduler);
        }

        /// <summary>
        /// Creates a <see cref="CombinedReactiveCommand{TParam, TResult}"/> that composes all the provided child commands.
        /// </summary>
        /// <param name="childCommands">
        /// The child commands that the combined command will compose.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution (in addition to the availability specified
        /// by each individual child command).
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler that is used to surface the results of command execution.
        /// </param>
        /// <returns>
        /// The <c>CombinedReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static CombinedReactiveCommand<TParam, TResult> CreateCombined<TParam, TResult>(
                IEnumerable<ReactiveCommandBase<TParam, TResult>> childCommands,
                IObservable<bool> canExecute = null,
                IScheduler scheduler = null)
        {
            return new CombinedReactiveCommand<TParam, TResult>(childCommands, canExecute ?? Observable.Return(true), scheduler ?? RxApp.MainThreadScheduler);
        }
    }

    // non-generic reactive command functionality
    public abstract partial class ReactiveCommand : IDisposable, ICommand
    {
        private EventHandler canExecuteChanged;

        /// <summary>
        /// An observable whose value indicates whether the command can currently execute.
        /// </summary>
        /// <remarks>
        /// The value provided by this observable is governed both by any <c>canExecute</c> observable provided during
        /// command creation, as well as the current execution status of the command. A command that is currently executing
        /// will always yield <c>false</c> from this observable, even if the <c>canExecute</c> pipeline is currently <c>true</c>.
        /// </remarks>
        public abstract IObservable<bool> CanExecute
        {
            get;
        }

        /// <summary>
        /// An observable whose value indicates whether the command is currently executing.
        /// </summary>
        /// <remarks>
        /// This observable can be particularly useful for updating UI, such as showing an activity indicator whilst a command
        /// is executing.
        /// </remarks>
        public abstract IObservable<bool> IsExecuting
        {
            get;
        }

        /// <summary>
        /// An observable that ticks any exceptions in command execution logic.
        /// </summary>
        /// <remarks>
        /// Any exceptions that are not observed via this observable will propagate out and cause the application to be torn
        /// down. Therefore, you will always want to subscribe to this observable if you expect errors could occur (e.g. if
        /// your command execution includes network activity).
        /// </remarks>
        public abstract IObservable<Exception> ThrownExceptions
        {
            get;
        }

        /// <summary>
        /// Disposes of this <c>ReactiveCommand</c>.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        protected abstract void Dispose(bool disposing);

        event EventHandler ICommand.CanExecuteChanged
        {
            add { this.canExecuteChanged += value; }
            remove { this.canExecuteChanged -= value; }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return this.ICommandCanExecute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            this.ICommandExecute(parameter);
        }

        protected abstract bool ICommandCanExecute(object parameter);

        protected abstract void ICommandExecute(object parameter);

        protected void OnCanExecuteChanged()
        {
            var handler = this.canExecuteChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// A base class for generic reactive commands.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class extends <see cref="ReactiveCommand"/> and adds generic type parameters for the parameter values passed
    /// into command execution, and the return values of command execution.
    /// </para>
    /// <para>
    /// Because the result type is known by this class, it can implement <see cref="IObservable{T}"/>. However, the implementation
    /// is defined as abstract, so subclasses must provide it.
    /// </para>
    /// </remarks>
    /// <typeparam name="TParam">
    /// The type of parameter values passed in during command execution.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the values that are the result of command execution.
    /// </typeparam>
    public abstract class ReactiveCommandBase<TParam, TResult> : ReactiveCommand, IObservable<TResult>
    {
        /// <summary>
        /// Subscribes to execution results from this command.
        /// </summary>
        /// <param name="observer">
        /// The observer.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> that, when disposed, will unsubscribe the observer.
        /// </returns>
        public abstract IDisposable Subscribe(IObserver<TResult> observer);

        /// <summary>
        /// Asynchronously executes this command.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Invoking this method will execute the logic encapsulated by the command. If no parameter value is provided,
        /// a default value of type <typeparamref name="TParam"/> will be passed into the execution logic.
        /// </para>
        /// <para>
        /// There is no requirement to subscribe to the returned observable in order to kick start the execution. And
        /// late subscribers are guaranteed to still receive the execution result value if there is one. In those cases
        /// where execution fails, there will be no result value. Instead, the failure will tick through the
        /// <see cref="ThrownExceptions"/> observable.
        /// </para>
        /// </remarks>
        /// <param name="parameter">
        /// The parameter to pass into command execution.
        /// </param>
        /// <returns>
        /// An observable that will tick the single result value if and when it becomes available.
        /// </returns>
        public abstract IObservable<TResult> ExecuteAsync(TParam parameter = default(TParam));

        protected override bool ICommandCanExecute(object parameter)
        {
            return this.CanExecute.First();
        }

        protected override void ICommandExecute(object parameter)
        {
            // ensure that null is coerced to default(TParam) so that commands taking value types will use a sensible default if no parameter is supplied
            if (parameter == null)
            {
                parameter = default(TParam);
            }

            if (!(parameter is TParam))
            {
                throw new InvalidOperationException(
                    String.Format(
                        "Command requires parameters of type {0}, but received parameter of type {1}.",
                        typeof(TParam).FullName,
                        parameter.GetType().FullName));
            }

            this.ExecuteAsync((TParam)parameter);
        }
    }

    /// <summary>
    /// Encapsulates a user interaction behind a reactive interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides the bulk of the actual implementation for reactive commands. You should not create instances
    /// of this class directly, but rather via the static creation methods on the non-generic <see cref="ReactiveCommand"/>
    /// class.
    /// </para>
    /// </remarks>
    /// <typeparam name="TParam">
    /// The type of parameter values passed in during command execution.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the values that are the result of command execution.
    /// </typeparam>
    public class ReactiveCommand<TParam, TResult> : ReactiveCommandBase<TParam, TResult>
    {
        private readonly Func<TParam, IObservable<TResult>> executeAsync;
        private readonly IScheduler scheduler;
        private readonly Subject<ExecutionInfo> executionInfo;
        private readonly ISubject<ExecutionInfo, ExecutionInfo> synchronizedExecutionInfo;
        private readonly IObservable<bool> isExecuting;
        private readonly IObservable<bool> canExecute;
        private readonly IObservable<TResult> results;
        private readonly ScheduledSubject<Exception> exceptions;
        private readonly IDisposable canExecuteSubscription;

        internal protected ReactiveCommand(
            IObservable<bool> canExecute,
            Func<TParam, IObservable<TResult>> executeAsync,
            IScheduler scheduler)
        {
            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            if (executeAsync == null)
            {
                throw new ArgumentNullException("executeAsync");
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }

            this.executeAsync = executeAsync;
            this.scheduler = scheduler;
            this.executionInfo = new Subject<ExecutionInfo>();
            this.synchronizedExecutionInfo = Subject.Synchronize(this.executionInfo, scheduler);
            this.isExecuting = this
                .synchronizedExecutionInfo
                .Select(x => x.Demarcation == ExecutionDemarcation.Begin)
                .StartWith(false)
                .DistinctUntilChanged()
                .Replay(1)
                .RefCount();
            this.canExecute = canExecute
                .Catch<bool, Exception>(
                    ex =>
                    {
                        this.exceptions.OnNext(ex);
                        return Observable.Return(false);
                    })
                .StartWith(true)
                .CombineLatest(this.isExecuting, (canEx, isEx) => canEx && !isEx)
                .DistinctUntilChanged()
                .Replay(1)
                .RefCount();
            this.results = this
                .synchronizedExecutionInfo
                .Where(x => x.Demarcation == ExecutionDemarcation.EndWithResult)
                .Select(x => x.Result);

            this.exceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);

            this
                .canExecute
                .Subscribe(_ => this.OnCanExecuteChanged());

            this.canExecuteSubscription = this.canExecute.Subscribe();
        }

        /// <inheritdoc/>
        public override IObservable<bool> CanExecute
        {
            get { return this.canExecute; }
        }
        
        /// <inheritdoc/>
        public override IObservable<bool> IsExecuting
        {
            get { return this.isExecuting; }
        }

        /// <inheritdoc/>
        public override IObservable<Exception> ThrownExceptions
        {
            get { return this.exceptions; }
        }

        /// <inheritdoc/>
        public override IDisposable Subscribe(IObserver<TResult> observer)
        {
            return results.Subscribe(observer);
        }

        /// <inheritdoc/>
        public override IObservable<TResult> ExecuteAsync(TParam parameter = default(TParam))
        {
            this.synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateBegin());

            return this
                .executeAsync(parameter)
                .Do(result => this.synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateResult(result)))
                .Catch<TResult, Exception>(
                    ex =>
                    {
                        this.synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateFail());
                        exceptions.OnNext(ex);
                        return Observable.Empty<TResult>();
                    })
                .FirstOrDefaultAsync()
                .RunAsync(CancellationToken.None);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.executionInfo.Dispose();
                this.exceptions.Dispose();
                this.canExecuteSubscription.Dispose();
            }
        }

        private enum ExecutionDemarcation
        {
            Begin,
            EndWithResult,
            EndWithException
        }

        private struct ExecutionInfo
        {
            private readonly ExecutionDemarcation demarcation;
            private readonly TResult result;

            private ExecutionInfo(ExecutionDemarcation demarcation, TResult result)
            {
                this.demarcation = demarcation;
                this.result = result;
            }

            public ExecutionDemarcation Demarcation
            {
                get { return this.demarcation; }
            }

            public TResult Result
            {
                get { return this.result; }
            }

            public static ExecutionInfo CreateBegin()
            {
                return new ExecutionInfo(ExecutionDemarcation.Begin, default(TResult));
            }

            public static ExecutionInfo CreateResult(TResult result)
            {
                return new ExecutionInfo(ExecutionDemarcation.EndWithResult, result);
            }

            public static ExecutionInfo CreateFail()
            {
                return new ExecutionInfo(ExecutionDemarcation.EndWithException, default(TResult));
            }
        }
    }

    /// <summary>
    /// Encapsulates a composite user interaction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides the bulk of the actual implementation for combined reactive commands. You should not
    /// create instances of this class directly, but rather via the static creation methods on the non-generic
    /// <see cref="ReactiveCommand"/> class.
    /// </para>
    /// <para>
    /// A <c>CombinedReactiveCommand</c> combines multiple reactive commands into a single command. Executing
    /// the combined command executes all child commands. Since all child commands will receive the same execution
    /// parameter, all child commands must accept a parameter of the same type.
    /// </para>
    /// <para>
    /// In order for the combined command to be executable, all child commands must themselves be executable.
    /// In addition, any <c>canExecute</c> observable passed in during construction must also yield <c>true</c>.
    /// </para>
    /// </remarks>
    /// <typeparam name="TParam">
    /// The type of parameter values passed in during command execution.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the values that are the result of command execution.
    /// </typeparam>
    public class CombinedReactiveCommand<TParam, TResult> : ReactiveCommandBase<TParam, IList<TResult>>
    {
        private readonly ReactiveCommand<TParam, IList<TResult>> innerCommand;
        private readonly ScheduledSubject<Exception> exceptions;
        private readonly IDisposable exceptionsSubscription;

        internal protected CombinedReactiveCommand(
            IEnumerable<ReactiveCommandBase<TParam, TResult>> childCommands,
            IObservable<bool> canExecute,
            IScheduler scheduler)
        {
            if (childCommands == null)
            {
                throw new ArgumentNullException("childCommands");
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }

            var childCommandsArray = childCommands.ToArray();

            if (childCommandsArray.Length == 0)
            {
                throw new ArgumentException("No child commands provided.", "childCommands");
            }
            
            var canChildrenExecute = Observable
                .CombineLatest(childCommandsArray.Select(x => x.CanExecute))
                .Select(x => x.All(y => y));
            var combinedCanExecute = canExecute
                .Catch<bool, Exception>(
                    ex =>
                    {
                        this.exceptions.OnNext(ex);
                        return Observable.Return(false);
                    })
                .StartWith(true)
                .CombineLatest(canChildrenExecute, (ce, cce) => ce && cce)
                .DistinctUntilChanged()
                .Replay(1)
                .RefCount();
            this.exceptionsSubscription= Observable
                .Merge(childCommandsArray.Select(x => x.ThrownExceptions))
                .Subscribe(ex => this.exceptions.OnNext(ex));

            this.innerCommand = new ReactiveCommand<TParam, IList<TResult>>(
                combinedCanExecute,
                param =>
                    Observable
                        .CombineLatest(
                            childCommandsArray
                                .Select(x => x.ExecuteAsync(param))),
                scheduler);

            this.exceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);

            this
                .CanExecute
                .Subscribe(_ => this.OnCanExecuteChanged());
        }

        /// <inheritdoc/>
        public override IObservable<bool> CanExecute
        {
            get { return this.innerCommand.CanExecute; }
        }

        /// <inheritdoc/>
        public override IObservable<bool> IsExecuting
        {
            get { return this.innerCommand.IsExecuting; }
        }

        /// <inheritdoc/>
        public override IObservable<Exception> ThrownExceptions
        {
            get { return this.exceptions; }
        }

        /// <inheritdoc/>
        public override IDisposable Subscribe(IObserver<IList<TResult>> observer)
        {
            return innerCommand.Subscribe(observer);
        }

        /// <inheritdoc/>
        public override IObservable<IList<TResult>> ExecuteAsync(TParam parameter = default(TParam))
        {
            return this.innerCommand.ExecuteAsync(parameter);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.innerCommand.Dispose();
                this.exceptions.Dispose();
                this.exceptionsSubscription.Dispose();
            }
        }
    }
}