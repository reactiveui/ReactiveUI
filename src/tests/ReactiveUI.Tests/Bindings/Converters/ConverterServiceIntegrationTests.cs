// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.Bindings.Converters;

/// <summary>
/// Integration tests for the ConverterService.
/// Verifies end-to-end converter resolution with typed and fallback converters.
/// </summary>
public class ConverterServiceIntegrationTests
{
    /// <summary>
    /// Verifies that typed converters are selected before fallback converters.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task TypedConverter_ShouldBePreferredOverFallback()
    {
        // Arrange
        var service = new ConverterService();
        var typedConverter = new TestTypedConverter<int, string>(affinity: 2);
        var fallbackConverter = new TestFallbackConverter(baseAffinity: 10); // Higher affinity but should lose to typed

        service.TypedConverters.Register(typedConverter);
        service.FallbackConverters.Register(fallbackConverter);

        // Act
        var result = service.ResolveConverter(typeof(int), typeof(string));

        // Assert - Typed converter should win even with lower affinity
        await Assert.That(result).IsEqualTo(typedConverter);
    }

    /// <summary>
    /// Verifies that fallback converters are used when no typed converter matches.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task FallbackConverter_ShouldBeUsedWhenNoTypedMatch()
    {
        // Arrange
        var service = new ConverterService();
        var typedConverter = new TestTypedConverter<int, string>(affinity: 5);
        var fallbackConverter = new TestFallbackConverter(baseAffinity: 3);

        service.TypedConverters.Register(typedConverter);
        service.FallbackConverters.Register(fallbackConverter);

        // Act - Request different type pair (not int->string)
        var result = service.ResolveConverter(typeof(double), typeof(bool));

        // Assert - Fallback converter should be used
        await Assert.That(result).IsEqualTo(fallbackConverter);
    }

    /// <summary>
    /// Verifies that null is returned when no converter matches.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task NoConverter_ShouldReturnNull()
    {
        // Arrange
        var service = new ConverterService();
        var converter = new TestTypedConverter<int, string>(affinity: 5);
        service.TypedConverters.Register(converter);

        // Act
        var result = service.ResolveConverter(typeof(double), typeof(bool));

        // Assert
        await Assert.That(result).IsNull();
    }

    /// <summary>
    /// Verifies that custom converters with high affinity can override defaults.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task CustomHighAffinityConverter_ShouldOverrideDefault()
    {
        // Arrange
        var service = new ConverterService();
        var defaultConverter = new TestTypedConverter<int, string>(affinity: 2);
        var customConverter = new TestTypedConverter<int, string>(affinity: 100);

        service.TypedConverters.Register(defaultConverter);
        service.TypedConverters.Register(customConverter);

        // Act
        var result = service.ResolveConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(result).IsEqualTo(customConverter);
    }

    /// <summary>
    /// Verifies that the highest affinity fallback converter is selected.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task MultipleFallbackConverters_ShouldSelectHighestAffinity()
    {
        // Arrange
        var service = new ConverterService();
        var lowAffinity = new TestFallbackConverter(baseAffinity: 2);
        var mediumAffinity = new TestFallbackConverter(baseAffinity: 5);
        var highAffinity = new TestFallbackConverter(baseAffinity: 10);

        service.FallbackConverters.Register(mediumAffinity);
        service.FallbackConverters.Register(lowAffinity);
        service.FallbackConverters.Register(highAffinity);

        // Act
        var result = service.ResolveConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(result).IsEqualTo(highAffinity);
    }

    /// <summary>
    /// Verifies end-to-end integration with real converters.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task RealConverters_ShouldResolveCorrectly()
    {
        // Arrange
        var service = new ConverterService();
        var intToString = new IntegerToStringTypeConverter();
        var stringToInt = new StringToIntegerTypeConverter();
        var equality = new EqualityTypeConverter();

        service.TypedConverters.Register(intToString);
        service.TypedConverters.Register(stringToInt);
        service.TypedConverters.Register(equality);

        // Act
        var result1 = service.ResolveConverter(typeof(int), typeof(string));
        var result2 = service.ResolveConverter(typeof(string), typeof(int));

        // Assert
        await Assert.That(result1).IsEqualTo(intToString);
        await Assert.That(result2).IsEqualTo(stringToInt);
    }

    /// <summary>
    /// Verifies that all three registries are accessible.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task ConverterService_ShouldExposeAllRegistries()
    {
        // Arrange
        var service = new ConverterService();

        // Assert
        await Assert.That(service.TypedConverters).IsNotNull();
        await Assert.That(service.FallbackConverters).IsNotNull();
        await Assert.That(service.SetMethodConverters).IsNotNull();
    }

    /// <summary>
    /// Verifies that set-method converters can be registered and retrieved.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task SetMethodConverter_ShouldBeRetrievable()
    {
        // Arrange
        var service = new ConverterService();
        var setMethodConverter = new TestSetMethodConverter(baseAffinity: 8);

        service.SetMethodConverters.Register(setMethodConverter);

        // Act
        var result = service.SetMethodConverters.TryGetConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(result).IsEqualTo(setMethodConverter);
    }

    /// <summary>
    /// Verifies that RxConverters.Current works after being set.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task RxConverters_CurrentShouldBeAccessible()
    {
        // Arrange
        var service = new ConverterService();
        var converter = new TestTypedConverter<int, string>(affinity: 5);
        service.TypedConverters.Register(converter);

        // Act
        RxConverters.SetService(service);
        var result = RxConverters.Current.ResolveConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(result).IsEqualTo(converter);

        // Cleanup - reset to default
        RxConverters.SetService(new ConverterService());
    }

    /// <summary>
    /// Verifies that converters with affinity 0 are ignored in resolution.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task ZeroAffinityConverter_ShouldBeIgnoredInResolution()
    {
        // Arrange
        var service = new ConverterService();
        var zeroAffinity = new TestTypedConverter<int, string>(affinity: 0);
        var validAffinity = new TestTypedConverter<int, string>(affinity: 2);

        service.TypedConverters.Register(zeroAffinity);
        service.TypedConverters.Register(validAffinity);

        // Act
        var result = service.ResolveConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(result).IsEqualTo(validAffinity);
    }

    private sealed class TestTypedConverter<TFrom, TTo> : BindingTypeConverter<TFrom, TTo>
    {
        private readonly int _affinity;

        public TestTypedConverter(int affinity) => _affinity = affinity;

        public override int GetAffinityForObjects() => _affinity;

        public override bool TryConvert(TFrom? from, object? conversionHint, [NotNullWhen(true)] out TTo? result)
        {
            result = default;
            return false;
        }
    }

    private sealed class TestFallbackConverter : IBindingFallbackConverter
    {
        private readonly int _baseAffinity;

        public TestFallbackConverter(int baseAffinity) => _baseAffinity = baseAffinity;

        public int GetAffinityForObjects([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType) => _baseAffinity;

        public bool TryConvert([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType, object from, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType, object? conversionHint, [NotNullWhen(true)] out object? result)
        {
            result = null;
            return false;
        }
    }

    private sealed class TestSetMethodConverter : ISetMethodBindingConverter
    {
        private readonly int _baseAffinity;

        public TestSetMethodConverter(int baseAffinity) => _baseAffinity = baseAffinity;

        public int GetAffinityForObjects(Type? fromType, Type? toType) => _baseAffinity;

        public object? PerformSet(object? toTarget, object? newValue, object?[]? arguments) => newValue;
    }
}
