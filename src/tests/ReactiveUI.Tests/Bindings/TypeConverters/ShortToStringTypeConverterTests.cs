// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class ShortToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new ShortToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_ShortToString_Succeeds()
    {
        var converter = new ShortToStringTypeConverter();
        short value = 12345;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("12345");
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new ShortToStringTypeConverter();
        short value = 42;

        var result = converter.TryConvert(value, 5, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("00042");
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new ShortToStringTypeConverter();
        short value = short.MinValue;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(short.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new ShortToStringTypeConverter();
        short value = short.MaxValue;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(short.MaxValue.ToString());
    }
}
