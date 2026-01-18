// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.Bindings.Converters;

/// <summary>
///     Tests for the ConverterMigrationHelper which assists in migrating from Splat-based
///     converter registration to the new ConverterService-based system.
/// </summary>
public class ConverterMigrationHelperTests
{
    [Test]
    public async Task ExtractConverters_ShouldThrowArgumentNullException_WhenResolverIsNull()
    {
        // Act & Assert
        await Assert.That(() => ConverterMigrationHelper.ExtractConverters(null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ExtractConverters_ShouldReturnEmptyLists_WhenNoConvertersRegistered()
    {
        // Arrange
        var resolver = new TestDependencyResolver();

        // Act
        var (typed, fallback, setMethod) = ConverterMigrationHelper.ExtractConverters(resolver);

        // Assert
        await Assert.That(typed).IsEmpty();
        await Assert.That(fallback).IsEmpty();
        await Assert.That(setMethod).IsEmpty();
    }

    [Test]
    public async Task ExtractConverters_ShouldExtractTypedConverters()
    {
        // Arrange
        var converter1 = new TestTypedConverter<int, string>(5);
        var converter2 = new TestTypedConverter<double, bool>(3);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<IBindingTypeConverter>(converter1);
        resolver.RegisterService<IBindingTypeConverter>(converter2);

        // Act
        var (typed, fallback, setMethod) = ConverterMigrationHelper.ExtractConverters(resolver);

        // Assert
        await Assert.That(typed).Count().IsEqualTo(2);
        await Assert.That(typed).Contains(converter1);
        await Assert.That(typed).Contains(converter2);
        await Assert.That(fallback).IsEmpty();
        await Assert.That(setMethod).IsEmpty();
    }

    [Test]
    public async Task ExtractConverters_ShouldExtractFallbackConverters()
    {
        // Arrange
        var converter1 = new TestFallbackConverter(5);
        var converter2 = new TestFallbackConverter(3);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<IBindingFallbackConverter>(converter1);
        resolver.RegisterService<IBindingFallbackConverter>(converter2);

        // Act
        var (typed, fallback, setMethod) = ConverterMigrationHelper.ExtractConverters(resolver);

        // Assert
        await Assert.That(typed).IsEmpty();
        await Assert.That(fallback).Count().IsEqualTo(2);
        await Assert.That(fallback).Contains(converter1);
        await Assert.That(fallback).Contains(converter2);
        await Assert.That(setMethod).IsEmpty();
    }

    [Test]
    public async Task ExtractConverters_ShouldExtractSetMethodConverters()
    {
        // Arrange
        var converter1 = new TestSetMethodConverter(5);
        var converter2 = new TestSetMethodConverter(3);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<ISetMethodBindingConverter>(converter1);
        resolver.RegisterService<ISetMethodBindingConverter>(converter2);

        // Act
        var (typed, fallback, setMethod) = ConverterMigrationHelper.ExtractConverters(resolver);

        // Assert
        await Assert.That(typed).IsEmpty();
        await Assert.That(fallback).IsEmpty();
        await Assert.That(setMethod).Count().IsEqualTo(2);
        await Assert.That(setMethod).Contains(converter1);
        await Assert.That(setMethod).Contains(converter2);
    }

    [Test]
    public async Task ExtractConverters_ShouldExtractAllConverterTypes()
    {
        // Arrange
        var typedConverter = new TestTypedConverter<int, string>(5);
        var fallbackConverter = new TestFallbackConverter(3);
        var setMethodConverter = new TestSetMethodConverter(2);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<IBindingTypeConverter>(typedConverter);
        resolver.RegisterService<IBindingFallbackConverter>(fallbackConverter);
        resolver.RegisterService<ISetMethodBindingConverter>(setMethodConverter);

        // Act
        var (typed, fallback, setMethod) = ConverterMigrationHelper.ExtractConverters(resolver);

        // Assert
        await Assert.That(typed).Count().IsEqualTo(1);
        await Assert.That(typed).Contains(typedConverter);
        await Assert.That(fallback).Count().IsEqualTo(1);
        await Assert.That(fallback).Contains(fallbackConverter);
        await Assert.That(setMethod).Count().IsEqualTo(1);
        await Assert.That(setMethod).Contains(setMethodConverter);
    }

    [Test]
    public async Task ExtractConverters_ShouldFilterOutNullConverters()
    {
        // Arrange
        var converter = new TestTypedConverter<int, string>(5);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<IBindingTypeConverter>(converter);
        resolver.RegisterService<IBindingTypeConverter>(null!);

        // Act
        var (typed, fallback, setMethod) = ConverterMigrationHelper.ExtractConverters(resolver);

        // Assert - Should only contain the non-null converter
        await Assert.That(typed).Count().IsEqualTo(1);
        await Assert.That(typed).Contains(converter);
    }

    [Test]
    public async Task ImportFrom_ShouldThrowArgumentNullException_WhenConverterServiceIsNull()
    {
        // Arrange
        var resolver = new TestDependencyResolver();

        // Act & Assert
        await Assert.That(() => ((ConverterService)null!).ImportFrom(resolver))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ImportFrom_ShouldThrowArgumentNullException_WhenResolverIsNull()
    {
        // Arrange
        var service = new ConverterService();

        // Act & Assert
        await Assert.That(() => service.ImportFrom(null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ImportFrom_ShouldImportTypedConverters()
    {
        // Arrange
        var converter1 = new TestTypedConverter<int, string>(5);
        var converter2 = new TestTypedConverter<double, bool>(3);
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

    [Test]
    public async Task ImportFrom_ShouldImportFallbackConverters()
    {
        // Arrange
        var converter = new TestFallbackConverter(5);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<IBindingFallbackConverter>(converter);
        var service = new ConverterService();

        // Act
        service.ImportFrom(resolver);

        // Assert - Fallback converters should be used when no typed converter matches
        var result = service.ResolveConverter(typeof(int), typeof(string));
        await Assert.That(result).IsEqualTo(converter);
    }

    [Test]
    public async Task ImportFrom_ShouldImportSetMethodConverters()
    {
        // Arrange
        var converter = new TestSetMethodConverter(5);
        var resolver = new TestDependencyResolver();
        resolver.RegisterService<ISetMethodBindingConverter>(converter);
        var service = new ConverterService();

        // Act
        service.ImportFrom(resolver);

        // Assert
        var result = service.ResolveSetMethodConverter(typeof(int), typeof(string));
        await Assert.That(result).IsEqualTo(converter);
    }

    [Test]
    public async Task ImportFrom_ShouldImportAllConverterTypes()
    {
        // Arrange
        var typedConverter = new TestTypedConverter<int, string>(5);
        var fallbackConverter = new TestFallbackConverter(3);
        var setMethodConverter = new TestSetMethodConverter(2);
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

    [Test]
    public async Task ImportFrom_ShouldNotImportNullConverters()
    {
        // Arrange
        var converter = new TestTypedConverter<int, string>(5);
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

    /// <summary>
    /// Test dependency resolver for testing converter extraction.
    /// </summary>
    private sealed class TestDependencyResolver : IReadonlyDependencyResolver
    {
        private readonly List<object> _services = [];

        public void RegisterService<T>(T? service)
        {
            _services.Add(service!);
        }

        public object? GetService(Type? serviceType) => _services.FirstOrDefault();

        public object? GetService(Type? serviceType, string? contract) => _services.FirstOrDefault();

        public T? GetService<T>() => _services.OfType<T>().FirstOrDefault();

        public T? GetService<T>(string? contract) => _services.OfType<T>().FirstOrDefault();

        public IEnumerable<object> GetServices(Type? serviceType) => _services.Where(s => s is not null)!;

        public IEnumerable<object> GetServices(Type? serviceType, string? contract) => _services.Where(s => s is not null)!;

        public IEnumerable<T> GetServices<T>() => _services.OfType<T>();

        public IEnumerable<T> GetServices<T>(string? contract) => _services.OfType<T>();
    }

    /// <summary>
    /// Test typed converter for testing purposes.
    /// </summary>
    private sealed class TestTypedConverter<TFrom, TTo> : BindingTypeConverter<TFrom, TTo>
    {
        private readonly int _affinity;

        public TestTypedConverter(int affinity) => _affinity = affinity;

        public override int GetAffinityForObjects() => _affinity;

        public override bool TryConvert(TFrom? from, object? conversionHint, [NotNullWhen(true)] out TTo result)
        {
            result = default!;
            return false;
        }
    }

    /// <summary>
    /// Test fallback converter for testing purposes.
    /// </summary>
    private sealed class TestFallbackConverter : IBindingFallbackConverter
    {
        private readonly int _affinity;

        public TestFallbackConverter(int affinity) => _affinity = affinity;

        public int GetAffinityForObjects(Type fromType, Type toType) => _affinity;

        public bool TryConvert(Type fromType, object from, Type toType, object? conversionHint, [NotNullWhen(true)] out object? result)
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Test set method converter for testing purposes.
    /// </summary>
    private sealed class TestSetMethodConverter : ISetMethodBindingConverter
    {
        private readonly int _affinity;

        public TestSetMethodConverter(int affinity) => _affinity = affinity;

        public int GetAffinityForObjects(Type? fromType, Type? toType) => _affinity;

        public object? PerformSet(object? toTarget, object? newValue, object?[]? arguments) => null;
    }
}
