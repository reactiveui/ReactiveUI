// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.Maui.Tests;

/// <summary>
/// Assembly-level hooks for test initialization and cleanup.
/// </summary>
public static class AssemblyHooks
{
    /// <summary>
    /// Called before any tests in this assembly start.
    /// </summary>
    [Before(HookType.Assembly)]
    public static void AssemblySetup()
    {
        ModeDetector.OverrideModeDetector(new TestModeDetector());
    }

    /// <summary>
    /// Called after all tests in this assembly complete.
    /// </summary>
    [After(HookType.Assembly)]
    public static void AssemblyTeardown()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    /// <summary>
    /// Mode detector implementation that always reports being in a unit test runner.
    /// </summary>
    private sealed class TestModeDetector : IModeDetector
    {
        /// <summary>
        /// Indicates whether the code is running in a unit test runner.
        /// </summary>
        /// <returns>Always returns <see langword="true"/> to force test mode behavior.</returns>
        public bool? InUnitTestRunner() => true;
    }
}
