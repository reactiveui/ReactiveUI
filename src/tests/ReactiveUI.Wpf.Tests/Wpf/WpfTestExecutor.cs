// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using System.Windows.Threading;
using ReactiveUI.Tests.Utilities.AppBuilder;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Test executor that provides WPF test isolation with STA threading.
/// Combines STAThreadExecutor with WPF AppBuilder setup/teardown.
/// Can be applied at class or method level depending on whether the test creates its own AppBuilder.
/// </summary>
/// <remarks>
/// This executor provides:
/// - STA thread context required for WPF controls
/// - WPF platform services (view locator, activation fetcher, platform operations)
/// - WPF scheduler configuration for test execution
/// - Automatic cleanup and state restoration after test completion
/// Tests using this executor should be marked with [NotInParallel] to prevent
/// concurrent modifications to shared state.
/// </remarks>
[SupportedOSPlatform("windows")]
public class WpfTestExecutor : STAThreadExecutor
{
    private readonly AppBuilderTestHelper _helper = new();

    /// <inheritdoc/>
    protected override void Initialize()
    {
        base.Initialize();

        _helper.Initialize(builder =>
        {
            // Include WPF platform services to ensure view locator, activation, etc. work
            builder
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
