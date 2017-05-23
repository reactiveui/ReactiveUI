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
    /// <see cref="Create"/> can be used when your execution logic is synchronous. <see cref="CreateFromObservable"/> and
    /// <see cref="CreateFromTask"/> can be used for asynchronous execution logic. Optionally, you can provide an observable that
    /// governs the availability of the command for execution, as well as a scheduler to which events will be delivered.
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
            if (execute == null) {
                throw new ArgumentNullException("execute");
            }

            return new ReactiveCommand<Unit, Unit>(
                _ => Observable.Create<Unit>(
                    observer => {
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
            if (execute == null) {
                throw new ArgumentNullException("execute");
            }

            return new ReactiveCommand<Unit, TResult>(
                _ => Observable.Create<TResult>(
                    observer => {
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
            if (execute == null) {
                throw new ArgumentNullException("execute");
            }

            return new ReactiveCommand<TParam, Unit>(
                param => Observable.Create<Unit>(
                    observer => {
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
            if (execute == null) {
                throw new ArgumentNullException("execute");
            }

            return new ReactiveCommand<TParam, TResult>(
                param => Observable.Create<TResult>(
                    observer => {
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
            if (execute == null) {
                throw new ArgumentNullException("execute");
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
                () => Observable.StartAsync(ct => execute(ct)),
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
                () => Observable.StartAsync(ct => execute(ct)),
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

            if (handler != null) {
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
        /// Gets an observable that, when subscribed, executes this command.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Invoking this method will return a cold (lazy) observable that, when subscribed, will execute the logic
        /// encapsulated by the command. It is worth restating that the returned observable is lazy. Nothing will
        /// happen if you call <c>Execute</c> and neglect to subscribe (directly or indirectly) to the returned observable.
        /// </para>
        /// <para>
        /// If no parameter value is provided, a default value of type <typeparamref name="TParam"/> will be passed into
        /// the execution logic.
        /// </para>
        /// <para>
        /// Any number of subscribers can subscribe to a given execution observable and the execution logic will only
        /// run once. That is, the result is broadcast to those subscribers.
        /// </para>
        /// <para>
        /// In those cases where execution fails, there will be no result value. Instead, the failure will tick through the
        /// <see cref="ThrownExceptions"/> observable.
        /// </para>
        /// </remarks>
        /// <param name="parameter">
        /// The parameter to pass into command execution.
        /// </param>
        /// <returns>
        /// An observable that will tick the single result value if and when it becomes available.
        /// </returns>
        public abstract IObservable<TResult> Execute(TParam parameter = default(TParam));

        protected override bool ICommandCanExecute(object parameter)
        {
            return this.CanExecute.FirstAsync().Wait();
        }

        protected override void ICommandExecute(object parameter)
        {
            // ensure that null is coerced to default(TParam) so that commands taking value types will use a sensible default if no parameter is supplied
            if (parameter == null) {
                parameter = default(TParam);
            }

            if (parameter != null && !(parameter is TParam)) {
                throw new InvalidOperationException(
                    String.Format(
                        "Command requires parameters of type {0}, but received parameter of type {1}.",
                        typeof(TParam).FullName,
                        parameter.GetType().FullName));
            }

            this
                .Execute((TParam)parameter)
                .Catch(Observable<TResult>.Empty)
                .Subscribe();
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
        private readonly Func<TParam, IObservable<TResult>> execute;
        private readonly IScheduler outputScheduler;
        private readonly Subject<ExecutionInfo> executionInfo;
        private readonly ISubject<ExecutionInfo, ExecutionInfo> synchronizedExecutionInfo;
        private readonly IObservable<bool> isExecuting;
        private readonly IObservable<bool> canExecute;
        private readonly IObservable<TResult> results;
        private readonly ScheduledSubject<Exception> exceptions;
        private readonly IDisposable canExecuteSubscription;

        internal protected ReactiveCommand(
            Func<TParam, IObservable<TResult>> execute,
            IObservable<bool> canExecute,
            IScheduler outputScheduler)
        {
            if (execute == null) {
                throw new ArgumentNullException("execute");
            }

            if (canExecute == null) {
                throw new ArgumentNullException("canExecute");
            }

            if (outputScheduler == null) {
                throw new ArgumentNullException("outputScheduler");
            }

            this.execute = execute;
            this.outputScheduler = outputScheduler;
            this.executionInfo = new Subject<ExecutionInfo>();
            this.synchronizedExecutionInfo = Subject.Synchronize(this.executionInfo, outputScheduler);
            this.isExecuting = this
                .synchronizedExecutionInfo
                .Select(x => x.Demarcation != ExecutionDemarcation.Ended && x.Demarcation != ExecutionDemarcation.EndWithException)
                .StartWith(false)
                .DistinctUntilChanged()
                .Replay(1)
                .RefCount();
            this.canExecute = canExecute
                .Catch<bool, Exception>(ex => {
                    this.exceptions.OnNext(ex);
                    return Observables.False;
                })
                .StartWith(false)
                .CombineLatest(this.isExecuting, (canEx, isEx) => canEx && !isEx)
                .DistinctUntilChanged()
                .Replay(1)
                .RefCount();
            this.results = this
                .synchronizedExecutionInfo
                .Where(x => x.Demarcation == ExecutionDemarcation.Result)
                .Select(x => x.Result);

            this.exceptions = new ScheduledSubject<Exception>(outputScheduler, RxApp.DefaultExceptionHandler);

            this.canExecuteSubscription = this
                .canExecute
                .Subscribe(_ => this.OnCanExecuteChanged());
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
            get { return this.exceptions.AsObservable(); }
        }

        /// <inheritdoc/>
        public override IDisposable Subscribe(IObserver<TResult> observer)
        {
            return results.Subscribe(observer);
        }

        /// <inheritdoc/>
        public override IObservable<TResult> Execute(TParam parameter = default(TParam))
        {
            try {
                return Observable
                    .Defer(
                        () => {
                            this.synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateBegin());
                            return Observable<TResult>.Empty;
                        })
                    .Concat(this.execute(parameter))
                    .Do(
                        result => this.synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateResult(result)),
                        () => this.synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateEnded()))
                    .Catch<TResult, Exception>(
                        ex => {
                            this.synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateFail());
                            exceptions.OnNext(ex);
                            return Observable.Throw<TResult>(ex);
                        })
                    .PublishLast()
                    .RefCount()
                    .ObserveOn(this.outputScheduler);
            } catch (Exception ex) {
                this.exceptions.OnNext(ex);
                return Observable.Throw<TResult>(ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                this.executionInfo.Dispose();
                this.exceptions.Dispose();
                this.canExecuteSubscription.Dispose();
            }
        }

        private enum ExecutionDemarcation
        {
            Begin,
            Result,
            EndWithException,
            Ended
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
                return new ExecutionInfo(ExecutionDemarcation.Result, result);
            }

            public static ExecutionInfo CreateFail()
            {
                return new ExecutionInfo(ExecutionDemarcation.EndWithException, default(TResult));
            }

            public static ExecutionInfo CreateEnded()
            {
                return new ExecutionInfo(ExecutionDemarcation.Ended, default(TResult));
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
            IScheduler outputScheduler)
        {
            if (childCommands == null) {
                throw new ArgumentNullException("childCommands");
            }

            if (canExecute == null) {
                throw new ArgumentNullException("canExecute");
            }

            if (outputScheduler == null) {
                throw new ArgumentNullException("outputScheduler");
            }

            var childCommandsArray = childCommands.ToArray();

            if (childCommandsArray.Length == 0) {
                throw new ArgumentException("No child commands provided.", "childCommands");
            }

            var canChildrenExecute = Observable
                .CombineLatest(childCommandsArray.Select(x => x.CanExecute))
                .Select(x => x.All(y => y));
            var combinedCanExecute = canExecute
                .Catch<bool, Exception>(ex => {
                    this.exceptions.OnNext(ex);
                    return Observables.False;
                })
                .StartWith(false)
                .CombineLatest(canChildrenExecute, (ce, cce) => ce && cce)
                .DistinctUntilChanged()
                .Replay(1)
                .RefCount();
            this.exceptionsSubscription = Observable
                .Merge(childCommandsArray.Select(x => x.ThrownExceptions))
                .Subscribe(ex => this.exceptions.OnNext(ex));

            this.innerCommand = new ReactiveCommand<TParam, IList<TResult>>(
                param =>
                    Observable
                        .CombineLatest(
                            childCommandsArray
                                .Select(x => x.Execute(param))),
                combinedCanExecute,
                outputScheduler);

            // we already handle exceptions on individual child commands above, but the same exception
            // will tick through innerCommand. Therefore, we need to ensure we ignore it or the default
            // handler will execute and the process will be torn down
            this.innerCommand
                .ThrownExceptions
                .Subscribe();

            this.exceptions = new ScheduledSubject<Exception>(outputScheduler, RxApp.DefaultExceptionHandler);

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
        public override IObservable<IList<TResult>> Execute(TParam parameter = default(TParam))
        {
            return this.innerCommand.Execute(parameter);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                this.innerCommand.Dispose();
                this.exceptions.Dispose();
                this.exceptionsSubscription.Dispose();
            }
        }
    }

    public static class ReactiveCommandMixins
    {
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
            var canExecuteChanged = Observable
                .FromEventPattern(h => command.CanExecuteChanged += h, h => command.CanExecuteChanged -= h)
                .Select(_ => Unit.Default)
                .StartWith(Unit.Default);

            return This
                .WithLatestFrom(canExecuteChanged, (value, _) => InvokeCommandInfo.From(command, command.CanExecute(value), value))
                .Where(ii => ii.CanExecute)
                .Do(ii => command.Execute(ii.Value))
                .Subscribe();
        }

        /// <summary>
        /// A utility method that will pipe an Observable to an ICommand (i.e.
        /// it will first call its CanExecute with the provided value, then if
        /// the command can be executed, Execute() will be called)
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <returns>An object that when disposes, disconnects the Observable
        /// from the command.</returns>
        public static IDisposable InvokeCommand<T, TResult>(this IObservable<T> This, ReactiveCommandBase<T, TResult> command)
        {
            return This
                .WithLatestFrom(command.CanExecute, (value, canExecute) => InvokeCommandInfo.From(command, canExecute, value))
                .Where(ii => ii.CanExecute)
                .SelectMany(ii => command.Execute(ii.Value).Catch(Observable<TResult>.Empty))
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
            var command = target.WhenAnyValue(commandProperty);
            var commandCanExecuteChanged = command
                .Select(c => c == null ? Observable<ICommand>.Empty : Observable
                    .FromEventPattern(h => c.CanExecuteChanged += h, h => c.CanExecuteChanged -= h)
                    .Select(_ => c)
                    .StartWith(c))
                .Switch();

            return This
                .WithLatestFrom(commandCanExecuteChanged, (value, cmd) => InvokeCommandInfo.From(cmd, cmd.CanExecute(value), value))
                .Where(ii => ii.CanExecute)
                .Do(ii => ii.Command.Execute(ii.Value))
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
        public static IDisposable InvokeCommand<T, TResult, TTarget>(this IObservable<T> This, TTarget target, Expression<Func<TTarget, ReactiveCommandBase<T, TResult>>> commandProperty)
        {
            var command = target.WhenAnyValue(commandProperty);
            var invocationInfo = command
                .Select(cmd => cmd == null ? Observable<InvokeCommandInfo<ReactiveCommandBase<T, TResult>, T>>.Empty : cmd
                    .CanExecute
                    .Select(canExecute => InvokeCommandInfo.From(cmd, canExecute, default(T))))
                .Switch();

            return This
                .WithLatestFrom(invocationInfo, (value, ii) => ii.WithValue(value))
                .Where(ii => ii.CanExecute)
                .SelectMany(ii => ii.Command.Execute(ii.Value).Catch(Observable<TResult>.Empty))
                .Subscribe();
        }

        private static class InvokeCommandInfo
        {
            public static InvokeCommandInfo<TCommand, TValue> From<TCommand, TValue>(TCommand command, bool canExecute, TValue value) =>
                new InvokeCommandInfo<TCommand, TValue>(command, canExecute, value);
        }

        private struct InvokeCommandInfo<TCommand, TValue>
        {
            private readonly TCommand command;
            private readonly bool canExecute;
            private readonly TValue value;

            public InvokeCommandInfo(TCommand command, bool canExecute)
                : this(command, canExecute, default(TValue))
            {
            }

            public InvokeCommandInfo(TCommand command, bool canExecute, TValue value)
            {
                this.command = command;
                this.canExecute = canExecute;
                this.value = value;
            }

            public TCommand Command => this.command;

            public bool CanExecute => this.canExecute;

            public TValue Value => this.value;

            public InvokeCommandInfo<TCommand, TValue> WithValue(TValue value) =>
                new InvokeCommandInfo<TCommand, TValue>(this.command, this.canExecute, value);
        }
    }
}

// TODO: dump this once we migrate to Rx 3
namespace System.Reactive.Linq
{
    internal static class WithLatestFromExtensions
    {
        public static IObservable<TResult> WithLatestFrom<TLeft, TRight, TResult>(
            this IObservable<TLeft> @this,
            IObservable<TRight> other,
            Func<TLeft, TRight, TResult> resultSelector)
        {
            return @this.Publish(os =>
                other
                    .Select(a => os.Select(b => resultSelector(b, a)))
                    .Switch());
        }
    }
}
