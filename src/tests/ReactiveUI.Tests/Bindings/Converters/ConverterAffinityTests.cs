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
    public static IEnumerable<Func<(IBindingTypeConverter converter, int expectedAffinity)>> GetStandardConverters()
    {
        // String identity converter
        yield return () => (new StringConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // Numeric → String converters
        yield return () => (new ByteToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new NullableByteToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new ShortToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new NullableShortToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new IntegerToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new NullableIntegerToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new LongToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new NullableLongToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new SingleToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new NullableSingleToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new DoubleToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new NullableDoubleToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new DecimalToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new NullableDecimalToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // String → Numeric converters
        yield return () => (new StringToByteTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToNullableByteTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToShortTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToNullableShortTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToIntegerTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToNullableIntegerTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToLongTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToNullableLongTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToSingleTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToNullableSingleTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToDoubleTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToNullableDoubleTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToDecimalTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToNullableDecimalTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // Boolean ↔ String converters
        yield return () => (new BooleanToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new NullableBooleanToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToBooleanTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToNullableBooleanTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // Guid ↔ String converters
        yield return () => (new GuidToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new NullableGuidToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToGuidTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToNullableGuidTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // DateTime ↔ String converters
        yield return () => (new DateTimeToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new NullableDateTimeToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToDateTimeTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToNullableDateTimeTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // DateTimeOffset ↔ String converters
        yield return () => (new DateTimeOffsetToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new NullableDateTimeOffsetToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToDateTimeOffsetTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToNullableDateTimeOffsetTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // TimeSpan ↔ String converters
        yield return () => (new TimeSpanToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new NullableTimeSpanToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToTimeSpanTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToNullableTimeSpanTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

#if NET6_0_OR_GREATER

        // DateOnly ↔ String converters (.NET 6+)
        yield return () => (new DateOnlyToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new NullableDateOnlyToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToDateOnlyTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToNullableDateOnlyTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);

        // TimeOnly ↔ String converters (.NET 6+)
        yield return () => (new TimeOnlyToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new NullableTimeOnlyToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToTimeOnlyTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToNullableTimeOnlyTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
#endif

        // Uri ↔ String converters
        yield return () => (new UriToStringTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
        yield return () => (new StringToUriTypeConverter(), BindingAffinity.DefaultInternalTypeConverter);
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
