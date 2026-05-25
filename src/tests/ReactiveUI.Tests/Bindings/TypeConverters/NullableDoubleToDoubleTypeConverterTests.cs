// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting nullable double to double.
/// </summary>
public class NullableDoubleToDoubleTypeConverterTests
{
    /// <summary>
    /// Verifies that the converter reports an affinity of 2.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new NullableDoubleToDoubleTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>
    /// Verifies that converting a value succeeds and yields the underlying value.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithValue_Succeeds()
    {
        var converter = new NullableDoubleToDoubleTypeConverter();
        const double Value = 123.456789;
        double? value = Value;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value);
    }

    /// <summary>
    /// Verifies that converting a null value fails.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_Null_ReturnsFalse()
    {
        var converter = new NullableDoubleToDoubleTypeConverter();
        double? value = null;

        var result = converter.TryConvert(value, null, out _);

        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Verifies that the converter source type is nullable double.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task FromType_ReturnsDoubleNullable()
    {
        var converter = new NullableDoubleToDoubleTypeConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(double?));
    }

    /// <summary>
    /// Verifies that the converter target type is double.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ToType_ReturnsDouble()
    {
        var converter = new NullableDoubleToDoubleTypeConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(double));
    }
}
