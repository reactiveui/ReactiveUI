// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
///     Tests for converting strings to integers.
/// </summary>
public class StringToIntegerTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new StringToIntegerTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsFalse()
    {
        var converter = new StringToIntegerTypeConverter();

        var result = converter.TryConvert(string.Empty, null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new StringToIntegerTypeConverter();

        var result = converter.TryConvert("invalid", null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_OutOfRangeValue_ReturnsFalse()
    {
        var converter = new StringToIntegerTypeConverter();

        var result = converter.TryConvert("9999999999", null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_StringToInt_Succeeds()
    {
        var converter = new StringToIntegerTypeConverter();

        var result = converter.TryConvert("123456", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(123456);
    }

    [Test]
    public async Task TryConvert_NullString_ReturnsFalse()
    {
        var converter = new StringToIntegerTypeConverter();

        var result = converter.TryConvert(null, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsEqualTo(0);
    }

    [Test]
    public async Task TryConvert_ZeroValue_Succeeds()
    {
        var converter = new StringToIntegerTypeConverter();

        var result = converter.TryConvert("0", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(0);
    }

    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new StringToIntegerTypeConverter();

        var result = converter.TryConvert("-123456", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(-123456);
    }

    [Test]
    public async Task TryConvertTyped_ValidString_Succeeds()
    {
        var converter = new StringToIntegerTypeConverter();

        var result = converter.TryConvertTyped("789", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(789);
    }

    [Test]
    public async Task TryConvertTyped_InvalidType_ReturnsFalse()
    {
        var converter = new StringToIntegerTypeConverter();

        var result = converter.TryConvertTyped(123.456, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsNull();
    }

    [Test]
    public async Task TryConvertTyped_NullInput_ReturnsFalse()
    {
        var converter = new StringToIntegerTypeConverter();

        var result = converter.TryConvertTyped(null, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsNull();
    }

    [Test]
    public async Task FromType_ReturnsStringType()
    {
        var converter = new StringToIntegerTypeConverter();

        await Assert.That(converter.FromType).IsEqualTo(typeof(string));
    }

    [Test]
    public async Task ToType_ReturnsIntType()
    {
        var converter = new StringToIntegerTypeConverter();

        await Assert.That(converter.ToType).IsEqualTo(typeof(int));
    }
}
