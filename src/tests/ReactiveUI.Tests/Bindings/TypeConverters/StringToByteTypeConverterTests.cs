// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting strings to bytes.
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
    public async Task TryConvert_StringToByte_Succeeds()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvert("123", null, out byte output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((byte)123);
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvert("invalid", null, out byte output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_OutOfRangeValue_ReturnsFalse()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvert("999", null, out byte output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_NegativeValue_ReturnsFalse()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvert("-1", null, out byte output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsFalse()
    {
        var converter = new StringToByteTypeConverter();

        var result = converter.TryConvert(string.Empty, null, out byte output);

        await Assert.That(result).IsFalse();
    }
}
