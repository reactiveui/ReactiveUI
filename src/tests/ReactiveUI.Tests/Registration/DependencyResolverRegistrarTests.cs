// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
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
    public async Task Constructor_NullResolver_ThrowsArgumentNullException() =>
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            _ = new DependencyResolverRegistrar(null!);
            await Task.CompletedTask;
        });

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
    ///     Verifies that <c>RegisterConstant</c>
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
    ///     Verifies that <c>RegisterConstant</c>
    ///     calls the underlying resolver's RegisterConstant method with a contract.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterConstant_WithContract_CallsResolverRegisterConstantWithContract()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);
        var service = new TestService();
        const string Contract = "test-contract";

        registrar.RegisterConstant(() => service, Contract);

        await Assert.That(resolver.RegisterConstantCalls.Count).IsEqualTo(1);
        await Assert.That(resolver.RegisterConstantCalls[0].Service).IsEqualTo(service);
        await Assert.That(resolver.RegisterConstantCalls[0].Contract).IsEqualTo(Contract);
    }

    /// <summary>
    ///     Verifies that <c>RegisterConstant</c>
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
    ///     Verifies that <c>RegisterLazySingleton</c>
    ///     calls the underlying resolver's RegisterLazySingleton method without a contract.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterLazySingleton_WithoutContract_CallsResolverRegisterLazySingleton()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);
        var factory = () => new TestService();

        registrar.RegisterLazySingleton(factory);

        await Assert.That(resolver.RegisterLazySingletonCalls.Count).IsEqualTo(1);
        await Assert.That(resolver.RegisterLazySingletonCalls[0].Factory).IsEqualTo(factory);
        await Assert.That(resolver.RegisterLazySingletonCalls[0].Contract).IsNull();
    }

    /// <summary>
    ///     Verifies that <c>RegisterLazySingleton</c>
    ///     calls the underlying resolver's RegisterLazySingleton method with a contract.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterLazySingleton_WithContract_CallsResolverRegisterLazySingletonWithContract()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);
        var factory = () => new TestService();
        const string Contract = "test-contract";

        registrar.RegisterLazySingleton(factory, Contract);

        await Assert.That(resolver.RegisterLazySingletonCalls.Count).IsEqualTo(1);
        await Assert.That(resolver.RegisterLazySingletonCalls[0].Factory).IsEqualTo(factory);
        await Assert.That(resolver.RegisterLazySingletonCalls[0].Contract).IsEqualTo(Contract);
    }

    /// <summary>
    ///     Verifies that <c>RegisterLazySingleton</c>
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
    ///     Verifies that <c>Register</c>
    ///     calls the underlying resolver's Register method without a contract.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Register_WithoutContract_CallsResolverRegister()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);
        var factory = () => new TestService();

        registrar.Register(factory);

        await Assert.That(resolver.RegisterCalls.Count).IsEqualTo(1);
        await Assert.That(resolver.RegisterCalls[0].Factory).IsEqualTo(factory);
        await Assert.That(resolver.RegisterCalls[0].Contract).IsNull();
    }

    /// <summary>
    ///     Verifies that <c>Register</c>
    ///     calls the underlying resolver's Register method with a contract.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Register_WithContract_CallsResolverRegisterWithContract()
    {
        var resolver = new MockDependencyResolver();
        var registrar = new DependencyResolverRegistrar(resolver);
        var factory = () => new TestService();
        const string Contract = "test-contract";

        registrar.Register(factory, Contract);

        await Assert.That(resolver.RegisterCalls.Count).IsEqualTo(1);
        await Assert.That(resolver.RegisterCalls[0].Factory).IsEqualTo(factory);
        await Assert.That(resolver.RegisterCalls[0].Contract).IsEqualTo(Contract);
    }

    /// <summary>
    ///     Verifies that <c>Register</c>
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
    [SuppressMessage(
        "Minor Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Empty type used as a test marker.")]
    private sealed class TestService;

    /// <summary>
    ///     Mock implementation of <see cref="IMutableDependencyResolver"/> for testing.
    /// </summary>
    [SuppressMessage("Major Code Smell", "S4018:Generic methods should provide type parameters", Justification = "Type parameter cannot be inferred.")]
    private sealed class MockDependencyResolver : IMutableDependencyResolver, IDisposable
    {
        /// <summary>
        /// Gets the recorded calls to RegisterConstant.
        /// </summary>
        public List<(object Service, string? Contract)> RegisterConstantCalls { get; } = [];

        /// <summary>
        /// Gets the recorded calls to RegisterLazySingleton.
        /// </summary>
        public List<(object Factory, string? Contract)> RegisterLazySingletonCalls { get; } = [];

        /// <summary>
        /// Gets the recorded calls to Register.
        /// </summary>
        public List<(object Factory, string? Contract)> RegisterCalls { get; } = [];

        /// <inheritdoc />
        public void Register(Func<object?> factory, Type? serviceType, string? contract) =>
            RegisterCalls.Add((factory, contract));

        /// <inheritdoc />
        public void Register(Func<object?> factory, Type? serviceType) => RegisterCalls.Add((factory, null));

        /// <inheritdoc />
        public void Register<T>(Func<T?> factory) => RegisterCalls.Add((factory, null));

        /// <inheritdoc />
        public void Register<T>(Func<T?> factory, string? contract) => RegisterCalls.Add((factory, contract));

        /// <inheritdoc />
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService, new()
        {
            // No-op: this mock only records the factory-based Register overloads exercised by the tests.
        }

        /// <inheritdoc />
        public void Register<TService, TImplementation>(string? contract)
            where TService : class
            where TImplementation : class, TService, new()
        {
            // No-op: this mock only records the factory-based Register overloads exercised by the tests.
        }

        /// <summary>
        /// Records a non-generic constant registration when the value is not null.
        /// </summary>
        /// <param name="value">The constant value to register.</param>
        /// <param name="serviceType">The service type to register against.</param>
        /// <param name="contract">The optional contract.</param>
        public void RegisterConstant(object? value, Type? serviceType, string? contract)
        {
            _ = serviceType;
            if (value == null)
            {
                return;
            }

            RegisterConstantCalls.Add((value, contract));
        }

        /// <inheritdoc />
        public void RegisterConstant<T>(T? value)
            where T : class
        {
            if (value == null)
            {
                return;
            }

            RegisterConstantCalls.Add((value, null));
        }

        /// <inheritdoc />
        public void RegisterConstant<T>(T? value, string? contract)
            where T : class
        {
            if (value == null)
            {
                return;
            }

            RegisterConstantCalls.Add((value, contract));
        }

        /// <summary>
        /// Records a non-generic lazy singleton registration.
        /// </summary>
        /// <param name="factory">The factory that produces the value.</param>
        /// <param name="serviceType">The service type to register against.</param>
        /// <param name="contract">The optional contract.</param>
        public void RegisterLazySingleton(Func<object?> factory, Type? serviceType, string? contract)
        {
            _ = serviceType;
            RegisterLazySingletonCalls.Add((factory, contract));
        }

        /// <inheritdoc />
        public void RegisterLazySingleton<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            T>(Func<T?> factory)
            where T : class =>
            RegisterLazySingletonCalls.Add((factory, null));

        /// <inheritdoc />
        public void RegisterLazySingleton<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            T>(Func<T?> factory, string? contract)
            where T : class =>
            RegisterLazySingletonCalls.Add((factory, contract));

        /// <inheritdoc />
        public bool HasRegistration(Type? serviceType, string? contract) => false;

        /// <inheritdoc />
        public bool HasRegistration(Type? serviceType) => false;

        /// <inheritdoc />
        public bool HasRegistration<T>() => false;

        /// <inheritdoc />
        public bool HasRegistration<T>(string? contract) => false;

        /// <inheritdoc />
        public void UnregisterCurrent(Type? serviceType, string? contract)
        {
            // No-op: this mock does not track unregistration; the tests only assert on registration calls.
        }

        /// <inheritdoc />
        public void UnregisterCurrent(Type? serviceType)
        {
            // No-op: this mock does not track unregistration; the tests only assert on registration calls.
        }

        /// <inheritdoc />
        public void UnregisterCurrent<T>()
        {
            // No-op: this mock does not track unregistration; the tests only assert on registration calls.
        }

        /// <inheritdoc />
        public void UnregisterCurrent<T>(string? contract)
        {
            // No-op: this mock does not track unregistration; the tests only assert on registration calls.
        }

        /// <inheritdoc />
        public void UnregisterAll(Type? serviceType, string? contract)
        {
            // No-op: this mock does not track unregistration; the tests only assert on registration calls.
        }

        /// <inheritdoc />
        public void UnregisterAll(Type? serviceType)
        {
            // No-op: this mock does not track unregistration; the tests only assert on registration calls.
        }

        /// <inheritdoc />
        public void UnregisterAll<T>()
        {
            // No-op: this mock does not track unregistration; the tests only assert on registration calls.
        }

        /// <inheritdoc />
        public void UnregisterAll<T>(string? contract)
        {
            // No-op: this mock does not track unregistration; the tests only assert on registration calls.
        }

        /// <inheritdoc />
        public IDisposable
            ServiceRegistrationCallback(Type serviceType, string? contract, Action<IDisposable> callback) =>
            Disposable.Empty;

        /// <inheritdoc />
        public IDisposable ServiceRegistrationCallback(Type serviceType, Action<IDisposable> callback) =>
            Disposable.Empty;

        /// <inheritdoc />
        public IDisposable ServiceRegistrationCallback<T>(Action<IDisposable> callback) => Disposable.Empty;

        /// <inheritdoc />
        public IDisposable ServiceRegistrationCallback<T>(string? contract, Action<IDisposable> callback) =>
            Disposable.Empty;

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Interface implementation")]
        public object? GetService(Type? serviceType, string? contract) => null;

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Interface implementation")]
        public IEnumerable<object> GetServices(Type? serviceType, string? contract) => [];

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Interface implementation")]
        public void Dispose()
        {
            // No-op: this mock holds no disposable resources.
        }
    }
}
