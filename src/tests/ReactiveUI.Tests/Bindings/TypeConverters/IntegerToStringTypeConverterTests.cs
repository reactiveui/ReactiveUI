// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
///     Tests for converting integers to strings.
/// </summary>
public class IntegerToStringTypeConverterTests
{
    /// <summary>
    /// Verifies the converter reports an affinity of 2.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new IntegerToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>
    /// Verifies that an int value converts to its string representation.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_IntToString_Succeeds()
    {
        var converter = new IntegerToStringTypeConverter();
        const int Value = 123_456;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123456");
    }

    /// <summary>
    /// Verifies that the maximum int value converts successfully.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new IntegerToStringTypeConverter();
        const int Value = int.MaxValue;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(int.MaxValue.ToString());
    }

    /// <summary>
    /// Verifies that the minimum int value converts successfully.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new IntegerToStringTypeConverter();
        const int Value = int.MinValue;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(int.MinValue.ToString());
    }

    /// <summary>
    /// Verifies that a negative int value converts successfully.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new IntegerToStringTypeConverter();
        const int Value = -123_456;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("-123456");
    }

    /// <summary>
    /// Verifies that a numeric conversion hint applies the expected formatting.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new IntegerToStringTypeConverter();
        const int Value = 42;
        const int PaddingWidth = 8;

        var result = converter.TryConvert(Value, PaddingWidth, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("00000042");
    }

    /// <summary>
    /// Verifies that a custom format string hint applies the expected formatting.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithStringFormatHint_CustomFormat()
    {
        var converter = new IntegerToStringTypeConverter();
        const int Value = 42;

        var result = converter.TryConvert(Value, "000", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("042");
    }

    /// <summary>
    /// Verifies that an uppercase hexadecimal format string hint applies the expected formatting.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithStringFormatHint_HexFormat()
    {
        var converter = new IntegerToStringTypeConverter();
        const int Value = 255;

        var result = converter.TryConvert(Value, "X", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("FF");
    }

    /// <summary>
    /// Verifies that a lowercase hexadecimal format string hint applies the expected formatting.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithStringFormatHint_HexFormatLowercase()
    {
        var converter = new IntegerToStringTypeConverter();
        const int Value = 255;

        var result = converter.TryConvert(Value, "x8", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("000000ff");
    }

    /// <summary>
    /// Verifies that a number format string hint applies the expected formatting.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithStringFormatHint_NumberFormat()
    {
        var converter = new IntegerToStringTypeConverter();
        const int Value = 1_234_567;

        var result = converter.TryConvert(Value, "N0", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString("N0"));
    }
}
