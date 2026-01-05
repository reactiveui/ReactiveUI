// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Detects if we are in production mode or not.
/// </summary>
public class ProductionMode : IModeDetector, IPlatformModeDetector
{
    private static readonly ProductionMode Instance = new();

    /// <summary>
    /// Sets the platform mode.
    /// </summary>
    /// <returns>A disposable to revert to the previous state.</returns>
    public static IDisposable Set()
    {
        PlatformModeDetector.OverrideModeDetector(Instance);
        ModeDetector.OverrideModeDetector(Instance);
        return Disposable.Create(static () =>
        {
            PlatformModeDetector.OverrideModeDetector(new DefaultPlatformModeDetector());
            ModeDetector.OverrideModeDetector(new DefaultModeDetector());
        });
    }

    /// <summary>
    /// Value indicating whether we are in the unit test runner.
    /// </summary>
    /// <returns>If we are in test mode.</returns>
    public bool? InUnitTestRunner() => false;

    /// <summary>
    /// Value indicating whether we are in the design mode.
    /// </summary>
    /// <returns>If we are in design mode.</returns>
    public bool? InDesignMode() => false;
}
