// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;

namespace ReactiveUI.Tests.Xaml.Utilities;

/// <summary>
/// A disposable scope that configures RxSchedulers.MainThreadScheduler to use the current thread's
/// WPF Dispatcher for the duration of the scope.
/// </summary>
/// <remarks>
/// This scope is necessary for tests that create WPF bindings which monitor control properties.
/// Without this, observable disposal may attempt to access WPF controls on background threads,
/// causing InvalidOperationException. By setting RxSchedulers.MainThreadScheduler to a DispatcherScheduler
/// for the current thread, all Rx operations (including disposal) execute on the correct UI thread.
/// <para>
/// The scope uses the current thread's Dispatcher (creating one if it doesn't exist) and
/// configures RxSchedulers.MainThreadScheduler to use it.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [TestExecutor&lt;STAThreadExecutor&gt;]
/// public class MyWpfTests
/// {
///     private DispatcherSchedulerScope? _dispatcherScope;
///
///     [Before(HookType.Test)]
///     public void SetUp()
///     {
///         _dispatcherScope = new DispatcherSchedulerScope();
///     }
///
///     [After(HookType.Test)]
///     public void TearDown()
///     {
///         _dispatcherScope?.Dispose();
///     }
/// }
/// </code>
/// </example>
public sealed class DispatcherSchedulerScope : IDisposable
{
    private readonly IScheduler _originalMainThreadScheduler;
    private readonly IScheduler _originalTaskpoolScheduler;
    private readonly Dispatcher _dispatcher;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatcherSchedulerScope"/> class.
    /// Uses the current thread's Dispatcher and configures RxSchedulers.MainThreadScheduler.
    /// </summary>
    public DispatcherSchedulerScope()
    {
        // Capture current scheduler state
        _originalMainThreadScheduler = RxSchedulers.MainThreadScheduler;
        _originalTaskpoolScheduler = RxSchedulers.TaskpoolScheduler;

        // Get or create a Dispatcher for the current thread
        // This works because [TestExecutor<STAThreadExecutor>] ensures we're on an STA thread
        _dispatcher = Dispatcher.CurrentDispatcher;

        // Configure RxSchedulers to use the current thread's Dispatcher
        RxSchedulers.MainThreadScheduler = new DispatcherScheduler(_dispatcher);
    }

    /// <summary>
    /// Disposes the scope, restoring the original RxSchedulers scheduler state.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        // Restore original schedulers
        RxSchedulers.MainThreadScheduler = _originalMainThreadScheduler;
        RxSchedulers.TaskpoolScheduler = _originalTaskpoolScheduler;

        _disposed = true;
    }
}
