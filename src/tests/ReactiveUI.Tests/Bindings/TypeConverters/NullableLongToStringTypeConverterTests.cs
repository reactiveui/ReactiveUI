// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class NullableLongToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_LongNullableToString_Returns10()
    {
        var converter = new NullableLongToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(long?), typeof(string));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_StringToLongNullable_Returns10()
    {
        var converter = new NullableLongToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(long?));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_WrongTypes_Returns0()
    {
        var converter = new NullableLongToStringTypeConverter();

        await Assert.That(converter.GetAffinityForObjects(typeof(int), typeof(string))).IsEqualTo(0);
        await Assert.That(converter.GetAffinityForObjects(typeof(string), typeof(int))).IsEqualTo(0);
        await Assert.That(converter.GetAffinityForObjects(typeof(long), typeof(string))).IsEqualTo(0);
    }

    [Test]
    public async Task TryConvert_LongNullableToString_Succeeds()
    {
        var converter = new NullableLongToStringTypeConverter();
        long? value = 123456789012;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123456789012");
    }

    [Test]
    public async Task TryConvert_StringToLongNullable_Succeeds()
    {
        var converter = new NullableLongToStringTypeConverter();

        var result = converter.TryConvert("123456789012", typeof(long?), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(123456789012L);
    }

    [Test]
    public async Task TryConvert_NullValue_ReturnsTrue()
    {
        var converter = new NullableLongToStringTypeConverter();

        var result = converter.TryConvert(null, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsTrue()
    {
        var converter = new NullableLongToStringTypeConverter();

        var result = converter.TryConvert(string.Empty, typeof(long?), null, out var output);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new NullableLongToStringTypeConverter();

        var result = converter.TryConvert("invalid", typeof(long?), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_OutOfRangeValue_ReturnsFalse()
    {
        var converter = new NullableLongToStringTypeConverter();

        var result = converter.TryConvert("99999999999999999999", typeof(long?), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new NullableLongToStringTypeConverter();
        long? value = 42;

        var result = converter.TryConvert(value, typeof(string), 10, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("0000000042");
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new NullableLongToStringTypeConverter();
        long? value = long.MinValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(long.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new NullableLongToStringTypeConverter();
        long? value = long.MaxValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(long.MaxValue.ToString());
    }
}
