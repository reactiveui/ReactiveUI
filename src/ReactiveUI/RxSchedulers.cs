// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace ReactiveUI;

/// <summary>
/// Provides access to ReactiveUI schedulers.
/// </summary>
/// <remarks>
/// This class provides scheduler functionality without requiring unreferenced code attributes,
/// making it suitable for AOT compilation scenarios. RxApp scheduler properties delegate to
/// this class and add builder initialization checks.
/// </remarks>
public static class RxSchedulers
{
#if NET9_0_OR_GREATER
    private static readonly Lock _lock = new();
#else
    private static readonly object _lock = new();
#endif

    private static volatile IScheduler? _mainThreadScheduler;

    private static volatile IScheduler? _taskpoolScheduler;

    /// <summary>
    /// Initializes static members of the <see cref="RxSchedulers"/> class.
    /// </summary>
    /// <remarks>This static constructor sets up default scheduler instances for use throughout the
    /// application. It ensures that TaskpoolScheduler and MainThreadScheduler are assigned appropriate default values
    /// before any static members are accessed.</remarks>
    static RxSchedulers()
    {
        TaskpoolScheduler = TaskPoolScheduler.Default;
        MainThreadScheduler ??= DefaultScheduler.Instance;
    }

    /// <summary>
    /// Gets or sets a scheduler used to schedule work items that
    /// should be run "on the UI thread". In normal mode, this will be
    /// DispatcherScheduler. This defaults to DefaultScheduler.Instance.
    /// </summary>
    public static IScheduler MainThreadScheduler
    {
        get
        {
            if (_mainThreadScheduler is not null)
            {
                return _mainThreadScheduler;
            }

            lock (_lock)
            {
                return _mainThreadScheduler ??= DefaultScheduler.Instance;
            }
        }

        set
        {
            lock (_lock)
            {
                _mainThreadScheduler = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the scheduler used to schedule work items to
    /// run in a background thread. This defaults to TaskPoolScheduler.Default.
    /// </summary>
    public static IScheduler TaskpoolScheduler
    {
        get
        {
            if (_taskpoolScheduler is not null)
            {
                return _taskpoolScheduler;
            }

            lock (_lock)
            {
                if (_taskpoolScheduler is not null)
                {
                    return _taskpoolScheduler;
                }

#if !PORTABLE
                return _taskpoolScheduler ??= TaskPoolScheduler.Default;
#else
                return _taskpoolScheduler ??= DefaultScheduler.Instance;
#endif
            }
        }

        set
        {
            lock (_lock)
            {
                _taskpoolScheduler = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether log messages should be suppressed for command bindings in the view.
    /// Platform registrations may set this to true to reduce logging noise.
    /// </summary>
    public static bool SuppressViewCommandBindingMessage { get; set; }

    /// <summary>
    /// Resets the schedulers back to their default values.
    /// This method is intended for testing purposes only.
    /// </summary>
    /// <remarks>
    /// WARNING: This method should ONLY be used in unit tests to reset state between test runs.
    /// Never call this in production code as it can lead to inconsistent application state.
    /// Resets:
    /// - MainThreadScheduler to DefaultScheduler.Instance.
    /// - TaskpoolScheduler to TaskPoolScheduler.Default.
    /// </remarks>
    internal static void ResetForTesting()
    {
        lock (_lock)
        {
            _mainThreadScheduler = DefaultScheduler.Instance;
#if !PORTABLE
            _taskpoolScheduler = TaskPoolScheduler.Default;
#else
            _taskpoolScheduler = DefaultScheduler.Instance;
#endif
        }
    }

    /// <summary>
    /// Set up default initializations for static constructor.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoOptimization)]
    internal static void EnsureStaticConstructorRun()
    {
        // NB: This method only exists to invoke the static constructor if needed
    }
}
