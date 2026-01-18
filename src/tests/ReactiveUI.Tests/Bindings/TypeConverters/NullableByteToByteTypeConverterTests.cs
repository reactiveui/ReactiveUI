// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting nullable byte to byte.
/// </summary>
public class NullableByteToByteTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new NullableByteToByteTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_WithValue_Succeeds()
    {
        var converter = new NullableByteToByteTypeConverter();
        byte? value = 42;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((byte)42);
    }

    [Test]
    public async Task TryConvert_Null_ReturnsFalse()
    {
        var converter = new NullableByteToByteTypeConverter();
        byte? value = null;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task FromType_ReturnsByteNullable()
    {
        var converter = new NullableByteToByteTypeConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(byte?));
    }

    [Test]
    public async Task ToType_ReturnsByte()
    {
        var converter = new NullableByteToByteTypeConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(byte));
    }
}
