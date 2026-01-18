// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting strings to Guid.
/// </summary>
public class StringToGuidTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new StringToGuidTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_ValidString_Succeeds()
    {
        var converter = new StringToGuidTypeConverter();
        var expected = Guid.Parse("12345678-1234-1234-1234-123456789abc");

        var result = converter.TryConvert("12345678-1234-1234-1234-123456789abc", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(expected);
    }

    [Test]
    public async Task TryConvert_EmptyGuidString_Succeeds()
    {
        var converter = new StringToGuidTypeConverter();

        var result = converter.TryConvert("00000000-0000-0000-0000-000000000000", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Guid.Empty);
    }

    [Test]
    public async Task TryConvert_Null_ReturnsFalse()
    {
        var converter = new StringToGuidTypeConverter();

        var result = converter.TryConvert(null, null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsFalse()
    {
        var converter = new StringToGuidTypeConverter();

        var result = converter.TryConvert(string.Empty, null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new StringToGuidTypeConverter();

        var result = converter.TryConvert("invalid", null, out var output);

        await Assert.That(result).IsFalse();
    }
}
