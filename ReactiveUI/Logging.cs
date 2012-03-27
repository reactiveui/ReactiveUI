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
    /// <summary>
    /// "Implement" this interface in your class to get access to the Log() 
    /// Mixin, which will give you a Logger that includes the class name in the
    /// log.
    /// </summary>
    public interface IEnableLogger { }

    public static class LogHost
    {
        static LogHost()
        {
#if !WINRT
            if (LogManager.Configuration == null)
            {
                var target = new ConsoleTarget() { Layout = "${level:uppercase=true} ${logger}: ${message}${onexception:inner=${newline}${exception:format=tostring}}" };
                var config = new LoggingConfiguration();

                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, target));
                LogManager.Configuration = config;

                LogHost.Default.Info("*** NLog was not configured, setting up a default configuration ***");
            }
#endif
        }


#if WINRT
        public static dynamic Default {
            get { return LogManager.GetLogger("Logger"); }
        }

        public static dynamic Log<T>(this T This) where T : IEnableLogger
        {
            return LogManager.GetLogger(typeof(T).FullName);
        }
#else
        /// <summary>
        /// Use this logger inside miscellaneous static methods where creating
        /// a class-specific logger isn't really worth it.
        /// </summary>
        public static Logger Default {
            get { return LogManager.GetLogger("Logger"); }
        }

        /// <summary>
        /// Call this method to write log entries on behalf of the current 
        /// class.
        /// </summary>
        public static Logger Log<T>(this T This) where T : IEnableLogger
        {
            return LogManager.GetLogger(typeof(T).FullName);
        }
#endif
    }
}

// vim: tw=120 ts=4 sw=4 et :
