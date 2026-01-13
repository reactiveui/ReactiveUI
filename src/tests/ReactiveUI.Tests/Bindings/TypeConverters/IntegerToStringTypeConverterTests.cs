// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
///     Tests for converting integers to strings.
/// </summary>
public class IntegerToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new IntegerToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_IntToString_Succeeds()
    {
        var converter = new IntegerToStringTypeConverter();
        var value = 123456;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123456");
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new IntegerToStringTypeConverter();
        var value = int.MaxValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(int.MaxValue.ToString());
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new IntegerToStringTypeConverter();
        var value = int.MinValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(int.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new IntegerToStringTypeConverter();
        var value = -123456;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("-123456");
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new IntegerToStringTypeConverter();
        var value = 42;

        var result = converter.TryConvert(value, 8, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("00000042");
    }

    [Test]
    public async Task TryConvert_WithStringFormatHint_CustomFormat()
    {
        var converter = new IntegerToStringTypeConverter();
        var value = 42;

        var result = converter.TryConvert(value, "000", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("042");
    }

    [Test]
    public async Task TryConvert_WithStringFormatHint_HexFormat()
    {
        var converter = new IntegerToStringTypeConverter();
        var value = 255;

        var result = converter.TryConvert(value, "X", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("FF");
    }

    [Test]
    public async Task TryConvert_WithStringFormatHint_HexFormatLowercase()
    {
        var converter = new IntegerToStringTypeConverter();
        var value = 255;

        var result = converter.TryConvert(value, "x8", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("000000ff");
    }

    [Test]
    public async Task TryConvert_WithStringFormatHint_NumberFormat()
    {
        var converter = new IntegerToStringTypeConverter();
        var value = 1234567;

        var result = converter.TryConvert(value, "N0", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString("N0"));
    }
}
