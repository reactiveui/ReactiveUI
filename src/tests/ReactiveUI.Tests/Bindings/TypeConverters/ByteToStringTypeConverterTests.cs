// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class ByteToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_ByteToString_Returns10()
    {
        var converter = new ByteToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(byte), typeof(string));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_StringToByte_Returns10()
    {
        var converter = new ByteToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(byte));
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task GetAffinityForObjects_WrongTypes_Returns0()
    {
        var converter = new ByteToStringTypeConverter();

        await Assert.That(converter.GetAffinityForObjects(typeof(int), typeof(string))).IsEqualTo(0);
        await Assert.That(converter.GetAffinityForObjects(typeof(string), typeof(int))).IsEqualTo(0);
    }

    [Test]
    public async Task TryConvert_ByteToString_Succeeds()
    {
        var converter = new ByteToStringTypeConverter();
        byte value = 123;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123");
    }

    [Test]
    public async Task TryConvert_StringToByte_Succeeds()
    {
        var converter = new ByteToStringTypeConverter();

        var result = converter.TryConvert("123", typeof(byte), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((byte)123);
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new ByteToStringTypeConverter();

        var result = converter.TryConvert("invalid", typeof(byte), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_OutOfRangeValue_ReturnsFalse()
    {
        var converter = new ByteToStringTypeConverter();

        var result = converter.TryConvert("999", typeof(byte), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_NegativeValue_ReturnsFalse()
    {
        var converter = new ByteToStringTypeConverter();

        var result = converter.TryConvert("-1", typeof(byte), null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new ByteToStringTypeConverter();
        byte value = 5;

        var result = converter.TryConvert(value, typeof(string), 3, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("005");
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new ByteToStringTypeConverter();
        byte value = byte.MinValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("0");
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new ByteToStringTypeConverter();
        byte value = byte.MaxValue;

        var result = converter.TryConvert(value, typeof(string), null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("255");
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsFalse()
    {
        var converter = new ByteToStringTypeConverter();

        var result = converter.TryConvert(string.Empty, typeof(byte), null, out var output);

        await Assert.That(result).IsFalse();
    }
}
