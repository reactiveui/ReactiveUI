// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
///     Tests for converting strings to bytes.
/// </summary>
public class StringToByteTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new StringToByteTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsFalse()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvert(string.Empty, null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvert("invalid", null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_NegativeValue_ReturnsFalse()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvert("-1", null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_OutOfRangeValue_ReturnsFalse()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvert("999", null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_StringToByte_Succeeds()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvert("123", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((byte)123);
    }

    [Test]
    public async Task TryConvert_NullString_ReturnsFalse()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvert(null, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsEqualTo((byte)0);
    }

    [Test]
    public async Task TryConvert_ZeroValue_Succeeds()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvert("0", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((byte)0);
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvert("255", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((byte)255);
    }

    [Test]
    public async Task TryConvertTyped_ValidString_Succeeds()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvertTyped("100", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((byte)100);
    }

    [Test]
    public async Task TryConvertTyped_InvalidType_ReturnsFalse()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvertTyped(123, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsNull();
    }

    [Test]
    public async Task TryConvertTyped_NullInput_ReturnsFalse()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvertTyped(null, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsNull();
    }

    [Test]
    public async Task FromType_ReturnsStringType()
    {
        var converter = new StringToByteTypeConverter();

        await Assert.That(converter.FromType).IsEqualTo(typeof(string));
    }

    [Test]
    public async Task ToType_ReturnsByteType()
    {
        var converter = new StringToByteTypeConverter();

        await Assert.That(converter.ToType).IsEqualTo(typeof(byte));
    }
}
