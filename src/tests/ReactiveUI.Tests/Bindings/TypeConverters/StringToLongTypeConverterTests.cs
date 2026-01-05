// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting strings to long integers.
/// </summary>
public class StringToLongTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new StringToLongTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_StringToLong_Succeeds()
    {
        var converter = new StringToLongTypeConverter();

        var result = converter.TryConvert("123456789012", null, out long output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(123456789012L);
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new StringToLongTypeConverter();

        var result = converter.TryConvert("invalid", null, out long output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_OutOfRangeValue_ReturnsFalse()
    {
        var converter = new StringToLongTypeConverter();

        var result = converter.TryConvert("99999999999999999999", null, out long output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsFalse()
    {
        var converter = new StringToLongTypeConverter();

        var result = converter.TryConvert(string.Empty, null, out long output);

        await Assert.That(result).IsFalse();
    }
}
