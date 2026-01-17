// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;
using ReactiveUI.Tests.Utilities.AppBuilder;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Provides a test executor that initializes and cleans up the ReactiveUI WPF environment for unit tests requiring WPF
/// view resolution and scheduler configuration.
/// </summary>
public class WpfViewResolverTestExecutor : STAThreadExecutor
{
    private readonly AppBuilderTestHelper _helper = new();

    /// <inheritdoc/>
    protected override void Initialize()
    {
        base.Initialize();

        _helper.Initialize(builder =>
        {
            // Include WPF platform services to ensure view locator, activation, etc. work
            // Register views from this assembly for view resolution tests
            builder
                .WithViewsFromAssembly(GetType().Assembly)
                .WithWpf()
                .WithCoreServices();

            // Configure WPF scheduler for test execution
            // Note: WithWpf() skips scheduler setup when InUnitTestRunner() is true,
            // so we must manually configure it for tests that need WPF controls
            var dispatcher = Dispatcher.CurrentDispatcher;
            RxSchedulers.MainThreadScheduler = new DispatcherScheduler(dispatcher);
            RxSchedulers.TaskpoolScheduler = TaskPoolScheduler.Default;
        });
    }

    /// <inheritdoc/>
    protected override void CleanUp()
    {
        _helper.CleanUp();
        base.CleanUp();
    }
}
