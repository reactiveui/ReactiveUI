// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>Tests for converting nullable decimal to decimal.</summary>
public class NullableDecimalToDecimalTypeConverterTests
{
    /// <summary>Verifies that the converter reports an affinity of 2.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new NullableDecimalToDecimalTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>Verifies that converting a value succeeds and yields the underlying value.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithValue_Succeeds()
    {
        var converter = new NullableDecimalToDecimalTypeConverter();
        const decimal Value = 123.456789m;
        decimal? value = Value;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(Value);
    }

    /// <summary>Verifies that converting a null value fails.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_Null_ReturnsFalse()
    {
        var converter = new NullableDecimalToDecimalTypeConverter();
        decimal? value = null;

        var result = converter.TryConvert(value, null, out _);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that the converter source type is nullable decimal.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task FromType_ReturnsDecimalNullable()
    {
        var converter = new NullableDecimalToDecimalTypeConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(decimal?));
    }

    /// <summary>Verifies that the converter target type is decimal.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ToType_ReturnsDecimal()
    {
        var converter = new NullableDecimalToDecimalTypeConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(decimal));
    }

    /// <summary>Verifies that the typed conversion of a valid value succeeds and yields the output.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_WithValidValue_ReturnsTrueAndOutput()
    {
        var converter = new NullableDecimalToDecimalTypeConverter();
        const decimal Value = 42.5m;
        decimal? value = Value;

        var success = converter.TryConvertTyped(value, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Value);
    }

    /// <summary>Verifies that the typed conversion of a null value fails.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_WithNullValue_ReturnsFalse()
    {
        var converter = new NullableDecimalToDecimalTypeConverter();

        var success = converter.TryConvertTyped(null, null, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }

    /// <summary>Verifies that the typed conversion of an invalid type fails.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_WithInvalidType_ReturnsFalse()
    {
        var converter = new NullableDecimalToDecimalTypeConverter();
        const string Value = "invalid";

        var success = converter.TryConvertTyped(Value, null, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }
}
