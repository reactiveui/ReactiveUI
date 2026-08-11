// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ReactiveUI.Tests.WinUI.Mocks;

/// <summary>A registrar that materializes and records everything registered through it.</summary>
/// <remarks>
/// Factories are invoked immediately so a test can assert on the concrete service instances. A platform module
/// declares what it contributes, not how long each contribution lives, so every lifetime is recorded alike.
/// </remarks>
public sealed class RecordingRegistrar : IRegistrar
{
    /// <summary>Gets the services registered so far, keyed by the service type they were registered as.</summary>
    public Dictionary<Type, List<(object Service, string? Contract)>> Registrations { get; } = [];

    /// <summary>Gets the services registered for a service type.</summary>
    /// <param name="serviceType">The service type to look up.</param>
    /// <returns>The registered instances, or an empty list when the type was never registered.</returns>
    public IReadOnlyList<object> For(Type serviceType) =>
        Registrations.TryGetValue(serviceType, out var registered)
            ? registered.Select(static registration => registration.Service).ToList()
            : [];

    /// <inheritdoc/>
    public void RegisterConstant<TService>(Func<TService> factory)
        where TService : class => Record(factory, null);

    /// <inheritdoc/>
    public void RegisterConstant<TService>(Func<TService> factory, string? contract)
        where TService : class => Record(factory, contract);

    /// <inheritdoc/>
    public void RegisterLazySingleton<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TService>(
        Func<TService> factory)
        where TService : class => RegisterConstant(factory);

    /// <inheritdoc/>
    public void RegisterLazySingleton<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TService>(
        Func<TService> factory,
        string? contract)
        where TService : class => RegisterConstant(factory, contract);

    /// <inheritdoc/>
    public void Register<TService>(Func<TService> factory)
        where TService : class => RegisterLazySingleton(factory);

    /// <inheritdoc/>
    public void Register<TService>(Func<TService> factory, string? contract)
        where TService : class => RegisterLazySingleton(factory, contract);

    /// <summary>Materializes and records a registration.</summary>
    /// <typeparam name="TService">The service type being registered.</typeparam>
    /// <param name="factory">The factory supplying the service.</param>
    /// <param name="contract">The contract the service was registered under, if any.</param>
    private void Record<TService>(Func<TService> factory, string? contract)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(factory);

        ref var registered = ref CollectionsMarshal.GetValueRefOrAddDefault(Registrations, typeof(TService), out _);
        registered ??= [];
        registered.Add((factory(), contract));
    }
}
