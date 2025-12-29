// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for <see cref="OrderedComparer"/> and <see cref="OrderedComparer{T}"/>.
/// </summary>
public class OrderedComparerTest
{
    /// <summary>
    /// Tests that For with enumerable returns a builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task For_WithEnumerable_ReturnsBuilder()
    {
        var items = new[] { new TestClass { Value = 1 } };

        var builder = OrderedComparer.For(items);

        await Assert.That(builder).IsNotNull();
    }

    /// <summary>
    /// Tests that For without argument returns a builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task For_Generic_ReturnsBuilder()
    {
        var builder = OrderedComparer.For<TestClass>();

        await Assert.That(builder).IsNotNull();
    }

    /// <summary>
    /// Tests that OrderBy creates a comparer that sorts ascending.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OrderBy_SortsAscending()
    {
        var comparer = OrderedComparer<TestClass>.OrderBy(x => x.Value);
        var obj1 = new TestClass { Value = 1 };
        var obj2 = new TestClass { Value = 2 };

        var result = comparer.Compare(obj1, obj2);

        await Assert.That(result).IsLessThan(0);
    }

    /// <summary>
    /// Tests that OrderBy with custom comparer works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OrderBy_WithCustomComparer_UsesCustomComparer()
    {
        var customComparer = Comparer<int>.Create((x, y) => y.CompareTo(x)); // Reverse order
        var comparer = OrderedComparer<TestClass>.OrderBy(x => x.Value, customComparer);
        var obj1 = new TestClass { Value = 1 };
        var obj2 = new TestClass { Value = 2 };

        var result = comparer.Compare(obj1, obj2);

        await Assert.That(result).IsGreaterThan(0);
    }

    /// <summary>
    /// Tests that OrderByDescending creates a comparer that sorts descending.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OrderByDescending_SortsDescending()
    {
        var comparer = OrderedComparer<TestClass>.OrderByDescending(x => x.Value);
        var obj1 = new TestClass { Value = 1 };
        var obj2 = new TestClass { Value = 2 };

        var result = comparer.Compare(obj1, obj2);

        await Assert.That(result).IsGreaterThan(0);
    }

    /// <summary>
    /// Tests that OrderByDescending with custom comparer works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OrderByDescending_WithCustomComparer_UsesCustomComparer()
    {
        var customComparer = Comparer<int>.Create((x, y) => y.CompareTo(x)); // Reverse order
        var comparer = OrderedComparer<TestClass>.OrderByDescending(x => x.Value, customComparer);
        var obj1 = new TestClass { Value = 1 };
        var obj2 = new TestClass { Value = 2 };

        var result = comparer.Compare(obj1, obj2);

        await Assert.That(result).IsLessThan(0);
    }

    /// <summary>
    /// Tests that builder OrderBy creates a comparer.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Builder_OrderBy_CreatesComparer()
    {
        var builder = OrderedComparer.For<TestClass>();

        var comparer = builder.OrderBy(x => x.Value);

        await Assert.That(comparer).IsNotNull();
    }

    /// <summary>
    /// Tests that builder OrderByDescending creates a comparer.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Builder_OrderByDescending_CreatesComparer()
    {
        var builder = OrderedComparer.For<TestClass>();

        var comparer = builder.OrderByDescending(x => x.Value);

        await Assert.That(comparer).IsNotNull();
    }

    /// <summary>
    /// Test class for comparison testing.
    /// </summary>
    private class TestClass
    {
        public int Value { get; set; }
    }
}
