// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Tests.Comparers;

public class OrderedComparerTests
{
    /// <summary>
    ///     Test for checking that chaining the onto regular IComparable works.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChainOntoRegularIComparables()
    {
        var items = new List<string> { "aaa", "AAA", "abb", "aaaa" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        items.Sort(comparer);
        await Assert.That(
            items.SequenceEqual(
                ["AAA", "aaa", "aaaa", "abb"],
                StringComparer.OrdinalIgnoreCase)).IsTrue();

        items.Sort(
            comparer.ThenByDescending(
                static x => x,
                StringComparer.Ordinal));
        await Assert.That(
            items.SequenceEqual(
                ["aaa", "AAA", "aaaa", "abb"],
                StringComparer.Ordinal)).IsTrue();
    }

    /// <summary>
    ///     A test which determines if customer comparers work.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CustomComparerTest()
    {
        List<string> items = ["aaa", "AAA", "abb", "aaaa"];

        items.Sort(
            OrderedComparer<string>.OrderBy(
                static x => x,
                StringComparer.Ordinal));
        await Assert.That(items.SequenceEqual(["AAA", "aaa", "aaaa", "abb"])).IsTrue();

        items.Sort(
            OrderedComparer<string>.OrderByDescending(static x => x.Length).ThenBy(
                static x => x,
                StringComparer.Ordinal));
        await Assert.That(items.SequenceEqual(["aaaa", "AAA", "aaa", "abb"])).IsTrue();

        items.Sort(
            OrderedComparer<string>.OrderBy(static x => x.Length).ThenBy(
                static x => x,
                StringComparer.Ordinal));
        await Assert.That(items.SequenceEqual(["AAA", "aaa", "abb", "aaaa"])).IsTrue();

        items.Sort(
            OrderedComparer<string>.OrderBy(static x => x.Length).ThenBy(
                static x => x,
                StringComparer.OrdinalIgnoreCase));
        await Assert.That(
            items.SequenceEqual(
                ["AAA", "AAA", "abb", "aaaa"],
                StringComparer.OrdinalIgnoreCase)).IsTrue();
    }

    /// <summary>
    ///     A general smoke test.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SmokeTest()
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
        await Assert.That(employees.SequenceEqual([adam, alice, bob, carol, xavier])).IsTrue();

        employees.Sort(
            OrderedComparer<Employee>
                .OrderByDescending(static x => x.Age)
                .ThenBy(static x => x.Name));
        await Assert.That(employees.SequenceEqual([adam, carol, xavier, bob, alice])).IsTrue();

        employees.Sort(
            OrderedComparer<Employee>
                .OrderByDescending(static x => x.Salary)
                .ThenBy(
                    static x => x.Name,
                    StringComparer.OrdinalIgnoreCase));
        await Assert.That(employees.SequenceEqual([adam, alice, carol, xavier, bob])).IsTrue();

        employees.Sort(
            OrderedComparer<Employee>
                .OrderByDescending(static x => x.Age)
                .ThenByDescending(static x => x.Salary)
                .ThenBy(static x => x.Name));
        await Assert.That(employees.SequenceEqual([adam, carol, xavier, bob, alice])).IsTrue();
    }

    /// <summary>
    ///     Test that checks it works with anonymous types.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WorksWithAnonymousTypes()
    {
        var source = new List<string> { "abc", "bcd", "cde" };
        var items = source.ConvertAll(static x => new { FirstLetter = x[0], AllOfIt = x });

        items.Sort(OrderedComparer.For(items).OrderBy(static x => x.FirstLetter));
        await Assert.That(items.Select(static x => x.FirstLetter).SequenceEqual("abc")).IsTrue();
    }

    [DebuggerDisplay("{Name}")]
    private class Employee
    {
        public int Age { get; init; }

        public string? Name { get; init; }

        public int Salary { get; init; }
    }
}
