// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Tests;

/// <summary>Assembly-level hooks for WinUI test initialization.</summary>
/// <remarks>
/// The XAML application host and the ReactiveUI builder are established per test by
/// <see cref="WinUI.WinUITestExecutor"/>, so only the process-wide mode detector is set here.
/// </remarks>
public static class AssemblyHooks
{
    /// <summary>Called before any tests in this assembly start.</summary>
    /// <remarks>
    /// Unlike the other test assemblies this one reports that it is <em>not</em> a unit test runner, because it
    /// really is running a live XAML application on a real dispatcher. The view hosts take a different path when
    /// <see cref="ModeDetector.InUnitTestRunner"/> is true: their view-contract stream is replaced by a silent
    /// one, which never emits, so the combined view-model/contract stream never produces a value and no view is
    /// ever resolved. Reporting the truth here keeps the hosts on the code path real applications run.
    /// </remarks>
    [Before(Assembly)]
    public static void AssemblySetup() => ModeDetector.OverrideModeDetector(new LiveApplicationModeDetector());

    /// <summary>Mode detector that reports a live application rather than a unit test runner.</summary>
    private sealed class LiveApplicationModeDetector : IModeDetector
    {
        /// <inheritdoc/>
        public bool? InUnitTestRunner() => false;
    }
}
