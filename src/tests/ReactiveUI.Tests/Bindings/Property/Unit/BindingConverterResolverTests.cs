// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Unit tests for <see cref="BindingConverterResolver"/>.
/// </summary>
/// <remarks>
/// These tests verify converter resolution logic without manipulating global Splat state.
/// Tests use the real implementation and rely on converters registered in the DI container.
/// </remarks>
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

        // Act - IntegerToStringTypeConverter is registered by default in RxConverters
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
        var converter1 = resolver.GetSetMethodConverter(typeof(int), typeof(string));
        var converter2 = resolver.GetSetMethodConverter(typeof(int), typeof(string));

        // Assert - Should return same cached instance (or both null)
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
}
