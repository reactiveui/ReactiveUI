// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Suspension;

/// <summary>
///     Tests for DummySuspensionDriver.
/// </summary>
public class DummySuspensionDriverTests
{
    /// <summary>
    ///     Tests that DummySuspensionDriver InvalidateState returns observable.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriver_InvalidateState_ReturnsObservable()
    {
        // Arrange
        var driver = new DummySuspensionDriver();

        // Act
        var result = driver.InvalidateState();

        // Assert
        await Assert.That(result).IsNotNull();
    }

    /// <summary>
    ///     Tests that DummySuspensionDriver LoadState returns observable.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriver_LoadState_ReturnsObservable()
    {
        // Arrange
        var driver = new DummySuspensionDriver();

        // Act
        var result = driver.LoadState();

        // Assert
        await Assert.That(result).IsNotNull();
    }

    /// <summary>
    ///     Tests that DummySuspensionDriver SaveState handles null state.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriver_SaveState_NullState_ReturnsObservable()
    {
        // Arrange
        var driver = new DummySuspensionDriver();

        // Act
        var result = driver.SaveState<object>(null!);

        // Assert
        await Assert.That(result).IsNotNull();
    }

    /// <summary>
    ///     Tests that DummySuspensionDriver SaveState returns observable.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriver_SaveState_ReturnsObservable()
    {
        // Arrange
        var driver = new DummySuspensionDriver();
        var state = new { TestProperty = "test" };

        // Act
        var result = driver.SaveState(state);

        // Assert
        await Assert.That(result).IsNotNull();
    }
}
