// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

using ReactiveUI.Builder;

namespace ReactiveUI;

/// <summary>
/// Extension methods to assist with Logging.
/// </summary>
public static class ObservableLoggingMixin
{
    /// <summary>
    /// Initializes static members of the <see cref="ObservableLoggingMixin"/> class.
    /// </summary>
    static ObservableLoggingMixin() => RxAppBuilder.EnsureInitialized();

    /// <summary>
    /// Returns an observable sequence that logs each notification using the specified logger object.
    /// </summary>
    /// <remarks>This method does not modify the elements of the sequence or affect its timing, but adds side
    /// effects for logging purposes. Logging occurs for each notification: OnNext (with the element value), OnError,
    /// and OnCompleted. The returned observable can be further composed or subscribed to as usual.</remarks>
    /// <typeparam name="T">The type of the elements in the source observable sequence.</typeparam>
    /// <typeparam name="TObj">The type of the logger object. Must implement <see cref="IEnableLogger"/>.</typeparam>
    /// <param name="this">The source observable sequence whose notifications will be logged.</param>
    /// <param name="logObject">An object that provides logging capabilities. Must implement <see cref="IEnableLogger"/>.</param>
    /// <param name="message">An optional message to include in each log entry. If null, an empty string is used.</param>
    /// <param name="stringifier">An optional function to convert each element to a string for logging. If null, the element's <see
    /// cref="object.ToString()"/> method is used.</param>
    /// <returns>An observable sequence that is functionally equivalent to the source, but logs each OnNext, OnError, and
    /// OnCompleted notification using the provided logger.</returns>
    public static IObservable<T> Log<T, TObj>(
        this IObservable<T> @this,
        TObj logObject,
        string? message = null,
        Func<T, string>? stringifier = null) // TODO: Create Test
        where TObj : IEnableLogger
    {
        message ??= string.Empty;

        if (stringifier is not null)
        {
            return @this.Do(
                            x => logObject.Log().Info(CultureInfo.InvariantCulture, "{0} OnNext: {1}", message, stringifier(x)),
                            ex => logObject.Log().Warn(ex, message + " OnError"),
                            () => logObject.Log().Info(CultureInfo.InvariantCulture, "{0} OnCompleted", message));
        }

        return @this.Do(
                        x => logObject.Log().Info(CultureInfo.InvariantCulture, "{0} OnNext: {1}", message, x),
                        ex => logObject.Log().Warn(ex, message + " OnError"),
                        () => logObject.Log().Info(CultureInfo.InvariantCulture, "{0} OnCompleted", message));
    }

    /// <summary>
    /// Returns an observable sequence that logs any exception using the specified logger and continues with the
    /// provided fallback sequence, if supplied.
    /// </summary>
    /// <remarks>This method is useful for handling errors in observable sequences by logging exceptions and
    /// optionally providing a fallback sequence to continue processing. The exception is logged at the warning level
    /// using the provided logger.</remarks>
    /// <typeparam name="T">The type of the elements in the observable sequence.</typeparam>
    /// <typeparam name="TObj">The type of the logger, which must implement <see cref="IEnableLogger"/>.</typeparam>
    /// <param name="this">The source observable sequence to monitor for exceptions.</param>
    /// <param name="class">An object that provides logging capabilities and is used to log any exceptions encountered.</param>
    /// <param name="next">An optional observable sequence to continue with after an exception is caught. If not specified, a default empty
    /// sequence is used.</param>
    /// <param name="message">An optional message to include in the log entry when an exception is caught. If null, an empty string is used.</param>
    /// <returns>An observable sequence that emits the original elements until an exception occurs, logs the exception, and then
    /// continues with the specified fallback sequence.</returns>
    public static IObservable<T> LoggedCatch<T, TObj>(this IObservable<T> @this, TObj @class, IObservable<T>? next = null, string? message = null) // TODO: Create Test
        where TObj : IEnableLogger
    {
        next ??= Observable<T>.Default;
        return @this.Catch<T, Exception>(ex =>
        {
            @class.Log().Warn(ex, message ?? string.Empty);
            return next;
        });
    }

    /// <summary>
    /// Handles exceptions of a specified type in the observable sequence by logging a warning and continuing with an
    /// alternative observable sequence.
    /// </summary>
    /// <remarks>This method is useful for handling recoverable errors in reactive streams while ensuring that
    /// exceptions are logged for diagnostic purposes. Only exceptions of type TException are caught and logged; other
    /// exceptions are propagated.</remarks>
    /// <typeparam name="T">The type of the elements in the source observable sequence.</typeparam>
    /// <typeparam name="TObj">The type of the logger-enabled object used for logging. Must implement IEnableLogger.</typeparam>
    /// <typeparam name="TException">The type of exception to catch and handle. Must derive from Exception.</typeparam>
    /// <param name="this">The source observable sequence to monitor for exceptions.</param>
    /// <param name="class">An object that provides logging capabilities. Used to log the caught exception as a warning.</param>
    /// <param name="next">A function that returns an alternative observable sequence to continue with when an exception of type TException
    /// is caught. The function receives the caught exception as its parameter.</param>
    /// <param name="message">An optional message to include in the warning log. If null, an empty string is used.</param>
    /// <returns>An observable sequence that continues with the sequence returned by the next function after logging the
    /// exception, or propagates other exceptions.</returns>
    public static IObservable<T> LoggedCatch<T, TObj, TException>(this IObservable<T> @this, TObj @class, Func<TException, IObservable<T>> next, string? message = null) // TODO: Create Test
        where TObj : IEnableLogger
        where TException : Exception =>
        @this.Catch<T, TException>(ex =>
        {
            @class.Log().Warn(ex, message ?? string.Empty);
            return next(ex);
        });
}
