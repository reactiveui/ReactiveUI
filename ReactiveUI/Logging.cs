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

    public static class ObservableLoggingMixin
    {
        public static IObservable<T> Log<T, TObj>(this IObservable<T> This, 
            TObj klass, 
            string message = null,
            Func<T, string> stringifier = null)
            where TObj : IEnableLogger
        {
            message = message ?? "";

            if (stringifier != null) {
                return This.Do(
                    x => klass.Log().Info("{0} OnNext: {1}", stringifier(x)),
                    ex => klass.Log().WarnException(message + " " + "OnError", ex),
                    () => klass.Log().Info("{0} OnCompleted", message));
            } else {
                return This.Do(
                    x => klass.Log().Info("{0} OnNext: {1}", x),
                    ex => klass.Log().WarnException(message + " " + "OnError", ex),
                    () => klass.Log().Info("{0} OnCompleted", message));
            }
        }

        public static IObservable<T> LoggedCatch<T, TObj>(this IObservable<T> This, TObj klass, IObservable<T> next = null, string message = null)
            where TObj : IEnableLogger
        {
            next = next ?? Observable.Return(default(T));
            return This.Catch<T, Exception>(ex => {
                klass.Log().WarnException(message ?? "", ex);
                return next;
            });
        }

        public static IObservable<T> LoggedCatch<T, TObj, TException>(this IObservable<T> This, TObj klass, Func<TException, IObservable<T>> next, string message = null)
            where TObj : IEnableLogger
            where TException : Exception
        {
            return This.Catch<T, TException>(ex => {
                klass.Log().WarnException(message ?? "", ex);
                return next(ex);
            });
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
