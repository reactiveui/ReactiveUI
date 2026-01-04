// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting strings to nullable floats (single-precision).
/// </summary>
public class StringToNullableSingleTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns10()
    {
        var converter = new StringToNullableSingleTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    public async Task TryConvert_StringToSingleNullable_Succeeds()
    {
        var converter = new StringToNullableSingleTypeConverter();

        var result = converter.TryConvert("123.456", null, out float? output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNotNull();
        await Assert.That(output!.Value).IsEqualTo(123.456f).Within(0.001f);
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsTrue()
    {
        var converter = new StringToNullableSingleTypeConverter();

        var result = converter.TryConvert(string.Empty, null, out float? output);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new StringToNullableSingleTypeConverter();

        var result = converter.TryConvert("invalid", null, out float? output);

        await Assert.That(result).IsFalse();
    }
}
