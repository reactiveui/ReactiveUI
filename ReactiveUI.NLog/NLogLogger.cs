using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NLog;

namespace ReactiveUI.NLog
{
    public class NLogLogger : IRxUIFullLogger
    {
        readonly Logger _inner;

        public NLogLogger(global::NLog.Logger inner)
        {
            _inner = inner;
        }

        global::NLog.LogLevel rxUIToNLogLevel(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Debug) return global::NLog.LogLevel.Debug;
            if (logLevel == LogLevel.Info) return global::NLog.LogLevel.Info;
            if (logLevel == LogLevel.Warn) return global::NLog.LogLevel.Warn;
            if (logLevel == LogLevel.Error) return global::NLog.LogLevel.Error;
            if (logLevel == LogLevel.Fatal) return global::NLog.LogLevel.Fatal;
            throw new Exception("LogLevel not defined");
        }

        public void Write(string message, LogLevel logLevel)
        {
            _inner.Log(rxUIToNLogLevel(logLevel), message);
        }

        public LogLevel Level { get; set; }

        public void Debug<T>(T value)
        {
            _inner.Debug(value);
        }

        public void Debug<T>(IFormatProvider formatProvider, T value)
        {
            _inner.Debug(formatProvider, value);
        }

        public void DebugException(string message, Exception exception)
        {
            _inner.DebugException(message, exception);
        }

        MethodInfo debugFp = null;
        public void Debug(IFormatProvider formatProvider, string message, params object[] args)
        {
            if (debugFp == null) {
                debugFp = _inner.GetType().GetMethod("Debug", new[] {typeof (IFormatProvider), typeof (string), typeof (object[])});
            }

            var innerArgs = new object[args.Length + 2];
            innerArgs[0] = formatProvider; innerArgs[1] = message;
            Array.Copy(args, 0, innerArgs, 2, args.Length);
            debugFp.Invoke(_inner, innerArgs);
        }

        public void Debug(string message)
        {
            _inner.Debug(message);
        }

        MethodInfo debugNoFp = null;
        public void Debug(string message, params object[] args)
        {
             if (debugNoFp == null) {
                debugNoFp = _inner.GetType().GetMethod("Debug", new[] {typeof (string), typeof (object[])});
            }

            var innerArgs = new object[args.Length + 1];
            innerArgs[0] = message;
            Array.Copy(args, 0, innerArgs, 1, args.Length);
            debugNoFp.Invoke(_inner, innerArgs);           
        }

        public void Debug<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
        {
            _inner.Debug(formatProvider, message, argument);
        }

        public void Debug<TArgument>(string message, TArgument argument)
        {
            _inner.Debug(message, argument);
        }

        public void Debug<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1,
            TArgument2 argument2)
        {
            _inner.Debug(formatProvider, message, argument1, argument2);
        }

        public void Debug<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Debug(message, argument1, argument2);
        }

        public void Debug<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message,
            TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Debug(formatProvider, message, argument1, argument2, argument3);
        }

        public void Debug<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2,
            TArgument3 argument3)
        {
            _inner.Debug(message, argument1, argument2, argument3);
        }



        public void Info<T>(T value)
        {
            _inner.Info(value);
        }

        public void Info<T>(IFormatProvider formatProvider, T value)
        {
            _inner.Info(formatProvider, value);
        }

        public void InfoException(string message, Exception exception)
        {
            _inner.InfoException(message, exception);
        }

        MethodInfo infoFp = null;
        public void Info(IFormatProvider formatProvider, string message, params object[] args)
        {
            if (infoFp == null) {
                infoFp = _inner.GetType().GetMethod("Info", new[] {typeof (IFormatProvider), typeof (string), typeof (object[])});
            }

            var innerArgs = new object[args.Length + 2];
            innerArgs[0] = formatProvider; innerArgs[1] = message;
            Array.Copy(args, 0, innerArgs, 2, args.Length);
            infoFp.Invoke(_inner, innerArgs);
        }

        public void Info(string message)
        {
            _inner.Info(message);
        }

        MethodInfo infoNoFp = null;
        public void Info(string message, params object[] args)
        {
             if (infoNoFp == null) {
                infoNoFp = _inner.GetType().GetMethod("Info", new[] {typeof (string), typeof (object[])});
            }

            var innerArgs = new object[args.Length + 1];
            innerArgs[0] = message;
            Array.Copy(args, 0, innerArgs, 1, args.Length);
            infoNoFp.Invoke(_inner, innerArgs);           
        }

        public void Info<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
        {
            _inner.Info(formatProvider, message, argument);
        }

        public void Info<TArgument>(string message, TArgument argument)
        {
            _inner.Info(message, argument);
        }

        public void Info<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1,
            TArgument2 argument2)
        {
            _inner.Info(formatProvider, message, argument1, argument2);
        }

        public void Info<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Info(message, argument1, argument2);
        }

        public void Info<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message,
            TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Info(formatProvider, message, argument1, argument2, argument3);
        }

        public void Info<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2,
            TArgument3 argument3)
        {
            _inner.Info(message, argument1, argument2, argument3);
        }



        public void Warn<T>(T value)
        {
            _inner.Warn(value);
        }

        public void Warn<T>(IFormatProvider formatProvider, T value)
        {
            _inner.Warn(formatProvider, value);
        }

        public void WarnException(string message, Exception exception)
        {
            _inner.WarnException(message, exception);
        }

        MethodInfo warnFp = null;
        public void Warn(IFormatProvider formatProvider, string message, params object[] args)
        {
            if (warnFp == null) {
                warnFp = _inner.GetType().GetMethod("Warn", new[] {typeof (IFormatProvider), typeof (string), typeof (object[])});
            }

            var innerArgs = new object[args.Length + 2];
            innerArgs[0] = formatProvider; innerArgs[1] = message;
            Array.Copy(args, 0, innerArgs, 2, args.Length);
            warnFp.Invoke(_inner, innerArgs);
        }

        public void Warn(string message)
        {
            _inner.Warn(message);
        }

        MethodInfo warnNoFp = null;
        public void Warn(string message, params object[] args)
        {
             if (warnNoFp == null) {
                warnNoFp = _inner.GetType().GetMethod("Warn", new[] {typeof (string), typeof (object[])});
            }

            var innerArgs = new object[args.Length + 1];
            innerArgs[0] = message;
            Array.Copy(args, 0, innerArgs, 1, args.Length);
            warnNoFp.Invoke(_inner, innerArgs);           
        }

        public void Warn<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
        {
            _inner.Warn(formatProvider, message, argument);
        }

        public void Warn<TArgument>(string message, TArgument argument)
        {
            _inner.Warn(message, argument);
        }

        public void Warn<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1,
            TArgument2 argument2)
        {
            _inner.Warn(formatProvider, message, argument1, argument2);
        }

        public void Warn<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Warn(message, argument1, argument2);
        }

        public void Warn<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message,
            TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Warn(formatProvider, message, argument1, argument2, argument3);
        }

        public void Warn<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2,
            TArgument3 argument3)
        {
            _inner.Warn(message, argument1, argument2, argument3);
        }


        public void Error<T>(T value)
        {
            _inner.Error(value);
        }

        public void Error<T>(IFormatProvider formatProvider, T value)
        {
            _inner.Error(formatProvider, value);
        }

        public void ErrorException(string message, Exception exception)
        {
            _inner.ErrorException(message, exception);
        }

        MethodInfo errorFp = null;
        public void Error(IFormatProvider formatProvider, string message, params object[] args)
        {
            if (errorFp == null) {
                errorFp = _inner.GetType().GetMethod("Error", new[] {typeof (IFormatProvider), typeof (string), typeof (object[])});
            }

            var innerArgs = new object[args.Length + 2];
            innerArgs[0] = formatProvider; innerArgs[1] = message;
            Array.Copy(args, 0, innerArgs, 2, args.Length);
            errorFp.Invoke(_inner, innerArgs);
        }

        public void Error(string message)
        {
            _inner.Error(message);
        }

        MethodInfo errorNoFp = null;
        public void Error(string message, params object[] args)
        {
             if (errorNoFp == null) {
                errorNoFp = _inner.GetType().GetMethod("Error", new[] {typeof (string), typeof (object[])});
            }

            var innerArgs = new object[args.Length + 1];
            innerArgs[0] = message;
            Array.Copy(args, 0, innerArgs, 1, args.Length);
            errorNoFp.Invoke(_inner, innerArgs);           
        }

        public void Error<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
        {
            _inner.Error(formatProvider, message, argument);
        }

        public void Error<TArgument>(string message, TArgument argument)
        {
            _inner.Error(message, argument);
        }

        public void Error<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1,
            TArgument2 argument2)
        {
            _inner.Error(formatProvider, message, argument1, argument2);
        }

        public void Error<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Error(message, argument1, argument2);
        }

        public void Error<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message,
            TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Error(formatProvider, message, argument1, argument2, argument3);
        }

        public void Error<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2,
            TArgument3 argument3)
        {
            _inner.Error(message, argument1, argument2, argument3);
        }



        public void Fatal<T>(T value)
        {
            _inner.Fatal(value);
        }

        public void Fatal<T>(IFormatProvider formatProvider, T value)
        {
            _inner.Fatal(formatProvider, value);
        }

        public void FatalException(string message, Exception exception)
        {
            _inner.FatalException(message, exception);
        }

        MethodInfo fatalFp = null;
        public void Fatal(IFormatProvider formatProvider, string message, params object[] args)
        {
            if (fatalFp == null) {
                fatalFp = _inner.GetType().GetMethod("Fatal", new[] {typeof (IFormatProvider), typeof (string), typeof (object[])});
            }

            var innerArgs = new object[args.Length + 2];
            innerArgs[0] = formatProvider; innerArgs[1] = message;
            Array.Copy(args, 0, innerArgs, 2, args.Length);
            fatalFp.Invoke(_inner, innerArgs);
        }

        public void Fatal(string message)
        {
            _inner.Fatal(message);
        }

        MethodInfo fatalNoFp = null;
        public void Fatal(string message, params object[] args)
        {
             if (fatalNoFp == null) {
                fatalNoFp = _inner.GetType().GetMethod("Fatal", new[] {typeof (string), typeof (object[])});
            }

            var innerArgs = new object[args.Length + 1];
            innerArgs[0] = message;
            Array.Copy(args, 0, innerArgs, 1, args.Length);
            fatalNoFp.Invoke(_inner, innerArgs);           
        }

        public void Fatal<TArgument>(IFormatProvider formatProvider, string message, TArgument argument)
        {
            _inner.Fatal(formatProvider, message, argument);
        }

        public void Fatal<TArgument>(string message, TArgument argument)
        {
            _inner.Fatal(message, argument);
        }

        public void Fatal<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1,
            TArgument2 argument2)
        {
            _inner.Fatal(formatProvider, message, argument1, argument2);
        }

        public void Fatal<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2)
        {
            _inner.Fatal(message, argument1, argument2);
        }

        public void Fatal<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message,
            TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            _inner.Fatal(formatProvider, message, argument1, argument2, argument3);
        }

        public void Fatal<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2,
            TArgument3 argument3)
        {
            _inner.Fatal(message, argument1, argument2, argument3);
        }

    }
}
