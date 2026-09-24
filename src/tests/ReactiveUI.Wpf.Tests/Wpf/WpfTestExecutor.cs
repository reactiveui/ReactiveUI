// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
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
public class WpfTestExecutor : DispatcherThreadExecutor
{
    /// <summary>Helper that manages app builder setup and teardown for the test.</summary>
    private readonly AppBuilderTestHelper _helper = new();

    /// <inheritdoc/>
    protected override void Initialize()
    {
        base.Initialize();

        _helper.Initialize(static builder =>
        {
            // Include WPF platform services to ensure view locator, activation, etc. work.
            _ = builder
                .WithWpf()
                .WithCoreServices();
        });

        // Configure the WPF scheduler AFTER BuildApp. WithWpf() registers DispatcherSequencer.Main and BuildApp
        // applies it to RxSchedulers.MainThreadScheduler, so setting it inside the builder callback would be
        // overwritten. Main is process-wide and binds to the application's dispatcher once any test creates an
        // Application, which DispatcherUtilities.DoEvents on this thread never pumps. Binding this executor's
        // dedicated STA dispatcher keeps marshalled work on the dispatcher the test pumps. Initialize and the test
        // body run on the same dedicated thread (see DedicatedThreadExecutor).
        RxSchedulers.MainThreadScheduler = DispatcherSequencer.Current;
        RxSchedulers.TaskpoolScheduler = TaskPoolSequencer.Default;
    }

    /// <inheritdoc/>
    protected override void CleanUp()
    {
        _helper.CleanUp();
        base.CleanUp();
    }
}
