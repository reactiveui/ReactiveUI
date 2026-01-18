// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting short to nullable short.
/// </summary>
public class ShortToNullableShortTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new ShortToNullableShortTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_AlwaysSucceeds()
    {
        var converter = new ShortToNullableShortTypeConverter();
        short value = 1234;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((short?)1234);
    }

    [Test]
    public async Task FromType_ReturnsShort()
    {
        var converter = new ShortToNullableShortTypeConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(short));
    }

    [Test]
    public async Task ToType_ReturnsShortNullable()
    {
        var converter = new ShortToNullableShortTypeConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(short?));
    }

    [Test]
    public async Task TryConvertTyped_WithValidValue_ReturnsTrueAndOutput()
    {
        var converter = new ShortToNullableShortTypeConverter();
        short value = 42;

        var success = converter.TryConvertTyped(value, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo((short?)42);
    }

    [Test]
    public async Task TryConvertTyped_WithNullValue_ReturnsFalse()
    {
        var converter = new ShortToNullableShortTypeConverter();

        var success = converter.TryConvertTyped(null, null, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task TryConvertTyped_WithInvalidType_ReturnsFalse()
    {
        var converter = new ShortToNullableShortTypeConverter();
        string value = "invalid";

        var success = converter.TryConvertTyped(value, null, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }
}
