// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using TUnit.Core;

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
        // CRITICAL: Override ModeDetector to indicate we're in a unit test runner
        // This prevents the MAUI registrations from trying to initialize DispatcherQueueScheduler
        // on Windows, which would hang because there's no DispatcherQueue on the test thread
        ModeDetector.OverrideModeDetector(new TestModeDetector());
    }

    /// <summary>
    /// Called after all tests in this assembly complete.
    /// </summary>
    [After(HookType.Assembly)]
    public static void AssemblyTeardown()
    {
        // Clean up resources
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    /// <summary>
    /// Mode detector that always indicates we're in a unit test runner.
    /// </summary>
    private sealed class TestModeDetector : IModeDetector
    {
        public bool? InUnitTestRunner() => true;
    }
}
