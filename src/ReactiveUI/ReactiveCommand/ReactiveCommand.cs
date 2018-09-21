// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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

namespace ReactiveUI
{
    /// <summary>
    /// Encapsulates a user action behind a reactive interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This non-generic base class defines the base behavior for all reactive commands.
    /// </para>
    /// <para>
    /// Reactive commands encapsulate the behavior of running some execution logic and then surfacing the results on the UI
    /// thread. Importantly, no scheduling is performed against input observables (the <c>canExecute</c> and execution pipelines).
    /// </para>
    /// <para>
    /// To create an instance of <c>ReactiveCommand</c>, call one of the static creation methods defined by this class.
    /// <see cref="Create"/> can be used when your execution logic is synchronous.
    /// <see cref="CreateFromObservable{TResult}(Func{IObservable{TResult}}, IObservable{bool}, IScheduler)"/> and
    /// <see cref="CreateFromTask(Func{Task}, IObservable{bool}, IScheduler)"/> (and overloads) can be used for asynchronous
    /// execution logic. Optionally, you can provide an observable that governs the availability of the command for execution,
    /// as well as a scheduler to which events will be delivered.
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
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        public static ReactiveCommand<Unit, Unit> Create(
            Action execute,
            IObservable<bool> canExecute = null,
            IScheduler outputScheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return new ReactiveCommand<Unit, Unit>(
                _ => Observable.Create<Unit>(
                    observer =>
                    {
                        execute();
                        observer.OnNext(Unit.Default);
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
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
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
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
            IScheduler outputScheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return new ReactiveCommand<Unit, TResult>(
                _ => Observable.Create<TResult>(
                    observer =>
                    {
                        observer.OnNext(execute());
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
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
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
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
            IScheduler outputScheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return new ReactiveCommand<TParam, Unit>(
                param => Observable.Create<Unit>(
                    observer =>
                    {
                        execute(param);
                        observer.OnNext(Unit.Default);
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
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
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
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
            IScheduler outputScheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return new ReactiveCommand<TParam, TResult>(
                param => Observable.Create<TResult>(
                    observer =>
                    {
                        observer.OnNext(execute(param));
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic.
        /// </summary>
        /// <param name="execute">
        /// Provides an observable representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static ReactiveCommand<Unit, TResult> CreateFromObservable<TResult>(
            Func<IObservable<TResult>> execute,
            IObservable<bool> canExecute = null,
            IScheduler outputScheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return new ReactiveCommand<Unit, TResult>(
                _ => execute(),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static ReactiveCommand<Unit, TResult> CreateFromTask<TResult>(
            Func<Task<TResult>> execute,
            IObservable<bool> canExecute = null,
            IScheduler outputScheduler = null)
        {
            return CreateFromObservable(
                () => execute().ToObservable(),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a parameterless, cancellable <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static ReactiveCommand<Unit, TResult> CreateFromTask<TResult>(
            Func<CancellationToken, Task<TResult>> execute,
            IObservable<bool> canExecute = null,
            IScheduler outputScheduler = null)
        {
            return CreateFromObservable(
                () => Observable.StartAsync(execute),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        public static ReactiveCommand<Unit, Unit> CreateFromTask(
            Func<Task> execute,
            IObservable<bool> canExecute = null,
            IScheduler outputScheduler = null)
        {
            return CreateFromObservable(
                () => execute().ToObservable(),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a parameterless, cancellable <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        public static ReactiveCommand<Unit, Unit> CreateFromTask(
            Func<CancellationToken, Task> execute,
            IObservable<bool> canExecute = null,
            IScheduler outputScheduler = null)
        {
            return CreateFromObservable(
                () => Observable.StartAsync(execute),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic that takes a parameter of type <typeparamref name="TParam"/>.
        /// </summary>
        /// <param name="execute">
        /// Provides an observable representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
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
        public static ReactiveCommand<TParam, TResult> CreateFromObservable<TParam, TResult>(
            Func<TParam, IObservable<TResult>> execute,
            IObservable<bool> canExecute = null,
            IScheduler outputScheduler = null)
        {
            return new ReactiveCommand<TParam, TResult>(
                execute,
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic that takes a parameter of type <typeparamref name="TParam"/>.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
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
        public static ReactiveCommand<TParam, TResult> CreateFromTask<TParam, TResult>(
            Func<TParam, Task<TResult>> execute,
            IObservable<bool> canExecute = null,
            IScheduler outputScheduler = null)
        {
            return CreateFromObservable<TParam, TResult>(
                param => execute(param).ToObservable(),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous, cancellable execution logic that takes a parameter of type <typeparamref name="TParam"/>.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
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
        public static ReactiveCommand<TParam, TResult> CreateFromTask<TParam, TResult>(
            Func<TParam, CancellationToken, Task<TResult>> execute,
            IObservable<bool> canExecute = null,
            IScheduler outputScheduler = null)
        {
            return CreateFromObservable<TParam, TResult>(
                param => Observable.StartAsync(ct => execute(param, ct)),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic that takes a parameter of type <typeparamref name="TParam"/>.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        public static ReactiveCommand<TParam, Unit> CreateFromTask<TParam>(
            Func<TParam, Task> execute,
            IObservable<bool> canExecute = null,
            IScheduler outputScheduler = null)
        {
            return CreateFromObservable<TParam, Unit>(
                param => execute(param).ToObservable(),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous, cancellable execution logic that takes a parameter of type <typeparamref name="TParam"/>.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        public static ReactiveCommand<TParam, Unit> CreateFromTask<TParam>(
            Func<TParam, CancellationToken, Task> execute,
            IObservable<bool> canExecute = null,
            IScheduler outputScheduler = null)
        {
            return CreateFromObservable<TParam, Unit>(
                param => Observable.StartAsync(ct => execute(param, ct)),
                canExecute,
                outputScheduler);
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
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
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
            IScheduler outputScheduler = null)
        {
            return new CombinedReactiveCommand<TParam, TResult>(childCommands, canExecute ?? Observables.True, outputScheduler ?? RxApp.MainThreadScheduler);
        }
    }

    /// <summary>
    /// Abstract base class of the ReactiveCommand's. Meant only for interop with the ICommand interface.
    /// </summary>
    public abstract partial class ReactiveCommand : IDisposable, ICommand, IHandleObservableErrors
    {
        private EventHandler _canExecuteChanged;

        /// <inheritdoc/>
        event EventHandler ICommand.CanExecuteChanged
        {
            add => _canExecuteChanged += value;
            remove => _canExecuteChanged -= value;
        }

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

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        bool ICommand.CanExecute(object parameter)
        {
            return ICommandCanExecute(parameter);
        }

        /// <inheritdoc/>
        void ICommand.Execute(object parameter)
        {
            ICommandExecute(parameter);
        }

        /// <summary>
        /// Disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">If its getting called by the Dispose() method.</param>
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Will be called by the methods from the ICommand interface.
        /// This method is called when the Command should evaluate if it can execute.
        /// </summary>
        /// <param name="parameter">The parameter being passed to the ICommand.</param>
        /// <returns>If the command can be executed.</returns>
        protected abstract bool ICommandCanExecute(object parameter);

        /// <summary>
        /// Will be called by the methods from the ICommand interface.
        /// This method is called when the Command should execute.
        /// </summary>
        /// <param name="parameter">The parameter being passed to the ICommand.</param>
        protected abstract void ICommandExecute(object parameter);

        /// <summary>
        /// Will trigger a event when the CanExecute condition has changed.
        /// </summary>
        protected void OnCanExecuteChanged()
        {
            _canExecuteChanged?.Invoke(this, EventArgs.Empty);
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
        private readonly Func<TParam, IObservable<TResult>> _execute;
        private readonly IScheduler _outputScheduler;
        private readonly Subject<ExecutionInfo> _executionInfo;
        private readonly ISubject<ExecutionInfo, ExecutionInfo> _synchronizedExecutionInfo;
        private readonly IObservable<bool> _isExecuting;
        private readonly IObservable<bool> _canExecute;
        private readonly IObservable<TResult> _results;
        private readonly ScheduledSubject<Exception> _exceptions;
        private readonly IDisposable _canExecuteSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCommand{TParam, TResult}"/> class.
        /// </summary>
        /// <param name="execute">The Func to perform when the command is executed.</param>
        /// <param name="canExecute">A observable which has a value if the command can execute.</param>
        /// <param name="outputScheduler">The scheduler where to send output after the main execution.</param>
        /// <exception cref="ArgumentNullException">Thrown if any dependent parameters are null.</exception>
        protected internal ReactiveCommand(
            Func<TParam, IObservable<TResult>> execute,
            IObservable<bool> canExecute,
            IScheduler outputScheduler)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException(nameof(canExecute));
            }

            if (outputScheduler == null)
            {
                throw new ArgumentNullException(nameof(outputScheduler));
            }

            _execute = execute;
            _outputScheduler = outputScheduler;
            _executionInfo = new Subject<ExecutionInfo>();
            _synchronizedExecutionInfo = Subject.Synchronize(_executionInfo, outputScheduler);
            _isExecuting = _synchronizedExecutionInfo
                .Scan(
                    0,
                    (acc, next) =>
                    {
                        if (next.Demarcation == ExecutionDemarcation.Begin)
                        {
                            return acc + 1;
                        }

                        if (next.Demarcation == ExecutionDemarcation.End)
                        {
                            return acc - 1;
                        }

                        return acc;
                    })
                .Select(inFlightCount => inFlightCount > 0)
                .StartWith(false)
                .DistinctUntilChanged()
                .Replay(1)
                .RefCount();
            _canExecute = canExecute
                .Catch<bool, Exception>(ex =>
                {
                    _exceptions.OnNext(ex);
                    return Observables.False;
                })
                .StartWith(false)
                .CombineLatest(_isExecuting, (canEx, isEx) => canEx && !isEx)
                .DistinctUntilChanged()
                .Replay(1)
                .RefCount();
            _results = _synchronizedExecutionInfo
                .Where(x => x.Demarcation == ExecutionDemarcation.Result)
                .Select(x => x.Result);

            _exceptions = new ScheduledSubject<Exception>(outputScheduler, RxApp.DefaultExceptionHandler);

            _canExecuteSubscription = _canExecute
                .Subscribe(_ => OnCanExecuteChanged());
        }

        private enum ExecutionDemarcation
        {
            Begin,
            Result,
            End
        }

        /// <inheritdoc/>
        public override IObservable<bool> CanExecute => _canExecute;

        /// <inheritdoc/>
        public override IObservable<bool> IsExecuting => _isExecuting;

        /// <inheritdoc/>
        public override IObservable<Exception> ThrownExceptions => _exceptions.AsObservable();

        /// <inheritdoc/>
        public override IDisposable Subscribe(IObserver<TResult> observer)
        {
            return _results.Subscribe(observer);
        }

        /// <inheritdoc/>
        public override IObservable<TResult> Execute(TParam parameter = default(TParam))
        {
            try
            {
                return Observable
                    .Defer(
                        () =>
                        {
                            _synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateBegin());
                            return Observable<TResult>.Empty;
                        })
                    .Concat(_execute(parameter))
                    .Do(result => _synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateResult(result)))
                    .Catch<TResult, Exception>(
                        ex =>
                        {
                            _exceptions.OnNext(ex);
                            return Observable.Throw<TResult>(ex);
                        })
                    .Finally(() => _synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateEnd()))
                    .PublishLast()
                    .RefCount()
                    .ObserveOn(_outputScheduler);
            }
            catch (Exception ex)
            {
                _exceptions.OnNext(ex);
                return Observable.Throw<TResult>(ex);
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _executionInfo?.Dispose();
                _exceptions?.Dispose();
                _canExecuteSubscription?.Dispose();
            }
        }

        private struct ExecutionInfo
        {
            private readonly ExecutionDemarcation _demarcation;
            private readonly TResult _result;

            private ExecutionInfo(ExecutionDemarcation demarcation, TResult result)
            {
                _demarcation = demarcation;
                _result = result;
            }

            public ExecutionDemarcation Demarcation => _demarcation;

            public TResult Result => _result;

            public static ExecutionInfo CreateBegin() =>
                new ExecutionInfo(ExecutionDemarcation.Begin, default(TResult));

            public static ExecutionInfo CreateResult(TResult result) =>
                new ExecutionInfo(ExecutionDemarcation.Result, result);

            public static ExecutionInfo CreateEnd() =>
                new ExecutionInfo(ExecutionDemarcation.End, default(TResult));
        }
    }
}
