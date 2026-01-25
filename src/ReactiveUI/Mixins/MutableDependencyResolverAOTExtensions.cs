// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI;

/// <summary>
/// AOT-friendly generic registration helpers for IMutableDependencyResolver.
/// These avoid reflection by relying on generic constraints and parameterless constructors.
/// </summary>
internal static class MutableDependencyResolverAOTExtensions
{
    /// <summary>
    /// Initializes static members of the <see cref="MutableDependencyResolverAOTExtensions"/> class.
    /// </summary>
    static MutableDependencyResolverAOTExtensions() => RxAppBuilder.EnsureInitialized();

    /// <summary>
    /// Registers a view type for a specified view model type with the dependency resolver, enabling ahead-of-time (AOT)
    /// instantiation support.
    /// </summary>
    /// <remarks>This method is intended for use in environments where ahead-of-time (AOT) compilation is
    /// required and dynamic type registration is not available. It registers the view so that it can be resolved for
    /// the specified view model type at runtime.</remarks>
    /// <typeparam name="TView">The view type to register. Must implement IViewFor{TViewModel} and have a parameterless constructor.</typeparam>
    /// <typeparam name="TViewModel">The view model type for which the view is registered.</typeparam>
    /// <param name="resolver">The dependency resolver to which the view registration is added. Cannot be null.</param>
    /// <param name="contract">An optional contract string to distinguish this registration from others. If null, the registration is made
    /// without a contract.</param>
    /// <returns>The dependency resolver instance, enabling method chaining.</returns>
    internal static IMutableDependencyResolver RegisterViewForViewModelAOT<TView, TViewModel>(this IMutableDependencyResolver resolver, string? contract = null)
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
    /// Registers a singleton view implementation for the specified view model type in the dependency resolver, using
    /// ahead-of-time (AOT) instantiation.
    /// </summary>
    /// <remarks>This method registers the view as a lazy singleton, ensuring that only one instance of the
    /// view is created and reused for the specified view model type. Use this method when ahead-of-time registration is
    /// required, such as in environments where runtime code generation is not available.</remarks>
    /// <typeparam name="TView">The concrete view type to register. Must implement IViewFor{TViewModel} and have a parameterless constructor.</typeparam>
    /// <typeparam name="TViewModel">The view model type for which the view is registered.</typeparam>
    /// <param name="resolver">The dependency resolver in which to register the singleton view. Cannot be null.</param>
    /// <param name="contract">An optional contract string to associate with the registration. If null, the registration is made without a
    /// contract.</param>
    /// <returns>The dependency resolver instance, enabling method chaining.</returns>
    internal static IMutableDependencyResolver RegisterSingletonViewForViewModelAOT<TView, TViewModel>(this IMutableDependencyResolver resolver, string? contract = null)
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
