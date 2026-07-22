// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using ReactiveUI.Tests.Utilities.AppBuilder;
using Splat;
using TUnit.Core.Executors;

[assembly: NotInParallel]
[assembly: TestExecutor<AppBuilderTestExecutor>]
[assembly: System.Resources.NeutralResourcesLanguage("en-US")]

namespace ReactiveUI.Tests;

/// <summary>Assembly-level hooks for test initialization and cleanup.</summary>
public static class AssemblyHooks
{
    /// <summary>Called before any tests in this assembly start.</summary>
    [Before(Assembly)]
    public static void AssemblySetup()
    {
        // Pin the test culture to invariant so culture-sensitive converter tests (dates, currency, percent) are
        // deterministic regardless of the host machine's locale. The converters themselves use CurrentCulture by
        // design (correct for UI display); fixing the culture here makes that observable output match expectations.
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        // Override ModeDetector to ensure we're detected as being in a unit test runner.
        // App builder initialization is handled per-test via AppBuilderTestExecutor.
        ModeDetector.OverrideModeDetector(new TestModeDetector());
    }

    /// <summary>Called after all tests in this assembly complete.</summary>
    [After(Assembly)]
    [SuppressMessage(
        "Performance",
        "PSH1021:Remove this 'GC.Collect'/'GC.WaitForPendingFinalizers' call",
        Justification = "assembly teardown deliberately forces a final collection to release test-fixture resources before the process reports results.")]
    public static void AssemblyTeardown()
    {
        // Clean up resources
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    /// <summary>Mode detector that always indicates we're in a unit test runner.</summary>
    private sealed class TestModeDetector : IModeDetector
    {
        /// <inheritdoc/>
        public bool? InUnitTestRunner() => true;
    }
}
