// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class NullableDoubleToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_DoubleNullableToString_Returns10()
    {
        var converter = new NullableDoubleToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(double?), typeof(string));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_StringToDoubleNullable_Returns10()
    {
        var converter = new NullableDoubleToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(double?));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_WrongTypes_Returns0()
    {
        var converter = new NullableDoubleToStringTypeConverter();

        await Assert.That(converter.GetAffinityForObjects(typeof(int), typeof(string))).IsEqualTo(0);
        await Assert.That(converter.GetAffinityForObjects(typeof(string), typeof(int))).IsEqualTo(0);
        await Assert.That(converter.GetAffinityForObjects(typeof(double), typeof(string))).IsEqualTo(0);
    }

    [Test]
    public async Task TryConvert_DoubleNullableToString_Succeeds()
    {
        var converter = new NullableDoubleToStringTypeConverter();
        double? value = 123.456;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString());
    }

    [Test]
    public async Task TryConvert_StringToDoubleNullable_Succeeds()
    {
        var converter = new NullableDoubleToStringTypeConverter();

        var result = converter.TryConvert("123.456", typeof(double?), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(123.456);
    }

    [Test]
    public async Task TryConvert_NullValue_ReturnsTrue()
    {
        var converter = new NullableDoubleToStringTypeConverter();

        var result = converter.TryConvert(null, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsTrue()
    {
        var converter = new NullableDoubleToStringTypeConverter();

        var result = converter.TryConvert(string.Empty, typeof(double?), null, out var output);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new NullableDoubleToStringTypeConverter();

        var result = converter.TryConvert("invalid", typeof(double?), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new NullableDoubleToStringTypeConverter();
        double? value = 42.5;

        var result = converter.TryConvert(value, typeof(string), 2, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("42.50");
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new NullableDoubleToStringTypeConverter();
        double? value = double.MinValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(double.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new NullableDoubleToStringTypeConverter();
        double? value = double.MaxValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(double.MaxValue.ToString());
    }

    [Test]
    public async Task TryConvert_ScientificNotation_Succeeds()
    {
        var converter = new NullableDoubleToStringTypeConverter();

        var result = converter.TryConvert("1.23E+10", typeof(double?), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(1.23E+10);
    }

    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new NullableDoubleToStringTypeConverter();
        double? value = -123.456;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(value.ToString());
    }

    [Test]
    public async Task TryConvert_StringToDoubleWithRounding_RoundsCorrectly()
    {
        var converter = new NullableDoubleToStringTypeConverter();

        var result = converter.TryConvert("123.456789", typeof(double?), 2, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That((double)output!).IsEqualTo(123.46).Within(0.01);
    }
}
