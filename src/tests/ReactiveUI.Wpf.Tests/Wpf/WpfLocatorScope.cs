// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using System.Windows.Threading;

using ReactiveUI.Builder;

using Splat;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// A disposable scope that snapshots and restores Splat's Locator.Current static state
/// with WPF-specific service registrations and scheduler configuration.
/// Use this in WPF test fixtures that read or modify Locator.CurrentMutable to ensure
/// static state is properly restored after tests complete.
/// </summary>
/// <remarks>
/// This is the WPF-specific version of LocatorScope that includes:
/// - WPF platform services (view locator, activation fetcher, platform operations)
/// - WPF scheduler configuration for test execution
/// Tests using this scope should be marked with [NotInParallel] and [TestExecutor&lt;STAThreadExecutor&gt;]
/// to prevent concurrent modifications to shared state and ensure proper STA thread context.
/// </remarks>
public sealed class WpfLocatorScope : IDisposable
{
    private readonly IReadonlyDependencyResolver _previousLocator;
    private readonly IScheduler _originalMainThreadScheduler;
    private readonly IScheduler _originalTaskpoolScheduler;

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfLocatorScope"/> class.
    /// Captures the current Locator state and sets up a fresh locator for WPF testing.
    /// </summary>
    public WpfLocatorScope()
    {
        // Save the current locator so we can restore it later
        _previousLocator = Locator.Current;

        // Capture current scheduler state before WPF initialization
        _originalMainThreadScheduler = RxSchedulers.MainThreadScheduler;
        _originalTaskpoolScheduler = RxSchedulers.TaskpoolScheduler;

        // Replace with a new locator that tests can modify
        // Include WPF platform services to ensure view locator, activation, etc. work
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithWpf()
            .WithCoreServices()
            .BuildApp();

        // Configure WPF scheduler for test execution
        // Note: WithWpf() skips scheduler setup when InUnitTestRunner() is true,
        // so we must manually configure it for tests that need WPF controls
        var dispatcher = Dispatcher.CurrentDispatcher;
        RxSchedulers.MainThreadScheduler = new DispatcherScheduler(dispatcher);
        RxSchedulers.TaskpoolScheduler = TaskPoolScheduler.Default;
    }

    /// <summary>
    /// Restores the Locator and scheduler state to their previous values.
    /// </summary>
    public void Dispose()
    {
        RxAppBuilder.ResetForTesting();

        // Restore the previous locator
        // Cast is safe because we saved it from Locator.Current
        Locator.SetLocator((IDependencyResolver)_previousLocator);

        // Restore original schedulers
        RxSchedulers.MainThreadScheduler = _originalMainThreadScheduler;
        RxSchedulers.TaskpoolScheduler = _originalTaskpoolScheduler;
    }
}
