using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Reactive.Subjects;

namespace ReactiveUI.Xaml
{
    public enum StockUserErrorIcon {
        Critical,
        Error, 
        Question,
        Warning,
        Notice,
    };

    public interface IRecoveryCommand : IReactiveCommand
    {
        string LocalizedCommandName { get; }
        RecoveryOptionResult RecoveryResult { get; }
    }

    public enum RecoveryOptionResult {
        CancelOperation,         // We should give up, but no longer an error
        RetryOperation,          // Recovery succeeded, try again
        FailOperation,           // Recovery failed or not possible, you should rethrow
    };

    public class UserError : ReactiveObject
    {
        public UserError(
                string localizedDescription,
                string localizedFailureReason = null,
                IEnumerable<IRecoveryCommand> recoveryOptions = null,
                Dictionary<string, object> contextInfo = null,
                Exception innerException = null)
        {
            RecoveryOptions = new ReactiveCollection<IRecoveryCommand>(recoveryOptions ?? Enumerable.Empty<IRecoveryCommand>());

            LocalizedFailureReason = localizedFailureReason;
            Domain = Assembly.GetCallingAssembly().FullName;
            ContextInfo = contextInfo ?? new Dictionary<string, object>();
            UserErrorIcon = StockUserErrorIcon.Warning;
            InnerException = innerException;
            LocalizedDescription = localizedDescription;
        }

        public string Domain { get; protected set; }
        public Dictionary<string, object> ContextInfo { get; protected set; }

        ReactiveCollection<IRecoveryCommand> recoveryOptions;
        public ReactiveCollection<IRecoveryCommand> RecoveryOptions
        {
            get { return recoveryOptions; }
            protected set { this.RaiseAndSetIfChanged(x => x.RecoveryOptions, value); }
        }

        public string LocalizedDescription { get; set; }
        public string LocalizedFailureReason { get; set; }
        public object UserErrorIcon { get; set; }
        public Exception InnerException { get; protected set; }


        //
        // Static API 
        //

        [ThreadStatic] static Func<UserError, IObservable<RecoveryOptionResult>> overriddenRegisteredUserErrorHandlers;
        static readonly List<Func<UserError, IObservable<RecoveryOptionResult>>> registeredUserErrorHandlers = new List<Func<UserError, IObservable<RecoveryOptionResult>>>();

        public static IObservable<RecoveryOptionResult> Throw(string localizedErrorMessage, Exception innerException = null)
        {
            return Throw(new UserError(localizedErrorMessage, innerException: innerException));
        }

        public static IObservable<RecoveryOptionResult> Throw(UserError error)
        {
            var handlers = (overriddenRegisteredUserErrorHandlers != null) ?
                new[] { overriddenRegisteredUserErrorHandlers } :
                registeredUserErrorHandlers.ToArray().Reverse();

            // NB: This is a little complicated - here's the idea: we have a 
            // list of handlers that we're running down *in order*. If we find
            // one that doesn't return null, we're going to return this as an 
            // Observable with one item (the result).
            //
            // If *none* of the handlers are interested in this UserError, we're
            // going to OnError the Observable.
            var ret = handlers.ToObservable()
                .Select(handler => handler(error)).Concat()
                .Concat(Observable.Throw<RecoveryOptionResult>(new UnhandledUserErrorException(error)))
                .Take(1)
                .Multicast(new AsyncSubject<RecoveryOptionResult>());

            ret.Connect();
            return ret;
        }

        public static IDisposable RegisterHandler(Func<UserError, IObservable<RecoveryOptionResult>> errorHandler)
        {
            registeredUserErrorHandlers.Add(errorHandler);

            return Disposable.Create(() => registeredUserErrorHandlers.Remove(errorHandler));
        }

        public static IDisposable RegisterHandler<TException>(Func<TException, IObservable<RecoveryOptionResult>> errorHandler)
            where TException : UserError
        {
            return RegisterHandler(x => {
                if (!(x is TException)) {
                    return null;
                }

                return errorHandler((TException) x);
            });
        }

        public static IDisposable AddRecoveryOption(IRecoveryCommand command, Func<UserError, bool> filter = null)
        {
            return RegisterHandler(x => {
                if (filter != null && !filter(x)) {
                    return null;
                }

                if (!x.RecoveryOptions.Contains(command)) {
                    x.RecoveryOptions.Add(command);
                }

                return Observable.Empty<RecoveryOptionResult>();
            });
        }

        public static IDisposable OverrideHandlersForTesting(Func<UserError, IObservable<RecoveryOptionResult>> errorHandler)
        {
            overriddenRegisteredUserErrorHandlers = errorHandler;
            return Disposable.Create(() => overriddenRegisteredUserErrorHandlers = null);
        }
    }

    public class UnhandledUserErrorException : Exception 
    {
        public UnhandledUserErrorException(UserError error) : base(error.LocalizedDescription, error.InnerException)
        {
            ReportedError = error;
        }

        public UserError ReportedError { get; protected set; }
    }

    public class RecoveryCommand : ReactiveCommand, IRecoveryCommand
    {
        public bool IsDefault { get; set; }
        public bool IsCancel { get; set; }
        public string LocalizedCommandName { get; protected set; }
        public RecoveryOptionResult RecoveryResult { get; set; }

        public RecoveryCommand(string localizedCommandName, Func<object, RecoveryOptionResult> handler = null)
        {
            LocalizedCommandName = localizedCommandName;

            if (handler != null)
            {
                this.Subscribe(x => RecoveryResult = handler(x));
            }
        }

        public static IRecoveryCommand Ok
        {
            get { var ret = new RecoveryCommand("Ok"); ret.Subscribe(_ => ret.RecoveryResult = RecoveryOptionResult.RetryOperation); return ret; }
        }

        public static IRecoveryCommand Cancel
        {
            get { var ret = new RecoveryCommand("Cancel"); ret.Subscribe(_ => ret.RecoveryResult = RecoveryOptionResult.FailOperation); return ret; }
        }

        public static IRecoveryCommand Yes
        {
            get { var ret = new RecoveryCommand("Yes"); ret.Subscribe(_ => ret.RecoveryResult = RecoveryOptionResult.RetryOperation); return ret; }
        }

        public static IRecoveryCommand No
        {
            get { var ret = new RecoveryCommand("No"); ret.Subscribe(_ => ret.RecoveryResult = RecoveryOptionResult.FailOperation); return ret; }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :