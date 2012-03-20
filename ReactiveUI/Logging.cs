using System;
using System.Reactive.Linq;
using System.Reflection;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ReactiveUI
{
    public interface IEnableLogger { }

    public static class LogHost
    {
        static LogHost()
        {
            if (LogManager.Configuration == null)
            {
                var target = new ConsoleTarget() { Layout = "${level:uppercase=true} ${logger}: ${message}${onexception:inner=${newline}${exception:format=tostring}}" };
                var config = new LoggingConfiguration();

                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, target));
                LogManager.Configuration = config;

                LogHost.Default.Info("*** NLog was not configured, setting up a default configuration ***");
            }
        }

        public static Logger Default {
            get { return LogManager.GetLogger("Logger"); }
        }

        public static Logger Log<T>(this T This) where T : IEnableLogger
        {
            return LogManager.GetLogger(typeof(T).FullName);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
