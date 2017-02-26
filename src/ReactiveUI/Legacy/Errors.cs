using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace ReactiveUI.Legacy
{
    /// <summary>
    /// Describes a stock error icon situation - it is up to the UI to decide how to interpret these icons.
    /// </summary>
    [Obsolete("This type is obsolete and will be removed in a future version of ReactiveUI. Please switch to using user interactions instead.")]
    public enum StockUserErrorIcon
    {
        /// <summary>
        /// The critical Icon
        /// </summary>
        Critical = 0,

        /// <summary>
        /// The error Icon
        /// </summary>
        Error = 1,

        /// <summary>
        /// The question Icon
        /// </summary>
        Question = 2,

        /// <summary>
        /// The warning Icon
        /// </summary>
        Warning = 3,

        /// <summary>
        /// The notice Icon
        /// </summary>
        Notice = 4,
    };

    /// <summary>
    /// A command that represents a recovery from an error. These commands will typically be
    /// displayed as buttons in the error dialog.
    /// </summary>
    [Obsolete("This type is obsolete and will be removed in a future version of ReactiveUI. Please switch to using user interactions instead.")]
    public interface IRecoveryCommand : IReactiveCommand
    {
        /// <summary>
        /// The command name, typically displayed as the button text.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// When the command is invoked and a result is determined, the command should set the
        /// recovery result to indicate the action the throwing code should take.
        /// </summary>
        RecoveryOptionResult? RecoveryResult { get; }
    }

    /// <summary>
    /// RecoveryOptionResult describes to the code throwing the UserError what to do once the error
    /// is resolved.
    /// </summary>
    [Obsolete("This type is obsolete and will be removed in a future version of ReactiveUI. Please switch to using user interactions instead.")]
    public enum RecoveryOptionResult
    {
        /// <summary>
        /// The operation should be cancelled, but it is no longer an error.
        /// </summary>
        CancelOperation = 0,

        /// <summary>
        /// The operation should be retried with the same parameters.
        /// </summary>
        RetryOperation = 1,

        /// <summary>
        /// Recovery failed or not possible, you should rethrow as an Exception.
        /// </summary>
        FailOperation = 2,
    }

    /// <summary>
    /// User Errors are similar to Exceptions, except that they are intended to be displayed to the
    /// user. As such, your error messages should be phrased in a friendly way. When a UserError is
    /// thrown, code higher up in the stack has a chance to resolve the UserError via a user
    /// interaction. /// Code can also add "Recovery Options" which resolve user errors: for example
    /// an "Out of Disk Space" error might have an "Open Explorer" recovery option.
    /// </summary>
    [Obsolete("This type is obsolete and will be removed in a future version of ReactiveUI. Please switch to using user interactions instead.")]
    public class UserError : ReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserError"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="errorCauseOrResolution">The error cause or resolution.</param>
        /// <param name="recoveryOptions">The recovery options.</param>
        /// <param name="contextInfo">The context information.</param>
        /// <param name="innerException">The inner exception.</param>
        public UserError(
                string errorMessage,
                string errorCauseOrResolution = null,
                IEnumerable<IRecoveryCommand> recoveryOptions = null,
                Dictionary<string, object> contextInfo = null,
                Exception innerException = null)
        {
            this.RecoveryOptions = new ReactiveList<IRecoveryCommand>(recoveryOptions ?? Enumerable.Empty<IRecoveryCommand>());

            this.ErrorCauseOrResolution = errorCauseOrResolution;
            this.ContextInfo = contextInfo ?? new Dictionary<string, object>();
            this.UserErrorIcon = StockUserErrorIcon.Warning;
            this.InnerException = innerException;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// A Dictionary that allows UserErrors to contain arbitrary application data.
        /// </summary>
        public Dictionary<string, object> ContextInfo { get; protected set; }

        private ReactiveList<IRecoveryCommand> _RecoveryOptions;

        /// <summary>
        /// The list of available Recovery Options that will be presented to the user to resolve the
        /// issue - these usually correspond to buttons in the dialog.
        /// </summary>
        public ReactiveList<IRecoveryCommand> RecoveryOptions
        {
            get { return this._RecoveryOptions; }
            protected set { this.RaiseAndSetIfChanged(ref this._RecoveryOptions, value); }
        }

        /// <summary>
        /// The "Newspaper Headline" of the message being conveyed to the user. This should be one
        /// line, short, and informative.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Additional optional information to describe what is happening, or the resolution to an
        /// information-only error (i.e. a dialog to tell the user that something has happened)
        /// </summary>
        public string ErrorCauseOrResolution { get; set; }

        /// <summary>
        /// This object is either a custom icon (usually an ImageSource), or it can also be a
        /// StockUserErrorIcon. It can also be an application-defined type that the handlers know to interpret.
        /// </summary>
        public object UserErrorIcon { get; set; }

        /// <summary>
        /// Optionally, The actual Exception that warranted throwing the UserError.
        /// </summary>
        public Exception InnerException { get; protected set; }

        // // Static API

        [ThreadStatic] private static Func<UserError, IObservable<RecoveryOptionResult>> overriddenRegisteredUserErrorHandlers;
        private static readonly List<Func<UserError, IObservable<RecoveryOptionResult>>> registeredUserErrorHandlers = new List<Func<UserError, IObservable<RecoveryOptionResult>>>();

        /// <summary>
        /// Initiate a user interaction (i.e. "Throw the error to the user to deal with") - this
        /// method is the simplest way to prompt the user that an error has occurred.
        /// </summary>
        /// <param name="errorMessage">
        /// The message to show to the user. The upper level handlers registered with RegisterHandler
        /// are ultimately responsible for displaying this information.
        /// </param>
        /// <param name="innerException">
        /// The Exception that was thrown, if relevant - this will *not* ever be shown to the user.
        /// </param>
        /// <returns>
        /// An Observable representing the action the code should attempt to take, if any.
        /// </returns>
        public static IObservable<RecoveryOptionResult> Throw(string errorMessage, Exception innerException = null)
        {
            return Throw(new UserError(errorMessage, innerException: innerException));
        }

        /// <summary>
        /// Initiate a user interaction (i.e. "Throw the error to the user to deal with").
        /// </summary>
        /// <param name="error">
        /// The UserError to show to the user. The upper level handlers registered with
        /// RegisterHandler are ultimately responsible for displaying this information.
        /// </param>
        /// <returns></returns>
        public static IObservable<RecoveryOptionResult> Throw(UserError error)
        {
            var handlers = (overriddenRegisteredUserErrorHandlers != null) ?
                new[] { overriddenRegisteredUserErrorHandlers } :
                registeredUserErrorHandlers.ToArray().Reverse();

            // NB: This is a little complicated - here's the idea: we have a list of handlers that
            // we're running down *in order*. If we find one that doesn't return null, we're going to
            // return this as an Observable with one item (the result). // If *none* of the handlers
            // are interested in this UserError, we're going to OnError the Observable.
            var handler = handlers.Select(x => x(error)).FirstOrDefault(x => x != null) ?? Observable<RecoveryOptionResult>.Empty
                .Concat(Observable.Throw<RecoveryOptionResult>(new UnhandledUserErrorException(error)));

            var ret = handler.Take(1).PublishLast();
            ret.Connect();

            return ret;
        }

        /// <summary>
        /// Register code to handle a UserError. Registered handlers are called in reverse order to
        /// their registration (i.e. the newest handler is called first), and they each have a chance
        /// to handle a UserError. /// If a Handler cannot resolve a UserError, it should return null
        /// instead of an Observable result.
        /// </summary>
        /// <param name="errorHandler">
        /// A method that can handle a UserError, usually by presenting it to the user. If the
        /// handler cannot handle the error, it should return null.
        /// </param>
        /// <returns>An IDisposable which will unregister the handler.</returns>
        public static IDisposable RegisterHandler(Func<UserError, IObservable<RecoveryOptionResult>> errorHandler)
        {
            registeredUserErrorHandlers.Add(errorHandler);

            return Disposable.Create(() => registeredUserErrorHandlers.Remove(errorHandler));
        }

        /// <summary>
        /// Register code to handle a specific type of UserError. Registered handlers are called in
        /// reverse order to their registration (i.e. the newest handler is called first), and they
        /// each have a chance to handle a UserError. /// If a Handler cannot resolve a UserError, it
        /// should return null instead of an Observable result.
        /// </summary>
        /// <param name="errorHandler">
        /// A method that can handle a UserError, usually by presenting it to the user. If the
        /// handler cannot handle the error, it should return null.
        /// </param>
        /// <returns>An IDisposable which will unregister the handler.</returns>
        public static IDisposable RegisterHandler<TException>(Func<TException, IObservable<RecoveryOptionResult>> errorHandler)
            where TException : UserError
        {
            return RegisterHandler(x => {
                if (!(x is TException)) {
                    return null;
                }

                return errorHandler((TException)x);
            });
        }

        /// <summary>
        /// Register code to handle a UserError. Registered handlers are called in reverse order to
        /// their registration (i.e. the newest handler is called first), and they each have a chance
        /// to handle a UserError. /// If a Handler cannot resolve a UserError, it should return null
        /// instead of an Observable result.
        /// </summary>
        /// <param name="errorHandler">
        /// A method that can handle a UserError, usually by presenting it to the user. If the
        /// handler cannot handle the error, it should return null.
        /// </param>
        /// <returns>An IDisposable which will unregister the handler.</returns>
        public static IDisposable RegisterHandler(Func<UserError, Task<RecoveryOptionResult>> errorHandler)
        {
            return RegisterHandler(x => errorHandler(x).ToObservable());
        }

        /// <summary>
        /// Register code to handle a specific type of UserError. Registered handlers are called in
        /// reverse order to their registration (i.e. the newest handler is called first), and they
        /// each have a chance to handle a UserError. /// If a Handler cannot resolve a UserError, it
        /// should return null instead of an Observable result.
        /// </summary>
        /// <param name="errorHandler">
        /// A method that can handle a UserError, usually by presenting it to the user. If the
        /// handler cannot handle the error, it should return null.
        /// </param>
        /// <returns>An IDisposable which will unregister the handler.</returns>
        public static IDisposable RegisterHandler<TException>(Func<TException, Task<RecoveryOptionResult>> errorHandler)
            where TException : UserError
        {
            return RegisterHandler(x => {
                if (!(x is TException)) {
                    return null;
                }

                return errorHandler((TException)x).ToObservable();
            });
        }

        /// <summary>
        /// This method is a convenience wrapper around RegisterHandler that adds the specified
        /// RecoveryCommand to any UserErrors that match its filter.
        /// </summary>
        /// <param name="command">The RecoveryCommand to add.</param>
        /// <param name="filter">
        /// An optional filter to determine which UserErrors to add the command to.
        /// </param>
        /// <returns>An IDisposable which will unregister the handler.</returns>
        public static IDisposable AddRecoveryOption(IRecoveryCommand command, Func<UserError, bool> filter = null)
        {
            return RegisterHandler(x => {
                if (filter != null && !filter(x)) {
                    return null;
                }

                if (!x.RecoveryOptions.Contains(command)) {
                    x.RecoveryOptions.Add(command);
                }

                return Observable<RecoveryOptionResult>.Empty;
            });
        }

        /// <summary>
        /// This method replaces *all* UserError handlers with the specified handler. Use it for
        /// testing code that may throw UserErrors.
        /// </summary>
        /// <param name="errorHandler">The replacement UserError handler.</param>
        /// <returns>An IDisposable which will unregister the test handler.</returns>
        public static IDisposable OverrideHandlersForTesting(Func<UserError, IObservable<RecoveryOptionResult>> errorHandler)
        {
            overriddenRegisteredUserErrorHandlers = errorHandler;
            return Disposable.Create(() => overriddenRegisteredUserErrorHandlers = null);
        }

        /// <summary>
        /// This method replaces *all* UserError handlers with the specified handler. Use it for
        /// testing code that may throw UserErrors.
        /// </summary>
        /// <param name="errorHandler">The replacement UserError handler.</param>
        /// <returns>An IDisposable which will unregister the test handler.</returns>
        public static IDisposable OverrideHandlersForTesting(Func<UserError, RecoveryOptionResult> errorHandler)
        {
            return OverrideHandlersForTesting(x => Observable.Return(errorHandler(x)));
        }
    }

#pragma warning disable 618

    /// <summary>
    /// This Exception will be thrown when a UserError is not handled by any of the registered handlers.
    /// </summary>
    public class UnhandledUserErrorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledUserErrorException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public UnhandledUserErrorException(UserError error) : base(error.ErrorMessage, error.InnerException)
        {
            this.ReportedError = error;
        }

        /// <summary>
        /// Gets or sets the reported error.
        /// </summary>
        /// <value>The reported error.</value>
        public UserError ReportedError { get; protected set; }
    }

#pragma warning restore 618

    /// <summary>
    /// RecoveryCommand is a straightforward implementation of a recovery command - this class
    /// represents a command presented to the user (usually in the form of a button) that will help
    /// resolve or mitigate a UserError.
    /// </summary>
    [Obsolete("This type is obsolete and will be removed in a future version of ReactiveUI. Please switch to using user interactions instead.")]
    public class RecoveryCommand : ReactiveCommand<Unit>, IRecoveryCommand
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value><c>true</c> if this instance is default; otherwise, <c>false</c>.</value>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is cancel.
        /// </summary>
        /// <value><c>true</c> if this instance is cancel; otherwise, <c>false</c>.</value>
        public bool IsCancel { get; set; }

        /// <summary>
        /// The command name, typically displayed as the button text.
        /// </summary>
        public string CommandName { get; protected set; }

        /// <summary>
        /// When the command is invoked and a result is determined, the command should set the
        /// recovery result to indicate the action the throwing code should take.
        /// </summary>
        public RecoveryOptionResult? RecoveryResult { get; set; }

        /// <summary>
        /// Constructs a RecoveryCommand.
        /// </summary>
        /// <param name="commandName">The user-visible name of this Command.</param>
        /// <param name="handler">
        /// A convenience handler - equivalent to Subscribing to the command and setting the RecoveryResult.
        /// </param>
        public RecoveryCommand(string commandName, Func<object, RecoveryOptionResult> handler = null)
            : base(Observables.True, _ => Observables.Unit)
        {
            this.CommandName = commandName;

            if (handler != null) {
                this.Subscribe(x => this.RecoveryResult = handler(x));
            }
        }

        /// <summary>
        /// A default command whose caption is "Ok"
        /// </summary>
        /// <value>RetryOperation</value>
        public static IRecoveryCommand Ok
        {
            get { var ret = new RecoveryCommand("Ok") { IsDefault = true }; ret.Subscribe(_ => ret.RecoveryResult = RecoveryOptionResult.RetryOperation); return ret; }
        }

        /// <summary>
        /// A default command whose caption is "Cancel"
        /// </summary>
        /// <value>FailOperation</value>
        public static IRecoveryCommand Cancel
        {
            get { var ret = new RecoveryCommand("Cancel") { IsCancel = true }; ret.Subscribe(_ => ret.RecoveryResult = RecoveryOptionResult.FailOperation); return ret; }
        }

        /// <summary>
        /// A default command whose caption is "Yes"
        /// </summary>
        /// <value>RetryOperation</value>
        public static IRecoveryCommand Yes
        {
            get { var ret = new RecoveryCommand("Yes") { IsDefault = true }; ret.Subscribe(_ => ret.RecoveryResult = RecoveryOptionResult.RetryOperation); return ret; }
        }

        /// <summary>
        /// A default command whose caption is "No"
        /// </summary>
        /// <value>FailOperation</value>
        public static IRecoveryCommand No
        {
            get { var ret = new RecoveryCommand("No") { IsCancel = true }; ret.Subscribe(_ => ret.RecoveryResult = RecoveryOptionResult.FailOperation); return ret; }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :