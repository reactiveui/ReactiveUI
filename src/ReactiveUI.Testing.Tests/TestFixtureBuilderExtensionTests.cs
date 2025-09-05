// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Testing.Tests;

/// <summary>
/// Test for <see cref="IBuilderExtensions"/>.
/// </summary>
[TestFixture]
public sealed class TestFixtureBuilderExtensionTests
{
    /// <summary>
    /// Gets data for the test execution.
    /// </summary>
    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] { "testing", string.Empty, string.Empty },
            new object[] { "testing", "testing", string.Empty },
            new object[] { "testing", "testing", "one" },
            new object[] { "testing", "one", "two" }
        };

    /// <summary>
    /// Gets key value for the test execution.
    /// </summary>
    public static IEnumerable<object[]> KeyValues =>
        new List<object[]>
        {
            new object[] { "testing", string.Empty },
            new object[] { "testing", "one" },
            new object[] { "testing", "two" },
            new object[] { "testing", "one two" }
        };

    /// <summary>
    /// Gets key value pairs for the test execution.
    /// </summary>
    public static IEnumerable<object[]> KeyValuePairs => new List<object[]>
    {
        new object[] { new KeyValuePair<string, string>("latch", "key") },
        new object[] { new KeyValuePair<string, string>("skeleton", "key") },
        new object[] { new KeyValuePair<string, string>("electronic", "key") },
        new object[] { new KeyValuePair<string, string>("rsa", "key") }
    };

    /// <summary>
    /// A test to verify the a dictionary is added to the <see cref="TestFixture"/>.
    /// </summary>
    [Test]
    public void Should_Add_Dictionary()
    {
        // Given, When
        var dictionary = new Dictionary<string, string>
        {
            { "check", "one" },
            { "testing", "two" }
        };
        TestFixture builder =
            new TestFixtureBuilder()
                .WithDictionary(dictionary);

        // Then
        Assert.That(builder.Variables, Is.EqualTo(dictionary));
        Assert.That(builder.Variables, Is.EqualTo(dictionary));
    }

    /// <summary>
    /// A test to verify the key value pairs are added to the <see cref="TestFixture"/>.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    [Test]
    [TestCaseSource(nameof(KeyValues))]
    public void Should_Add_Key_Value(string key, string value)
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithKeyValue(key, value);

        // Then
        Assert.That(builder.Variables?[key], Is.EqualTo(value));
    }

    /// <summary>
    /// A test to verify the key value pairs are added to the <see cref="TestFixture"/>.
    /// </summary>
    /// <param name="keyValuePair">The key value pair.</param>
    [Test]
    [TestCaseSource(nameof(KeyValuePairs))]
    public void Should_Add_Key_Value_Pair(KeyValuePair<string, string> keyValuePair)
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithKeyValue(keyValuePair);

        // Then
        Assert.That(builder.Variables?[keyValuePair.Key], Is.EqualTo(keyValuePair.Value));
    }

    /// <summary>
    /// A test to verify a range of values are added to the <see cref="TestFixture"/>.
    /// </summary>
    /// <param name="test1">The first test.</param>
    /// <param name="test2">The second test.</param>
    /// <param name="test3">The third test.</param>
    [Test]
    [MemberData(nameof(Data))]
    public void Should_Add_Range_To_List(string test1, string test2, string test3)
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithTests(new[] { test1, test2, test3 });

        // Then
        builder.Tests; Assert.That(.Should().BeEquivalentTo(new[] { test1, test2, test3 })_placeholder, Is.EqualTo(new[] { test1, test2, test3 }));
    }

    /// <summary>
    /// A test to verify a value added to a list of tests on the <see cref="TestFixture"/>.
    /// </summary>
    [Test]
    public void Should_Add_Value_To_List()
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithTest("testing");

        // Then
        builder.Tests; Assert.That(.Should().BeEquivalentTo(new[] { "testing" })_placeholder, Is.EqualTo(new[] { "testing" }));
    }

    /// <summary>
    /// A test to verify the <see cref="TestFixture"/> count.
    /// </summary>
    /// <param name="count">The count.</param>
    [Test]
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
    /// A test to verify the <see cref="TestFixture"/> name.
    /// </summary>
    /// <param name="name">The name.</param>
    [Test]
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
