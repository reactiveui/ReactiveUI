// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui.Tests;

/// <summary>
/// Tests for <see cref="RoutedViewHost"/>.
/// </summary>
public class RoutedViewHostTest
{
    /// <summary>
    /// Tests that RouterProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RouterProperty_IsRegistered()
    {
        await Assert.That(ReactiveUI.Maui.RoutedViewHost.RouterProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that SetTitleOnNavigateProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetTitleOnNavigateProperty_IsRegistered()
    {
        await Assert.That(ReactiveUI.Maui.RoutedViewHost.SetTitleOnNavigateProperty).IsNotNull();
    }
}
