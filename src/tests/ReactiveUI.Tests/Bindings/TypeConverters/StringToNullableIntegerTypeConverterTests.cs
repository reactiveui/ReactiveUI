// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>Tests for converting strings to nullable integers.</summary>
public class StringToNullableIntegerTypeConverterTests
{
    /// <summary>Verifies the converter reports an affinity of 2.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new StringToNullableIntegerTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>Verifies that an empty string converts successfully to a null result.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_EmptyString_ReturnsTrue()
    {
        var converter = new StringToNullableIntegerTypeConverter();

        var result = converter.TryConvert(string.Empty, null, out _);

        await Assert.That(result).IsTrue();
    }

    /// <summary>Verifies that an invalid string fails to convert.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new StringToNullableIntegerTypeConverter();

        var result = converter.TryConvert("invalid", null, out _);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that an out-of-range value fails to convert.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_OutOfRangeValue_ReturnsFalse()
    {
        var converter = new StringToNullableIntegerTypeConverter();

        var result = converter.TryConvert("9999999999", null, out _);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that a valid string converts to a nullable integer.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_StringToIntNullable_Succeeds()
    {
        var converter = new StringToNullableIntegerTypeConverter();
        const int ExpectedValue = 123_456;

        var result = converter.TryConvert("123456", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(ExpectedValue);
    }
}
