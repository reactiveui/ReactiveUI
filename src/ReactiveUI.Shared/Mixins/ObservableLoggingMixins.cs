// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using ReactiveUI.Primitives.Disposables;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Extension methods to assist with Logging.</summary>
public static class ObservableLoggingMixins
{
    /// <summary>Initializes static members of the <see cref="ObservableLoggingMixins"/> class.</summary>
    static ObservableLoggingMixins() => RxAppBuilder.EnsureInitialized();

    /// <summary>Provides logging extension members for <see cref="IObservable{T}"/>.</summary>
    /// <typeparam name="T">The type of the elements in the source observable sequence.</typeparam>
    /// <param name="this">The source observable sequence whose notifications will be logged.</param>
    extension<T>(IObservable<T> @this)
    {
        /// <summary>Returns an observable sequence that logs each notification using the specified logger object.</summary>
        /// <typeparam name="TObj">The type of the logger object. Must implement IEnableLogger.</typeparam>
        /// <param name="logObject">An object that provides logging capabilities.</param>
        /// <returns>An observable sequence that logs each notification using the provided logger.</returns>
        public IObservable<T> Log<TObj>(
            TObj logObject)
            where TObj : IEnableLogger
            =>
            Log(@this, logObject, null, null);

        /// <summary>Returns an observable sequence that logs each notification using the specified logger object and message.</summary>
        /// <typeparam name="TObj">The type of the logger object. Must implement IEnableLogger.</typeparam>
        /// <param name="logObject">An object that provides logging capabilities.</param>
        /// <param name="message">An optional message to include in each log entry. If null, an empty string is used.</param>
        /// <returns>An observable sequence that logs each notification using the provided logger.</returns>
        public IObservable<T> Log<TObj>(
            TObj logObject,
            string? message)
            where TObj : IEnableLogger
            =>
            Log(@this, logObject, message, null);

        /// <summary>Returns an observable sequence that logs each notification using the specified logger object.</summary>
        /// <remarks>This method does not modify the elements of the sequence or affect its timing, but adds side
        /// effects for logging purposes. Logging occurs for each notification: OnNext (with the element value), OnError,
        /// and OnCompleted. The returned observable can be further composed or subscribed to as usual.</remarks>
        /// <typeparam name="TObj">The type of the logger object. Must implement IEnableLogger.</typeparam>
        /// <param name="logObject">An object that provides logging capabilities.</param>
        /// <param name="message">An optional message to include in each log entry. If null, an empty string is used.</param>
        /// <param name="stringifier">An optional function to convert each element to a string for logging. If null, the element's ToString method is used.</param>
        /// <returns>An observable sequence that is functionally equivalent to the source, but logs each OnNext, OnError, and
        /// OnCompleted notification using the provided logger.</returns>
        public IObservable<T> Log<TObj>(
            TObj logObject,
            string? message,
            Func<T, string>? stringifier)
            where TObj : IEnableLogger
        {
            message ??= string.Empty;

            Action<T> onNext = stringifier is not null
                ? x => logObject.Log().Info(CultureInfo.InvariantCulture, "{0} OnNext: {1}", message, stringifier(x))
                : x => logObject.Log().Info(CultureInfo.InvariantCulture, "{0} OnNext: {1}", message, x);

            return new LoggingTeeObservable<T>(
                @this,
                onNext,
                ex => logObject.Log().Warn(ex, message + " OnError"),
                () => logObject.Log().Info(CultureInfo.InvariantCulture, "{0} OnCompleted", message));
        }

        /// <summary>Returns an observable sequence that logs any exception and continues with a default empty sequence.</summary>
        /// <typeparam name="TObj">The type of the logger, which must implement IEnableLogger.</typeparam>
        /// <param name="class">An object that provides logging capabilities.</param>
        /// <returns>An observable sequence that logs exceptions and continues with an empty sequence.</returns>
        public IObservable<T> LoggedCatch<TObj>(
            TObj @class)
            where TObj : IEnableLogger =>
            LoggedCatch(@this, @class, null, null);

        /// <summary>Returns an observable sequence that logs any exception and continues with the provided fallback sequence.</summary>
        /// <typeparam name="TObj">The type of the logger, which must implement IEnableLogger.</typeparam>
        /// <param name="class">An object that provides logging capabilities.</param>
        /// <param name="next">An observable sequence to continue with after an exception is caught.</param>
        /// <returns>An observable sequence that logs exceptions and continues with the fallback sequence.</returns>
        public IObservable<T> LoggedCatch<TObj>(
            TObj @class,
            IObservable<T>? next)
            where TObj : IEnableLogger =>
            LoggedCatch(@this, @class, next, null);

        /// <summary>
        /// Returns an observable sequence that logs any exception using the specified logger and continues with the
        /// provided fallback sequence, if supplied.
        /// </summary>
        /// <remarks>This method is useful for handling errors in observable sequences by logging exceptions and
        /// optionally providing a fallback sequence to continue processing. The exception is logged at the warning level
        /// using the provided logger.</remarks>
        /// <typeparam name="TObj">The type of the logger, which must implement IEnableLogger.</typeparam>
        /// <param name="class">An object that provides logging capabilities and is used to log any exceptions encountered.</param>
        /// <param name="next">An observable sequence to continue with after an exception is caught. If null, a default empty sequence is used.</param>
        /// <param name="message">An optional message to include in the log entry when an exception is caught. If null, an empty string is used.</param>
        /// <returns>An observable sequence that emits the original elements until an exception occurs, logs the exception, and then
        /// continues with the specified fallback sequence.</returns>
        public IObservable<T> LoggedCatch<TObj>(
            TObj @class,
            IObservable<T>? next,
            string? message)
            where TObj : IEnableLogger
        {
            next ??= new SingleValueObservable<T>(default!);
            return new LoggedCatchObservable<T, Exception>(@this, ex =>
            {
                @class.Log().Warn(ex, message ?? string.Empty);
                return next;
            });
        }

        /// <summary>
        /// Handles exceptions of a specified type in the observable sequence by logging a warning and continuing with an
        /// alternative observable sequence.
        /// </summary>
        /// <typeparam name="TObj">The type of the logger-enabled object used for logging. Must implement IEnableLogger.</typeparam>
        /// <typeparam name="TException">The type of exception to catch and handle. Must derive from Exception.</typeparam>
        /// <param name="class">An object that provides logging capabilities.</param>
        /// <param name="next">A function that returns an alternative observable sequence for the caught exception.</param>
        /// <returns>An observable sequence that continues with the next function after logging the exception.</returns>
        public IObservable<T> LoggedCatch<TObj, TException>(
            TObj @class,
            Func<TException, IObservable<T>> next)
            where TObj : IEnableLogger
            where TException : Exception =>
            LoggedCatch(@this, @class, next, null);

        /// <summary>
        /// Handles exceptions of a specified type in the observable sequence by logging a warning and continuing with an
        /// alternative observable sequence.
        /// </summary>
        /// <remarks>This method is useful for handling recoverable errors in reactive streams while ensuring that
        /// exceptions are logged for diagnostic purposes. Only exceptions of type TException are caught and logged; other
        /// exceptions are propagated.</remarks>
        /// <typeparam name="TObj">The type of the logger-enabled object used for logging. Must implement IEnableLogger.</typeparam>
        /// <typeparam name="TException">The type of exception to catch and handle. Must derive from Exception.</typeparam>
        /// <param name="class">An object that provides logging capabilities. Used to log the caught exception as a warning.</param>
        /// <param name="next">A function that returns an alternative observable sequence to continue with when an exception of type TException
        /// is caught. The function receives the caught exception as its parameter.</param>
        /// <param name="message">An optional message to include in the warning log. If null, an empty string is used.</param>
        /// <returns>An observable sequence that continues with the sequence returned by the next function after logging the
        /// exception, or propagates other exceptions.</returns>
        public IObservable<T> LoggedCatch<TObj, TException>(
            TObj @class,
            Func<TException, IObservable<T>> next,
            string? message)
            where TObj : IEnableLogger
            where TException : Exception =>
            new LoggedCatchObservable<T, TException>(@this, ex =>
            {
                @class.Log().Warn(ex, message ?? string.Empty);
                return next(ex);
            });
    }

    /// <summary>
    /// A fused tee sink that invokes side-effect callbacks for each notification before forwarding it unchanged —
    /// replacing the <c>Do(onNext, onError, onCompleted)</c> used by <see cref="Log{T, TObj}(IObservable{T}, TObj, string?, Func{T, string}?)"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source observable.</param>
    /// <param name="onNext">Invoked with each value before it is forwarded.</param>
    /// <param name="onError">Invoked with the error before it is forwarded.</param>
    /// <param name="onCompleted">Invoked on completion before it is forwarded.</param>
    private sealed class LoggingTeeObservable<T>(
        IObservable<T> source,
        Action<T> onNext,
        Action<Exception> onError,
        Action onCompleted) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(onNext, onError, onCompleted, observer));
        }

        /// <summary>Runs each side-effect callback, then forwards the notification (forwarding the callback's own error if it throws).</summary>
        /// <param name="onNext">Invoked with each value before it is forwarded.</param>
        /// <param name="onError">Invoked with the error before it is forwarded.</param>
        /// <param name="onCompleted">Invoked on completion before it is forwarded.</param>
        /// <param name="downstream">The observer that receives the forwarded notifications.</param>
        private sealed class Sink(Action<T> onNext, Action<Exception> onError, Action onCompleted, IObserver<T> downstream)
            : IObserver<T>
        {
            /// <inheritdoc/>
            public void OnNext(T value)
            {
                try
                {
                    onNext(value);
                }
                catch (Exception ex)
                {
                    downstream.OnError(ex);
                    return;
                }

                downstream.OnNext(value);
            }

            /// <inheritdoc/>
            public void OnError(Exception error)
            {
                try
                {
                    onError(error);
                }
                catch (Exception ex)
                {
                    downstream.OnError(ex);
                    return;
                }

                downstream.OnError(error);
            }

            /// <inheritdoc/>
            public void OnCompleted()
            {
                try
                {
                    onCompleted();
                }
                catch (Exception ex)
                {
                    downstream.OnError(ex);
                    return;
                }

                downstream.OnCompleted();
            }
        }
    }

    /// <summary>
    /// A fused sink that forwards the source until an exception of type <typeparamref name="TException"/> occurs, then
    /// switches to the observable returned by <paramref name="handler"/> — replacing the <c>Catch&lt;T, TException&gt;</c>
    /// used by the <c>LoggedCatch</c> overloads. Exceptions of other types are propagated unchanged.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TException">The exception type to catch.</typeparam>
    /// <param name="source">The source observable.</param>
    /// <param name="handler">Produces the continuation observable for a caught exception.</param>
    private sealed class LoggedCatchObservable<T, TException>(
        IObservable<T> source,
        Func<TException, IObservable<T>> handler) : IObservable<T>
        where TException : Exception
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            var sink = new Sink(handler, observer);
            sink.Run(source);
            return sink;
        }

        /// <summary>Forwards the source, switching to the handler's continuation on a matching exception.</summary>
        /// <param name="handler">Produces the continuation observable for a caught exception.</param>
        /// <param name="downstream">The observer that receives the forwarded notifications.</param>
        private sealed class Sink(Func<TException, IObservable<T>> handler, IObserver<T> downstream)
            : IObserver<T>, IDisposable
        {
            /// <summary>The source subscription; disposed when switching to the continuation.</summary>
            private readonly OnceDisposable _source = new();

            /// <summary>The continuation subscription created after a caught exception.</summary>
            private readonly MutableDisposable _continuation = new();

            /// <summary>Begins observing the source.</summary>
            /// <param name="source">The source observable.</param>
            public void Run(IObservable<T> source) => _source.Disposable = source.Subscribe(this);

            /// <inheritdoc/>
            public void OnNext(T value) => downstream.OnNext(value);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();

            /// <inheritdoc/>
            public void OnError(Exception error)
            {
                if (error is not TException typed)
                {
                    downstream.OnError(error);
                    return;
                }

                IObservable<T> continuation;
                try
                {
                    continuation = handler(typed);
                }
                catch (Exception ex)
                {
                    downstream.OnError(ex);
                    return;
                }

                _source.Dispose();
                _continuation.Disposable = continuation.Subscribe(downstream);
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                _source.Dispose();
                _continuation.Dispose();
            }
        }
    }
}
