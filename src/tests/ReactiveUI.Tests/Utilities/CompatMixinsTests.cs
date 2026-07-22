// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities;

/// <summary>Tests for CompatMixins utility methods.</summary>
public class CompatMixinsTests
{
    /// <summary>Tests that Run extension method processes all items.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Run_ProcessesAllItems()
    {
        // Arrange
        const int Multiplier = 2;
        const int SequenceLength = 5;
        var items = Enumerable.Range(1, SequenceLength);
        var processedItems = new List<int>();

        // Act
        items.Run(x => processedItems.Add(x * Multiplier));

        // Assert
        await Assert.That(processedItems).IsEquivalentTo(Enumerable.Range(1, SequenceLength).Select(static x => x * Multiplier));
    }

    /// <summary>Tests that SkipLast with count greater than collection returns empty.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SkipLast_CountGreaterThanCollection_ReturnsEmpty()
    {
        // Arrange
        const int SkipCount = 10;
        const int SequenceLength = 3;
        var items = Enumerable.Range(1, SequenceLength);

        // Act
        var result = items.SkipLast(SkipCount).ToList();

        // Assert
        await Assert.That(result).IsEmpty();
    }

    /// <summary>Tests that SkipLast extension method removes last N items.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SkipLast_RemovesLastNItems()
    {
        // Arrange
        const int SkipCount = 2;
        const int SequenceLength = 5;
        const int ExpectedLength = 3;
        var items = Enumerable.Range(1, SequenceLength);

        // Act
        var result = items.SkipLast(SkipCount).ToList();

        // Assert
        await Assert.That(result).IsEquivalentTo(Enumerable.Range(1, ExpectedLength));
    }

    /// <summary>Tests that SkipLast with zero count returns all items.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SkipLast_ZeroCount_ReturnsAllItems()
    {
        // Arrange
        const int SequenceLength = 5;
        var items = Enumerable.Range(1, SequenceLength);

        // Act
        var result = items.SkipLast(0).ToList();

        // Assert
        await Assert.That(result).IsEquivalentTo(Enumerable.Range(1, SequenceLength));
    }
}
