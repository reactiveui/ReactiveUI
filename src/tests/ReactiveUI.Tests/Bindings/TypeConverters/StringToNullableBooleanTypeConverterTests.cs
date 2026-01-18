// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting strings to nullable booleans.
/// </summary>
public class StringToNullableBooleanTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new StringToNullableBooleanTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_TrueString_Succeeds()
    {
        var converter = new StringToNullableBooleanTypeConverter();

        var result = converter.TryConvert("True", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(true);
    }

    [Test]
    public async Task TryConvert_FalseString_Succeeds()
    {
        var converter = new StringToNullableBooleanTypeConverter();

        var result = converter.TryConvert("False", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(false);
    }

    [Test]
    public async Task TryConvert_TrueLowercase_Succeeds()
    {
        var converter = new StringToNullableBooleanTypeConverter();

        var result = converter.TryConvert("true", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(true);
    }

    [Test]
    public async Task TryConvert_FalseLowercase_Succeeds()
    {
        var converter = new StringToNullableBooleanTypeConverter();

        var result = converter.TryConvert("false", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(false);
    }

    [Test]
    public async Task TryConvert_Null_ReturnsNull()
    {
        var converter = new StringToNullableBooleanTypeConverter();

        var result = converter.TryConvert(null, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNull();
    }

    [Test]
    public async Task TryConvert_EmptyString_ReturnsNull()
    {
        var converter = new StringToNullableBooleanTypeConverter();

        var result = converter.TryConvert(string.Empty, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNull();
    }

    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new StringToNullableBooleanTypeConverter();

        var result = converter.TryConvert("invalid", null, out var output);

        await Assert.That(result).IsFalse();
    }
}
