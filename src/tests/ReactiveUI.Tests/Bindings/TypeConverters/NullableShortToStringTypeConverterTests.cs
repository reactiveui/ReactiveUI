// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class NullableShortToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_ShortNullableToString_Returns10()
    {
        var converter = new NullableShortToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(short?), typeof(string));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_StringToShortNullable_Returns10()
    {
        var converter = new NullableShortToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(short?));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_WrongTypes_Returns0()
    {
        var converter = new NullableShortToStringTypeConverter();

        await Assert.That(converter.GetAffinityForObjects(typeof(int), typeof(string))).IsEqualTo(0);
        await Assert.That(converter.GetAffinityForObjects(typeof(string), typeof(int))).IsEqualTo(0);
        await Assert.That(converter.GetAffinityForObjects(typeof(short), typeof(string))).IsEqualTo(0);
    }

    [Test]
    public async Task TryConvert_ShortNullableToString_Succeeds()
    {
        var converter = new NullableShortToStringTypeConverter();
        short? value = 12345;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("12345");
    }

    [Test]
    public async Task TryConvert_StringToShortNullable_Succeeds()
    {
        var converter = new NullableShortToStringTypeConverter();

        var result = converter.TryConvert("12345", typeof(short?), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((short)12345);
    }

    [Test]
    public async Task TryConvert_NullValue_ReturnsTrue()
    {
        var converter = new NullableShortToStringTypeConverter();

        var result = converter.TryConvert(null, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsTrue()
    {
        var converter = new NullableShortToStringTypeConverter();

        var result = converter.TryConvert(string.Empty, typeof(short?), null, out var output);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new NullableShortToStringTypeConverter();

        var result = converter.TryConvert("invalid", typeof(short?), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_OutOfRangeValue_ReturnsFalse()
    {
        var converter = new NullableShortToStringTypeConverter();

        var result = converter.TryConvert("99999", typeof(short?), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new NullableShortToStringTypeConverter();
        short? value = 42;

        var result = converter.TryConvert(value, typeof(string), 5, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("00042");
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new NullableShortToStringTypeConverter();
        short? value = short.MinValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(short.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new NullableShortToStringTypeConverter();
        short? value = short.MaxValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(short.MaxValue.ToString());
    }
}
