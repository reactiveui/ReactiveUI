// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting nullable byte to strings.
/// </summary>
public class NullableByteToStringTypeConverterTests
{
    /// <summary>
    /// Verifies that the converter reports an affinity of 2.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new NullableByteToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>
    /// Verifies that converting a nullable byte to a string succeeds.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_ByteNullableToString_Succeeds()
    {
        var converter = new NullableByteToStringTypeConverter();
        const byte Value = 123;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("123");
    }

    /// <summary>
    /// Verifies that converting the maximum value succeeds.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new NullableByteToStringTypeConverter();
        byte? value = byte.MaxValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("255");
    }

    /// <summary>
    /// Verifies that converting the minimum value succeeds.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new NullableByteToStringTypeConverter();
        byte? value = byte.MinValue;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("0");
    }

    /// <summary>
    /// Verifies that converting a null value succeeds and yields a null output.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_NullValue_ReturnsTrue()
    {
        var converter = new NullableByteToStringTypeConverter();

        var result = converter.TryConvert(null, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNull();
    }

    /// <summary>
    /// Verifies that a conversion hint is used to format the output.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new NullableByteToStringTypeConverter();
        const byte Value = 5;
        const int PaddingWidth = 3;

        var result = converter.TryConvert(Value, PaddingWidth, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("005");
    }
}
