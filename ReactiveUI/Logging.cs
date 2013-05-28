using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace ReactiveUI
{
    /*
     * Interfaces
     */

    public enum LogLevel {
        Debug = 1, Info, Warn, Error, Fatal,
    }

    public interface ILogger
    {
        void Write([Localizable(false)] string message, LogLevel logLevel);
        LogLevel Level { get; set; }
    }

    public interface IFullLogger : ILogger
    {
        void Debug<T>(T value);
        void Debug<T>(IFormatProvider formatProvider, T value);
        void DebugException([Localizable(false)] string message, Exception exception);
        void Debug(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);
        void Debug([Localizable(false)] string message);
        void Debug([Localizable(false)] string message, params object[] args);
        void Debug<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument);
        void Debug<TArgument>([Localizable(false)] string message, TArgument argument);
        void Debug<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);
        void Debug<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);
        void Debug<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);
        void Debug<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        void Info<T>(T value);
        void Info<T>(IFormatProvider formatProvider, T value);
        void InfoException([Localizable(false)] string message, Exception exception);
        void Info(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);
        void Info([Localizable(false)] string message);
        void Info([Localizable(false)] string message, params object[] args);
        void Info<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument);
        void Info<TArgument>([Localizable(false)] string message, TArgument argument);
        void Info<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);
        void Info<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);
        void Info<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);
        void Info<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        void Warn<T>(T value);
        void Warn<T>(IFormatProvider formatProvider, T value);
        void WarnException([Localizable(false)] string message, Exception exception);
        void Warn(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);
        void Warn([Localizable(false)] string message);
        void Warn([Localizable(false)] string message, params object[] args);
        void Warn<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument);
        void Warn<TArgument>([Localizable(false)] string message, TArgument argument);
        void Warn<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);
        void Warn<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);
        void Warn<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);
        void Warn<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        void Error<T>(T value);
        void Error<T>(IFormatProvider formatProvider, T value);
        void ErrorException([Localizable(false)] string message, Exception exception);
        void Error(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);
        void Error([Localizable(false)] string message);
        void Error([Localizable(false)] string message, params object[] args);
        void Error<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument);
        void Error<TArgument>([Localizable(false)] string message, TArgument argument);
        void Error<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);
        void Error<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);
        void Error<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);
        void Error<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        void Fatal<T>(T value);
        void Fatal<T>(IFormatProvider formatProvider, T value);
        void FatalException([Localizable(false)] string message, Exception exception);
        void Fatal(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);
        void Fatal([Localizable(false)] string message);
        void Fatal([Localizable(false)] string message, params object[] args);
        void Fatal<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument);
        void Fatal<TArgument>([Localizable(false)] string message, TArgument argument);
        void Fatal<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);
        void Fatal<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);
        void Fatal<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);
        void Fatal<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);
    }

    public interface ILogManager
    {
        IFullLogger GetLogger(Type type);
    }

    class DefaultLogManager : ILogManager
    {
        readonly MemoizingMRUCache<Type, IFullLogger> loggerCache;

        public DefaultLogManager(IDependencyResolver dependencyResolver = null)
        {
            dependencyResolver = dependencyResolver ?? RxApp.DependencyResolver;

            loggerCache = new MemoizingMRUCache<Type, IFullLogger>((type, _) => {
                var ret = dependencyResolver.GetService<ILogger>();
                return new WrappingFullLogger(ret, type);
            }, 30);
        }

        static readonly IFullLogger nullLogger = new WrappingFullLogger(new NullLogger(), typeof(MemoizingMRUCache<Type, IFullLogger>));
        public IFullLogger GetLogger(Type type)
        {
            if (RxApp.suppressLogging) return nullLogger;
            if (type == typeof(MemoizingMRUCache<Type, IFullLogger>)) return nullLogger;

            lock (loggerCache) {
                return loggerCache.Get(type);
            }
        }
    }

    public class FuncLogManager : ILogManager
    {
        readonly Func<Type, IFullLogger> _inner;
        public FuncLogManager(Func<Type, IFullLogger> getLogger)
        {
            _inner = getLogger;
        }

        public IFullLogger GetLogger(Type type)
        {
            return _inner(type);
        }
    }

    public static class LogManagerMixin
    {
        public static IFullLogger GetLogger<T>(this ILogManager This)
        {
            return This.GetLogger(typeof(T));
        }
    }

    public class NullLogger : ILogger
    {
        public void Write(string message, LogLevel logLevel) {}
        public LogLevel Level { get; set; }
    }

    public class DebugLogger : ILogger
    {
        public void Write(string message, LogLevel logLevel)
        {
            if ((int)logLevel < (int)Level) return;
            Debug.WriteLine(message);
        }

        public LogLevel Level { get; set; }
    }


    /*
     * LogHost / Logging Mixin
     */

    /// <summary>
    /// "Implement" this interface in your class to get access to the Log() 
    /// Mixin, which will give you a Logger that includes the class name in the
    /// log.
    /// </summary>
    public interface IEnableLogger { }

    public static class LogHost
    {
        static readonly IFullLogger nullLogger = new WrappingFullLogger(new NullLogger(), typeof(string));

        /// <summary>
        /// Use this logger inside miscellaneous static methods where creating
        /// a class-specific logger isn't really worth it.
        /// </summary>
        public static IFullLogger Default {
            get {
                if (RxApp.suppressLogging) return nullLogger;

                var factory = RxApp.DependencyResolver.GetService<ILogManager>();
                if (factory == null) {
                    throw new Exception("ILogManager is null. This should never happen, your dependency resolver is broken");
                }
                return factory.GetLogger(typeof(LogHost));
            }
        }

        /// <summary>
        /// Call this method to write log entries on behalf of the current 
        /// class.
        /// </summary>
        public static IFullLogger Log<T>(this T This) where T : IEnableLogger
        {
            if (RxApp.suppressLogging) return nullLogger;

            var factory = RxApp.DependencyResolver.GetService<ILogManager>();
            if (factory == null) {
                throw new Exception("ILogManager is null. This should never happen, your dependency resolver is broken");
            }

            return factory.GetLogger<T>();
        }
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
                    x => klass.Log().Info("{0} OnNext: {1}", message, stringifier(x)),
                    ex => klass.Log().WarnException(message + " " + "OnError", ex),
                    () => klass.Log().Info("{0} OnCompleted", message));
            } else {
                return This.Do(
                    x => klass.Log().Info("{0} OnNext: {1}", message, x),
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

    #region Extremely Dull Code Ahead
    internal class WrappingFullLogger : IFullLogger
    {
        readonly ILogger _inner;
        readonly string prefix;
        readonly MethodInfo stringFormat;

        public WrappingFullLogger(ILogger inner, Type callingType)
        {
            _inner = inner;
            prefix = String.Format(CultureInfo.InvariantCulture, "{0}: ", callingType.Name);

            stringFormat = typeof (String).GetMethod("Format", new[] {typeof (IFormatProvider), typeof (string), typeof (object[])});
            Contract.Requires(inner != null);
            Contract.Requires(stringFormat != null);
        }

        public void Debug<T>(T value)
        {
            _inner.Write(prefix + value, LogLevel.Debug);
        }

        public void Debug<T>(IFormatProvider formatProvider, T value)
        {
            _inner.Write(String.Format(formatProvider, "{0}{1}", prefix, value), LogLevel.Debug);
        }

        public void DebugException(string message, Exception exception)
        {
            _inner.Write(String.Format("{0}{1}: {2}", prefix, message, exception), LogLevel.Debug);
        }

        public void Debug(IFormatProvider formatProvider, string message, params object[] args)
        {
            var sfArgs = new object[args.Length + 2];
            sfArgs[0] = formatProvider; sfArgs[1] = message;
            Array.Copy(args, 0, sfArgs, 2, args.Length);
            string result = (string)stringFormat.Invoke(null, sfArgs);

            _inner.Write(prefix + result, LogLevel.Debug);
        }

        public void Debug(string message)
        {
            _inner.Write(prefix + message, LogLevel.Debug);
        }

        public void Debug(string message, params object[] args)
        {
            var sfArgs = new object[args.Length + 2];
            sfArgs[0] = CultureInfo.InvariantCulture; sfArgs[1] = message;
            Array.Copy(args, 0, sfArgs, 2, args.Length);
            string result = (string)stringFormat.Invoke(null, sfArgs);

            _inner.Write(prefix + result, LogLevel.Debug);
        }

        public void Debug<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument), LogLevel.Debug);
        }

        public void Debug<TArgument>(string message, TArgument argument)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument), LogLevel.Debug);
        }

        public void Debug<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument1, argument2), LogLevel.Debug);
        }

        public void Debug<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument1, argument2), LogLevel.Debug);
        }

        public void Debug<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument1, argument2, argument3), LogLevel.Debug);
        }

        public void Debug<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument1, argument2, argument3), LogLevel.Debug);
        }

        public void Info<T>(T value)
        {
            _inner.Write(prefix + value, LogLevel.Info);
        }

        public void Info<T>(IFormatProvider formatProvider, T value)
        {
            _inner.Write(String.Format(formatProvider, "{0}{1}", prefix, value), LogLevel.Info);
        }

        public void InfoException(string message, Exception exception)
        {
            _inner.Write(String.Format("{0}{1}: {2}", prefix, message, exception), LogLevel.Info);
        }

        public void Info(IFormatProvider formatProvider, string message, params object[] args)
        {
            var sfArgs = new object[args.Length + 2];
            sfArgs[0] = formatProvider; sfArgs[1] = message;
            Array.Copy(args, 0, sfArgs, 2, args.Length);
            string result = (string)stringFormat.Invoke(null, sfArgs);

            _inner.Write(prefix + result, LogLevel.Info);
        }

        public void Info(string message)
        {
            _inner.Write(prefix + message, LogLevel.Info);
        }

        public void Info(string message, params object[] args)
        {
            var sfArgs = new object[args.Length + 2];
            sfArgs[0] = CultureInfo.InvariantCulture; sfArgs[1] = message;
            Array.Copy(args, 0, sfArgs, 2, args.Length);
            string result = (string)stringFormat.Invoke(null, sfArgs);

            _inner.Write(prefix + result, LogLevel.Info);
        }

        public void Info<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument), LogLevel.Info);
        }

        public void Info<TArgument>(string message, TArgument argument)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument), LogLevel.Info);
        }

        public void Info<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument1, argument2), LogLevel.Info);
        }

        public void Info<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument1, argument2), LogLevel.Info);
        }

        public void Info<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument1, argument2, argument3), LogLevel.Info);
        }

        public void Info<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument1, argument2, argument3), LogLevel.Info);
        }

        public void Warn<T>(T value)
        {
            _inner.Write(prefix + value, LogLevel.Warn);
        }

        public void Warn<T>(IFormatProvider formatProvider, T value)
        {
            _inner.Write(String.Format(formatProvider, "{0}{1}", prefix, value), LogLevel.Warn);
        }

        public void WarnException(string message, Exception exception)
        {
            _inner.Write(String.Format("{0}{1}: {2}", prefix, message, exception), LogLevel.Warn);
        }

        public void Warn(IFormatProvider formatProvider, string message, params object[] args)
        {
            var sfArgs = new object[args.Length + 2];
            sfArgs[0] = formatProvider; sfArgs[1] = message;
            Array.Copy(args, 0, sfArgs, 2, args.Length);
            string result = (string)stringFormat.Invoke(null, sfArgs);

            _inner.Write(prefix + result, LogLevel.Warn);
        }

        public void Warn(string message)
        {
            _inner.Write(prefix + message, LogLevel.Warn);
        }

        public void Warn(string message, params object[] args)
        {
            var sfArgs = new object[args.Length + 2];
            sfArgs[0] = CultureInfo.InvariantCulture; sfArgs[1] = message;
            Array.Copy(args, 0, sfArgs, 2, args.Length);
            string result = (string)stringFormat.Invoke(null, sfArgs);

            _inner.Write(prefix + result, LogLevel.Warn);
        }

        public void Warn<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument), LogLevel.Warn);
        }

        public void Warn<TArgument>(string message, TArgument argument)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument), LogLevel.Warn);
        }

        public void Warn<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument1, argument2), LogLevel.Warn);
        }

        public void Warn<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument1, argument2), LogLevel.Warn);
        }

        public void Warn<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument1, argument2, argument3), LogLevel.Warn);
        }

        public void Warn<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument1, argument2, argument3), LogLevel.Warn);
        }


        public void Error<T>(T value)
        {
            _inner.Write(prefix + value, LogLevel.Error);
        }

        public void Error<T>(IFormatProvider formatProvider, T value)
        {
            _inner.Write(String.Format(formatProvider, "{0}{1}", prefix, value), LogLevel.Error);
        }

        public void ErrorException(string message, Exception exception)
        {
            _inner.Write(String.Format("{0}{1}: {2}", prefix, message, exception), LogLevel.Error);
        }

        public void Error(IFormatProvider formatProvider, string message, params object[] args)
        {
            var sfArgs = new object[args.Length + 2];
            sfArgs[0] = formatProvider; sfArgs[1] = message;
            Array.Copy(args, 0, sfArgs, 2, args.Length);
            string result = (string)stringFormat.Invoke(null, sfArgs);

            _inner.Write(prefix + result, LogLevel.Error);
        }

        public void Error(string message)
        {
            _inner.Write(prefix + message, LogLevel.Error);
        }

        public void Error(string message, params object[] args)
        {
            var sfArgs = new object[args.Length + 2];
            sfArgs[0] = CultureInfo.InvariantCulture; sfArgs[1] = message;
            Array.Copy(args, 0, sfArgs, 2, args.Length);
            string result = (string)stringFormat.Invoke(null, sfArgs);

            _inner.Write(prefix + result, LogLevel.Error);
        }

        public void Error<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument), LogLevel.Error);
        }

        public void Error<TArgument>(string message, TArgument argument)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument), LogLevel.Error);
        }

        public void Error<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument1, argument2), LogLevel.Error);
        }

        public void Error<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument1, argument2), LogLevel.Error);
        }

        public void Error<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument1, argument2, argument3), LogLevel.Error);
        }

        public void Error<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument1, argument2, argument3), LogLevel.Error);
        }


        public void Fatal<T>(T value)
        {
            _inner.Write(prefix + value, LogLevel.Fatal);
        }

        public void Fatal<T>(IFormatProvider formatProvider, T value)
        {
            _inner.Write(String.Format(formatProvider, "{0}{1}", prefix, value), LogLevel.Fatal);
        }

        public void FatalException(string message, Exception exception)
        {
            _inner.Write(String.Format("{0}{1}: {2}", prefix, message, exception), LogLevel.Fatal);
        }

        public void Fatal(IFormatProvider formatProvider, string message, params object[] args)
        {
            var sfArgs = new object[args.Length + 2];
            sfArgs[0] = formatProvider; sfArgs[1] = message;
            Array.Copy(args, 0, sfArgs, 2, args.Length);
            string result = (string)stringFormat.Invoke(null, sfArgs);

            _inner.Write(prefix + result, LogLevel.Fatal);
        }

        public void Fatal(string message)
        {
            _inner.Write(prefix + message, LogLevel.Fatal);
        }

        public void Fatal(string message, params object[] args)
        {
            var sfArgs = new object[args.Length + 2];
            sfArgs[0] = CultureInfo.InvariantCulture; sfArgs[1] = message;
            Array.Copy(args, 0, sfArgs, 2, args.Length);
            string result = (string)stringFormat.Invoke(null, sfArgs);

            _inner.Write(prefix + result, LogLevel.Fatal);
        }

        public void Fatal<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument), LogLevel.Fatal);
        }

        public void Fatal<TArgument>(string message, TArgument argument)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument), LogLevel.Fatal);
        }

        public void Fatal<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument1, argument2), LogLevel.Fatal);
        }

        public void Fatal<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument1, argument2), LogLevel.Fatal);
        }

        public void Fatal<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Write(prefix + String.Format(formatProvider, message, argument1, argument2, argument3), LogLevel.Fatal);
        }

        public void Fatal<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Write(prefix + String.Format(CultureInfo.InvariantCulture, message, argument1, argument2, argument3), LogLevel.Fatal);
        }

        public void Write([Localizable(false)] string message, LogLevel logLevel)
        {
            _inner.Write(message, logLevel);
        }

        public LogLevel Level {
            get { return _inner.Level; }
            set { _inner.Level = value; }
        }
    }
    #endregion
}

// vim: tw=120 ts=4 sw=4 et :
