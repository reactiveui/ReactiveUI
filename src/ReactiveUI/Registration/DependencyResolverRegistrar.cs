// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// An AOT-friendly implementation of <see cref="IRegistrar"/> that wraps
/// an <see cref="IMutableDependencyResolver"/> from Splat.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DependencyResolverRegistrar"/> class.
/// </remarks>
/// <param name="resolver">The dependency resolver to wrap.</param>
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
