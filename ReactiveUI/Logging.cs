using System;
using System.Reactive.Linq;
using System.Reflection;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.Contracts;

#if DOTNETISOLDANDSAD
using System.Reactive.Concurrency;
#endif

namespace ReactiveUI
{
    public enum LogLevel {
        Debug = 1,
        Info,
        Warn,
        Error,
        Fatal,
    }

    public interface ILog
    {
        void Debug(object message);
        void Debug(object message, Exception exception);
        void DebugFormat(string format, params object[] args);
        void DebugFormat(IFormatProvider provider, string format, params object[] args);
        void DebugFormat(string format, object arg0);
        void DebugFormat(string format, object arg0, object arg1);
        void DebugFormat(string format, object arg0, object arg1, object arg2);
        void Error(object message);
        void Error(object message, Exception exception);
        void ErrorFormat(string format, params object[] args);
        void ErrorFormat(IFormatProvider provider, string format, params object[] args);
        void ErrorFormat(string format, object arg0);
        void ErrorFormat(string format, object arg0, object arg1);
        void ErrorFormat(string format, object arg0, object arg1, object arg2);
        void Fatal(object message);
        void Fatal(object message, Exception exception);
        void FatalFormat(string format, params object[] args);
        void FatalFormat(IFormatProvider provider, string format, params object[] args);
        void FatalFormat(string format, object arg0);
        void FatalFormat(string format, object arg0, object arg1);
        void FatalFormat(string format, object arg0, object arg1, object arg2);
        void Info(object message);
        void Info(object message, Exception exception);
        void InfoFormat(string format, params object[] args);
        void InfoFormat(IFormatProvider provider, string format, params object[] args);
        void InfoFormat(string format, object arg0);
        void InfoFormat(string format, object arg0, object arg1);
        void InfoFormat(string format, object arg0, object arg1, object arg2);
        void Warn(object message);
        void Warn(object message, Exception exception);
        void WarnFormat(string format, params object[] args);
        void WarnFormat(IFormatProvider provider, string format, params object[] args);
        void WarnFormat(string format, object arg0);
        void WarnFormat(string format, object arg0, object arg1);
        void WarnFormat(string format, object arg0, object arg1, object arg2);

        LogLevel CurrentLogLevel { get; set; }
    }

    /// <summary>
    /// IEnableLogger is a dummy interface - attaching it to any class will give
    /// you access to the Log() method.
    /// </summary>
    public interface IEnableLogger { }

    public static class EnableLoggerMixin
    {
        static MemoizingMRUCache<int, ILog> loggerCache = new MemoizingMRUCache<int, ILog>(
            (_, obj) => { Type t; t = obj.GetType(); return RxApp.LoggerFactory(t.Namespace + "." + t.Name); }, 50);

        readonly static ILog mruLogger = new NullLogger();

        /// <summary>
        /// Log returns the current logger object, which allows the object to
        /// log messages with the type name attached.
        /// </summary>
        /// <returns></returns>
        public static ILog Log(this IEnableLogger This)
        {
            // Prevent recursive meta-logging
            if (This is MemoizingMRUCache<int, ILog>)
                return mruLogger;

            lock (loggerCache) {
                return loggerCache.Get(This.GetHashCode(), This);
            }
        }
    }

    public static class ObservableLoggerMixin
    {
        class ObservableLog : IEnableLogger { }

        static readonly ObservableLog logname = new ObservableLog();
        public static IObservable<T> DebugObservable<T>(this IObservable<T> This, string message = "")
        {
            int hash = This.GetHashCode();
            return This.Do(
                x => logname.Log().InfoFormat("0x{0:X} '{1}' OnNext: {2}", hash, message, x),
                ex => logname.Log().Info(String.Format("0x{0:X} '{1}' OnError", hash, message), ex),
                () => logname.Log().InfoFormat("0x{0:X} '{1}' OnCompleted", hash, message));
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
        public void DebugFormat(string format, params object[] args) { }
        public void DebugFormat(IFormatProvider provider, string format, params object[] args) { }
        public void DebugFormat(string format, object arg0) { }
        public void DebugFormat(string format, object arg0, object arg1) { }
        public void DebugFormat(string format, object arg0, object arg1, object arg2) { }
        public void Error(object message) { }
        public void Error(object message, Exception exception) { }
        public void ErrorFormat(string format, params object[] args) { }
        public void ErrorFormat(IFormatProvider provider, string format, params object[] args) { }
        public void ErrorFormat(string format, object arg0) { }
        public void ErrorFormat(string format, object arg0, object arg1) { }
        public void ErrorFormat(string format, object arg0, object arg1, object arg2) { }
        public void Fatal(object message) { }
        public void Fatal(object message, Exception exception) { } 
        public void FatalFormat(string format, params object[] args) { }
        public void FatalFormat(IFormatProvider provider, string format, params object[] args) { }
        public void FatalFormat(string format, object arg0) { }
        public void FatalFormat(string format, object arg0, object arg1) { }
        public void FatalFormat(string format, object arg0, object arg1, object arg2) { }
        public void Info(object message) { }
        public void Info(object message, Exception exception) { }
        public void InfoFormat(string format, params object[] args) { }
        public void InfoFormat(IFormatProvider provider, string format, params object[] args) { }
        public void InfoFormat(string format, object arg0) { }
        public void InfoFormat(string format, object arg0, object arg1) { }
        public void InfoFormat(string format, object arg0, object arg1, object arg2) { }
        public void Warn(object message) { }
        public void Warn(object message, Exception exception) { }
        public void WarnFormat(string format, params object[] args) { }
        public void WarnFormat(IFormatProvider provider, string format, params object[] args) { }
        public void WarnFormat(string format, object arg0) { }
        public void WarnFormat(string format, object arg0, object arg1) { }
        public void WarnFormat(string format, object arg0, object arg1, object arg2) { }

        public LogLevel CurrentLogLevel { get; set; }
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

        public LogLevel CurrentLogLevel { get; set; }


        /*
         * Formatting functions
         */

        string _prefixBuffer = "";
        DateTime _lastUpdated = DateTime.MinValue;
        readonly TimeSpan _fiftyMilliseconds = TimeSpan.FromMilliseconds(50.0);

        string getPrefix()
        {
            if (DateTime.Now - _lastUpdated < _fiftyMilliseconds) {
                return _prefixBuffer;
            }

            var now = DateTime.Now;
            StringBuilder buffer;

            buffer = new StringBuilder(128);
            buffer.Append(now.ToString());
            buffer.AppendFormat(" [{0}] ", Thread.CurrentThread.ManagedThreadId);
            buffer.Append(prefix);
            buffer.Append(": ");
            _prefixBuffer = buffer.ToString();

            _lastUpdated = now;
            return buffer.ToString();
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
            Contract.Requires(format != null);
            channel(getPrefix() + String.Format(format, arg0));
        }

        void write(Action<string> channel, string format, object arg0, object arg1)
        {
            Contract.Requires(format != null);
            channel(getPrefix() + String.Format(format, arg0, arg1));
        }

        void write(Action<string> channel, string format, object arg0, object arg1, object arg2)
        {
            Contract.Requires(format != null);
            channel(getPrefix() + String.Format(format, arg0, arg1, arg2));
        }

        void write(Action<string> channel, string format, object[] args, IFormatProvider provider = null)
        {
            Contract.Requires(format != null);

            object[] param = new object[3];
            param[0] = provider ?? (IFormatProvider)CultureInfo.InvariantCulture;
            param[1] = format;
            param[2] = args;

            channel(getPrefix() + (string)stringFormat.Invoke(null, param));
        }


        /* 
         * Extremely boring code ahead 
         */

        #region This code is extremely dull

        public void Debug(object message) { write(writeDebug, message); }
        public void Debug(object message, Exception exception) { write(writeDebug, message, exception); }
        public void DebugFormat(string format, params object[] args) { write(writeDebug, format, args, null); }
        public void DebugFormat(IFormatProvider provider, string format, params object[] args) { write(writeDebug, format, args, provider); }
        public void DebugFormat(string format, object arg0) { write(writeDebug, format, arg0); }
        public void DebugFormat(string format, object arg0, object arg1) { write(writeDebug, format, arg0, arg1); }
        public void DebugFormat(string format, object arg0, object arg1, object arg2) { write(writeDebug, format, arg0, arg1, arg2); }
        public void Warn(object message) { write(writeWarn, message); }
        public void Warn(object message, Exception exception) { write(writeWarn, message, exception); }
        public void WarnFormat(string format, params object[] args) { write(writeWarn, format, args, null); }
        public void WarnFormat(IFormatProvider provider, string format, params object[] args) { write(writeWarn, format, args, provider); }
        public void WarnFormat(string format, object arg0) { write(writeWarn, format, arg0); }
        public void WarnFormat(string format, object arg0, object arg1) { write(writeWarn, format, arg0, arg1); }
        public void WarnFormat(string format, object arg0, object arg1, object arg2) { write(writeWarn, format, arg0, arg1, arg2); }
        public void Info(object message) { write(writeInfo, message); }
        public void Info(object message, Exception exception) { write(writeInfo, message, exception); }
        public void InfoFormat(string format, params object[] args) { write(writeInfo, format, args, null); }
        public void InfoFormat(IFormatProvider provider, string format, params object[] args) { write(writeInfo, format, args, provider); }
        public void InfoFormat(string format, object arg0) { write(writeInfo, format, arg0); }
        public void InfoFormat(string format, object arg0, object arg1) { write(writeInfo, format, arg0, arg1); }
        public void InfoFormat(string format, object arg0, object arg1, object arg2) { write(writeInfo, format, arg0, arg1, arg2); }
        public void Error(object message) { write(writeError, message); }
        public void Error(object message, Exception exception) { write(writeError, message, exception); }
        public void ErrorFormat(string format, params object[] args) { write(writeError, format, args, null); }
        public void ErrorFormat(IFormatProvider provider, string format, params object[] args) { write(writeError, format, args, provider); }
        public void ErrorFormat(string format, object arg0) { write(writeError, format, arg0); }
        public void ErrorFormat(string format, object arg0, object arg1) { write(writeError, format, arg0, arg1); }
        public void ErrorFormat(string format, object arg0, object arg1, object arg2) { write(writeError, format, arg0, arg1, arg2); }
        public void Fatal(object message) { write(writeFatal, message); }
        public void Fatal(object message, Exception exception) { write(writeFatal, message, exception); }
        public void FatalFormat(string format, params object[] args) { write(writeFatal, format, args, null); }
        public void FatalFormat(IFormatProvider provider, string format, params object[] args) { write(writeFatal, format, args, provider); }
        public void FatalFormat(string format, object arg0) { write(writeFatal, format, arg0); }
        public void FatalFormat(string format, object arg0, object arg1) { write(writeFatal, format, arg0, arg1); }
        public void FatalFormat(string format, object arg0, object arg1, object arg2) { write(writeFatal, format, arg0, arg1, arg2); }

        #endregion

        protected bool shouldWrite(LogLevel attemptedLogLevel)
        {
            return ((int)attemptedLogLevel >= (int)CurrentLogLevel);
        }
    }

    public class StdErrLogger : LoggerBase
    {
        public StdErrLogger(string prefix = null)
            : base(prefix) { }

        static object gate = 1; 

        protected override void writeDebug(string message)
        {
            if (!shouldWrite(LogLevel.Debug))
                return;

            lock (gate) { Console.WriteLine("Debug: " + message); } 
        }

        protected override void writeWarn(string message)
        {
            if (!shouldWrite(LogLevel.Warn))
                return; 

            lock (gate) { Console.WriteLine("Warn: " + message); }
        }

        protected override void writeInfo(string message)
        {
            if (!shouldWrite(LogLevel.Info))
                return;

            lock (gate) { Console.WriteLine("Info: " + message); }
        }

        protected override void writeError(string message)
        {
            if (!shouldWrite(LogLevel.Error))
                return;

            lock (gate) { Console.WriteLine("ERROR: " + message); }
        }

        protected override void writeFatal(string message)
        {
            lock (gate) { Console.WriteLine("FATAL ERROR: ******" + message + " ******"); }
        }
    }

#if SILVERLIGHT

    internal struct SilverlightSpinlock
    {
        int atomic;

        public void Enter(ref bool isAcquired)
        {
            int id = (int)Thread.CurrentThread.ManagedThreadId;
            while (Interlocked.CompareExchange(ref atomic, id, 0) != 0) { }
            isAcquired = true;
        }

        public void Exit()
        {
            long id = Thread.CurrentThread.ManagedThreadId;
            int thisAtomic = Interlocked.Exchange(ref atomic, 0);
            if (thisAtomic != id) {
                throw new Exception("Thread " + id + " exited a spinlock it didn't own! Owning thread was " + thisAtomic);
            }
        }
    }

#endif
}

// vim: tw=120 ts=4 sw=4 et :
