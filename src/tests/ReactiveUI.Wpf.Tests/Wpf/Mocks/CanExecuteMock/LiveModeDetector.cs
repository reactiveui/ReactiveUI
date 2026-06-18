// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Tests.Wpf.Mocks.CanExecuteMock;

/// <summary>Helper for switching the active mode detector during tests that require live runtime threads.</summary>
public static class LiveModeDetector
{
    /// <summary>The detector that always reports not being in a unit test runner.</summary>
    private static readonly AlwaysFalseModeDetector liveModeDetector = new();

    /// <summary>The default mode detector.</summary>
    private static readonly DefaultModeDetector defaultModeDetector = new();

    /// <summary>Overrides the mode detector so the runtime behaves as if not in a unit test runner.</summary>
    public static void UseRuntimeThreads() =>
        ModeDetector.OverrideModeDetector(liveModeDetector);

    /// <summary>Restores the default mode detector.</summary>
    public static void UseDefaultModeDetector() =>
        ModeDetector.OverrideModeDetector(defaultModeDetector);

    /// <summary>Gets a value indicating whether the code is running in a unit test runner.</summary>
    /// <returns><see langword="true"/> if in a unit test runner; otherwise <see langword="false"/> or <see langword="null"/>.</returns>
    public static bool? InUnitTestRunner() => ModeDetector.InUnitTestRunner();
}
