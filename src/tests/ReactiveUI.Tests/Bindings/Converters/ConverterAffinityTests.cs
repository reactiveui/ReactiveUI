// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
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
    /// <summary>Data source for standard converters (affinity = 2).</summary>
    /// <returns>A sequence of converter test data with expected affinity values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1024:Use properties where appropriate",
        Justification = "Data source must be a method for TUnit MethodDataSource.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Maintainability",
        "SST1523:Method is over the maximum line count",
        Justification = "Exhaustive one-per-converter registration table; splitting it fragments a single lookup that is read top to bottom.")]
    public static IEnumerable<Func<(IBindingTypeConverter converter, int expectedAffinity)>> GetStandardConverters()
    {
        // String identity converter
        yield return static () => (new StringConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // Numeric → String converters
        yield return static () => (new ByteToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new NullableByteToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new ShortToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new NullableShortToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new IntegerToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new NullableIntegerToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new LongToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new NullableLongToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new SingleToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new NullableSingleToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new DoubleToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new NullableDoubleToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new DecimalToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new NullableDecimalToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // String → Numeric converters
        yield return static () => (new StringToByteTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToNullableByteTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToShortTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToNullableShortTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToIntegerTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToNullableIntegerTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToLongTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToNullableLongTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToSingleTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToNullableSingleTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToDoubleTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToNullableDoubleTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToDecimalTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToNullableDecimalTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // Boolean ↔ String converters
        yield return static () => (new BooleanToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new NullableBooleanToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToBooleanTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToNullableBooleanTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // Guid ↔ String converters
        yield return static () => (new GuidToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new NullableGuidToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToGuidTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToNullableGuidTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // DateTime ↔ String converters
        yield return static () => (new DateTimeToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new NullableDateTimeToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToDateTimeTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToNullableDateTimeTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // DateTimeOffset ↔ String converters
        yield return static () => (new DateTimeOffsetToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new NullableDateTimeOffsetToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToDateTimeOffsetTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToNullableDateTimeOffsetTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // TimeSpan ↔ String converters
        yield return static () => (new TimeSpanToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new NullableTimeSpanToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToTimeSpanTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToNullableTimeSpanTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

#if NET6_0_OR_GREATER

        // DateOnly ↔ String converters (.NET 6+)
        yield return static () => (new DateOnlyToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new NullableDateOnlyToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToDateOnlyTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToNullableDateOnlyTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // TimeOnly ↔ String converters (.NET 6+)
        yield return static () => (new TimeOnlyToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new NullableTimeOnlyToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToTimeOnlyTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToNullableTimeOnlyTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
#endif

        // Uri ↔ String converters
        yield return static () => (new UriToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return static () => (new StringToUriTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>Verifies that the EqualityTypeConverter has affinity 1 (last resort).</summary>
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
