// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class NullableDecimalToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_DecimalNullableToString_Returns10()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(decimal?), typeof(string));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_StringToDecimalNullable_Returns10()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(decimal?));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_WrongTypes_Returns0()
    {
        var converter = new NullableDecimalToStringTypeConverter();

        await Assert.That(converter.GetAffinityForObjects(typeof(int), typeof(string))).IsEqualTo(0);
        await Assert.That(converter.GetAffinityForObjects(typeof(string), typeof(int))).IsEqualTo(0);
        await Assert.That(converter.GetAffinityForObjects(typeof(decimal), typeof(string))).IsEqualTo(0);
    }

    [Test]
    public async Task TryConvert_DecimalNullableToString_Succeeds()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        decimal? value = 123.456m;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123.456");
    }

    [Test]
    public async Task TryConvert_StringToDecimalNullable_Succeeds()
    {
        var converter = new NullableDecimalToStringTypeConverter();

        var result = converter.TryConvert("123.456", typeof(decimal?), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(123.456m);
    }

    [Test]
    public async Task TryConvert_NullValue_ReturnsTrue()
    {
        var converter = new NullableDecimalToStringTypeConverter();

        var result = converter.TryConvert(null, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsTrue()
    {
        var converter = new NullableDecimalToStringTypeConverter();

        var result = converter.TryConvert(string.Empty, typeof(decimal?), null, out var output);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new NullableDecimalToStringTypeConverter();

        var result = converter.TryConvert("invalid", typeof(decimal?), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        decimal? value = 42.5m;

        var result = converter.TryConvert(value, typeof(string), 2, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("42.50");
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        decimal? value = decimal.MinValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(decimal.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        decimal? value = decimal.MaxValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(decimal.MaxValue.ToString());
    }

    [Test]
    public async Task TryConvert_Zero_Succeeds()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        decimal? value = 0m;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("0");
    }

    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new NullableDecimalToStringTypeConverter();
        decimal? value = -123.456m;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("-123.456");
    }

    [Test]
    public async Task TryConvert_StringToDecimalWithRounding_RoundsCorrectly()
    {
        var converter = new NullableDecimalToStringTypeConverter();

        var result = converter.TryConvert("123.456789", typeof(decimal?), 2, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(123.46m);
    }
}
