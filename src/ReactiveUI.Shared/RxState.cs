// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Provides global state and configuration for ReactiveUI's exception handling in observable pipelines.</summary>
/// <remarks>This static class manages the default exception handler used by ReactiveUI to process unhandled
/// errors in observables. It is primarily intended for internal framework use and advanced scenarios where custom error
/// handling is required. Most application code does not need to interact with this class directly.</remarks>
public static class RxState
{
    /// <summary>The published exception handler, or null until one is published.</summary>
    /// <remarks>
    /// The field is its own initialization flag. A handler is published once with an atomic
    /// compare-and-exchange, so a reader sees either null or a fully constructed handler, and the
    /// first handler published wins.
    /// </remarks>
    private static IObserver<Exception>? _defaultExceptionHandler;

    /// <summary>
    /// Gets the default exception handler for unhandled errors in ReactiveUI observables.
    /// Auto-initializes with debugger break + UnhandledErrorException if not configured via builder.
    /// </summary>
    public static IObserver<Exception> DefaultExceptionHandler =>
        Volatile.Read(ref _defaultExceptionHandler) ?? InitializeDefaultExceptionHandler();

    /// <summary>Initializes the exception handler with a custom observer. Called by ReactiveUIBuilder.</summary>
    /// <param name="exceptionHandler">The custom exception handler to use.</param>
    internal static void InitializeExceptionHandler(IObserver<Exception> exceptionHandler)
    {
        ArgumentExceptionHelper.ThrowIfNull(exceptionHandler);
        _ = Interlocked.CompareExchange(ref _defaultExceptionHandler, exceptionHandler, null);
    }

    /// <summary>Resets the exception handler state for testing purposes.</summary>
    /// <remarks>
    /// WARNING: This method should ONLY be used in unit tests to reset state between test runs.
    /// Never call this in production code as it can lead to inconsistent application state.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ResetForTesting() => Volatile.Write(ref _defaultExceptionHandler, null);

    /// <summary>
    /// Publishes the default exception handler if no handler has been published yet.
    /// The default handler breaks into the debugger and throws UnhandledErrorException.
    /// </summary>
    /// <returns>The published handler, which is another thread's handler when that thread published first.</returns>
    private static IObserver<Exception> InitializeDefaultExceptionHandler()
    {
        var handler = new DelegateObserver<Exception>(static ex =>
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            _ = RxSchedulers.MainThreadScheduler.Schedule(ex, static (_, capturedException) => throw new UnhandledErrorException(
                "An object implementing IHandleObservableErrors (often a ReactiveCommand or ObservableAsPropertyHelper) has errored,"
                + " thereby breaking its observable pipeline. To prevent this, ensure the pipeline does not error, or Subscribe to the "
                + "ThrownExceptions property of the object in question to handle the erroneous case.",
                capturedException));
        });

        return Interlocked.CompareExchange(ref _defaultExceptionHandler, handler, null) ?? handler;
    }
}
