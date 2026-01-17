// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
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
        IMutableDependencyResolver resolver = null!;

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
    private class TestResolver : IMutableDependencyResolver, IReadonlyDependencyResolver
    {
        public object? GetService(Type? serviceType) => null;

        public object? GetService(Type? serviceType, string? contract) => null;

        public T? GetService<T>() => default;

        public T? GetService<T>(string? contract) => default;

        public IEnumerable<object> GetServices(Type? serviceType) => [];

        public IEnumerable<object> GetServices(Type? serviceType, string? contract) => [];

        public IEnumerable<T> GetServices<T>() => [];

        public IEnumerable<T> GetServices<T>(string? contract) => [];

        public bool HasRegistration(Type? serviceType) => false;

        public bool HasRegistration(Type? serviceType, string? contract) => false;

        public bool HasRegistration<T>() => false;

        public bool HasRegistration<T>(string? contract) => false;

        public void Register(Func<object?> factory, Type? serviceType)
        {
        }

        public void Register(Func<object?> factory, Type? serviceType, string? contract)
        {
        }

        public void Register<T>(Func<T?> factory)
        {
        }

        public void Register<T>(Func<T?> factory, string? contract)
        {
        }

        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService, new()
        {
        }

        public void Register<TService, TImplementation>(string? contract)
            where TService : class
            where TImplementation : class, TService, new()
        {
        }

        public void RegisterConstant<T>(T? value)
            where T : class
        {
        }

        public void RegisterConstant<T>(T? value, string? contract)
            where T : class
        {
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Interface implementation")]
        public void RegisterConstant(object? value, Type? serviceType, string? contract)
        {
        }

        public void RegisterLazySingleton<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            T>(Func<T?> factory)
            where T : class
        {
        }

        public void RegisterLazySingleton<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            T>(
            Func<T?> factory,
            string? contract)
            where T : class
        {
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Interface implementation")]
        public void RegisterLazySingleton(Func<object?> factory, Type? serviceType, string? contract)
        {
        }

        public IDisposable ServiceRegistrationCallback(Type serviceType, Action<IDisposable> callback) =>
            Disposable.Empty;

        public IDisposable ServiceRegistrationCallback(
            Type serviceType,
            string? contract,
            Action<IDisposable> callback) => Disposable.Empty;

        public IDisposable ServiceRegistrationCallback<T>(Action<IDisposable> callback) => Disposable.Empty;

        public IDisposable ServiceRegistrationCallback<T>(string? contract, Action<IDisposable> callback) =>
            Disposable.Empty;

        public void UnregisterAll(Type? serviceType)
        {
        }

        public void UnregisterAll(Type? serviceType, string? contract)
        {
        }

        public void UnregisterAll<T>()
        {
        }

        public void UnregisterAll<T>(string? contract)
        {
        }

        public void UnregisterCurrent(Type? serviceType)
        {
        }

        public void UnregisterCurrent(Type? serviceType, string? contract)
        {
        }

        public void UnregisterCurrent<T>()
        {
        }

        public void UnregisterCurrent<T>(string? contract)
        {
        }
    }
}
