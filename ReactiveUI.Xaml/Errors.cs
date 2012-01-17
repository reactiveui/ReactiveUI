using System;
using System.Reactive.Disposables;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ReactiveUI.Xaml
{
    public enum StockUserErrorIcon {
        Critical,
        Question,
        Warning,
    };

    public interface IRecoveryCommand : ICommand
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
                string domain = null,
                IEnumerable<IRecoveryCommand> recoveryOptions = null,
                Dictionary<string, object> contextInfo = null,
                Exception innerException = null)
        {
            RecoveryOptions = new List<IRecoveryCommand>(recoveryOptions ?? Enumerable.Empty<IRecoveryCommand>());

            Domain = domain ?? Assembly.GetCallingAssembly().FullName;
            ContextInfo = contextInfo ?? new Dictionary<string, object>();
            UserErrorIcon = StockUserErrorIcon.Warning;
            InnerException = innerException;
            LocalizedDescription = localizedDescription;
        }

        public string Domain { get; protected set; }
        public Dictionary<string, object> ContextInfo { get; protected set; }

        public List<IRecoveryCommand> RecoveryOptions;

        public string LocalizedDescription { get; set; }
        public string LocalizedFailureReason { get; set; }
        public string LocalizedRecoverySuggestion { get; set; }
        public object UserErrorIcon { get; set; }
        public string FilePathError { get; set; }
        public Exception InnerException { get; protected set; }


        //
        // Static API 
        //

        [ThreadStatic] static Func<UserError, RecoveryOptionResult?> overriddenRegisteredUserErrorHandlers;
        static readonly List<Func<UserError, RecoveryOptionResult?>> registeredUserErrorHandlers = new List<Func<UserError, RecoveryOptionResult?>>();

        public static RecoveryOptionResult Throw(string localizedErrorMessage, Exception innerException)
        {
            return Throw(new UserError(localizedErrorMessage, innerException: innerException));
        }

        public static RecoveryOptionResult Throw(UserError error)
        {
            var handlers = (overriddenRegisteredUserErrorHandlers != null) ?
                new[] { overriddenRegisteredUserErrorHandlers } :
                registeredUserErrorHandlers.ToArray().Reverse();

            foreach(var handler in handlers) {
                var result = handler(error);
                if (result == null) continue;

                if (result.Value == RecoveryOptionResult.FailOperation)
                    throw new UnhandledUserErrorException(error);

                return result.Value;
            }

            throw new UnhandledUserErrorException(error);
        }

        public static IDisposable RegisterHandler(Func<UserError, RecoveryOptionResult?> errorHandler)
        {
            registeredUserErrorHandlers.Add(errorHandler);

            return Disposable.Create(() => registeredUserErrorHandlers.Remove(errorHandler));
        }

        public static IDisposable RegisterHandler<TException>(Func<TException, RecoveryOptionResult?> errorHandler)
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

                return null;
            });
        }

        public static IDisposable OverrideHandlersForTesting(Func<UserError, RecoveryOptionResult?> errorHandler)
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
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :