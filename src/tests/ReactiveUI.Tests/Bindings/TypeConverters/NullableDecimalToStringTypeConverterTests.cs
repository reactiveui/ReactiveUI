// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting nullable decimal to strings.
/// </summary>
public class NullableDecimalToStringTypeConverterTests
{
    /// <summary>
    /// Verifies that the converter reports an affinity of 2.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>
    /// Verifies that converting a nullable decimal to a string succeeds.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_DecimalNullableToString_Succeeds()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        const decimal Value = 123.456m;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123.456");
    }

    /// <summary>
    /// Verifies that converting the maximum value succeeds.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        decimal? value = decimal.MaxValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(decimal.MaxValue.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Verifies that converting the minimum value succeeds.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        decimal? value = decimal.MinValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(decimal.MinValue.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Verifies that converting a negative value succeeds.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        const decimal Value = -123.456m;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("-123.456");
    }

    /// <summary>
    /// Verifies that converting a null value succeeds.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_NullValue_ReturnsTrue()
    {
        var converter = new NullableDecimalToStringTypeConverter();

        var result = converter.TryConvert(null, null, out _);

        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// Verifies that a conversion hint is used to format the output.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        const decimal Value = 42.5m;
        const int DecimalPlaces = 2;

        var result = converter.TryConvert(Value, DecimalPlaces, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("42.50");
    }

    /// <summary>
    /// Verifies that converting zero succeeds.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_Zero_Succeeds()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        decimal? value = 0m;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("0");
    }
}
