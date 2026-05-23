// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Testing.Tests;

/// <summary>
/// Tests the <see cref="TestFixtureBuilder"/> extension methods.
/// </summary>
public sealed class TestFixtureBuilderExtensionTests
{
    private const string TestingValue = "testing";

    /// <summary>
    /// Gets the data.
    /// </summary>
    /// <returns>The data.</returns>
    public static IEnumerable<(string test1, string test2, string test3)> Data()
    {
        yield return (TestingValue, string.Empty, string.Empty);
        yield return (TestingValue, TestingValue, string.Empty);
        yield return (TestingValue, TestingValue, "one");
        yield return (TestingValue, "one", "two");
    }

    /// <summary>
    /// Gets the key values.
    /// </summary>
    /// <returns>The key values.</returns>
    public static IEnumerable<(string key, string value)> KeyValues()
    {
        yield return (TestingValue, string.Empty);
        yield return (TestingValue, "one");
        yield return (TestingValue, "two");
        yield return (TestingValue, "one two");
    }

    /// <summary>
    /// Gets the key values test case.
    /// </summary>
    /// <returns>The values.</returns>
    public static IEnumerable<KeyValuePair<string, string>> KeyValuePairs()
    {
        yield return new("latch", "key");
        yield return new("skeleton", "key");
        yield return new("electronic", "key");
        yield return new("rsa", "key");
    }

    /// <summary>
    /// Verifies a dictionary is added to the <see cref="TestFixture"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Should_Add_Dictionary()
    {
        // Given, When
        var dictionary = new Dictionary<string, string> { { "check", "one" }, { TestingValue, "two" } };

        TestFixture builder = new TestFixtureBuilder()
            .WithDictionary(dictionary);

        // Then
        await Assert.That(builder.Variables!).IsNotNull();
        await Assert.That(builder.Variables!).IsEquivalentTo(dictionary);
    }

    /// <summary>
    /// Verifies a key/value pair is added to the <see cref="TestFixture"/>.
    /// </summary>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to associate with the key.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [MethodDataSource(nameof(KeyValues))]
    public async Task Should_Add_Key_Value(string key, string value)
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithKeyValue(key, value);

        // Then
        await Assert.That(builder.Variables!).IsNotNull();
        await Assert.That(builder.Variables!).ContainsKey(key);
        await Assert.That(builder.Variables![key]).IsEqualTo(value);
    }

    /// <summary>
    /// Verifies a key/value pair is added to the <see cref="TestFixture"/>.
    /// </summary>
    /// <param name="keyValuePair">The key/value pair to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [MethodDataSource(nameof(KeyValuePairs))]
    public async Task Should_Add_Key_Value_Pair(KeyValuePair<string, string> keyValuePair)
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithKeyValue(keyValuePair);

        // Then
        await Assert.That(builder.Variables!).IsNotNull();
        await Assert.That(builder.Variables!).ContainsKey(keyValuePair.Key);
        await Assert.That(builder.Variables![keyValuePair.Key]).IsEqualTo(keyValuePair.Value);
    }

    /// <summary>
    /// Verifies a range of values are added to <see cref="TestFixture.Tests"/>.
    /// </summary>
    /// <param name="test1">The first test value.</param>
    /// <param name="test2">The second test value.</param>
    /// <param name="test3">The third test value.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [MethodDataSource(nameof(Data))]
    public async Task Should_Add_Range_To_List(string test1, string test2, string test3)
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithTests([test1, test2, test3]);

        // Then
        await Assert.That(builder.Tests).IsEquivalentTo([test1, test2, test3]);
    }

    /// <summary>
    /// Verifies a single value is added to <see cref="TestFixture.Tests"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Should_Add_Value_To_List()
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithTest(TestingValue);

        // Then
        await Assert.That(builder.Tests).IsEquivalentTo([TestingValue]);
    }

    /// <summary>
    /// Verifies the <see cref="TestFixture"/> count is correctly returned.
    /// </summary>
    /// <param name="count">The expected count of the <see cref="TestFixture"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(1)]
    [Arguments(100)]
    [Arguments(1000)]
    [Arguments(10000)]
    [Arguments(100000)]
    public async Task Should_Return_Count(int count)
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithCount(count);

        // Then
        await Assert.That(builder.Count).IsEqualTo(count);
    }

    /// <summary>
    /// Verifies that the <see cref="TestFixture"/> is assigned the expected name.
    /// </summary>
    /// <param name="name">The expected name to be verified.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Arguments("ReactiveUI")]
    [Arguments("Splat")]
    [Arguments("Sextant")]
    [Arguments("Akavache")]
    public async Task Should_Return_Name(string name)
    {
        // Given, When
        TestFixture builder = new TestFixtureBuilder().WithName(name);

        // Then
        await Assert.That(builder.Name).IsEqualTo(name);
    }
}
