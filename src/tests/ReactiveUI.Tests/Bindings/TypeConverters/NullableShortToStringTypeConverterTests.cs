// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class NullableShortToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new NullableShortToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new NullableShortToStringTypeConverter();
        short? value = short.MaxValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(short.MaxValue.ToString());
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new NullableShortToStringTypeConverter();
        short? value = short.MinValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(short.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_NullValue_ReturnsTrue()
    {
        var converter = new NullableShortToStringTypeConverter();

        var result = converter.TryConvert(null, null, out var output);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TryConvert_ShortNullableToString_Succeeds()
    {
        var converter = new NullableShortToStringTypeConverter();
        short? value = 12345;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("12345");
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new NullableShortToStringTypeConverter();
        short? value = 42;

        var result = converter.TryConvert(value, 5, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("00042");
    }
}
