using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ReactiveXaml
{
    public enum StockUserErrorIcon {
        Critical,
        Question,
        Warning,
    };

    class RecoveryOptionEntry {
        public string LocalizedText { get; set; }
        public ICommand RecoveryFunc;
    }

    public class UserException : IEnableLogger
    {
        public UserException(
                string Domain,
                string LocalizedDescription,
                string[] LocalizedRecoveryOptionNames = null,
                ICommand[] RecoveryOptionCommands = null,
                Dictionary<string, object> ContextInfo = null)
        {
            RecoveryOptions = (LocalizedRecoveryOptionNames ?? Enumerable.Empty<string>())
                .Zip(RecoveryOptionCommands ?? Enumerable.Empty<ICommand>(), 
                    (name, cmd) => new RecoveryOptionEntry() { LocalizedText = name, RecoveryFunc = cmd })
                .ToList();

            this.Domain = Domain;
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

        public void AddRecoveryOption(string LocalizedRecoveryOptionName, ICommand RecoveryOptionCommand)
        {
            RecoveryOptions.Add(new RecoveryOptionEntry() { LocalizedText = LocalizedRecoveryOptionName, RecoveryFunc = RecoveryOptionCommand });
        }
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :