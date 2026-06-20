// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.Bindings.Converters;

/// <summary>
///     Tests for the lock-free converter registries.
///     Verifies thread-safety, affinity-based selection, and snapshot pattern behavior.
/// </summary>
public class ConverterRegistryTests
{
    /// <summary>Verifies that the registry supports concurrent reads during registration. This tests the lock-free snapshot pattern.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task ConcurrentReads_DuringRegistration_ShouldBeThreadSafe()
    {
        // Arrange
        const int FirstAffinity = 5;
        const int SecondAffinity = 3;
        const int IterationCount = 100;
        const int WriteIteration = 50;
        var registry = new BindingTypeConverterRegistry();
        var converter1 = new TestConverter<int, string>(FirstAffinity);
        var converter2 = new TestConverter<double, bool>(SecondAffinity);
        registry.Register(converter1);

        var readTasks = new List<Task<IBindingTypeConverter?>>();
        var writeTasks = new List<Task>();

        // Act - Start concurrent reads and writes
        for (var i = 0; i < IterationCount; i++)
        {
            // Concurrent reads
            readTasks.Add(Task.Run(() => registry.TryGetConverter(typeof(int), typeof(string))));

            // Concurrent write
            if (i == WriteIteration)
            {
                writeTasks.Add(Task.Run(() => registry.Register(converter2)));
            }
        }

        await Task.WhenAll(readTasks.Concat(writeTasks));

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

    /// <summary>Verifies that converters with negative affinity are ignored.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task ConverterWithNegativeAffinity_ShouldBeIgnored()
    {
        // Arrange
        const int NegativeAffinity = -5;
        const int ValidAffinity = 2;
        var registry = new BindingTypeConverterRegistry();
        var negativeAffinity = new TestConverter<int, string>(NegativeAffinity);
        var validAffinity = new TestConverter<int, string>(ValidAffinity);

        // Act
        registry.Register(negativeAffinity);
        registry.Register(validAffinity);

        var selected = registry.TryGetConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(selected).IsEqualTo(validAffinity);
    }

    /// <summary>Verifies that converters with affinity 0 are ignored.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task ConverterWithZeroAffinity_ShouldBeIgnored()
    {
        // Arrange
        const int ValidAffinity = 2;
        var registry = new BindingTypeConverterRegistry();
        var zeroAffinity = new TestConverter<int, string>(0);
        var validAffinity = new TestConverter<int, string>(ValidAffinity);

        // Act
        registry.Register(zeroAffinity);
        registry.Register(validAffinity);

        var selected = registry.TryGetConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(selected).IsEqualTo(validAffinity);
    }

    /// <summary>Verifies that an empty registry returns null.</summary>
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

    /// <summary>Verifies that fallback converter registry works correctly.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task FallbackRegistry_ShouldSelectHighestAffinity()
    {
        // Arrange
        const int LowAffinity = 2;
        const int HighAffinity = 10;
        var registry = new BindingFallbackConverterRegistry();
        var lowAffinity = new TestFallbackConverter(LowAffinity);
        var highAffinity = new TestFallbackConverter(HighAffinity);

        // Act
        registry.Register(lowAffinity);
        registry.Register(highAffinity);

        var selected = registry.TryGetConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(selected).IsEqualTo(highAffinity);
    }

    /// <summary>Verifies that GetAllConverters returns all registered converters.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAllConverters_ShouldReturnAllRegistered()
    {
        // Arrange
        const int FirstAffinity = 5;
        const int SecondAffinity = 3;
        const int ThirdAffinity = 2;
        const int ExpectedConverterCount = 3;
        var registry = new BindingTypeConverterRegistry();
        var converter1 = new TestConverter<int, string>(FirstAffinity);
        var converter2 = new TestConverter<double, bool>(SecondAffinity);
        var converter3 = new TestConverter<string, int>(ThirdAffinity);

        // Act
        registry.Register(converter1);
        registry.Register(converter2);
        registry.Register(converter3);

        var allConverters = registry.GetAllConverters().ToList();

        // Assert
        await Assert.That(allConverters.Count).IsEqualTo(ExpectedConverterCount);
        await Assert.That(allConverters).Contains(converter1);
        await Assert.That(allConverters).Contains(converter2);
        await Assert.That(allConverters).Contains(converter3);
    }

    /// <summary>
    ///     Verifies that when multiple converters are registered for the same type pair,
    ///     the one with the highest affinity is selected.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task MultipleConverters_ShouldSelectHighestAffinity()
    {
        // Arrange
        const int LowAffinity = 2;
        const int MediumAffinity = 5;
        const int HighAffinity = 10;
        var registry = new BindingTypeConverterRegistry();
        var lowAffinity = new TestConverter<int, string>(LowAffinity);
        var mediumAffinity = new TestConverter<int, string>(MediumAffinity);
        var highAffinity = new TestConverter<int, string>(HighAffinity);

        // Act - register in random order
        registry.Register(mediumAffinity);
        registry.Register(lowAffinity);
        registry.Register(highAffinity);

        var selected = registry.TryGetConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(selected).IsEqualTo(highAffinity);
    }

    /// <summary>Verifies that requesting a non-existent type pair returns null.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task NonExistentTypePair_ShouldReturnNull()
    {
        // Arrange
        const int ConverterAffinity = 5;
        var registry = new BindingTypeConverterRegistry();
        var converter = new TestConverter<int, string>(ConverterAffinity);
        registry.Register(converter);

        // Act
        var result = registry.TryGetConverter(typeof(double), typeof(bool));

        // Assert
        await Assert.That(result).IsNull();
    }

    /// <summary>Verifies that a registered converter can be retrieved.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task Register_AndRetrieve_ShouldReturnConverter()
    {
        // Arrange
        const int ConverterAffinity = 5;
        var registry = new BindingTypeConverterRegistry();
        var converter = new TestConverter<int, string>(ConverterAffinity);

        // Act
        registry.Register(converter);
        var retrieved = registry.TryGetConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved).IsEqualTo(converter);
    }

    /// <summary>Verifies that set-method converter registry works correctly.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task SetMethodRegistry_ShouldSelectHighestAffinity()
    {
        // Arrange
        const int LowAffinity = 2;
        const int HighAffinity = 8;
        var registry = new SetMethodBindingConverterRegistry();
        var lowAffinity = new TestSetMethodConverter(LowAffinity);
        var highAffinity = new TestSetMethodConverter(HighAffinity);

        // Act
        registry.Register(lowAffinity);
        registry.Register(highAffinity);

        var selected = registry.TryGetConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(selected).IsEqualTo(highAffinity);
    }

    /// <summary>Test typed converter that reports a configurable affinity.</summary>
    /// <typeparam name="TFrom">The source type.</typeparam>
    /// <typeparam name="TTo">The target type.</typeparam>
    private sealed class TestConverter<TFrom, TTo> : BindingTypeConverter<TFrom, TTo>
    {
        /// <summary>The affinity value to report.</summary>
        private readonly int _affinity;

        /// <summary>Initializes a new instance of the <see cref="TestConverter{TFrom, TTo}"/> class.</summary>
        /// <param name="affinity">The affinity value to report.</param>
        public TestConverter(int affinity) => _affinity = affinity;

        /// <inheritdoc/>
        public override int GetAffinityForObjects() => _affinity;

        /// <inheritdoc/>
        public override bool TryConvert(TFrom? from, object? conversionHint, [NotNullWhen(true)] out TTo? result)
        {
            result = default;
            return false;
        }
    }

    /// <summary>Test fallback converter that reports a configurable affinity.</summary>
    private sealed class TestFallbackConverter : IBindingFallbackConverter
    {
        /// <summary>The affinity value to report.</summary>
        private readonly int _baseAffinity;

        /// <summary>Initializes a new instance of the <see cref="TestFallbackConverter"/> class.</summary>
        /// <param name="baseAffinity">The affinity value to report.</param>
        public TestFallbackConverter(int baseAffinity) => _baseAffinity = baseAffinity;

        /// <inheritdoc/>
        public int GetAffinityForObjects(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
            Type fromType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
            Type toType) => _baseAffinity;

        /// <inheritdoc/>
        public bool TryConvert(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
            Type fromType,
            object from,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
            Type toType,
            object? conversionHint,
            [NotNullWhen(true)] out object? result)
        {
            result = null;
            return false;
        }
    }

    /// <summary>Test set-method converter that reports a configurable affinity.</summary>
    private sealed class TestSetMethodConverter : ISetMethodBindingConverter
    {
        /// <summary>The affinity value to report.</summary>
        private readonly int _baseAffinity;

        /// <summary>Initializes a new instance of the <see cref="TestSetMethodConverter"/> class.</summary>
        /// <param name="baseAffinity">The affinity value to report.</param>
        public TestSetMethodConverter(int baseAffinity) => _baseAffinity = baseAffinity;

        /// <inheritdoc/>
        public int GetAffinityForObjects(Type? fromType, Type? toType) => _baseAffinity;

        /// <inheritdoc/>
        public object? PerformSet(object? toTarget, object? newValue, object?[]? arguments) => newValue;
    }
}
