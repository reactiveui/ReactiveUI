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
    /// Registers a view type for a specified view model type with the dependency resolver, optionally using a contract.
    /// </summary>
    /// <remarks>This method enables the dependency resolver to resolve the specified view type when an
    /// IViewFor{TViewModel} is requested. Use the contract parameter to distinguish between multiple registrations for
    /// the same view model type.</remarks>
    /// <typeparam name="TView">The view type to register. Must implement IViewFor{TViewModel} and have a parameterless constructor.</typeparam>
    /// <typeparam name="TViewModel">The view model type for which the view is registered.</typeparam>
    /// <param name="resolver">The dependency resolver to which the view registration is added. Cannot be null.</param>
    /// <param name="contract">An optional contract to associate with the registration. If null, the registration is made without a contract.</param>
    /// <returns>The dependency resolver instance, enabling method chaining.</returns>
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
    /// Registers a singleton view implementation for the specified view model type in the dependency resolver.
    /// </summary>
    /// <remarks>This method registers a singleton instance of the specified view type for the given view
    /// model type. The view will be created lazily upon first resolution. Use the contract parameter to distinguish
    /// between multiple registrations of the same view model type, if needed.</remarks>
    /// <typeparam name="TView">The type of the view to register. Must implement IViewFor{TViewModel} and have a parameterless constructor.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model associated with the view.</typeparam>
    /// <param name="resolver">The dependency resolver in which to register the singleton view.</param>
    /// <param name="contract">An optional contract string to associate with the registration. If null, the registration is made without a
    /// contract.</param>
    /// <returns>The dependency resolver instance, enabling method chaining.</returns>
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
