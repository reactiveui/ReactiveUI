// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
///     Tests for <see cref="ChainedComparer{T}" />.
/// </summary>
public class ChainedComparerTest
{
    /// <summary>
    ///     Tests that Compare returns 0 when both values are null.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Compare_BothNull_ReturnsZero()
    {
        var comparer = new ChainedComparer<string>(null, string.CompareOrdinal);

        var result = comparer.Compare(null, null);

        await Assert.That(result).IsEqualTo(0);
    }

    /// <summary>
    ///     Tests that Compare chains multiple comparers correctly.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Compare_ChainedComparers_WorksCorrectly()
    {
        var parent = Comparer<TestClass>.Create((x, y) => x.Priority.CompareTo(y.Priority));
        var comparer = new ChainedComparer<TestClass>(parent, (x, y) => x.Value.CompareTo(y.Value));

        var obj1 = new TestClass { Priority = 1, Value = 10 };
        var obj2 = new TestClass { Priority = 1, Value = 20 };
        var obj3 = new TestClass { Priority = 2, Value = 5 };

        var result1 = comparer.Compare(obj1, obj2);
        var result2 = comparer.Compare(obj1, obj3);

        await Assert.That(result1).IsLessThan(0);
        await Assert.That(result2).IsLessThan(0);
    }

    /// <summary>
    ///     Tests that Compare uses comparison when parent is null.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Compare_NoParent_UsesComparison()
    {
        var comparer = new ChainedComparer<int>(null, (x, y) => x.CompareTo(y));

        var result = comparer.Compare(1, 2);

        await Assert.That(result).IsLessThan(0);
    }

    /// <summary>
    ///     Tests that Compare uses parent result when non-zero.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Compare_ParentReturnsNonZero_UsesParentResult()
    {
        var parent = Comparer<int>.Create((x, y) => x.CompareTo(y));
        var comparer = new ChainedComparer<int>(parent, (x, y) => 0);

        var result = comparer.Compare(1, 2);

        await Assert.That(result).IsLessThan(0);
    }

    /// <summary>
    ///     Tests that Compare uses comparison when parent returns zero.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Compare_ParentReturnsZero_UsesComparison()
    {
        var parent = Comparer<TestClass>.Create((x, y) => 0);
        var comparer = new ChainedComparer<TestClass>(parent, (x, y) => x.Value.CompareTo(y.Value));

        var result = comparer.Compare(new TestClass { Value = 1 }, new TestClass { Value = 2 });

        await Assert.That(result).IsLessThan(0);
    }

    /// <summary>
    ///     Test class for comparison testing.
    /// </summary>
    private class TestClass
    {
        public int Priority { get; set; }

        public int Value { get; set; }
    }
}
