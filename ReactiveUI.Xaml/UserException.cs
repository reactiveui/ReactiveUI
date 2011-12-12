using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ReactiveUI
{
    public enum StockUserErrorIcon {
        Critical,
        Question,
        Warning,
    };

    public interface IRecoveryCommand : ICommand {
        RecoveryOptionResult RecoveryResult { get; }
    }

    class RecoveryOptionEntry {
        public string LocalizedText { get; set; }
        public IRecoveryCommand RecoveryFunc;
    }

    public enum RecoveryOptionResult {
        CancelOperation,         // We should give up, but no longer an error
        RetryOperation,          // Recovery succeeded, try again
        FailOperation,           // Recovery failed or not possible, you should rethrow
    };

    public class UserException : IEnableLogger
    {
        public UserException(
                string LocalizedDescription,
                string[] LocalizedRecoveryOptionNames = null,
                string Domain = null,
                IRecoveryCommand[] RecoveryOptionCommands = null,
                Dictionary<string, object> ContextInfo = null)
        {
            var option_names = (LocalizedRecoveryOptionNames ?? Enumerable.Empty<string>()).ToArray();
            var commands = (RecoveryOptionCommands ?? Enumerable.Empty<IRecoveryCommand>()).ToArray();
            int count = Math.Min(option_names.Length, commands.Length);

            RecoveryOptions = new List<RecoveryOptionEntry>();
            for(int i=0; i < count; i++) {
                RecoveryOptions.Add(new RecoveryOptionEntry() { LocalizedText = option_names[i], RecoveryFunc = commands[i] });
            }

            this.Domain = Domain ?? Assembly.GetCallingAssembly().FullName;
            this.ContextInfo = ContextInfo ?? new Dictionary<string, object>();
            this.UserErrorIcon = StockUserErrorIcon.Warning;
        }

        public string Domain { get; protected set; }
        public Dictionary<string, object> ContextInfo { get; protected set; }

        public string LocalizedDescription { get; set; }
        public string LocalizedFailureReason { get; set; }
        public string LocalizedRecoverySuggestion { get; set; }
        public object UserErrorIcon { get; set; }
        public string FilePathError { get; set; }
        
        List<RecoveryOptionEntry> RecoveryOptions;

        public void AddRecoveryOption(string LocalizedRecoveryOptionName, IRecoveryCommand RecoveryOptionCommand)
        {
            RecoveryOptions.Add(new RecoveryOptionEntry() { LocalizedText = LocalizedRecoveryOptionName, RecoveryFunc = RecoveryOptionCommand });
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
