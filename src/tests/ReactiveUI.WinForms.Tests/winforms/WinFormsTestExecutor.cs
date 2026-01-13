// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;

using TUnit.Core;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>
/// Test executor that provides WinForms test isolation with STA threading.
/// Combines STAThreadExecutor with WinForms AppBuilder setup/teardown.
/// Can be applied at class or method level depending on whether the test creates its own AppBuilder.
/// </summary>
/// <remarks>
/// This executor provides:
/// - STA thread context required for WinForms controls
/// - WinForms platform services (view locator, activation fetcher, platform operations)
/// - Automatic cleanup and state restoration after test completion
/// Tests using this executor should be marked with [NotInParallel] to prevent
/// concurrent modifications to shared state.
/// </remarks>
[SupportedOSPlatform("windows")]
public class WinFormsTestExecutor : STAThreadExecutor
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

        // Include WinForms platform services to ensure view locator, activation, etc. work
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithWinForms()
            .WithCoreServices()
            .BuildApp();
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
