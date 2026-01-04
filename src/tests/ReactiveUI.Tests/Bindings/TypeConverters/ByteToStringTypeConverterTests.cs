// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

public class ByteToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns10()
    {
        var converter = new ByteToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task TryConvert_ByteToString_Succeeds()
    {
        var converter = new ByteToStringTypeConverter();
        byte value = 123;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123");
    }

    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new ByteToStringTypeConverter();
        byte value = 5;

        var result = converter.TryConvert(value, 3, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("005");
    }

    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new ByteToStringTypeConverter();
        byte value = byte.MinValue;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("0");
    }

    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new ByteToStringTypeConverter();
        byte value = byte.MaxValue;

        var result = converter.TryConvert(value, null, out string? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("255");
    }
}
