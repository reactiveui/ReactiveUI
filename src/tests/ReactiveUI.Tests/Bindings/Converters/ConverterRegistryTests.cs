// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.Bindings.Converters;

/// <summary>
/// Tests for the lock-free converter registries.
/// Verifies thread-safety, affinity-based selection, and snapshot pattern behavior.
/// </summary>
public class ConverterRegistryTests
{
    /// <summary>
    /// Verifies that a registered converter can be retrieved.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task Register_AndRetrieve_ShouldReturnConverter()
    {
        // Arrange
        var registry = new BindingTypeConverterRegistry();
        var converter = new TestConverter<int, string>(affinity: 5);

        // Act
        registry.Register(converter);
        var retrieved = registry.TryGetConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved).IsEqualTo(converter);
    }

    /// <summary>
    /// Verifies that when multiple converters are registered for the same type pair,
    /// the one with the highest affinity is selected.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task MultipleConverters_ShouldSelectHighestAffinity()
    {
        // Arrange
        var registry = new BindingTypeConverterRegistry();
        var lowAffinity = new TestConverter<int, string>(affinity: 2);
        var mediumAffinity = new TestConverter<int, string>(affinity: 5);
        var highAffinity = new TestConverter<int, string>(affinity: 10);

        // Act - register in random order
        registry.Register(mediumAffinity);
        registry.Register(lowAffinity);
        registry.Register(highAffinity);

        var selected = registry.TryGetConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(selected).IsEqualTo(highAffinity);
    }

    /// <summary>
    /// Verifies that converters with affinity 0 are ignored.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task ConverterWithZeroAffinity_ShouldBeIgnored()
    {
        // Arrange
        var registry = new BindingTypeConverterRegistry();
        var zeroAffinity = new TestConverter<int, string>(affinity: 0);
        var validAffinity = new TestConverter<int, string>(affinity: 2);

        // Act
        registry.Register(zeroAffinity);
        registry.Register(validAffinity);

        var selected = registry.TryGetConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(selected).IsEqualTo(validAffinity);
    }

    /// <summary>
    /// Verifies that converters with negative affinity are ignored.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task ConverterWithNegativeAffinity_ShouldBeIgnored()
    {
        // Arrange
        var registry = new BindingTypeConverterRegistry();
        var negativeAffinity = new TestConverter<int, string>(affinity: -5);
        var validAffinity = new TestConverter<int, string>(affinity: 2);

        // Act
        registry.Register(negativeAffinity);
        registry.Register(validAffinity);

        var selected = registry.TryGetConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(selected).IsEqualTo(validAffinity);
    }

    /// <summary>
    /// Verifies that requesting a non-existent type pair returns null.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task NonExistentTypePair_ShouldReturnNull()
    {
        // Arrange
        var registry = new BindingTypeConverterRegistry();
        var converter = new TestConverter<int, string>(affinity: 5);
        registry.Register(converter);

        // Act
        var result = registry.TryGetConverter(typeof(double), typeof(bool));

        // Assert
        await Assert.That(result).IsNull();
    }

    /// <summary>
    /// Verifies that an empty registry returns null.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task EmptyRegistry_ShouldReturnNull()
    {
        // Arrange
        var registry = new BindingTypeConverterRegistry();

        // Act
        var result = registry.TryGetConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(result).IsNull();
    }

    /// <summary>
    /// Verifies that the registry supports concurrent reads during registration.
    /// This tests the lock-free snapshot pattern.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task ConcurrentReads_DuringRegistration_ShouldBeThreadSafe()
    {
        // Arrange
        var registry = new BindingTypeConverterRegistry();
        var converter1 = new TestConverter<int, string>(affinity: 5);
        var converter2 = new TestConverter<double, bool>(affinity: 3);
        registry.Register(converter1);

        var readTasks = new List<Task<IBindingTypeConverter?>>();
        var writeTasks = new List<Task>();

        // Act - Start concurrent reads and writes
        for (var i = 0; i < 100; i++)
        {
            // Concurrent reads
            readTasks.Add(Task.Run(() => registry.TryGetConverter(typeof(int), typeof(string))));

            // Concurrent write
            if (i == 50)
            {
                writeTasks.Add(Task.Run(() => registry.Register(converter2)));
            }
        }

        await Task.WhenAll(readTasks.Cast<Task>().Concat(writeTasks));

        // Assert - All reads should have completed successfully
        foreach (var task in readTasks)
        {
            var result = await task;
            await Assert.That(result).IsNotNull(); // Should always get converter1
        }

        // Verify both converters are registered
        var finalCheck1 = registry.TryGetConverter(typeof(int), typeof(string));
        var finalCheck2 = registry.TryGetConverter(typeof(double), typeof(bool));

        await Assert.That(finalCheck1).IsEqualTo(converter1);
        await Assert.That(finalCheck2).IsEqualTo(converter2);
    }

    /// <summary>
    /// Verifies that GetAllConverters returns all registered converters.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAllConverters_ShouldReturnAllRegistered()
    {
        // Arrange
        var registry = new BindingTypeConverterRegistry();
        var converter1 = new TestConverter<int, string>(affinity: 5);
        var converter2 = new TestConverter<double, bool>(affinity: 3);
        var converter3 = new TestConverter<string, int>(affinity: 2);

        // Act
        registry.Register(converter1);
        registry.Register(converter2);
        registry.Register(converter3);

        var allConverters = registry.GetAllConverters().ToList();

        // Assert
        await Assert.That(allConverters.Count).IsEqualTo(3);
        await Assert.That(allConverters).Contains(converter1);
        await Assert.That(allConverters).Contains(converter2);
        await Assert.That(allConverters).Contains(converter3);
    }

    /// <summary>
    /// Verifies that fallback converter registry works correctly.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task FallbackRegistry_ShouldSelectHighestAffinity()
    {
        // Arrange
        var registry = new BindingFallbackConverterRegistry();
        var lowAffinity = new TestFallbackConverter(baseAffinity: 2);
        var highAffinity = new TestFallbackConverter(baseAffinity: 10);

        // Act
        registry.Register(lowAffinity);
        registry.Register(highAffinity);

        var selected = registry.TryGetConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(selected).IsEqualTo(highAffinity);
    }

    /// <summary>
    /// Verifies that set-method converter registry works correctly.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task SetMethodRegistry_ShouldSelectHighestAffinity()
    {
        // Arrange
        var registry = new SetMethodBindingConverterRegistry();
        var lowAffinity = new TestSetMethodConverter(baseAffinity: 2);
        var highAffinity = new TestSetMethodConverter(baseAffinity: 8);

        // Act
        registry.Register(lowAffinity);
        registry.Register(highAffinity);

        var selected = registry.TryGetConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(selected).IsEqualTo(highAffinity);
    }

    private sealed class TestConverter<TFrom, TTo> : BindingTypeConverter<TFrom, TTo>
    {
        private readonly int _affinity;

        public TestConverter(int affinity) => _affinity = affinity;

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
