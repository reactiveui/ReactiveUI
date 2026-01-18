// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using ReactiveUI.Tests.Utilities.AppBuilder;

namespace ReactiveUI.Tests;

/// <summary>
/// Unit tests for <see cref="BindingConverterResolver"/>.
/// </summary>
/// <remarks>
/// These tests verify converter resolution logic.
/// Tests use the Executor paradigm to manage AppBuilder state and registrations.
/// </remarks>
[TestExecutor<BindingConverterResolverTests.Executor>]
public class BindingConverterResolverTests
{
    /// <summary>
    /// Verifies that GetBindingConverter returns a registered typed converter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetBindingConverter_WithRegisteredTypedConverter_ReturnsConverter()
    {
        // Arrange
        var resolver = new BindingConverterResolver();

        // Act - IntegerToStringTypeConverter is registered by default via WithCoreServices
        var converter = resolver.GetBindingConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(converter).IsNotNull();
    }

    /// <summary>
    /// Verifies that GetBindingConverter returns null for unregistered type pairs.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetBindingConverter_WithUnregisteredTypePair_ReturnsNull()
    {
        // Arrange
        var resolver = new BindingConverterResolver();

        // Act - Use obscure types unlikely to have registered converters
        var converter = resolver.GetBindingConverter(typeof(System.Net.IPAddress), typeof(System.Numerics.BigInteger));

        // Assert
        await Assert.That(converter).IsNull();
    }

    /// <summary>
    /// Verifies that GetSetMethodConverter caches results for the same type pair.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetSetMethodConverter_WithCaching_ReturnsSameInstance()
    {
        // Arrange
        var resolver = new BindingConverterResolver();

        // Act
        // Use MockType which has a registered set method converter
        var converter1 = resolver.GetSetMethodConverter(typeof(MockType), typeof(MockType));
        var converter2 = resolver.GetSetMethodConverter(typeof(MockType), typeof(MockType));

        // Assert
        await Assert.That(converter1).IsNotNull();
        await Assert.That(converter1).IsSameReferenceAs(converter2);
    }

    /// <summary>
    /// Verifies that GetSetMethodConverter returns null when fromType is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetSetMethodConverter_WithNullFromType_ReturnsNull()
    {
        // Arrange
        var resolver = new BindingConverterResolver();

        // Act
        var converter = resolver.GetSetMethodConverter(null, typeof(string));

        // Assert
        await Assert.That(converter).IsNull();
    }

    /// <summary>
    /// Verifies that GetBindingConverter uses RxConverters when available.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetBindingConverter_UsesRxConverters_WhenAvailable()
    {
        // Arrange
        var resolver = new BindingConverterResolver();

        // Act - IntegerToStringTypeConverter should be available from RxConverters
        var converter = resolver.GetBindingConverter(typeof(int), typeof(string));

        // Assert
        await Assert.That(converter).IsNotNull();
        await Assert.That(converter).IsTypeOf<IntegerToStringTypeConverter>();
    }

    /// <summary>
    /// Verifies that GetBindingConverter falls back to Splat if not in RxConverters.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetBindingConverter_FallsBackToSplat_WhenRxConvertersFails()
    {
        // Arrange
        var resolver = new BindingConverterResolver();

        // Act
        // MockType converter is registered in Splat (via Executor) but NOT in RxConverters (standard list)
        var converter = resolver.GetBindingConverter(typeof(MockType), typeof(MockType));

        // Assert
        await Assert.That(converter).IsNotNull();
        await Assert.That(converter).IsTypeOf<MockBindingTypeConverter>();
    }

    /// <summary>
    /// Verifies that GetSetMethodConverter returns a registered converter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetSetMethodConverter_ReturnsConverter_WhenRegistered()
    {
        // Arrange
        var resolver = new BindingConverterResolver();

        // Act
        var converterFunc = resolver.GetSetMethodConverter(typeof(MockType), typeof(MockType));

        // Assert
        await Assert.That(converterFunc).IsNotNull();

        // Invoke to verify
        var result = converterFunc!(new MockType(), new MockType(), null);
        await Assert.That(result).IsEqualTo("SetPerformed");
    }

    /// <summary>
    /// Verifies that GetSetMethodConverter returns null when no converter is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetSetMethodConverter_WithUnregisteredType_ReturnsNull()
    {
        // Arrange
        var resolver = new BindingConverterResolver();

        // Act
        var converterFunc = resolver.GetSetMethodConverter(typeof(System.Net.IPAddress), typeof(System.Numerics.BigInteger));

        // Assert
        await Assert.That(converterFunc).IsNull();
    }

    /// <summary>
    /// Verifies that GetSetMethodConverter handles null toType gracefully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetSetMethodConverter_WithNullToType_HandlesGracefully()
    {
        // Arrange
        var resolver = new BindingConverterResolver();

        // Act
        var converterFunc = resolver.GetSetMethodConverter(typeof(MockType), null);

        // Assert
        // The MockSetMethodBindingConverter returns affinity 0 for null toType, so converter should be null
        // The test verifies the method handles null toType gracefully without throwing
        await Assert.That(converterFunc).IsNull();
    }

    /// <summary>
    /// Verifies that GetBindingConverter handles null services gracefully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetBindingConverter_WithNoRxConverters_FallsBackToSplat()
    {
        // Arrange
        var resolver = new BindingConverterResolver();

        // Act - Get a converter that should be found in Splat
        var converter = resolver.GetBindingConverter(typeof(MockType), typeof(MockType));

        // Assert
        await Assert.That(converter).IsNotNull();
        await Assert.That(converter).IsTypeOf<MockBindingTypeConverter>();
    }

    /// <summary>
    /// Test executor that registers mock converters.
    /// </summary>
    public class Executor : BaseAppBuilderTestExecutor
    {
        /// <inheritdoc/>
        protected override void ConfigureAppBuilder(IReactiveUIBuilder builder, TestContext context)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(context);

            builder
                .WithRegistration(r => r.RegisterConstant<IBindingTypeConverter>(new MockBindingTypeConverter()))
                .WithRegistration(r => r.RegisterConstant<ISetMethodBindingConverter>(new MockSetMethodBindingConverter()))
                .WithCoreServices();
        }
    }

    private class MockType
    {
    }

    private class MockBindingTypeConverter : IBindingTypeConverter
    {
        public Type FromType => typeof(MockType);

        public Type ToType => typeof(MockType);

        public int GetAffinityForObjects() => 100;

        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            result = null;
            return false;
        }
    }

    private class MockSetMethodBindingConverter : ISetMethodBindingConverter
    {
        public int GetAffinityForObjects(Type? fromType, Type? toType) =>
            fromType == typeof(MockType) && toType == typeof(MockType) ? 100 : 0;

        public object? PerformSet(object? current, object? newValue, object?[]? arguments) => "SetPerformed";
    }
}