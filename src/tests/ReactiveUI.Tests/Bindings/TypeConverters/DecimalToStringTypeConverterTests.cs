// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class DecimalToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns10()
    {
        var converter = new DecimalToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task TryConvert_DecimalToString_Succeeds()
    {
        var converter = new DecimalToStringTypeConverter();
        decimal value = 123.456m;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123.456");
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new DecimalToStringTypeConverter();
        decimal value = 42.5m;

        var result = converter.TryConvert(value, 2, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("42.50");
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new DecimalToStringTypeConverter();
        decimal value = decimal.MinValue;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(decimal.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new DecimalToStringTypeConverter();
        decimal value = decimal.MaxValue;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(decimal.MaxValue.ToString());
    }

    [Test]
    public async Task TryConvert_Zero_Succeeds()
    {
        var converter = new DecimalToStringTypeConverter();
        decimal value = 0m;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("0");
    }

    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new DecimalToStringTypeConverter();
        decimal value = -123.456m;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("-123.456");
    }

    [Test]
    public async Task TryConvert_WithStringFormatHint_FormatsCorrectly()
    {
        var converter = new DecimalToStringTypeConverter();
        decimal value = 1234.5678m;

        var result = converter.TryConvert(value, "N2", out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString("N2"));
    }

    [Test]
    public async Task TryConvert_WithStringFormatHint_CurrencyFormat()
    {
        var converter = new DecimalToStringTypeConverter();
        decimal value = 1234.56m;

        var result = converter.TryConvert(value, "C", out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString("C"));
    }

    [Test]
    public async Task TryConvert_WithStringFormatHint_PercentFormat()
    {
        var converter = new DecimalToStringTypeConverter();
        decimal value = 0.1234m;

        var result = converter.TryConvert(value, "P2", out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString("P2"));
    }

    [Test]
    public async Task TryConvert_WithStringFormatHint_ExponentialFormat()
    {
        var converter = new DecimalToStringTypeConverter();
        decimal value = 1234.5678m;

        var result = converter.TryConvert(value, "E2", out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString("E2"));
    }
}
