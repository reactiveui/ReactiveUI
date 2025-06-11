// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the ordered comparer.
/// </summary>
public class OrderedComparerTests
{
    /// <summary>
    /// A general smoke test.
    /// </summary>
    [Fact]
    public void SmokeTest()
    {
        var adam = new Employee { Name = "Adam", Age = 50, Salary = 125 };
        var alice = new Employee { Name = "Alice", Age = 25, Salary = 100 };
        var bob = new Employee { Name = "Bob", Age = 30, Salary = 75 };
        var carol = new Employee { Name = "Carol", Age = 35, Salary = 100 };
        var xavier = new Employee { Name = "Xavier", Age = 35, Salary = 100 };

        var employees = new List<Employee> { adam, alice, bob, carol, xavier };

        employees.Sort(OrderedComparer<Employee>.OrderBy(x => x.Name));
        Assert.True(employees.SequenceEqual(new[] { adam, alice, bob, carol, xavier }));

        employees.Sort(OrderedComparer<Employee>
            .OrderByDescending(x => x.Age)
            .ThenBy(x => x.Name));
        Assert.True(employees.SequenceEqual(new[] { adam, carol, xavier, bob, alice }));

        employees.Sort(OrderedComparer<Employee>
            .OrderByDescending(x => x.Salary)
            .ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase));
        Assert.True(employees.SequenceEqual(new[] { adam, alice, carol, xavier, bob }));

        employees.Sort(OrderedComparer<Employee>
            .OrderByDescending(x => x.Age)
            .ThenByDescending(x => x.Salary)
            .ThenBy(x => x.Name));
        Assert.True(employees.SequenceEqual(new[] { adam, carol, xavier, bob, alice }));
    }

    /// <summary>
    /// A test which determines if customer comparers work.
    /// </summary>
    [Fact]
    public void CustomComparerTest()
    {
        var items = new List<string> { "aaa", "AAA", "abb", "aaaa" };

        items.Sort(OrderedComparer<string>.OrderBy(x => x, StringComparer.Ordinal));
        Assert.True(items.SequenceEqual(new[] { "AAA", "aaa", "aaaa", "abb" }));

        items.Sort(OrderedComparer<string>.OrderByDescending(x => x.Length).ThenBy(x => x, StringComparer.Ordinal));
        Assert.True(items.SequenceEqual(new[] { "aaaa", "AAA", "aaa", "abb" }));

        items.Sort(OrderedComparer<string>.OrderBy(x => x.Length).ThenBy(x => x, StringComparer.Ordinal));
        Assert.True(items.SequenceEqual(new[] { "AAA", "aaa", "abb", "aaaa" }));

        items.Sort(OrderedComparer<string>.OrderBy(x => x.Length).ThenBy(x => x, StringComparer.OrdinalIgnoreCase));
        Assert.True(items.SequenceEqual(new[] { "AAA", "AAA", "abb", "aaaa" }, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Test for checking that chaining the onto regular IComparable works.
    /// </summary>
    [Fact]
    public void ChainOntoRegularIComparables()
    {
        var items = new List<string> { "aaa", "AAA", "abb", "aaaa" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        items.Sort(comparer);
        Assert.True(items.SequenceEqual(new[] { "AAA", "aaa", "aaaa", "abb" }, StringComparer.OrdinalIgnoreCase));

        items.Sort(comparer.ThenByDescending(x => x, StringComparer.Ordinal));
        Assert.True(items.SequenceEqual(new[] { "aaa", "AAA", "aaaa", "abb" }, StringComparer.Ordinal));
    }

    /// <summary>
    /// Test that checks it works with anonymous types.
    /// </summary>
    [Fact]
    public void WorksWithAnonymousTypes()
    {
        var source = new List<string> { "abc", "bcd", "cde" };
        var items = source.ConvertAll(x => new { FirstLetter = x[0], AllOfIt = x });

        items.Sort(OrderedComparer.For(items).OrderBy(x => x.FirstLetter));
        Assert.True(items.Select(x => x.FirstLetter).SequenceEqual("abc"));
    }

    [DebuggerDisplay("{Name}")]
    private class Employee
    {
        public string? Name { get; set; }

        public int Age { get; set; }

        public int Salary { get; set; }
    }
}
