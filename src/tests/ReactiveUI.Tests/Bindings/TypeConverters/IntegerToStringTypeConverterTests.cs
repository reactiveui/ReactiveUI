// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class IntegerToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_IntToString_Returns10()
    {
        var converter = new IntegerToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(int), typeof(string));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_StringToInt_Returns10()
    {
        var converter = new IntegerToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(int));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_WrongTypes_Returns0()
    {
        var converter = new IntegerToStringTypeConverter();

        await Assert.That(converter.GetAffinityForObjects(typeof(long), typeof(string))).IsEqualTo(0);
        await Assert.That(converter.GetAffinityForObjects(typeof(string), typeof(long))).IsEqualTo(0);
    }

    [Test]
    public async Task TryConvert_IntToString_Succeeds()
    {
        var converter = new IntegerToStringTypeConverter();
        int value = 123456;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123456");
    }

    [Test]
    public async Task TryConvert_StringToInt_Succeeds()
    {
        var converter = new IntegerToStringTypeConverter();

        var result = converter.TryConvert("123456", typeof(int), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(123456);
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new IntegerToStringTypeConverter();

        var result = converter.TryConvert("invalid", typeof(int), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_OutOfRangeValue_ReturnsFalse()
    {
        var converter = new IntegerToStringTypeConverter();

        var result = converter.TryConvert("9999999999", typeof(int), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new IntegerToStringTypeConverter();
        int value = 42;

        var result = converter.TryConvert(value, typeof(string), 8, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("00000042");
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new IntegerToStringTypeConverter();
        int value = int.MinValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(int.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new IntegerToStringTypeConverter();
        int value = int.MaxValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(int.MaxValue.ToString());
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsFalse()
    {
        var converter = new IntegerToStringTypeConverter();

        var result = converter.TryConvert(string.Empty, typeof(int), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new IntegerToStringTypeConverter();
        int value = -123456;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("-123456");
    }
}
