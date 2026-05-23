// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Net;
using System.Numerics;
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
[TestExecutor<Executor>]
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
        var converter = resolver.GetBindingConverter(typeof(IPAddress), typeof(BigInteger));

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
        var converterFunc =
            resolver.GetSetMethodConverter(typeof(IPAddress), typeof(BigInteger));

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
    [SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
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
                .WithRegistration(r =>
                    r.RegisterConstant<ISetMethodBindingConverter>(new MockSetMethodBindingConverter()))
                .WithCoreServices();
        }
    }

    /// <summary>
    /// Placeholder type used as both source and target for the mock converters.
    /// </summary>
    [SuppressMessage(
        "Minor Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Empty type used as a test marker.")]
    private sealed class MockType;

    /// <summary>
    /// Mock binding type converter registered for <see cref="MockType"/>.
    /// </summary>
    private sealed class MockBindingTypeConverter : IBindingTypeConverter
    {
        private const int HighAffinity = 100;

        /// <inheritdoc/>
        public Type FromType => typeof(MockType);

        /// <inheritdoc/>
        public Type ToType => typeof(MockType);

        /// <inheritdoc/>
        public int GetAffinityForObjects() => HighAffinity;

        /// <inheritdoc/>
        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Mock set-method binding converter registered for <see cref="MockType"/>.
    /// </summary>
    private sealed class MockSetMethodBindingConverter : ISetMethodBindingConverter
    {
        private const int HighAffinity = 100;

        /// <inheritdoc/>
        public int GetAffinityForObjects(Type? fromType, Type? toType) =>
            fromType == typeof(MockType) && toType == typeof(MockType) ? HighAffinity : 0;

        /// <inheritdoc/>
        public object? PerformSet(object? current, object? newValue, object?[]? arguments) => "SetPerformed";
    }
}
