// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities;

/// <summary>
///     Tests for CompatMixins utility methods.
/// </summary>
public class CompatMixinsTests
{
    /// <summary>
    ///     Tests that Run extension method processes all items.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Run_ProcessesAllItems()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };
        var processedItems = new List<int>();

        // Act
        items.Run(x => processedItems.Add(x * 2));

        // Assert
        await Assert.That(processedItems).IsEquivalentTo([2, 4, 6, 8, 10]);
    }

    /// <summary>
    ///     Tests that SkipLast with count greater than collection returns empty.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SkipLast_CountGreaterThanCollection_ReturnsEmpty()
    {
        // Arrange
        var items = new[] { 1, 2, 3 };

        // Act
        var result = items.SkipLast(10).ToList();

        // Assert
        await Assert.That(result).IsEmpty();
    }

    /// <summary>
    ///     Tests that SkipLast extension method removes last N items.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SkipLast_RemovesLastNItems()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = items.SkipLast(2).ToList();

        // Assert
        await Assert.That(result).IsEquivalentTo([1, 2, 3]);
    }

    /// <summary>
    ///     Tests that SkipLast with zero count returns all items.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SkipLast_ZeroCount_ReturnsAllItems()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = items.SkipLast(0).ToList();

        // Assert
        await Assert.That(result).IsEquivalentTo([1, 2, 3, 4, 5]);
    }
}
