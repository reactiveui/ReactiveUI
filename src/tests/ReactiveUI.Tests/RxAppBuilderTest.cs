// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI.Tests;

/// <summary>
///     Tests for <see cref="RxAppBuilder" />.
/// </summary>
public class RxAppBuilderTest
{
    /// <summary>
    ///     Tests that CreateReactiveUIBuilder throws for null resolver.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateReactiveUIBuilder_NullResolver_Throws()
    {
        const IMutableDependencyResolver resolver = null!;

        await Assert.That(() => resolver.CreateReactiveUIBuilder())
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    ///     Tests that CreateReactiveUIBuilder returns a builder.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateReactiveUIBuilder_ReturnsBuilder()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        await Assert.That(builder).IsNotNull();
        await Assert.That(builder).IsTypeOf<ReactiveUIBuilder>();
    }

    /// <summary>
    ///     Tests that CreateReactiveUIBuilder with resolver returns a builder.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateReactiveUIBuilder_WithResolver_ReturnsBuilder()
    {
        var resolver = new TestResolver();

        var builder = resolver.CreateReactiveUIBuilder();

        await Assert.That(builder).IsNotNull();
        await Assert.That(builder).IsTypeOf<ReactiveUIBuilder>();
    }

    /// <summary>
    ///     Test resolver for testing.
    /// </summary>
    [SuppressMessage("Major Code Smell", "S4018:Generic methods should provide type parameters", Justification = "Type parameter cannot be inferred.")]
    private sealed class TestResolver : IMutableDependencyResolver, IReadonlyDependencyResolver
    {
        /// <inheritdoc/>
        public object? GetService(Type? serviceType) => null;

        /// <inheritdoc/>
        public object? GetService(Type? serviceType, string? contract) => null;

        /// <inheritdoc/>
        public T? GetService<T>() => default;

        /// <inheritdoc/>
        public T? GetService<T>(string? contract) => default;

        /// <inheritdoc/>
        public IEnumerable<object> GetServices(Type? serviceType) => [];

        /// <inheritdoc/>
        public IEnumerable<object> GetServices(Type? serviceType, string? contract) => [];

        /// <inheritdoc/>
        public IEnumerable<T> GetServices<T>() => [];

        /// <inheritdoc/>
        public IEnumerable<T> GetServices<T>(string? contract) => [];

        /// <inheritdoc/>
        public bool HasRegistration(Type? serviceType) => false;

        /// <inheritdoc/>
        public bool HasRegistration(Type? serviceType, string? contract) => false;

        /// <inheritdoc/>
        public bool HasRegistration<T>() => false;

        /// <inheritdoc/>
        public bool HasRegistration<T>(string? contract) => false;

        /// <inheritdoc/>
        public void Register(Func<object?> factory, Type? serviceType)
        {
            // No-op: this test resolver does not store registrations.
        }

        /// <inheritdoc/>
        public void Register(Func<object?> factory, Type? serviceType, string? contract)
        {
            // No-op: this test resolver does not store registrations.
        }

        /// <inheritdoc/>
        public void Register<T>(Func<T?> factory)
        {
            // No-op: this test resolver does not store registrations.
        }

        /// <inheritdoc/>
        public void Register<T>(Func<T?> factory, string? contract)
        {
            // No-op: this test resolver does not store registrations.
        }

        /// <inheritdoc/>
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService, new()
        {
            // No-op: this test resolver does not store registrations.
        }

        /// <inheritdoc/>
        public void Register<TService, TImplementation>(string? contract)
            where TService : class
            where TImplementation : class, TService, new()
        {
            // No-op: this test resolver does not store registrations.
        }

        /// <inheritdoc/>
        public void RegisterConstant<T>(T? value)
            where T : class
        {
            // No-op: this test resolver does not store registrations.
        }

        /// <inheritdoc/>
        public void RegisterConstant<T>(T? value, string? contract)
            where T : class
        {
            // No-op: this test resolver does not store registrations.
        }

        /// <summary>
        ///     Records a non-generic constant registration.
        /// </summary>
        /// <param name="value">The constant value to register.</param>
        /// <param name="serviceType">The service type to register against.</param>
        /// <param name="contract">The optional contract.</param>
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Interface implementation")]
        public void RegisterConstant(object? value, Type? serviceType, string? contract)
        {
            // No-op: this test resolver does not store registrations.
        }

        /// <inheritdoc/>
        public void RegisterLazySingleton<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            T>(Func<T?> factory)
            where T : class
        {
            // No-op: this test resolver does not store registrations.
        }

        /// <inheritdoc/>
        public void RegisterLazySingleton<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            T>(
            Func<T?> factory,
            string? contract)
            where T : class
        {
            // No-op: this test resolver does not store registrations.
        }

        /// <summary>
        ///     Records a non-generic lazy singleton registration.
        /// </summary>
        /// <param name="factory">The factory that produces the value.</param>
        /// <param name="serviceType">The service type to register against.</param>
        /// <param name="contract">The optional contract.</param>
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Interface implementation")]
        public void RegisterLazySingleton(Func<object?> factory, Type? serviceType, string? contract)
        {
            // No-op: this test resolver does not store registrations.
        }

        /// <inheritdoc/>
        public IDisposable ServiceRegistrationCallback(Type serviceType, Action<IDisposable> callback) =>
            Disposable.Empty;

        /// <inheritdoc/>
        public IDisposable ServiceRegistrationCallback(
            Type serviceType,
            string? contract,
            Action<IDisposable> callback) => Disposable.Empty;

        /// <inheritdoc/>
        public IDisposable ServiceRegistrationCallback<T>(Action<IDisposable> callback) => Disposable.Empty;

        /// <inheritdoc/>
        public IDisposable ServiceRegistrationCallback<T>(string? contract, Action<IDisposable> callback) =>
            Disposable.Empty;

        /// <inheritdoc/>
        public void UnregisterAll(Type? serviceType)
        {
            // No-op: this test resolver stores no registrations to unregister.
        }

        /// <inheritdoc/>
        public void UnregisterAll(Type? serviceType, string? contract)
        {
            // No-op: this test resolver stores no registrations to unregister.
        }

        /// <inheritdoc/>
        public void UnregisterAll<T>()
        {
            // No-op: this test resolver stores no registrations to unregister.
        }

        /// <inheritdoc/>
        public void UnregisterAll<T>(string? contract)
        {
            // No-op: this test resolver stores no registrations to unregister.
        }

        /// <inheritdoc/>
        public void UnregisterCurrent(Type? serviceType)
        {
            // No-op: this test resolver stores no registrations to unregister.
        }

        /// <inheritdoc/>
        public void UnregisterCurrent(Type? serviceType, string? contract)
        {
            // No-op: this test resolver stores no registrations to unregister.
        }

        /// <inheritdoc/>
        public void UnregisterCurrent<T>()
        {
            // No-op: this test resolver stores no registrations to unregister.
        }

        /// <inheritdoc/>
        public void UnregisterCurrent<T>(string? contract)
        {
            // No-op: this test resolver stores no registrations to unregister.
        }
    }
}
