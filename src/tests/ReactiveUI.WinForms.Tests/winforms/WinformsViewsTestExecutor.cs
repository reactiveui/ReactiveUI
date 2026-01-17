// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using ReactiveUI.Tests.Utilities.AppBuilder;

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
public class WinFormsViewsTestExecutor : STAThreadExecutor
{
    private readonly AppBuilderTestHelper _helper = new();

    /// <inheritdoc/>
    protected override void Initialize()
    {
        base.Initialize();

        _helper.Initialize(builder =>
        {
            // Include WinForms platform services to ensure view locator, activation, etc. work
            // Register views from this assembly for view resolution tests
            builder
                .WithWinForms()
                .WithViewsFromAssembly(typeof(WinFormsViewsTestExecutor).Assembly)
                .WithCoreServices();
        });
    }

    /// <inheritdoc/>
    protected override void CleanUp()
    {
        _helper.CleanUp();
        base.CleanUp();
    }
}
