// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>Tests for converting nullable long to strings.</summary>
public class NullableLongToStringTypeConverterTests
{
    /// <summary>Verifies that the converter reports an affinity of 2.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new NullableLongToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>Verifies that converting a nullable long to a string succeeds.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_LongNullableToString_Succeeds()
    {
        var converter = new NullableLongToStringTypeConverter();
        const long Value = 123_456_789_012;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123456789012");
    }

    /// <summary>Verifies that converting the maximum value succeeds.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new NullableLongToStringTypeConverter();
        long? value = long.MaxValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(long.MaxValue.ToString());
    }

    /// <summary>Verifies that converting the minimum value succeeds.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new NullableLongToStringTypeConverter();
        long? value = long.MinValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(long.MinValue.ToString());
    }

    /// <summary>Verifies that converting a null value succeeds.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_NullValue_ReturnsTrue()
    {
        var converter = new NullableLongToStringTypeConverter();

        var result = converter.TryConvert(null, null, out _);

        await Assert.That(result).IsTrue();
    }

    /// <summary>Verifies that a conversion hint is used to format the output.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new NullableLongToStringTypeConverter();
        const long Value = 42;
        const int PaddingWidth = 10;

        var result = converter.TryConvert(Value, PaddingWidth, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("0000000042");
    }
}
