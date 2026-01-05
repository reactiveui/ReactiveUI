// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting strings to nullable decimals.
/// </summary>
public class StringToNullableDecimalTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new StringToNullableDecimalTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_StringToDecimalNullable_Succeeds()
    {
        var converter = new StringToNullableDecimalTypeConverter();

        var result = converter.TryConvert("123.456", null, out decimal? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(123.456m);
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsTrue()
    {
        var converter = new StringToNullableDecimalTypeConverter();

        var result = converter.TryConvert(string.Empty, null, out decimal? output);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new StringToNullableDecimalTypeConverter();

        var result = converter.TryConvert("invalid", null, out decimal? output);

        await Assert.That(result).IsFalse();
    }
}
