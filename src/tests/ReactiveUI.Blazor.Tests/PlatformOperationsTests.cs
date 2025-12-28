// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Blazor.Tests;

/// <summary>
/// Tests for the <see cref="PlatformOperations"/> class.
/// These tests verify the platform-specific operations for Blazor.
/// </summary>
public class PlatformOperationsTests
{
    /// <summary>
    /// Verifies that PlatformOperations implements IPlatformOperations.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task PlatformOperations_ImplementsIPlatformOperations()
    {
        var platformOperations = new PlatformOperations();

        await Assert.That(platformOperations).IsAssignableTo<IPlatformOperations>();
    }

    /// <summary>
    /// Verifies that GetOrientation returns null.
    /// Blazor runs in a browser environment where device orientation is not directly accessible.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task GetOrientation_ReturnsNull()
    {
        var platformOperations = new PlatformOperations();

        var result = platformOperations.GetOrientation();

        await Assert.That(result).IsNull();
    }

    /// <summary>
    /// Verifies that multiple calls to GetOrientation return null consistently.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task GetOrientation_ReturnsNull_Consistently()
    {
        var platformOperations = new PlatformOperations();

        var result1 = platformOperations.GetOrientation();
        var result2 = platformOperations.GetOrientation();
        var result3 = platformOperations.GetOrientation();

        await Assert.That(result1).IsNull();
        await Assert.That(result2).IsNull();
        await Assert.That(result3).IsNull();
    }

    /// <summary>
    /// Verifies that PlatformOperations can be created without throwing exceptions.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Constructor_DoesNotThrow()
    {
        PlatformOperations? platformOperations = null;

        await Assert.That(() => platformOperations = new PlatformOperations()).ThrowsNothing();
        await Assert.That(platformOperations).IsNotNull();
    }
}
