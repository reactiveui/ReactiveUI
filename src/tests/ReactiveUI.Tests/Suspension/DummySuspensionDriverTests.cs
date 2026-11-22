// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Suspension;

/// <summary>Tests for DummySuspensionDriver.</summary>
public class DummySuspensionDriverTests
{
    /// <summary>Tests that DummySuspensionDriver InvalidateState returns observable.</summary>
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

    /// <summary>Tests that DummySuspensionDriver LoadState returns observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriver_LoadState_ReturnsObservable()
    {
        // Arrange
        var driver = new DummySuspensionDriverUnsafe();

        // Act
        var result = driver.LoadState();

        // Assert
        await Assert.That(result).IsNotNull();
    }

    /// <summary>Tests that DummySuspensionDriver SaveState handles null state.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriver_SaveState_NullState_ReturnsObservable()
    {
        // Arrange
        var driver = new DummySuspensionDriverUnsafe();

        // Act
        var result = driver.SaveState<object>(null!);

        // Assert
        await Assert.That(result).IsNotNull();
    }

    /// <summary>Tests that DummySuspensionDriver SaveState returns observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriver_SaveState_ReturnsObservable()
    {
        // Arrange
        var driver = new DummySuspensionDriverUnsafe();
        var state = new { TestProperty = "test" };

        // Act
        var result = driver.SaveState(state);

        // Assert
        await Assert.That(result).IsNotNull();
    }

    /// <summary>Tests that DummySuspensionDriverUnsafe typed LoadState returns a default observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriverUnsafe_TypedLoadState_ReturnsDefaultObservable()
    {
        var driver = new DummySuspensionDriverUnsafe();
        var received = (string?)"initial";

        using var subscription = driver.LoadState<string>(null!).Subscribe(value => received = value);

        await Assert.That(received).IsNull();
    }

    /// <summary>Tests that DummySuspensionDriverUnsafe typed SaveState returns a unit observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriverUnsafe_TypedSaveState_ReturnsUnitObservable()
    {
        var driver = new DummySuspensionDriverUnsafe();
        var called = false;

        using var subscription = driver.SaveState("state", null!).Subscribe(_ => called = true);

        await Assert.That(called).IsTrue();
    }
}
