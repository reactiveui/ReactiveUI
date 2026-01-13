// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Provides a test executor that initializes and cleans up the ReactiveUI WPF environment for unit tests requiring WPF
/// view resolution and scheduler configuration.
/// </summary>
public class WpfViewResolverTestExecutor : STAThreadExecutor
{
    private IScheduler? _originalMainThreadScheduler;
    private IScheduler? _originalTaskpoolScheduler;

    /// <inheritdoc/>
    protected override void Initialize()
    {
        base.Initialize();

        // Save the current schedulers so we can restore them later
        _originalMainThreadScheduler = RxSchedulers.MainThreadScheduler;
        _originalTaskpoolScheduler = RxSchedulers.TaskpoolScheduler;

        RxAppBuilder.ResetForTesting();

        // Replace with a new locator that tests can modify
        // Include WPF platform services to ensure view locator, activation, etc. work
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithViewsFromAssembly(GetType().Assembly)
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

    /// <inheritdoc/>
    protected override void CleanUp()
    {
        // Restore original schedulers before resetting
        if (_originalMainThreadScheduler != null)
        {
            RxSchedulers.MainThreadScheduler = _originalMainThreadScheduler;
        }

        if (_originalTaskpoolScheduler != null)
        {
            RxSchedulers.TaskpoolScheduler = _originalTaskpoolScheduler;
        }

        // Reset AppBuilder state to clean up test-specific registrations
        RxAppBuilder.ResetForTesting();

        base.CleanUp();
    }
}
