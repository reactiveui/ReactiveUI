// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting decimals to strings.
/// </summary>
public class DecimalToStringTypeConverterTests
{
    /// <summary>
    /// Verifies the converter reports an affinity of 2.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new DecimalToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>
    /// Verifies that a decimal value converts to its string representation.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_DecimalToString_Succeeds()
    {
        var converter = new DecimalToStringTypeConverter();
        const decimal Value = 123.456m;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123.456");
    }

    /// <summary>
    /// Verifies that the maximum decimal value converts successfully.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new DecimalToStringTypeConverter();
        const decimal Value = decimal.MaxValue;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(decimal.MaxValue.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Verifies that the minimum decimal value converts successfully.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new DecimalToStringTypeConverter();
        const decimal Value = decimal.MinValue;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(decimal.MinValue.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Verifies that a negative decimal value converts successfully.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new DecimalToStringTypeConverter();
        const decimal Value = -123.456m;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("-123.456");
    }

    /// <summary>
    /// Verifies that a numeric conversion hint applies the expected formatting.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new DecimalToStringTypeConverter();
        const decimal Value = 42.5m;
        const int DecimalPlaces = 2;

        var result = converter.TryConvert(Value, DecimalPlaces, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("42.50");
    }

    /// <summary>
    /// Verifies that a currency format string hint applies the expected formatting.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithStringFormatHint_CurrencyFormat()
    {
        var converter = new DecimalToStringTypeConverter();
        const decimal Value = 1234.56m;

        var result = converter.TryConvert(Value, "C", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString("C", CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Verifies that an exponential format string hint applies the expected formatting.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithStringFormatHint_ExponentialFormat()
    {
        var converter = new DecimalToStringTypeConverter();
        const decimal Value = 1234.5678m;

        var result = converter.TryConvert(Value, "E2", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString("E2", CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Verifies that a numeric format string hint applies the expected formatting.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithStringFormatHint_FormatsCorrectly()
    {
        var converter = new DecimalToStringTypeConverter();
        const decimal Value = 1234.5678m;

        var result = converter.TryConvert(Value, "N2", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString("N2", CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Verifies that a percent format string hint applies the expected formatting.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithStringFormatHint_PercentFormat()
    {
        var converter = new DecimalToStringTypeConverter();
        const decimal Value = 0.1234m;

        var result = converter.TryConvert(Value, "P2", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString("P2", CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Verifies that a zero decimal value converts successfully.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_Zero_Succeeds()
    {
        var converter = new DecimalToStringTypeConverter();
        const decimal Value = 0m;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("0");
    }
}
