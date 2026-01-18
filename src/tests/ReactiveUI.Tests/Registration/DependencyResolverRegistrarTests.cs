// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Registration;

/// <summary>
///     Comprehensive test suite for <see cref="DependencyResolverRegistrar" />.
///     Tests cover constructor validation, registration methods with and without contracts,
///     and proper delegation to the underlying resolver.
/// </summary>
public class DependencyResolverRegistrarTests
{
    /// <summary>
    ///     Verifies that the constructor throws <see cref="ArgumentNullException" />
    ///     when the resolver parameter is null.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Constructor_NullResolver_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            var registrar = new DependencyResolverRegistrar(null!);
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Verifies that the constructor succeeds when passed a valid resolver.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Constructor_ValidResolver_Succeeds()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);

        await Assert.That(registrar).IsNotNull();
    }

    /// <summary>
    ///     Verifies that <see cref="DependencyResolverRegistrar.RegisterConstant{TService}" />
    ///     calls the underlying resolver's RegisterConstant method without a contract.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterConstant_WithoutContract_CallsResolverRegisterConstant()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);
        var service = new TestService();

        registrar.RegisterConstant(() => service);

        await Assert.That(resolver.RegisterConstantCalls.Count).IsEqualTo(1);
        await Assert.That(resolver.RegisterConstantCalls[0].Service).IsEqualTo(service);
        await Assert.That(resolver.RegisterConstantCalls[0].Contract).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="DependencyResolverRegistrar.RegisterConstant{TService}" />
    ///     calls the underlying resolver's RegisterConstant method with a contract.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterConstant_WithContract_CallsResolverRegisterConstantWithContract()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);
        var service = new TestService();
        var contract = "test-contract";

        registrar.RegisterConstant(() => service, contract);

        await Assert.That(resolver.RegisterConstantCalls.Count).IsEqualTo(1);
        await Assert.That(resolver.RegisterConstantCalls[0].Service).IsEqualTo(service);
        await Assert.That(resolver.RegisterConstantCalls[0].Contract).IsEqualTo(contract);
    }

    /// <summary>
    ///     Verifies that <see cref="DependencyResolverRegistrar.RegisterConstant{TService}" />
    ///     throws <see cref="ArgumentNullException" /> when the factory is null.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterConstant_NullFactory_ThrowsArgumentNullException()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            registrar.RegisterConstant<TestService>(null!);
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Verifies that <see cref="DependencyResolverRegistrar.RegisterLazySingleton{TService}" />
    ///     calls the underlying resolver's RegisterLazySingleton method without a contract.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterLazySingleton_WithoutContract_CallsResolverRegisterLazySingleton()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);
        Func<TestService> factory = () => new TestService();

        registrar.RegisterLazySingleton(factory);

        await Assert.That(resolver.RegisterLazySingletonCalls.Count).IsEqualTo(1);
        await Assert.That(resolver.RegisterLazySingletonCalls[0].Factory).IsEqualTo(factory);
        await Assert.That(resolver.RegisterLazySingletonCalls[0].Contract).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="DependencyResolverRegistrar.RegisterLazySingleton{TService}" />
    ///     calls the underlying resolver's RegisterLazySingleton method with a contract.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterLazySingleton_WithContract_CallsResolverRegisterLazySingletonWithContract()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);
        Func<TestService> factory = () => new TestService();
        var contract = "test-contract";

        registrar.RegisterLazySingleton(factory, contract);

        await Assert.That(resolver.RegisterLazySingletonCalls.Count).IsEqualTo(1);
        await Assert.That(resolver.RegisterLazySingletonCalls[0].Factory).IsEqualTo(factory);
        await Assert.That(resolver.RegisterLazySingletonCalls[0].Contract).IsEqualTo(contract);
    }

    /// <summary>
    ///     Verifies that <see cref="DependencyResolverRegistrar.RegisterLazySingleton{TService}" />
    ///     throws <see cref="ArgumentNullException" /> when the factory is null.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterLazySingleton_NullFactory_ThrowsArgumentNullException()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            registrar.RegisterLazySingleton<TestService>(null!);
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Verifies that <see cref="DependencyResolverRegistrar.Register{TService}" />
    ///     calls the underlying resolver's Register method without a contract.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Register_WithoutContract_CallsResolverRegister()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);
        Func<TestService> factory = () => new TestService();

        registrar.Register(factory);

        await Assert.That(resolver.RegisterCalls.Count).IsEqualTo(1);
        await Assert.That(resolver.RegisterCalls[0].Factory).IsEqualTo(factory);
        await Assert.That(resolver.RegisterCalls[0].Contract).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="DependencyResolverRegistrar.Register{TService}" />
    ///     calls the underlying resolver's Register method with a contract.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Register_WithContract_CallsResolverRegisterWithContract()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);
        Func<TestService> factory = () => new TestService();
        var contract = "test-contract";

        registrar.Register(factory, contract);

        await Assert.That(resolver.RegisterCalls.Count).IsEqualTo(1);
        await Assert.That(resolver.RegisterCalls[0].Factory).IsEqualTo(factory);
        await Assert.That(resolver.RegisterCalls[0].Contract).IsEqualTo(contract);
    }

    /// <summary>
    ///     Verifies that <see cref="DependencyResolverRegistrar.Register{TService}" />
    ///     throws <see cref="ArgumentNullException" /> when the factory is null.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Register_NullFactory_ThrowsArgumentNullException()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            registrar.Register<TestService>(null!);
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Test service class used for testing registration.
    /// </summary>
    private sealed class TestService
    {
    }

    /// <summary>
    ///     Mock implementation of <see cref="IMutableDependencyResolver"/> for testing.
    /// </summary>
    private sealed class MockDependencyResolver : IMutableDependencyResolver
    {
        public List<(object Service, string? Contract)> RegisterConstantCalls { get; } = [];

        public List<(object Factory, string? Contract)> RegisterLazySingletonCalls { get; } = [];

        public List<(object Factory, string? Contract)> RegisterCalls { get; } = [];

        public void Register(Func<object?> factory, Type? serviceType = null, string? contract = null)
        {
            RegisterCalls.Add((factory, contract));
        }

        public void Register(Func<object?> factory, Type? serviceType)
        {
            RegisterCalls.Add((factory, null));
        }

        public void Register<T>(Func<T?> factory)
        {
            RegisterCalls.Add((factory, null));
        }

        public void Register<T>(Func<T?> factory, string? contract = null)
        {
            RegisterCalls.Add((factory, contract));
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

        public void RegisterConstant(object? value, Type? serviceType, string? contract)
        {
            if (value != null)
            {
                RegisterConstantCalls.Add((value, contract));
            }
        }

        public void RegisterConstant<T>(T? value)
            where T : class
        {
            if (value != null)
            {
                RegisterConstantCalls.Add((value, null));
            }
        }

        public void RegisterConstant<T>(T? value, string? contract)
            where T : class
        {
            if (value != null)
            {
                RegisterConstantCalls.Add((value, contract));
            }
        }

        public void RegisterLazySingleton(Func<object?> factory, Type? serviceType, string? contract)
        {
            RegisterLazySingletonCalls.Add((factory, contract));
        }

        public void RegisterLazySingleton<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            T>(Func<T?> factory)
            where T : class
        {
            RegisterLazySingletonCalls.Add((factory, null));
        }

        public void RegisterLazySingleton<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            T>(Func<T?> factory, string? contract)
            where T : class
        {
            RegisterLazySingletonCalls.Add((factory, contract));
        }

        public bool HasRegistration(Type? serviceType, string? contract = null)
        {
            return false;
        }

        public bool HasRegistration(Type? serviceType)
        {
            return false;
        }

        public bool HasRegistration<T>()
        {
            return false;
        }

        public bool HasRegistration<T>(string? contract)
        {
            return false;
        }

        public void UnregisterCurrent(Type? serviceType, string? contract = null)
        {
        }

        public void UnregisterCurrent(Type? serviceType)
        {
        }

        public void UnregisterCurrent<T>()
        {
        }

        public void UnregisterCurrent<T>(string? contract)
        {
        }

        public void UnregisterAll(Type? serviceType, string? contract = null)
        {
        }

        public void UnregisterAll(Type? serviceType)
        {
        }

        public void UnregisterAll<T>()
        {
        }

        public void UnregisterAll<T>(string? contract)
        {
        }

        public IDisposable ServiceRegistrationCallback(Type serviceType, string? contract, Action<IDisposable> callback)
        {
            return Disposable.Empty;
        }

        public IDisposable ServiceRegistrationCallback(Type serviceType, Action<IDisposable> callback)
        {
            return Disposable.Empty;
        }

        public IDisposable ServiceRegistrationCallback<T>(Action<IDisposable> callback)
        {
            return Disposable.Empty;
        }

        public IDisposable ServiceRegistrationCallback<T>(string? contract, Action<IDisposable> callback)
        {
            return Disposable.Empty;
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Interface implementation")]
        public object? GetService(Type? serviceType, string? contract = null)
        {
            return null;
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Interface implementation")]
        public IEnumerable<object> GetServices(Type? serviceType, string? contract = null)
        {
            return [];
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Interface implementation")]
        public void Dispose()
        {
        }
    }
}
