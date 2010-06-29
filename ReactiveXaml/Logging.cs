using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Threading;

namespace ReactiveXaml
{
    public interface ILog
    {
        void Debug(object message);
        void Debug(object message, Exception exception);
        void DebugFormat(string format, object arg0);
        void DebugFormat(string format, params object[] args);
        void DebugFormat(IFormatProvider provider, string format, params object[] args);
        void DebugFormat(string format, object arg0, object arg1);
        void DebugFormat(string format, object arg0, object arg1, object arg2);
        void Error(object message);
        void Error(object message, Exception exception);
        void ErrorFormat(string format, object arg0);
        void ErrorFormat(string format, params object[] args);
        void ErrorFormat(IFormatProvider provider, string format, params object[] args);
        void ErrorFormat(string format, object arg0, object arg1);
        void ErrorFormat(string format, object arg0, object arg1, object arg2);
        void Fatal(object message);
        void Fatal(object message, Exception exception);
        void FatalFormat(string format, object arg0);
        void FatalFormat(string format, params object[] args);
        void FatalFormat(IFormatProvider provider, string format, params object[] args);
        void FatalFormat(string format, object arg0, object arg1);
        void FatalFormat(string format, object arg0, object arg1, object arg2);
        void Info(object message);
        void Info(object message, Exception exception);
        void InfoFormat(string format, object arg0);
        void InfoFormat(string format, params object[] args);
        void InfoFormat(IFormatProvider provider, string format, params object[] args);
        void InfoFormat(string format, object arg0, object arg1);
        void InfoFormat(string format, object arg0, object arg1, object arg2);
        void Warn(object message);
        void Warn(object message, Exception exception);
        void WarnFormat(string format, object arg0);
        void WarnFormat(string format, params object[] args);
        void WarnFormat(IFormatProvider provider, string format, params object[] args);
        void WarnFormat(string format, object arg0, object arg1);
        void WarnFormat(string format, object arg0, object arg1, object arg2);
    }

    public interface IEnableLogger { }

    public static class EnableLoggerMixin
    {
        static MemoizingMRUCache<int, ILog> loggerCache = new MemoizingMRUCache<int, ILog>(
            (_, obj) => ReactiveXaml.LoggerFactory(obj.GetType().Name), 50);

        readonly static ILog mruLogger = new NullLogger();

        public static ILog Log(this IEnableLogger obj)
        {
            // Prevent recursive meta-logging
            if (obj is MemoizingMRUCache<int, ILog>)
                return mruLogger;

            lock (loggerCache) {
                return loggerCache.Get(obj.GetHashCode(), obj);
            }
        }
    }

    public static class ObservableLoggerMixin
    {
        class ObservableLog : IEnableLogger { }

        static readonly ObservableLog logname = new ObservableLog();
        public static IObservable<T> DebugObservable<T>(this IObservable<T> obj)
        {
            int hash = obj.GetHashCode();
            return obj.Do(
                x => logname.Log().DebugFormat("0x{0:X} OnNext: {1}", hash, x),
                ex => logname.Log().Debug(String.Format("0x{0:X} OnError", hash), ex),
                () => logname.Log().DebugFormat("0x{0:X} OnCompleted", hash));
        }
    }


    /*
     * Logger Implementations
     */

    public class NullLogger : ILog
    {
        public NullLogger(string _ = null) { }
        public void Debug(object message) { }
        public void Debug(object message, Exception exception) { }
        public void DebugFormat(string format, object arg0) { }
        public void DebugFormat(string format, params object[] args) { }
        public void DebugFormat(IFormatProvider provider, string format, params object[] args) { }
        public void DebugFormat(string format, object arg0, object arg1) { }
        public void DebugFormat(string format, object arg0, object arg1, object arg2) { }
        public void Error(object message) { }
        public void Error(object message, Exception exception) { }
        public void ErrorFormat(string format, object arg0) { }
        public void ErrorFormat(string format, params object[] args) { }
        public void ErrorFormat(IFormatProvider provider, string format, params object[] args) { }
        public void ErrorFormat(string format, object arg0, object arg1) { }
        public void ErrorFormat(string format, object arg0, object arg1, object arg2) { }
        public void Fatal(object message) { }
        public void Fatal(object message, Exception exception) { } 
        public void FatalFormat(string format, object arg0) { }
        public void FatalFormat(string format, params object[] args) { }
        public void FatalFormat(IFormatProvider provider, string format, params object[] args) { }
        public void FatalFormat(string format, object arg0, object arg1) { }
        public void FatalFormat(string format, object arg0, object arg1, object arg2) { }
        public void Info(object message) { }
        public void Info(object message, Exception exception) { }
        public void InfoFormat(string format, object arg0) { }
        public void InfoFormat(string format, params object[] args) { }
        public void InfoFormat(IFormatProvider provider, string format, params object[] args) { }
        public void InfoFormat(string format, object arg0, object arg1) { }
        public void InfoFormat(string format, object arg0, object arg1, object arg2) { }
        public void Warn(object message) { }
        public void Warn(object message, Exception exception) { }
        public void WarnFormat(string format, object arg0) { }
        public void WarnFormat(string format, object arg0, object arg1) { }
        public void WarnFormat(string format, object arg0, object arg1, object arg2) { }
        public void WarnFormat(string format, params object[] args) { }
        public void WarnFormat(IFormatProvider provider, string format, params object[] args) { }
    }

    public abstract class LoggerBase : ILog
    {
        static MethodInfo stringFormat;
        static LoggerBase()
        {
            var param_types = new[] {
                typeof(IFormatProvider),
                typeof(string),
                typeof(object[]),
            };

            stringFormat = (typeof(String)).GetMethod("Format", param_types);
        }

        string prefix;
        public LoggerBase(string prefix = null)
        {
            this.prefix = prefix;
        }

        protected abstract void writeDebug(string message);
        protected abstract void writeWarn(string message);
        protected abstract void writeInfo(string message);
        protected abstract void writeError(string message);
        protected abstract void writeFatal(string message);

        /*
         * Formatting functions
         */

        string getPrefix()
        {
            return String.Format("{0} [{1}] {2}: ", DateTimeOffset.Now.ToString(), Thread.CurrentThread.ManagedThreadId, prefix);
        }

        void write(Action<string> channel, object message)
        {
            channel(getPrefix() + message.ToString());
        }

        void write(Action<string> channel, object message, Exception exception)
        {
            // TODO: We can probably do better
            channel(getPrefix() + String.Format("{0}: {1}\n{2}", message, exception.Message, exception));
        }

        void write(Action<string> channel, string format, object arg0)
        {
            channel(getPrefix() + String.Format(format, arg0));
        }

        void write(Action<string> channel, string format, object arg0, object arg1)
        {
            channel(getPrefix() + String.Format(format, arg0, arg1));
        }

        void write(Action<string> channel, string format, object arg0, object arg1, object arg2)
        {
            channel(getPrefix() + String.Format(format, arg0, arg1, arg2));
        }

        void write(Action<string> channel, string format, object[] args, IFormatProvider provider = null)
        {
            object[] param = new object[args.Length + 2];
            param[0] = provider ?? (IFormatProvider)CultureInfo.InvariantCulture;
            param[1] = format;
            args.CopyTo(param, 2);
            channel(getPrefix() + (string)stringFormat.Invoke(null, param));
        }


        /* 
         * Extremely boring code ahead 
         */

        #region This code is extremely dull

        public void Debug(object message) { write(writeDebug, message); }
        public void Debug(object message, Exception exception) { write(writeDebug, message, exception); }
        public void DebugFormat(string format, params object[] args) { write(writeDebug, format, args); }
        public void DebugFormat(IFormatProvider provider, string format, params object[] args) { write(writeDebug, format, args, provider); }
        public void DebugFormat(string format, object arg0) { write(writeDebug, format, arg0); }
        public void DebugFormat(string format, object arg0, object arg1) { write(writeDebug, format, arg0, arg1); }
        public void DebugFormat(string format, object arg0, object arg1, object arg2) { write(writeDebug, format, arg0, arg1, arg2); }

        public void Warn(object message) { write(writeWarn, message); }
        public void Warn(object message, Exception exception) { write(writeWarn, message, exception); }
        public void WarnFormat(string format, params object[] args) { write(writeWarn, format, args); }
        public void WarnFormat(IFormatProvider provider, string format, params object[] args) { write(writeWarn, format, args, provider); }
        public void WarnFormat(string format, object arg0) { write(writeWarn, format, arg0); }
        public void WarnFormat(string format, object arg0, object arg1) { write(writeWarn, format, arg0, arg1); }
        public void WarnFormat(string format, object arg0, object arg1, object arg2) { write(writeWarn, format, arg0, arg1, arg2); }

        public void Info(object message) { write(writeInfo, message); }
        public void Info(object message, Exception exception) { write(writeInfo, message, exception); }
        public void InfoFormat(string format, params object[] args) { write(writeInfo, format, args); }
        public void InfoFormat(IFormatProvider provider, string format, params object[] args) { write(writeInfo, format, args, provider); }
        public void InfoFormat(string format, object arg0) { write(writeInfo, format, arg0); }
        public void InfoFormat(string format, object arg0, object arg1) { write(writeInfo, format, arg0, arg1); }
        public void InfoFormat(string format, object arg0, object arg1, object arg2) { write(writeInfo, format, arg0, arg1, arg2); }

        public void Error(object message) { write(writeError, message); }
        public void Error(object message, Exception exception) { write(writeError, message, exception); }
        public void ErrorFormat(string format, params object[] args) { write(writeError, format, args); }
        public void ErrorFormat(IFormatProvider provider, string format, params object[] args) { write(writeError, format, args, provider); }
        public void ErrorFormat(string format, object arg0) { write(writeError, format, arg0); }
        public void ErrorFormat(string format, object arg0, object arg1) { write(writeError, format, arg0, arg1); }
        public void ErrorFormat(string format, object arg0, object arg1, object arg2) { write(writeError, format, arg0, arg1, arg2); }

        public void Fatal(object message) { write(writeFatal, message); }
        public void Fatal(object message, Exception exception) { write(writeFatal, message, exception); }
        public void FatalFormat(string format, params object[] args) { write(writeFatal, format, args); }
        public void FatalFormat(IFormatProvider provider, string format, params object[] args) { write(writeFatal, format, args, provider); }
        public void FatalFormat(string format, object arg0) { write(writeFatal, format, arg0); }
        public void FatalFormat(string format, object arg0, object arg1) { write(writeFatal, format, arg0, arg1); }
        public void FatalFormat(string format, object arg0, object arg1, object arg2) { write(writeFatal, format, arg0, arg1, arg2); }

        #endregion
    }

    // N.B. Yes I know StdErrLogger writes to Stdout, but VS's output window 
    // ignores stderr.
    public class StdErrLogger : LoggerBase
    {
        public StdErrLogger(string prefix = null)
            : base(prefix) { }

        protected override void writeDebug(string message)
        {
            Console.WriteLine(message);
        }

        protected override void writeWarn(string message)
        {
            Console.WriteLine("Warn: " + message);
        }

        protected override void writeInfo(string message)
        {
            Console.WriteLine("Info: " + message);
        }

        protected override void writeError(string message)
        {
            Console.WriteLine("ERROR: " + message);
        }

        protected override void writeFatal(string message)
        {
            Console.WriteLine("FATAL ERROR: ******" + message + " ******");
        }
    }
}
