// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.Bindings;

/// <summary>
///     Comprehensive test suite for <see cref="BindingTypeConverter{TFrom, TTo}" /> base class.
///     Tests cover TryConvertTyped method with various input types and null handling scenarios,
///     as well as FromType and ToType properties.
/// </summary>
public class BindingTypeConverterTests
{
    /// <summary>
    ///     Verifies that <see cref="BindingTypeConverter{TFrom, TTo}.FromType" />
    ///     returns the correct source type.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task FromType_ReturnsCorrectType()
    {
        var converter = new TestConverter();

        await Assert.That(converter.FromType).IsEqualTo(typeof(string));
    }

    /// <summary>
    ///     Verifies that <see cref="BindingTypeConverter{TFrom, TTo}.ToType" />
    ///     returns the correct target type.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ToType_ReturnsCorrectType()
    {
        var converter = new TestConverter();

        await Assert.That(converter.ToType).IsEqualTo(typeof(int));
    }

    /// <summary>
    ///     Verifies that <see cref="BindingTypeConverter{TFrom, TTo}.TryConvertTyped" />
    ///     returns false when the input type doesn't match TFrom.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task TryConvertTyped_TypeMismatch_ReturnsFalse()
    {
        var converter = new TestConverter();

        var result = converter.TryConvertTyped(123.45, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="BindingTypeConverter{TFrom, TTo}.TryConvertTyped" />
    ///     successfully converts a valid input.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task TryConvertTyped_ValidInput_Succeeds()
    {
        var converter = new TestConverter();

        var result = converter.TryConvertTyped("42", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(42);
    }

    /// <summary>
    ///     Verifies that <see cref="BindingTypeConverter{TFrom, TTo}.TryConvertTyped" />
    ///     returns false when conversion fails.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task TryConvertTyped_ConversionFails_ReturnsFalse()
    {
        var converter = new TestConverter();

        var result = converter.TryConvertTyped("invalid", null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="BindingTypeConverter{TFrom, TTo}.TryConvertTyped" />
    ///     handles null input correctly when TFrom is a reference type.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task TryConvertTyped_NullInputWithReferenceTypeSource_HandlesCorrectly()
    {
        var converter = new TestConverter();

        var result = converter.TryConvertTyped(null, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="BindingTypeConverter{TFrom, TTo}.TryConvertTyped" />
    ///     returns false when input is null and TFrom is a non-nullable value type.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task TryConvertTyped_NullInputWithValueTypeSource_ReturnsFalse()
    {
        var converter = new ValueTypeConverter();

        var result = converter.TryConvertTyped(null, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="BindingTypeConverter{TFrom, TTo}.TryConvertTyped" />
    ///     succeeds when converting from nullable source type with null input.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task TryConvertTyped_NullInputWithNullableSource_Succeeds()
    {
        var converter = new NullableToStringConverter();

        var result = converter.TryConvertTyped(null, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("null");
    }

    /// <summary>
    ///     Verifies that <see cref="BindingTypeConverter{TFrom, TTo}.TryConvertTyped" />
    ///     handles the case when TryConvert returns null for a non-nullable target type.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task TryConvertTyped_NullResultWithNonNullableTarget_ReturnsFalse()
    {
        var converter = new NullReturningConverter();

        var result = converter.TryConvertTyped("test", null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="BindingTypeConverter{TFrom, TTo}.TryConvertTyped" />
    ///     succeeds when TryConvert returns null for a nullable target type.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task TryConvertTyped_NullResultWithNullableTarget_Succeeds()
    {
        var converter = new StringToNullableIntConverter();

        var result = converter.TryConvertTyped("null", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="BindingTypeConverter{TFrom, TTo}.TryConvertTyped" />
    ///     handles null input when source is nullable and conversion returns false.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task TryConvertTyped_NullInputWithNullableSourceButConversionFails_ReturnsFalse()
    {
        var converter = new NullableFailingConverter();

        var result = converter.TryConvertTyped(null, null, out var output);

        await Assert.That(result).IsFalse();
        await Assert.That(output).IsNull();
    }

    /// <summary>
    ///     Test converter from string to int.
    /// </summary>
    private sealed class TestConverter : BindingTypeConverter<string, int>
    {
        public override int GetAffinityForObjects() => 1;

        public override bool TryConvert(string? from, object? conversionHint, [NotNullWhen(true)] out int result)
        {
            if (from is null)
            {
                result = default;
                return false;
            }

            return int.TryParse(from, out result);
        }
    }

    /// <summary>
    ///     Test converter from int to string (value type to reference type).
    /// </summary>
    private sealed class ValueTypeConverter : BindingTypeConverter<int, string>
    {
        public override int GetAffinityForObjects() => 1;

        public override bool TryConvert(int from, object? conversionHint, [NotNullWhen(true)] out string? result)
        {
            result = from.ToString();
            return true;
        }
    }

    /// <summary>
    ///     Test converter from int? to string that handles null input.
    /// </summary>
    private sealed class NullableToStringConverter : BindingTypeConverter<int?, string>
    {
        public override int GetAffinityForObjects() => 1;

        public override bool TryConvert(int? from, object? conversionHint, [NotNullWhen(true)] out string? result)
        {
            result = from?.ToString() ?? "null";
            return true;
        }
    }

    /// <summary>
    ///     Test converter that fails conversion, used to test null handling for non-nullable target types.
    /// </summary>
    private sealed class NullReturningConverter : BindingTypeConverter<string, int>
    {
        public override int GetAffinityForObjects() => 1;

        public override bool TryConvert(string? from, object? conversionHint, [NotNullWhen(true)] out int result)
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    ///     Test converter from string to int? that returns null for "null" input.
    /// </summary>
    private sealed class StringToNullableIntConverter : BindingTypeConverter<string, int?>
    {
        public override int GetAffinityForObjects() => 1;

        public override bool TryConvert(string? from, object? conversionHint, out int? result)
        {
            if (from == "null")
            {
                result = null;
                return true;
            }

            if (int.TryParse(from, out var intResult))
            {
                result = intResult;
                return true;
            }

            result = null;
            return false;
        }
    }

    /// <summary>
    ///     Test converter from int? to string that always fails conversion for null input.
    /// </summary>
    private sealed class NullableFailingConverter : BindingTypeConverter<int?, string>
    {
        public override int GetAffinityForObjects() => 1;

        public override bool TryConvert(int? from, object? conversionHint, [NotNullWhen(true)] out string? result)
        {
            if (from is null)
            {
                result = null;
                return false;
            }

            result = from.Value.ToString();
            return true;
        }
    }
}
