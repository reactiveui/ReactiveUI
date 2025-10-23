// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace ReactiveUI;

/// <summary>
/// Provides access to ReactiveUI schedulers without requiring unreferenced code attributes.
/// This is a lightweight alternative to RxApp for consuming scheduler properties.
/// </summary>
/// <remarks>
/// This class provides basic scheduler functionality without the overhead of dependency injection
/// or unit test detection, allowing consumers to access schedulers without needing
/// RequiresUnreferencedCode attributes. For full functionality including unit test support,
/// use RxApp schedulers instead.
/// </remarks>
public static class RxSchedulers
{
    private static readonly object _lock = new();

    private static volatile IScheduler? _mainThreadScheduler;

    private static volatile IScheduler? _taskpoolScheduler;

    /// <summary>
    /// Gets or sets a scheduler used to schedule work items that
    /// should be run "on the UI thread". In normal mode, this will be
    /// DispatcherScheduler. This defaults to DefaultScheduler.Instance.
    /// </summary>
    /// <remarks>
    /// This is a simplified version that doesn't include unit test detection.
    /// For full functionality including unit test support, use RxApp.MainThreadScheduler.
    /// </remarks>
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
    /// <remarks>
    /// This is a simplified version that doesn't include unit test detection.
    /// For full functionality including unit test support, use RxApp.TaskpoolScheduler.
    /// </remarks>
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
    /// Set up default initializations.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoOptimization)]
    internal static void EnsureInitialized()
    {
        // NB: This method only exists to invoke the static constructor if needed
    }
}
