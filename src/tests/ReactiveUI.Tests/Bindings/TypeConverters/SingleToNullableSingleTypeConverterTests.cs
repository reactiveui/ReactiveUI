// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting float to nullable float.
/// </summary>
public class SingleToNullableSingleTypeConverterTests
{
    /// <summary>
    /// Verifies the converter reports an affinity of 2.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new SingleToNullableSingleTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>
    /// Verifies that converting a float value always succeeds.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_AlwaysSucceeds()
    {
        var converter = new SingleToNullableSingleTypeConverter();
        const float Value = 123.45f;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((float?)Value);
    }

    /// <summary>
    /// Verifies the converter source type is <see cref="float"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task FromType_ReturnsFloat()
    {
        var converter = new SingleToNullableSingleTypeConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(float));
    }

    /// <summary>
    /// Verifies the converter target type is nullable <see cref="float"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ToType_ReturnsSingleNullable()
    {
        var converter = new SingleToNullableSingleTypeConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(float?));
    }

    /// <summary>
    /// Verifies that a valid value converts successfully and produces output.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_WithValidValue_ReturnsTrueAndOutput()
    {
        var converter = new SingleToNullableSingleTypeConverter();
        const float Value = 42.5f;

        var success = converter.TryConvertTyped(Value, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo((float?)Value);
    }

    /// <summary>
    /// Verifies that a null value fails to convert.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_WithNullValue_ReturnsFalse()
    {
        var converter = new SingleToNullableSingleTypeConverter();

        var success = converter.TryConvertTyped(null, null, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }

    /// <summary>
    /// Verifies that a value of an invalid type fails to convert.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_WithInvalidType_ReturnsFalse()
    {
        var converter = new SingleToNullableSingleTypeConverter();
        const string Value = "invalid";

        var success = converter.TryConvertTyped(Value, null, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }
}
