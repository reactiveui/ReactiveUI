// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>Tests for converting strings to floats (single-precision).</summary>
public class StringToSingleTypeConverterTests
{
    /// <summary>Verifies the converter reports an affinity of 2.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new StringToSingleTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>Verifies that an empty string fails to convert.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_EmptyString_ReturnsFalse()
    {
        var converter = new StringToSingleTypeConverter();

        var result = converter.TryConvert(string.Empty, null, out _);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that an invalid string fails to convert.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_InvalidString_ReturnsFalse()
    {
        var converter = new StringToSingleTypeConverter();

        var result = converter.TryConvert("invalid", null, out _);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that a valid string converts to a float.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_StringToSingle_Succeeds()
    {
        var converter = new StringToSingleTypeConverter();
        const float ExpectedValue = 123.456f;
        const float Tolerance = 0.001f;

        var result = converter.TryConvert("123.456", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(ExpectedValue).Within(Tolerance);
    }

    /// <summary>Verifies that a null string fails to convert.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_NullString_ReturnsFalse()
    {
        var converter = new StringToSingleTypeConverter();

        var result = converter.TryConvert(null, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsEqualTo(0.0f);
    }

    /// <summary>Verifies that a zero value converts successfully.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_ZeroValue_Succeeds()
    {
        var converter = new StringToSingleTypeConverter();

        var result = converter.TryConvert("0", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(0.0f);
    }

    /// <summary>Verifies that a negative value converts successfully.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_NegativeValue_Succeeds()
    {
        var converter = new StringToSingleTypeConverter();
        const float ExpectedValue = -123.456f;
        const float Tolerance = 0.001f;

        var result = converter.TryConvert("-123.456", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(ExpectedValue).Within(Tolerance);
    }

    /// <summary>Verifies that a string in scientific notation converts successfully.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_ScientificNotation_Succeeds()
    {
        var converter = new StringToSingleTypeConverter();
        const float ExpectedValue = 1.23E+5f;
        const float Tolerance = 0.1f;

        var result = converter.TryConvert("1.23E+5", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(ExpectedValue).Within(Tolerance);
    }

    /// <summary>Verifies that a valid string converts via the typed overload.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_ValidString_Succeeds()
    {
        var converter = new StringToSingleTypeConverter();
        const float ExpectedValue = 456.789f;
        const float Tolerance = 0.001f;

        var result = converter.TryConvertTyped("456.789", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsTypeOf<float>();
        await Assert.That((float)output!).IsEqualTo(ExpectedValue).Within(Tolerance);
    }

    /// <summary>Verifies that an input of an invalid type fails to convert via the typed overload.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_InvalidType_ReturnsFalse()
    {
        var converter = new StringToSingleTypeConverter();
        const double InvalidInput = 123.456;

        var result = converter.TryConvertTyped(InvalidInput, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsNull();
    }

    /// <summary>Verifies that a null input fails to convert via the typed overload.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TryConvertTyped_NullInput_ReturnsFalse()
    {
        var converter = new StringToSingleTypeConverter();

        var result = converter.TryConvertTyped(null, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsNull();
    }

    /// <summary>Verifies the converter source type is <see cref="string"/>.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task FromType_ReturnsStringType()
    {
        var converter = new StringToSingleTypeConverter();

        await Assert.That(converter.FromType).IsEqualTo(typeof(string));
    }

    /// <summary>Verifies the converter target type is <see cref="float"/>.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ToType_ReturnsFloatType()
    {
        var converter = new StringToSingleTypeConverter();

        await Assert.That(converter.ToType).IsEqualTo(typeof(float));
    }
}
