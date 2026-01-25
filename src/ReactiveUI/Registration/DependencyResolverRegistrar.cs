// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides an implementation of the IRegistrar interface that registers services with an underlying
/// IMutableDependencyResolver.
/// </summary>
/// <remarks>This class acts as an adapter, allowing services to be registered with the specified dependency
/// resolver using various registration methods. All registrations are forwarded to the underlying
/// IMutableDependencyResolver instance.</remarks>
/// <param name="resolver">The dependency resolver used to register service instances and factories. Cannot be null.</param>
internal sealed class DependencyResolverRegistrar(IMutableDependencyResolver resolver) : IRegistrar
{
    private readonly IMutableDependencyResolver _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));

    /// <inheritdoc/>
    public void RegisterConstant<TService>(Func<TService> factory, string? contract = null)
        where TService : class
    {
        ArgumentExceptionHelper.ThrowIfNull(factory);
        if (contract is null)
        {
            _resolver.RegisterConstant(factory());
        }
        else
        {
            _resolver.RegisterConstant(factory(), contract);
        }
    }

    /// <inheritdoc/>
    public void RegisterLazySingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TService>(Func<TService> factory, string? contract = null)
        where TService : class
    {
        ArgumentExceptionHelper.ThrowIfNull(factory);
        if (contract is null)
        {
            _resolver.RegisterLazySingleton(factory);
        }
        else
        {
            _resolver.RegisterLazySingleton(factory, contract);
        }
    }

    /// <inheritdoc/>
    public void Register<TService>(Func<TService> factory, string? contract = null)
        where TService : class
    {
        ArgumentExceptionHelper.ThrowIfNull(factory);
        if (contract is null)
        {
            _resolver.Register(factory);
        }
        else
        {
            _resolver.Register(factory, contract);
        }
    }
}
