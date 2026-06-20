// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;
using Splat;
using TUnit.Core.Executors;

[assembly: NotInParallel]
[assembly: TestExecutor<AppBuilderTestExecutor>]

namespace ReactiveUI.Tests;

/// <summary>Assembly-level hooks for the routing (DynamicData) test leaf.</summary>
public static class AssemblyHooks
{
    /// <summary>Called before any tests in this assembly start.</summary>
    [Before(Assembly)]
    public static void AssemblySetup() =>

        // Detect as a unit-test runner so platform registrations skip live scheduler wiring.
        // Per-test ReactiveUI builder initialization is handled by AppBuilderTestExecutor, which
        // the routing leaf otherwise lacked — without it the first WhenAny* call hits the
        // ReactiveNotifyPropertyChangedMixins static ctor and EnsureInitialized() throws.
        ModeDetector.OverrideModeDetector(new TestModeDetector());

    /// <summary>Mode detector that always indicates we're in a unit test runner.</summary>
    private sealed class TestModeDetector : IModeDetector
    {
        /// <inheritdoc/>
        public bool? InUnitTestRunner() => true;
    }
}
