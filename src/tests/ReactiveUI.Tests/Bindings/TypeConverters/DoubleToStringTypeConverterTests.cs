// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>Tests for converting doubles to strings.</summary>
public class DoubleToStringTypeConverterTests
{
    /// <summary>Verifies the converter reports an affinity of 2.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new DoubleToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>Verifies that a double value converts to its string representation.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_DoubleToString_Succeeds()
    {
        var converter = new DoubleToStringTypeConverter();
        const double Value = 123.456;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies that the maximum double value converts successfully.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new DoubleToStringTypeConverter();
        const double Value = double.MaxValue;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(double.MaxValue.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies that the minimum double value converts successfully.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new DoubleToStringTypeConverter();
        const double Value = double.MinValue;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(double.MinValue.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies that a negative double value converts successfully.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new DoubleToStringTypeConverter();
        const double Value = -123.456;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies that a numeric conversion hint applies the expected formatting.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new DoubleToStringTypeConverter();
        const double Value = 42.5;
        const int DecimalPlaces = 2;

        var result = converter.TryConvert(Value, DecimalPlaces, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("42.50");
    }

    /// <summary>Verifies that a custom precision format string hint applies the expected formatting.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithStringFormatHint_CustomPrecision()
    {
        var converter = new DoubleToStringTypeConverter();
        const double Value = 0.123456789;

        var result = converter.TryConvert(Value, "0.0000", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString("0.0000", CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies that a general format string hint applies the expected formatting.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithStringFormatHint_GeneralFormat()
    {
        var converter = new DoubleToStringTypeConverter();
        const double Value = 123.456;

        var result = converter.TryConvert(Value, "G", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString("G", CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies that a round-trip format string hint applies the expected formatting.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithStringFormatHint_RoundTripFormat()
    {
        var converter = new DoubleToStringTypeConverter();
        const double Value = 123.456789012345;

        var result = converter.TryConvert(Value, "R", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString("R", CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies that a scientific format string hint applies the expected formatting.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithStringFormatHint_ScientificFormat()
    {
        var converter = new DoubleToStringTypeConverter();
        const double Value = 12345.6789;

        var result = converter.TryConvert(Value, "E3", out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString("E3", CultureInfo.InvariantCulture));
    }
}
