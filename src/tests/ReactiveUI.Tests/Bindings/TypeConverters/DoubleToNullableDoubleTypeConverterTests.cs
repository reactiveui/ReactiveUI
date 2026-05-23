// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting double to nullable double.
/// </summary>
public class DoubleToNullableDoubleTypeConverterTests
{
    /// <summary>
    /// Verifies the converter reports an affinity of 2.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new DoubleToNullableDoubleTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>
    /// Verifies that converting a double value always succeeds.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_AlwaysSucceeds()
    {
        var converter = new DoubleToNullableDoubleTypeConverter();
        const double Value = 123.456789;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((double?)Value);
    }

    /// <summary>
    /// Verifies the converter source type is <see cref="double"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task FromType_ReturnsDouble()
    {
        var converter = new DoubleToNullableDoubleTypeConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(double));
    }

    /// <summary>
    /// Verifies the converter target type is nullable <see cref="double"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ToType_ReturnsDoubleNullable()
    {
        var converter = new DoubleToNullableDoubleTypeConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(double?));
    }
}
