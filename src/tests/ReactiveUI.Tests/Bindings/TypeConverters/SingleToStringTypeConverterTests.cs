// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting floats to strings.
/// </summary>
public class SingleToStringTypeConverterTests
{
    /// <summary>
    /// Verifies the converter reports an affinity of 2.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new SingleToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>
    /// Verifies that the maximum float value converts successfully.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MaxValue_Succeeds()
    {
        var converter = new SingleToStringTypeConverter();
        const float Value = float.MaxValue;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(float.MaxValue.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Verifies that the minimum float value converts successfully.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MinValue_Succeeds()
    {
        var converter = new SingleToStringTypeConverter();
        const float Value = float.MinValue;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(float.MinValue.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Verifies that a negative float value converts successfully.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new SingleToStringTypeConverter();
        const float Value = -123.456f;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Verifies that a float value converts to its string representation.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_SingleToString_Succeeds()
    {
        var converter = new SingleToStringTypeConverter();
        const float Value = 123.456f;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Verifies that a conversion hint applies the expected formatting.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithConversionHint_FormatsCorrectly()
    {
        var converter = new SingleToStringTypeConverter();
        const float Value = 42.5f;
        const int DecimalPlaces = 2;

        var result = converter.TryConvert(Value, DecimalPlaces, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("42.50");
    }
}
