// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.Converters;

/// <summary>
///     Tests for verifying converter affinity values are correctly set.
///     Uses TUnit's MethodDataSource for theory-style testing with compile-time safety.
/// </summary>
public class ConverterAffinityTests
{
    /// <summary>
    ///     Data source for standard converters (affinity = 2).
    /// </summary>
    /// <returns>A sequence of converter test data with expected affinity values.</returns>
    public static IEnumerable<Func<(IBindingTypeConverter converter, int expectedAffinity)>> GetStandardConverters()
    {
        // String identity converter
        yield return () => (new StringConverter(), 2);

        // Numeric → String converters
        yield return () => (new ByteToStringTypeConverter(), 2);
        yield return () => (new NullableByteToStringTypeConverter(), 2);
        yield return () => (new ShortToStringTypeConverter(), 2);
        yield return () => (new NullableShortToStringTypeConverter(), 2);
        yield return () => (new IntegerToStringTypeConverter(), 2);
        yield return () => (new NullableIntegerToStringTypeConverter(), 2);
        yield return () => (new LongToStringTypeConverter(), 2);
        yield return () => (new NullableLongToStringTypeConverter(), 2);
        yield return () => (new SingleToStringTypeConverter(), 2);
        yield return () => (new NullableSingleToStringTypeConverter(), 2);
        yield return () => (new DoubleToStringTypeConverter(), 2);
        yield return () => (new NullableDoubleToStringTypeConverter(), 2);
        yield return () => (new DecimalToStringTypeConverter(), 2);
        yield return () => (new NullableDecimalToStringTypeConverter(), 2);

        // String → Numeric converters
        yield return () => (new StringToByteTypeConverter(), 2);
        yield return () => (new StringToNullableByteTypeConverter(), 2);
        yield return () => (new StringToShortTypeConverter(), 2);
        yield return () => (new StringToNullableShortTypeConverter(), 2);
        yield return () => (new StringToIntegerTypeConverter(), 2);
        yield return () => (new StringToNullableIntegerTypeConverter(), 2);
        yield return () => (new StringToLongTypeConverter(), 2);
        yield return () => (new StringToNullableLongTypeConverter(), 2);
        yield return () => (new StringToSingleTypeConverter(), 2);
        yield return () => (new StringToNullableSingleTypeConverter(), 2);
        yield return () => (new StringToDoubleTypeConverter(), 2);
        yield return () => (new StringToNullableDoubleTypeConverter(), 2);
        yield return () => (new StringToDecimalTypeConverter(), 2);
        yield return () => (new StringToNullableDecimalTypeConverter(), 2);

        // Boolean ↔ String converters
        yield return () => (new BooleanToStringTypeConverter(), 2);
        yield return () => (new NullableBooleanToStringTypeConverter(), 2);
        yield return () => (new StringToBooleanTypeConverter(), 2);
        yield return () => (new StringToNullableBooleanTypeConverter(), 2);

        // Guid ↔ String converters
        yield return () => (new GuidToStringTypeConverter(), 2);
        yield return () => (new NullableGuidToStringTypeConverter(), 2);
        yield return () => (new StringToGuidTypeConverter(), 2);
        yield return () => (new StringToNullableGuidTypeConverter(), 2);

        // DateTime ↔ String converters
        yield return () => (new DateTimeToStringTypeConverter(), 2);
        yield return () => (new NullableDateTimeToStringTypeConverter(), 2);
        yield return () => (new StringToDateTimeTypeConverter(), 2);
        yield return () => (new StringToNullableDateTimeTypeConverter(), 2);

        // DateTimeOffset ↔ String converters
        yield return () => (new DateTimeOffsetToStringTypeConverter(), 2);
        yield return () => (new NullableDateTimeOffsetToStringTypeConverter(), 2);
        yield return () => (new StringToDateTimeOffsetTypeConverter(), 2);
        yield return () => (new StringToNullableDateTimeOffsetTypeConverter(), 2);

        // TimeSpan ↔ String converters
        yield return () => (new TimeSpanToStringTypeConverter(), 2);
        yield return () => (new NullableTimeSpanToStringTypeConverter(), 2);
        yield return () => (new StringToTimeSpanTypeConverter(), 2);
        yield return () => (new StringToNullableTimeSpanTypeConverter(), 2);

#if NET6_0_OR_GREATER

        // DateOnly ↔ String converters (.NET 6+)
        yield return () => (new DateOnlyToStringTypeConverter(), 2);
        yield return () => (new NullableDateOnlyToStringTypeConverter(), 2);
        yield return () => (new StringToDateOnlyTypeConverter(), 2);
        yield return () => (new StringToNullableDateOnlyTypeConverter(), 2);

        // TimeOnly ↔ String converters (.NET 6+)
        yield return () => (new TimeOnlyToStringTypeConverter(), 2);
        yield return () => (new NullableTimeOnlyToStringTypeConverter(), 2);
        yield return () => (new StringToTimeOnlyTypeConverter(), 2);
        yield return () => (new StringToNullableTimeOnlyTypeConverter(), 2);
#endif

        // Uri ↔ String converters
        yield return () => (new UriToStringTypeConverter(), 2);
        yield return () => (new StringToUriTypeConverter(), 2);
    }

    /// <summary>
    ///     Verifies that the EqualityTypeConverter has affinity 1 (last resort).
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task EqualityConverter_ShouldHaveAffinity1()
    {
        // Arrange
        var converter = new EqualityTypeConverter();

        // Act
        var affinity = converter.GetAffinityForObjects();

        // Assert
        await Assert.That(affinity).IsEqualTo(1);
    }

    /// <summary>
    ///     Verifies that all standard converters have affinity 2.
    ///     Standard converters are the core ReactiveUI converters (numeric, string, datetime, etc.).
    /// </summary>
    /// <param name="converter">The converter to test.</param>
    /// <param name="expectedAffinity">The expected affinity value.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    [MethodDataSource(nameof(GetStandardConverters))]
    public async Task StandardConverters_ShouldHaveAffinity2(IBindingTypeConverter converter, int expectedAffinity)
    {
        ArgumentNullException.ThrowIfNull(converter);

        // Act
        var actualAffinity = converter.GetAffinityForObjects();

        // Assert
        await Assert.That(actualAffinity).IsEqualTo(expectedAffinity);
    }
}
