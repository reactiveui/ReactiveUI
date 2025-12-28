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
    /// <remarks>
    /// Overrides the mode detector to prevent MAUI platform initializations from attempting to
    /// create Windows-specific UI infrastructure (DispatcherQueueScheduler) that would hang on
    /// test threads without a UI message pump. This ensures tests run in a compatible mode.
    /// </remarks>
    [Before(HookType.Assembly)]
    public static void AssemblySetup()
    {
        // CRITICAL: Override ModeDetector to indicate we're in a unit test runner
        // This prevents the MAUI registrations from trying to initialize DispatcherQueueScheduler
        // on Windows, which would hang because there's no DispatcherQueue on the test thread
        ModeDetector.OverrideModeDetector(new TestModeDetector());
    }

    /// <summary>
    /// Called after all tests in this assembly complete.
    /// </summary>
    /// <remarks>
    /// Performs aggressive garbage collection to ensure proper cleanup of MAUI resources
    /// and finalizers before the test host process terminates.
    /// </remarks>
    [After(HookType.Assembly)]
    public static void AssemblyTeardown()
    {
        // Clean up resources
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    /// <summary>
    /// Mode detector implementation that always reports being in a unit test runner.
    /// </summary>
    /// <remarks>
    /// Used to override ReactiveUI's platform detection so that MAUI-specific initialization
    /// code takes the test-friendly path instead of attempting to create real UI infrastructure.
    /// </remarks>
    private sealed class TestModeDetector : IModeDetector
    {
        /// <summary>
        /// Indicates whether the code is running in a unit test runner.
        /// </summary>
        /// <returns>Always returns <see langword="true"/> to force test mode behavior.</returns>
        public bool? InUnitTestRunner() => true;
    }
}
