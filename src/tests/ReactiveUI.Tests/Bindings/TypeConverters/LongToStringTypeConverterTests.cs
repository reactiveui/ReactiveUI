// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class LongToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new LongToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_LongToString_Succeeds()
    {
        var converter = new LongToStringTypeConverter();
        var value = 123456789012;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123456789012");
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new LongToStringTypeConverter();
        var value = long.MaxValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(long.MaxValue.ToString());
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new LongToStringTypeConverter();
        var value = long.MinValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(long.MinValue.ToString());
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new LongToStringTypeConverter();
        long value = 42;

        var result = converter.TryConvert(value, 10, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("0000000042");
    }
}
