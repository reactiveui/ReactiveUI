// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Testing.Tests;

/// <summary>
/// Tests for <see cref="IBuilderExtensions"/>.
/// </summary>
[TestFixture]
public sealed class TestFixtureBuilderExtensionTests
{
    private static readonly object[][] Data =
    [
        ["testing", string.Empty, string.Empty],
        ["testing", "testing", string.Empty],
        ["testing", "testing", "one"],
        ["testing", "one", "two"]
    ];

    private static readonly object[][] KeyValues =
    [
        ["testing", string.Empty],
        ["testing", "one"],
        ["testing", "two"],
        ["testing", "one two"]
    ];

    private static readonly object[][] KeyValuePairs =
    [
        [new KeyValuePair<string, string>("latch", "key")],
        [new KeyValuePair<string, string>("skeleton", "key")],
        [new KeyValuePair<string, string>("electronic", "key")],
        [new KeyValuePair<string, string>("rsa", "key")]
    ];

    /// <summary>
    /// Verifies a dictionary is added to the <see cref="TestFixture"/>.
    /// </summary>
    [Test]
    public void Should_Add_Dictionary()
    {
        // Given, When
        var dictionary = new Dictionary<string, string>
        {
            { "check", "one" },
            { "testing", "two" },
        };

        TestFixture builder = new TestFixtureBuilder()
            .WithDictionary(dictionary);

        // Then
        Assert.That(builder.Variables, Is.Not.Null);
        Assert.That(builder.Variables!, Is.EquivalentTo(dictionary));
    }

    /// <summary>
    /// Verifies a key/value pair is added to the <see cref="TestFixture"/>.
    /// </summary>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to associate with the key.</param>
    [TestCaseSource(nameof(KeyValues))]
    public void Should_Add_Key_Value(string key, string value)
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithKeyValue(key, value);

        // Then
        Assert.That(builder.Variables, Is.Not.Null);
        Assert.That(builder.Variables, Does.ContainKey(key));
        Assert.That(builder.Variables![key], Is.EqualTo(value));
    }

    /// <summary>
    /// Verifies a key/value pair is added to the <see cref="TestFixture"/>.
    /// </summary>
    /// <param name="keyValuePair">The key/value pair to add.</param>
    [TestCaseSource(nameof(KeyValuePairs))]
    public void Should_Add_Key_Value_Pair(KeyValuePair<string, string> keyValuePair)
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithKeyValue(keyValuePair);

        // Then
        Assert.That(builder.Variables, Is.Not.Null);
        Assert.That(builder.Variables, Does.ContainKey(keyValuePair.Key));
        Assert.That(builder.Variables![keyValuePair.Key], Is.EqualTo(keyValuePair.Value));
    }

    /// <summary>
    /// Verifies a range of values are added to <see cref="TestFixture.Tests"/>.
    /// </summary>
    /// <param name="test1">The first test value.</param>
    /// <param name="test2">The second test value.</param>
    /// <param name="test3">The third test value.</param>
    [TestCaseSource(nameof(Data))]
    public void Should_Add_Range_To_List(string test1, string test2, string test3)
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithTests([test1, test2, test3]);

        // Then
        Assert.That(builder.Tests, Is.EqualTo([test1, test2, test3]));
    }

    /// <summary>
    /// Verifies a single value is added to <see cref="TestFixture.Tests"/>.
    /// </summary>
    [Test]
    public void Should_Add_Value_To_List()
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithTest("testing");

        // Then
        Assert.That(builder.Tests, Is.EqualTo(["testing"]));
    }

    /// <summary>
    /// Verifies the <see cref="TestFixture"/> count is correctly returned.
    /// </summary>
    /// <param name="count">The expected count of the <see cref="TestFixture"/>.</param>
    [TestCase(1)]
    [TestCase(100)]
    [TestCase(1000)]
    [TestCase(10000)]
    [TestCase(100000)]
    public void Should_Return_Count(int count)
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithCount(count);

        // Then
        Assert.That(builder.Count, Is.EqualTo(count));
    }

    /// <summary>
    /// Verifies that the <see cref="TestFixture"/> is assigned the expected name.
    /// </summary>
    /// <param name="name">The expected name to be verified.</param>
    [TestCase("ReactiveUI")]
    [TestCase("Splat")]
    [TestCase("Sextant")]
    [TestCase("Akavache")]
    public void Should_Return_Name(string name)
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithName(name);

        // Then
        Assert.That(builder.Name, Is.EqualTo(name));
    }
}
