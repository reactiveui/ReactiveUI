// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.WinUI.Tests;

/// <summary>Verifies the non-Windows fallback used by the cross-platform solution build.</summary>
public sealed class WinUIFallbackTest
{
    /// <summary>Verifies that the fallback target does not load WinUI.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WinUIAssembly_IsNotLoaded() =>
        await Assert.That(Type.GetType("Microsoft.UI.Xaml.Application, Microsoft.WinUI")).IsNull();
}
