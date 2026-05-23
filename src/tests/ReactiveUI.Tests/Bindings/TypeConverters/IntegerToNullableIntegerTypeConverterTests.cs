// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting int to nullable int.
/// </summary>
public class IntegerToNullableIntegerTypeConverterTests
{
    /// <summary>
    /// Verifies the converter reports an affinity of 2.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new IntegerToNullableIntegerTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>
    /// Verifies that converting an int value always succeeds.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_AlwaysSucceeds()
    {
        var converter = new IntegerToNullableIntegerTypeConverter();
        const int Value = 123456;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((int?)Value);
    }

    /// <summary>
    /// Verifies the converter source type is <see cref="int"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task FromType_ReturnsInt()
    {
        var converter = new IntegerToNullableIntegerTypeConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(int));
    }

    /// <summary>
    /// Verifies the converter target type is nullable <see cref="int"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ToType_ReturnsIntNullable()
    {
        var converter = new IntegerToNullableIntegerTypeConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(int?));
    }

    /// <summary>
    /// Verifies that a valid value converts successfully and produces output.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_WithValidValue_ReturnsTrueAndOutput()
    {
        var converter = new IntegerToNullableIntegerTypeConverter();
        const int Value = 42;

        var success = converter.TryConvertTyped(Value, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo((int?)Value);
    }

    /// <summary>
    /// Verifies that a null value fails to convert.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_WithNullValue_ReturnsFalse()
    {
        var converter = new IntegerToNullableIntegerTypeConverter();

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
        var converter = new IntegerToNullableIntegerTypeConverter();
        const string Value = "invalid";

        var success = converter.TryConvertTyped(Value, null, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }
}
