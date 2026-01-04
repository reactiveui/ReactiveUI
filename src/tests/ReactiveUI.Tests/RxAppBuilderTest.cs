// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for <see cref="RxAppBuilder"/>.
/// </summary>
public class RxAppBuilderTest
{
    /// <summary>
    /// Tests that CreateReactiveUIBuilder returns a builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateReactiveUIBuilder_ReturnsBuilder()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        await Assert.That(builder).IsNotNull();
        await Assert.That(builder).IsTypeOf<ReactiveUIBuilder>();
    }

    /// <summary>
    /// Tests that CreateReactiveUIBuilder with resolver returns a builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateReactiveUIBuilder_WithResolver_ReturnsBuilder()
    {
        var resolver = new TestResolver();

        var builder = resolver.CreateReactiveUIBuilder();

        await Assert.That(builder).IsNotNull();
        await Assert.That(builder).IsTypeOf<ReactiveUIBuilder>();
    }

    /// <summary>
    /// Tests that CreateReactiveUIBuilder throws for null resolver.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateReactiveUIBuilder_NullResolver_Throws()
    {
        IMutableDependencyResolver resolver = null!;

        await Assert.That(() => resolver.CreateReactiveUIBuilder())
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Test resolver for testing.
    /// </summary>
    private class TestResolver : IMutableDependencyResolver, IReadonlyDependencyResolver
    {
        public object? GetService(Type? serviceType, string? contract = null) => null;

        public object? GetService(Type? serviceType) => null;

        public IEnumerable<object> GetServices(Type? serviceType, string? contract = null) => [];

        public IEnumerable<object> GetServices(Type? serviceType) => [];

        public T GetService<T>(string? contract = null) => default!;

        public T GetService<T>() => default!;

        public IEnumerable<T> GetServices<T>(string? contract = null) => [];

        public IEnumerable<T> GetServices<T>() => [];

        public bool HasRegistration(Type? serviceType, string? contract = null) => false;

        public bool HasRegistration(Type? serviceType) => false;

        public bool HasRegistration<T>(string? contract = null) => false;

        public bool HasRegistration<T>() => false;

        public void Register(Func<object?> factory, Type? serviceType, string? contract = null)
        {
        }

        public void Register(Func<object?> factory, Type? serviceType)
        {
        }

        public void Register<T>(Func<T?> factory, string? contract)
        {
        }

        public void Register<T>(Func<T?> factory)
        {
        }

        public void UnregisterCurrent(Type? serviceType, string? contract = null)
        {
        }

        public void UnregisterCurrent(Type? serviceType)
        {
        }

        public void UnregisterCurrent<T>(string? contract)
        {
        }

        public void UnregisterCurrent<T>()
        {
        }

        public void UnregisterAll(Type? serviceType, string? contract = null)
        {
        }

        public void UnregisterAll(Type? serviceType)
        {
        }

        public void UnregisterAll<T>(string? contract)
        {
        }

        public void UnregisterAll<T>()
        {
        }

        public IDisposable ServiceRegistrationCallback(Type serviceType, string? contract, Action<IDisposable> callback) => Disposable.Empty;

        public IDisposable ServiceRegistrationCallback(Type serviceType, Action<IDisposable> callback) => Disposable.Empty;

        public IDisposable ServiceRegistrationCallback<T>(string? contract, Action<IDisposable> callback) => Disposable.Empty;

        public IDisposable ServiceRegistrationCallback<T>(Action<IDisposable> callback) => Disposable.Empty;

        public void Dispose() => GC.SuppressFinalize(this);

        void IMutableDependencyResolver.Register<TService, TImplementation>()
        {
        }

        void IMutableDependencyResolver.Register<TService, TImplementation>(string? contract)
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

        public void RegisterLazySingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>(Func<T?> valueFactory)
            where T : class
        {
        }

        public void RegisterLazySingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>(Func<T?> valueFactory, string? contract)
            where T : class
        {
        }
    }
#nullable restore
}
