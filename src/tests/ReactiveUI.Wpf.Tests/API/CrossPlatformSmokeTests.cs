// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>A platform-agnostic smoke test that keeps this WPF test assembly non-empty on non-Windows targets.</summary>
/// <remarks>
/// The WPF-specific tests only compile and run on the Windows TFMs (see the project's Windows <c>&lt;Choose&gt;</c>
/// block). The solution format has no per-OS conditionals, so the assembly is still built and executed on
/// Linux/macOS CI, where it would otherwise contain no tests — and the Microsoft Testing Platform treats a
/// "Zero tests ran" result as a failure. This single cross-platform test keeps the non-Windows run valid.
/// </remarks>
public class CrossPlatformSmokeTests
{
    /// <summary>Verifies the testing platform can load and execute this assembly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TestHostCanRunThisAssembly() =>
        await Assert.That(GetType().Assembly).IsNotNull();
}
