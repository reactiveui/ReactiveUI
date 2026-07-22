// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Splat;

namespace ReactiveUI.Tests.Bindings.Converters;

/// <summary>
///     Tests for the ConverterMigrationHelperMixins which assists in migrating from Splat-based
///     converter registration to the new ConverterService-based system.
/// </summary>
public class ConverterMigrationHelperTests
{
    /// <summary>Verifies that ExtractConverters throws when the resolver is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ExtractConverters_ShouldThrowArgumentNullException_WhenResolverIsNull() =>

        // Act & Assert
        await Assert.That(static () => ConverterMigrationHelperMixins.ExtractConverters(null!))
            .Throws<ArgumentException>();

    /// <summary>Verifies that ExtractConverters returns empty lists when no converters are registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ExtractConverters_ShouldReturnEmptyLists_WhenNoConvertersRegistered()
    {
        // Arrange
        var resolver = new TestDependencyResolver();

        // Act
        var (typed, fallback, setMethod) = ConverterMigrationHelperMixins.ExtractConverters(resolver);

        // Assert
        await Assert.That(typed).IsEmpty();
        await Assert.That(fallback).IsEmpty();
        await Assert.That(setMethod).IsEmpty();
    }

    /// <summary>Verifies that ExtractConverters extracts registered typed converters.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ExtractConverters_ShouldExtractTypedConverters()
    {
        // Arrange
        const int FirstAffinity = 5;
        const int SecondAffinity = 3;
        const int ExpectedCount = 2;
        var converter1 = new TestTypedConverter<int, string>(FirstAffinity);
        var converter2 = new TestTypedConverter<double, bool>(SecondAffinity);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<IBindingTypeConverter>(converter1);
        resolver.RegisterService<IBindingTypeConverter>(converter2);

        // Act
        var (typed, fallback, setMethod) = ConverterMigrationHelperMixins.ExtractConverters(resolver);

        // Assert
        await Assert.That(typed).Count().IsEqualTo(ExpectedCount);
        await Assert.That(typed).Contains(converter1);
        await Assert.That(typed).Contains(converter2);
        await Assert.That(fallback).IsEmpty();
        await Assert.That(setMethod).IsEmpty();
    }

    /// <summary>Verifies that ExtractConverters extracts registered fallback converters.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ExtractConverters_ShouldExtractFallbackConverters()
    {
        // Arrange
        const int FirstAffinity = 5;
        const int SecondAffinity = 3;
        const int ExpectedCount = 2;
        var converter1 = new TestFallbackConverter(FirstAffinity);
        var converter2 = new TestFallbackConverter(SecondAffinity);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<IBindingFallbackConverter>(converter1);
        resolver.RegisterService<IBindingFallbackConverter>(converter2);

        // Act
        var (typed, fallback, setMethod) = ConverterMigrationHelperMixins.ExtractConverters(resolver);

        // Assert
        await Assert.That(typed).IsEmpty();
        await Assert.That(fallback).Count().IsEqualTo(ExpectedCount);
        await Assert.That(fallback).Contains(converter1);
        await Assert.That(fallback).Contains(converter2);
        await Assert.That(setMethod).IsEmpty();
    }

    /// <summary>Verifies that ExtractConverters extracts registered set-method converters.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ExtractConverters_ShouldExtractSetMethodConverters()
    {
        // Arrange
        const int FirstAffinity = 5;
        const int SecondAffinity = 3;
        const int ExpectedCount = 2;
        var converter1 = new TestSetMethodConverter(FirstAffinity);
        var converter2 = new TestSetMethodConverter(SecondAffinity);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<ISetMethodBindingConverter>(converter1);
        resolver.RegisterService<ISetMethodBindingConverter>(converter2);

        // Act
        var (typed, fallback, setMethod) = ConverterMigrationHelperMixins.ExtractConverters(resolver);

        // Assert
        await Assert.That(typed).IsEmpty();
        await Assert.That(fallback).IsEmpty();
        await Assert.That(setMethod).Count().IsEqualTo(ExpectedCount);
        await Assert.That(setMethod).Contains(converter1);
        await Assert.That(setMethod).Contains(converter2);
    }

    /// <summary>Verifies that ExtractConverters extracts typed, fallback, and set-method converters together.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ExtractConverters_ShouldExtractAllConverterTypes()
    {
        // Arrange
        const int TypedAffinity = 5;
        const int FallbackAffinity = 3;
        const int SetMethodAffinity = 2;
        var typedConverter = new TestTypedConverter<int, string>(TypedAffinity);
        var fallbackConverter = new TestFallbackConverter(FallbackAffinity);
        var setMethodConverter = new TestSetMethodConverter(SetMethodAffinity);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<IBindingTypeConverter>(typedConverter);
        resolver.RegisterService<IBindingFallbackConverter>(fallbackConverter);
        resolver.RegisterService<ISetMethodBindingConverter>(setMethodConverter);

        // Act
        var (typed, fallback, setMethod) = ConverterMigrationHelperMixins.ExtractConverters(resolver);

        // Assert
        await Assert.That(typed).Count().IsEqualTo(1);
        await Assert.That(typed).Contains(typedConverter);
        await Assert.That(fallback).Count().IsEqualTo(1);
        await Assert.That(fallback).Contains(fallbackConverter);
        await Assert.That(setMethod).Count().IsEqualTo(1);
        await Assert.That(setMethod).Contains(setMethodConverter);
    }

    /// <summary>Verifies that ExtractConverters filters out null converter entries.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ExtractConverters_ShouldFilterOutNullConverters()
    {
        // Arrange
        const int ConverterAffinity = 5;
        var converter = new TestTypedConverter<int, string>(ConverterAffinity);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<IBindingTypeConverter>(converter);
        resolver.RegisterService<IBindingTypeConverter>(null!);

        // Act
        var (typed, _, _) = ConverterMigrationHelperMixins.ExtractConverters(resolver);

        // Assert - Should only contain the non-null converter
        await Assert.That(typed).Count().IsEqualTo(1);
        await Assert.That(typed).Contains(converter);
    }

    /// <summary>Verifies that ImportFrom throws when the converter service is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ImportFrom_ShouldThrowArgumentNullException_WhenConverterServiceIsNull()
    {
        // Arrange
        var resolver = new TestDependencyResolver();

        // Act & Assert
        await Assert.That(() => ((ConverterService)null!).ImportFrom(resolver))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that ImportFrom throws when the resolver is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ImportFrom_ShouldThrowArgumentNullException_WhenResolverIsNull()
    {
        // Arrange
        var service = new ConverterService();

        // Act & Assert
        await Assert.That(() => service.ImportFrom(null!))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that ImportFrom imports typed converters into the converter service.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ImportFrom_ShouldImportTypedConverters()
    {
        // Arrange
        const int FirstAffinity = 5;
        const int SecondAffinity = 3;
        var converter1 = new TestTypedConverter<int, string>(FirstAffinity);
        var converter2 = new TestTypedConverter<double, bool>(SecondAffinity);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<IBindingTypeConverter>(converter1);
        resolver.RegisterService<IBindingTypeConverter>(converter2);
        var service = new ConverterService();

        // Act
        service.ImportFrom(resolver);

        // Assert
        var result1 = service.ResolveConverter(typeof(int), typeof(string));
        var result2 = service.ResolveConverter(typeof(double), typeof(bool));
        await Assert.That(result1).IsEqualTo(converter1);
        await Assert.That(result2).IsEqualTo(converter2);
    }

    /// <summary>Verifies that ImportFrom imports fallback converters into the converter service.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ImportFrom_ShouldImportFallbackConverters()
    {
        // Arrange
        const int ConverterAffinity = 5;
        var converter = new TestFallbackConverter(ConverterAffinity);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<IBindingFallbackConverter>(converter);
        var service = new ConverterService();

        // Act
        service.ImportFrom(resolver);

        // Assert - Fallback converters should be used when no typed converter matches
        var result = service.ResolveConverter(typeof(int), typeof(string));
        await Assert.That(result).IsEqualTo(converter);
    }

    /// <summary>Verifies that ImportFrom imports set-method converters into the converter service.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ImportFrom_ShouldImportSetMethodConverters()
    {
        // Arrange
        const int ConverterAffinity = 5;
        var converter = new TestSetMethodConverter(ConverterAffinity);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<ISetMethodBindingConverter>(converter);
        var service = new ConverterService();

        // Act
        service.ImportFrom(resolver);

        // Assert
        var result = service.ResolveSetMethodConverter(typeof(int), typeof(string));
        await Assert.That(result).IsEqualTo(converter);
    }

    /// <summary>Verifies that ImportFrom imports typed, fallback, and set-method converters together.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ImportFrom_ShouldImportAllConverterTypes()
    {
        // Arrange
        const int TypedAffinity = 5;
        const int FallbackAffinity = 3;
        const int SetMethodAffinity = 2;
        var typedConverter = new TestTypedConverter<int, string>(TypedAffinity);
        var fallbackConverter = new TestFallbackConverter(FallbackAffinity);
        var setMethodConverter = new TestSetMethodConverter(SetMethodAffinity);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<IBindingTypeConverter>(typedConverter);
        resolver.RegisterService<IBindingFallbackConverter>(fallbackConverter);
        resolver.RegisterService<ISetMethodBindingConverter>(setMethodConverter);
        var service = new ConverterService();

        // Act
        service.ImportFrom(resolver);

        // Assert
        var typedResult = service.ResolveConverter(typeof(int), typeof(string));
        var setMethodResult = service.ResolveSetMethodConverter(typeof(int), typeof(string));

        await Assert.That(typedResult).IsEqualTo(typedConverter);
        await Assert.That(setMethodResult).IsEqualTo(setMethodConverter);
    }

    /// <summary>Verifies that ImportFrom does not import null converter entries.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ImportFrom_ShouldNotImportNullConverters()
    {
        // Arrange
        const int ConverterAffinity = 5;
        var converter = new TestTypedConverter<int, string>(ConverterAffinity);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<IBindingTypeConverter>(converter);
        resolver.RegisterService<IBindingTypeConverter>(null!);
        var service = new ConverterService();

        // Act
        service.ImportFrom(resolver);

        // Assert - Should only import the non-null converter
        var result = service.ResolveConverter(typeof(int), typeof(string));
        await Assert.That(result).IsEqualTo(converter);
    }

    /// <summary>Test dependency resolver for testing converter extraction.</summary>
    private sealed class TestDependencyResolver : IReadonlyDependencyResolver
    {
        /// <summary>The registered service instances.</summary>
        private readonly List<object> _services = [];

        /// <summary>Registers a service instance for later resolution.</summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="service">The service instance to register.</param>
        public void RegisterService<T>(T? service) => _services.Add(service!);

        /// <inheritdoc/>
        public object? GetService(Type? serviceType) => _services.FirstOrDefault();

        /// <inheritdoc/>
        public object? GetService(Type? serviceType, string? contract) => _services.FirstOrDefault();

        /// <inheritdoc/>
        public T? GetService<T>() => _services.OfType<T>().FirstOrDefault();

        /// <inheritdoc/>
        public T? GetService<T>(string? contract) => _services.OfType<T>().FirstOrDefault();

        /// <inheritdoc/>
        public IEnumerable<object> GetServices(Type? serviceType) => _services.Where(static s => s is not null);

        /// <inheritdoc/>
        public IEnumerable<object> GetServices(Type? serviceType, string? contract) =>
            _services.Where(static s => s is not null);

        /// <inheritdoc/>
        public IEnumerable<T> GetServices<T>() => _services.OfType<T>();

        /// <inheritdoc/>
        public IEnumerable<T> GetServices<T>(string? contract) => _services.OfType<T>();
    }

    /// <summary>Test typed converter for testing purposes.</summary>
    /// <typeparam name="TFrom">The source type.</typeparam>
    /// <typeparam name="TTo">The target type.</typeparam>
    private sealed class TestTypedConverter<TFrom, TTo> : BindingTypeConverter<TFrom, TTo>
    {
        /// <summary>The affinity value to report.</summary>
        private readonly int _affinity;

        /// <summary>Initializes a new instance of the <see cref="TestTypedConverter{TFrom, TTo}"/> class.</summary>
        /// <param name="affinity">The affinity value to report.</param>
        public TestTypedConverter(int affinity) => _affinity = affinity;

        /// <inheritdoc/>
        public override int GetAffinityForObjects() => _affinity;

        /// <inheritdoc/>
        public override bool TryConvert(TFrom? from, object? conversionHint, [NotNullWhen(true)] out TTo result)
        {
            result = default!;
            return false;
        }
    }

    /// <summary>Test fallback converter for testing purposes.</summary>
    private sealed class TestFallbackConverter : IBindingFallbackConverter
    {
        /// <summary>The affinity value to report.</summary>
        private readonly int _affinity;

        /// <summary>Initializes a new instance of the <see cref="TestFallbackConverter"/> class.</summary>
        /// <param name="affinity">The affinity value to report.</param>
        public TestFallbackConverter(int affinity) => _affinity = affinity;

        /// <inheritdoc/>
        public int GetAffinityForObjects(Type fromType, Type toType) => _affinity;

        /// <inheritdoc/>
        public bool TryConvert(
            Type fromType,
            object from,
            Type toType,
            object? conversionHint,
            [NotNullWhen(true)] out object? result)
        {
            result = null;
            return false;
        }
    }

    /// <summary>Test set method converter for testing purposes.</summary>
    private sealed class TestSetMethodConverter : ISetMethodBindingConverter
    {
        /// <summary>The affinity value to report.</summary>
        private readonly int _affinity;

        /// <summary>Initializes a new instance of the <see cref="TestSetMethodConverter"/> class.</summary>
        /// <param name="affinity">The affinity value to report.</param>
        public TestSetMethodConverter(int affinity) => _affinity = affinity;

        /// <inheritdoc/>
        public int GetAffinityForObjects(Type? fromType, Type? toType) => _affinity;

        /// <inheritdoc/>
        public object? PerformSet(object? toTarget, object? newValue, object?[]? arguments) => null;
    }
}
