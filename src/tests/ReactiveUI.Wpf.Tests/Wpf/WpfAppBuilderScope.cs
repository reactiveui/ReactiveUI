// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// A disposable scope that initializes and tears down the ReactiveUI app builder with WPF services for each test.
/// Captures the current app state and sets up a fresh WPF-configured app builder instance.
/// Use this in test setup/teardown to ensure each test has isolated, properly configured WPF services.
/// </summary>
/// <remarks>
/// This scope provides:
/// - WPF platform services (view locator, activation fetcher, platform operations)
/// - WPF scheduler configuration for test execution
/// - Automatic cleanup and state restoration after test completion
/// Tests using this scope should be marked with [NotInParallel] and [TestExecutor&lt;STAThreadExecutor&gt;]
/// to prevent concurrent modifications to shared state and ensure proper STA thread context.
/// </remarks>
public sealed class WpfAppBuilderScope : IDisposable
{
    private readonly IReadonlyDependencyResolver _previousLocator;
    private readonly IScheduler _originalMainThreadScheduler;
    private readonly IScheduler _originalTaskpoolScheduler;

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfAppBuilderScope"/> class.
    /// Captures the current app state and sets up a fresh app builder for WPF testing.
    /// </summary>
    public WpfAppBuilderScope()
    {
        // Save the current locator so we can restore it later
        _previousLocator = AppLocator.Current;

        // Capture current scheduler state before WPF initialization
        _originalMainThreadScheduler = RxSchedulers.MainThreadScheduler;
        _originalTaskpoolScheduler = RxSchedulers.TaskpoolScheduler;

        RxAppBuilder.ResetForTesting();

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

        // Replace with a new locator that tests can modify
        // Include WPF platform services to ensure view locator, activation, etc. work
        RxAppBuilder.CreateReactiveUIBuilder((IDependencyResolver)_previousLocator)
            .BuildApp();

        // Restore original schedulers
        RxSchedulers.MainThreadScheduler = _originalMainThreadScheduler;
        RxSchedulers.TaskpoolScheduler = _originalTaskpoolScheduler;
    }
}
