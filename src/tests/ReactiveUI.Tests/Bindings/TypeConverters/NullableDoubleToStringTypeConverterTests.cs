// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>Tests for converting nullable double to strings.</summary>
public class NullableDoubleToStringTypeConverterTests
{
    /// <summary>Verifies that the converter reports an affinity of 2.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new NullableDoubleToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>Verifies that converting a nullable double to a string succeeds.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_DoubleNullableToString_Succeeds()
    {
        var converter = new NullableDoubleToStringTypeConverter();
        const double Value = 123.456;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>Verifies that converting the maximum value succeeds.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new NullableDoubleToStringTypeConverter();
        double? value = double.MaxValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(double.MaxValue.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies that converting the minimum value succeeds.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new NullableDoubleToStringTypeConverter();
        double? value = double.MinValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(double.MinValue.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies that converting a negative value succeeds.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new NullableDoubleToStringTypeConverter();
        const double Value = -123.456;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>Verifies that converting a null value succeeds.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_NullValue_ReturnsTrue()
    {
        var converter = new NullableDoubleToStringTypeConverter();

        var result = converter.TryConvert(null, null, out _);

        await Assert.That(result).IsTrue();
    }

    /// <summary>Verifies that a conversion hint is used to format the output.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new NullableDoubleToStringTypeConverter();
        const double Value = 42.5;
        const int DecimalPlaces = 2;

        var result = converter.TryConvert(Value, DecimalPlaces, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("42.50");
    }
}
