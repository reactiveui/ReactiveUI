// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI;

/// <summary>
/// Static class that holds the default exception handler for ReactiveUI.
/// </summary>
public static class RxState
{
    private static IObserver<Exception>? _defaultExceptionHandler;
    private static int _exceptionHandlerInitialized; // 0 = false, 1 = true

    /// <summary>
    /// Gets the default exception handler for unhandled errors in ReactiveUI observables.
    /// Auto-initializes with debugger break + UnhandledErrorException if not configured via builder.
    /// </summary>
    public static IObserver<Exception> DefaultExceptionHandler
    {
        get
        {
            if (Interlocked.CompareExchange(ref _exceptionHandlerInitialized, 0, 0) == 0)
            {
                InitializeDefaultExceptionHandler();
            }

            return _defaultExceptionHandler!;
        }
    }

    /// <summary>
    /// Initializes the exception handler with a custom observer. Called by ReactiveUIBuilder.
    /// </summary>
    /// <param name="exceptionHandler">The custom exception handler to use.</param>
    internal static void InitializeExceptionHandler(IObserver<Exception> exceptionHandler)
    {
        if (Interlocked.CompareExchange(ref _exceptionHandlerInitialized, 1, 0) == 0)
        {
            _defaultExceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
        }
    }

    /// <summary>
    /// Resets the exception handler state for testing purposes.
    /// </summary>
    /// <remarks>
    /// WARNING: This method should ONLY be used in unit tests to reset state between test runs.
    /// Never call this in production code as it can lead to inconsistent application state.
    /// </remarks>
    internal static void ResetForTesting()
    {
        Interlocked.Exchange(ref _exceptionHandlerInitialized, 0);
        _defaultExceptionHandler = null;
    }

    /// <summary>
    /// Initializes the default exception handler if not already configured.
    /// Creates an observer that breaks debugger and throws UnhandledErrorException.
    /// </summary>
    private static void InitializeDefaultExceptionHandler()
    {
        if (Interlocked.CompareExchange(ref _exceptionHandlerInitialized, 1, 0) == 0)
        {
            _defaultExceptionHandler = Observer.Create<Exception>(ex =>
            {
                // NB: If you're seeing this, it means that an
                // ObservableAsPropertyHelper or the CanExecute of a
                // ReactiveCommand ended in an OnError. Instead of silently
                // breaking, ReactiveUI will halt here if a debugger is attached.
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

#pragma warning disable CA1065 // Avoid exceptions in constructors -- In scheduler.
                RxSchedulers.MainThreadScheduler.Schedule(() => throw new UnhandledErrorException(
                        "An object implementing IHandleObservableErrors (often a ReactiveCommand or ObservableAsPropertyHelper) has errored, thereby breaking its observable pipeline. To prevent this, ensure the pipeline does not error, or Subscribe to the ThrownExceptions property of the object in question to handle the erroneous case.",
                        ex));
#pragma warning restore CA1065
            });
        }
    }
}
