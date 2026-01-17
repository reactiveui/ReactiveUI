// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Interface for registering services in a dependency injection container in an AOT-friendly manner.
/// This interface provides generic registration methods that preserve type information at compile time,
/// avoiding the need for runtime Type reflection and DynamicallyAccessedMembers attributes.
/// </summary>
public interface IRegistrar
{
    /// <summary>
    /// Registers a constant value for a service type. The factory function is called once
    /// and the result is registered as a singleton.
    /// </summary>
    /// <typeparam name="TService">The service type to register.</typeparam>
    /// <param name="factory">A factory function that creates the service instance.</param>
    /// <param name="contract">An optional contract name for multiple registrations of the same type.</param>
    void RegisterConstant<TService>(Func<TService> factory, string? contract = null)
        where TService : class;

    /// <summary>
    /// Registers a lazy singleton for a service type. The factory function is called
    /// the first time the service is resolved, and the same instance is returned for all subsequent resolutions.
    /// </summary>
    /// <typeparam name="TService">The service type to register.</typeparam>
    /// <param name="factory">A factory function that creates the service instance.</param>
    /// <param name="contract">An optional contract name for multiple registrations of the same type.</param>
    void RegisterLazySingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TService>(Func<TService> factory, string? contract = null)
        where TService : class;

    /// <summary>
    /// Registers a factory for a service type. The factory function is called each time
    /// the service is resolved, creating a new instance.
    /// </summary>
    /// <typeparam name="TService">The service type to register.</typeparam>
    /// <param name="factory">A factory function that creates the service instance.</param>
    /// <param name="contract">An optional contract name for multiple registrations of the same type.</param>
    void Register<TService>(Func<TService> factory, string? contract = null)
        where TService : class;
}
