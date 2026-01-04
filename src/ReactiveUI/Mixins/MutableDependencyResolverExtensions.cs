// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI;

/// <summary>
/// Public AOT-friendly generic registration helpers for IMutableDependencyResolver.
/// These avoid reflection by relying on generic constraints and parameterless constructors.
/// </summary>
public static class MutableDependencyResolverExtensions
{
    /// <summary>
    /// Initializes static members of the <see cref="MutableDependencyResolverExtensions"/> class.
    /// </summary>
    static MutableDependencyResolverExtensions() => RxAppBuilder.EnsureInitialized();

    /// <summary>
    /// Registers a view for a view model via generics without reflection.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="resolver">Resolver to register into.</param>
    /// <param name="contract">Optional contract.</param>
    /// <returns>The resolver, for chaining.</returns>
    public static IMutableDependencyResolver RegisterViewForViewModel<TView, TViewModel>(this IMutableDependencyResolver resolver, string? contract = null)
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class
    {
        ArgumentExceptionHelper.ThrowIfNull(resolver);
        if (contract is null)
        {
            resolver.Register<IViewFor<TViewModel>>(static () => new TView());
        }
        else
        {
            resolver.Register<IViewFor<TViewModel>>(static () => new TView(), contract);
        }

        return resolver;
    }

    /// <summary>
    /// Registers a singleton view for a view model via generics without reflection.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="resolver">Resolver to register into.</param>
    /// <param name="contract">Optional contract.</param>
    /// <returns>The resolver, for chaining.</returns>
    public static IMutableDependencyResolver RegisterSingletonViewForViewModel<TView, TViewModel>(this IMutableDependencyResolver resolver, string? contract = null)
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class
    {
        ArgumentExceptionHelper.ThrowIfNull(resolver);
        if (contract is null)
        {
            resolver.RegisterLazySingleton<IViewFor<TViewModel>>(static () => new TView());
        }
        else
        {
            resolver.RegisterLazySingleton<IViewFor<TViewModel>>(static () => new TView(), contract);
        }

        return resolver;
    }
}
