// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>Tests for converting short to nullable short.</summary>
public class ShortToNullableShortTypeConverterTests
{
    /// <summary>Verifies the converter reports an affinity of 2.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new ShortToNullableShortTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>Verifies that converting a short value always succeeds.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_AlwaysSucceeds()
    {
        var converter = new ShortToNullableShortTypeConverter();
        const short Value = 1_234;

        var result = converter.TryConvert(Value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo((short?)Value);
    }

    /// <summary>Verifies the converter source type is <see cref="short"/>.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task FromType_ReturnsShort()
    {
        var converter = new ShortToNullableShortTypeConverter();
        await Assert.That(converter.FromType).IsEqualTo(typeof(short));
    }

    /// <summary>Verifies the converter target type is nullable <see cref="short"/>.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ToType_ReturnsShortNullable()
    {
        var converter = new ShortToNullableShortTypeConverter();
        await Assert.That(converter.ToType).IsEqualTo(typeof(short?));
    }

    /// <summary>Verifies that a valid value converts successfully and produces output.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_WithValidValue_ReturnsTrueAndOutput()
    {
        var converter = new ShortToNullableShortTypeConverter();
        const short Value = 42;

        var success = converter.TryConvertTyped(Value, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo((short?)Value);
    }

    /// <summary>Verifies that a null value fails to convert.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_WithNullValue_ReturnsFalse()
    {
        var converter = new ShortToNullableShortTypeConverter();

        var success = converter.TryConvertTyped(null, null, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }

    /// <summary>Verifies that a value of an invalid type fails to convert.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_WithInvalidType_ReturnsFalse()
    {
        var converter = new ShortToNullableShortTypeConverter();
        const string Value = "invalid";

        var success = converter.TryConvertTyped(Value, null, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }
}
