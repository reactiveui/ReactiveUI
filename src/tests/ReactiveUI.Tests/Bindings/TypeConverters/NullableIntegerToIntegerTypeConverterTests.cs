// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>Tests for converting nullable int to int.</summary>
public class NullableIntegerToIntegerTypeConverterTests
{
    /// <summary>Verifies that the converter reports an affinity of 2.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new NullableIntegerToIntegerTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>Verifies that converting a value succeeds and yields the underlying value.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithValue_Succeeds()
    {
        var converter = new NullableIntegerToIntegerTypeConverter();
        const int Value = 123_456;
        int? value = Value;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value);
    }

    /// <summary>Verifies that converting a null value fails.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_Null_ReturnsFalse()
    {
        var converter = new NullableIntegerToIntegerTypeConverter();
        int? value = null;

        var result = converter.TryConvert(value, null, out _);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that the converter source type is nullable int.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task FromType_ReturnsIntNullable()
    {
        var converter = new NullableIntegerToIntegerTypeConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(int?));
    }

    /// <summary>Verifies that the converter target type is int.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ToType_ReturnsInt()
    {
        var converter = new NullableIntegerToIntegerTypeConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(int));
    }

    /// <summary>Verifies that the typed conversion of a valid value succeeds and yields the output.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_WithValidValue_ReturnsTrueAndOutput()
    {
        var converter = new NullableIntegerToIntegerTypeConverter();
        const int Value = 42;
        int? value = Value;

        var success = converter.TryConvertTyped(value, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Value);
    }

    /// <summary>Verifies that the typed conversion of a null value fails.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_WithNullValue_ReturnsFalse()
    {
        var converter = new NullableIntegerToIntegerTypeConverter();

        var success = converter.TryConvertTyped(null, null, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }

    /// <summary>Verifies that the typed conversion of an invalid type fails.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_WithInvalidType_ReturnsFalse()
    {
        var converter = new NullableIntegerToIntegerTypeConverter();
        const string Value = "invalid";

        var success = converter.TryConvertTyped(Value, null, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }
}
