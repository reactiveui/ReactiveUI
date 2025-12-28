// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class NullableIntegerToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_IntNullableToString_Returns10()
    {
        var converter = new NullableIntegerToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(int?), typeof(string));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_StringToIntNullable_Returns10()
    {
        var converter = new NullableIntegerToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(int?));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_WrongTypes_Returns0()
    {
        var converter = new NullableIntegerToStringTypeConverter();

        await Assert.That(converter.GetAffinityForObjects(typeof(long), typeof(string))).IsEqualTo(0);
        await Assert.That(converter.GetAffinityForObjects(typeof(string), typeof(long))).IsEqualTo(0);
        await Assert.That(converter.GetAffinityForObjects(typeof(int), typeof(string))).IsEqualTo(0);
    }

    [Test]
    public async Task TryConvert_IntNullableToString_Succeeds()
    {
        var converter = new NullableIntegerToStringTypeConverter();
        int? value = 123456;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123456");
    }

    [Test]
    public async Task TryConvert_StringToIntNullable_Succeeds()
    {
        var converter = new NullableIntegerToStringTypeConverter();

        var result = converter.TryConvert("123456", typeof(int?), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(123456);
    }

    [Test]
    public async Task TryConvert_NullValue_ReturnsTrue()
    {
        var converter = new NullableIntegerToStringTypeConverter();

        var result = converter.TryConvert(null, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsTrue()
    {
        var converter = new NullableIntegerToStringTypeConverter();

        var result = converter.TryConvert(string.Empty, typeof(int?), null, out var output);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new NullableIntegerToStringTypeConverter();

        var result = converter.TryConvert("invalid", typeof(int?), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_OutOfRangeValue_ReturnsFalse()
    {
        var converter = new NullableIntegerToStringTypeConverter();

        var result = converter.TryConvert("9999999999", typeof(int?), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new NullableIntegerToStringTypeConverter();
        int? value = 42;

        var result = converter.TryConvert(value, typeof(string), 8, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("00000042");
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new NullableIntegerToStringTypeConverter();
        int? value = int.MinValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(int.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new NullableIntegerToStringTypeConverter();
        int? value = int.MaxValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(int.MaxValue.ToString());
    }
}
