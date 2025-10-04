// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the ordered comparer.
/// </summary>
[TestFixture]
public class OrderedComparerTests
{
    /// <summary>
    /// A general smoke test.
    /// </summary>
    [Test]
    public void SmokeTest()
    {
        var adam = new Employee { Name = "Adam", Age = 50, Salary = 125 };
        var alice = new Employee { Name = "Alice", Age = 25, Salary = 100 };
        var bob = new Employee { Name = "Bob", Age = 30, Salary = 75 };
        var carol = new Employee { Name = "Carol", Age = 35, Salary = 100 };
        var xavier = new Employee { Name = "Xavier", Age = 35, Salary = 100 };

        var employees = new List<Employee>
        {
            adam,
            alice,
            bob,
            carol,
            xavier
        };

        employees.Sort(OrderedComparer<Employee>.OrderBy(static x => x.Name));
        Assert.That(
                    employees.SequenceEqual([adam, alice, bob, carol, xavier]),
                    Is.True);

        employees.Sort(
                       OrderedComparer<Employee>
                           .OrderByDescending(static x => x.Age)
                           .ThenBy(static x => x.Name));
        Assert.That(
                    employees.SequenceEqual([adam, carol, xavier, bob, alice]),
                    Is.True);

        employees.Sort(
                       OrderedComparer<Employee>
                           .OrderByDescending(static x => x.Salary)
                           .ThenBy(
                                   static x => x.Name,
                                   StringComparer.OrdinalIgnoreCase));
        Assert.That(
                    employees.SequenceEqual([adam, alice, carol, xavier, bob]),
                    Is.True);

        employees.Sort(
                       OrderedComparer<Employee>
                           .OrderByDescending(static x => x.Age)
                           .ThenByDescending(static x => x.Salary)
                           .ThenBy(static x => x.Name));
        Assert.That(
                    employees.SequenceEqual([adam, carol, xavier, bob, alice]),
                    Is.True);
    }

    /// <summary>
    /// A test which determines if customer comparers work.
    /// </summary>
    [Test]
    public void CustomComparerTest()
    {
        List<string> items = ["aaa", "AAA", "abb", "aaaa"];

        items.Sort(
                   OrderedComparer<string>.OrderBy(
                                                   static x => x,
                                                   StringComparer.Ordinal));
        Assert.That(
                    items.SequenceEqual(["AAA", "aaa", "aaaa", "abb"]),
                    Is.True);

        items.Sort(
                   OrderedComparer<string>.OrderByDescending(static x => x.Length).ThenBy(
                    static x => x,
                    StringComparer.Ordinal));
        Assert.That(
                    items.SequenceEqual(["aaaa", "AAA", "aaa", "abb"]),
                    Is.True);

        items.Sort(
                   OrderedComparer<string>.OrderBy(static x => x.Length).ThenBy(
                                                                         static x => x,
                                                                         StringComparer.Ordinal));
        Assert.That(
                    items.SequenceEqual(["AAA", "aaa", "abb", "aaaa"]),
                    Is.True);

        items.Sort(
                   OrderedComparer<string>.OrderBy(static x => x.Length).ThenBy(
                                                                         static x => x,
                                                                         StringComparer.OrdinalIgnoreCase));
        Assert.That(
                    items.SequenceEqual(
                                        ["AAA", "AAA", "abb", "aaaa"],
                                        StringComparer.OrdinalIgnoreCase),
                    Is.True);
    }

    /// <summary>
    /// Test for checking that chaining the onto regular IComparable works.
    /// </summary>
    [Test]
    public void ChainOntoRegularIComparables()
    {
        var items = new List<string> { "aaa", "AAA", "abb", "aaaa" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        items.Sort(comparer);
        Assert.That(
                    items.SequenceEqual(
                                        ["AAA", "aaa", "aaaa", "abb"],
                                        StringComparer.OrdinalIgnoreCase),
                    Is.True);

        items.Sort(
                   comparer.ThenByDescending(
                                             static x => x,
                                             StringComparer.Ordinal));
        Assert.That(
                    items.SequenceEqual(
                                        ["aaa", "AAA", "aaaa", "abb"],
                                        StringComparer.Ordinal),
                    Is.True);
    }

    /// <summary>
    /// Test that checks it works with anonymous types.
    /// </summary>
    [Test]
    public void WorksWithAnonymousTypes()
    {
        var source = new List<string> { "abc", "bcd", "cde" };
        var items = source.ConvertAll(static x => new { FirstLetter = x[0], AllOfIt = x });

        items.Sort(OrderedComparer.For(items).OrderBy(static x => x.FirstLetter));
        Assert.That(
                    items.Select(static x => x.FirstLetter).SequenceEqual("abc"),
                    Is.True);
    }

    [DebuggerDisplay("{Name}")]
    private class Employee
    {
        public string? Name { get; init; }

        public int Age { get; init; }

        public int Salary { get; init; }
    }
}
