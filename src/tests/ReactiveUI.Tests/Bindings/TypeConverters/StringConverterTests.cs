// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for the StringConverter which converts strings to strings.
/// </summary>
public class StringConverterTests
{
    /// <summary>
    /// Verifies the converter source type is <see cref="string"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task FromType_ReturnsStringType()
    {
        var converter = new StringConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(string));
    }

    /// <summary>
    /// Verifies the converter reports an affinity of 2.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new StringConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>
    /// Verifies the converter target type is <see cref="string"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ToType_ReturnsStringType()
    {
        var converter = new StringConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(string));
    }

    /// <summary>
    /// Verifies that converting an empty string succeeds.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_EmptyString_Succeeds()
    {
        var converter = new StringConverter();
        var value = string.Empty;

        var result = converter.TryConvertTyped(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(string.Empty);
    }

    /// <summary>
    /// Verifies that the conversion hint is ignored during conversion.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_IgnoresConversionHint()
    {
        var converter = new StringConverter();
        const string Value = "test";

        var result = converter.TryConvertTyped(Value, "some hint", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("test");
    }

    /// <summary>
    /// Verifies that a non-string value fails to convert.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_NonStringValue_ReturnsFalse()
    {
        var converter = new StringConverter();
        const int Value = 123;

        var result = converter.TryConvertTyped(Value, null, out _);

        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Verifies that a null value fails to convert and produces null output.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_NullValue_ReturnsFalseAndNull()
    {
        var converter = new StringConverter();

        var result = converter.TryConvertTyped(null, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsNull();
    }

    /// <summary>
    /// Verifies that converting a string to a string succeeds.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_StringToString_Succeeds()
    {
        var converter = new StringConverter();
        const string Value = "test";

        var result = converter.TryConvertTyped(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("test");
    }

    /// <summary>
    /// A helper object used to exercise non-string conversion scenarios.
    /// </summary>
    private sealed class TestObject
    {
        /// <summary>
        /// Gets the value associated with this test object.
        /// </summary>
        public string Value { get; } = string.Empty;

        /// <inheritdoc/>
        public override string ToString() => $"TestObject: {Value}";
    }
}
