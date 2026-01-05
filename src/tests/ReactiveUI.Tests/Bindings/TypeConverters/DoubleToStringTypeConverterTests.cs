// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class DoubleToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns10()
    {
        var converter = new DoubleToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task TryConvert_DoubleToString_Succeeds()
    {
        var converter = new DoubleToStringTypeConverter();
        double value = 123.456;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString());
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new DoubleToStringTypeConverter();
        double value = 42.5;

        var result = converter.TryConvert(value, 2, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("42.50");
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new DoubleToStringTypeConverter();
        double value = double.MinValue;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(double.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new DoubleToStringTypeConverter();
        double value = double.MaxValue;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(double.MaxValue.ToString());
    }

    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new DoubleToStringTypeConverter();
        double value = -123.456;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString());
    }

    [Test]
    public async Task TryConvert_WithStringFormatHint_ScientificFormat()
    {
        var converter = new DoubleToStringTypeConverter();
        double value = 12345.6789;

        var result = converter.TryConvert(value, "E3", out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString("E3"));
    }

    [Test]
    public async Task TryConvert_WithStringFormatHint_GeneralFormat()
    {
        var converter = new DoubleToStringTypeConverter();
        double value = 123.456;

        var result = converter.TryConvert(value, "G", out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString("G"));
    }

    [Test]
    public async Task TryConvert_WithStringFormatHint_RoundTripFormat()
    {
        var converter = new DoubleToStringTypeConverter();
        double value = 123.456789012345;

        var result = converter.TryConvert(value, "R", out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString("R"));
    }

    [Test]
    public async Task TryConvert_WithStringFormatHint_CustomPrecision()
    {
        var converter = new DoubleToStringTypeConverter();
        double value = 0.123456789;

        var result = converter.TryConvert(value, "0.0000", out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString("0.0000"));
    }
}
