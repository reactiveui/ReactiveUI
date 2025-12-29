// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Maui;

/// <summary>
/// Tests for <see cref="PlatformOperations"/>.
/// </summary>
public class PlatformOperationsTest
{
    /// <summary>
    /// Tests that GetOrientation returns null on MAUI.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetOrientation_ReturnsNull()
    {
        var platformOps = new PlatformOperations();

        var orientation = platformOps.GetOrientation();

        await Assert.That(orientation).IsNull();
    }

    /// <summary>
    /// Tests that PlatformOperations implements IPlatformOperations.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ImplementsIPlatformOperations()
    {
        var type = typeof(PlatformOperations);
        var interfaces = type.GetInterfaces();

        await Assert.That(interfaces).Contains(typeof(IPlatformOperations));
    }
}
