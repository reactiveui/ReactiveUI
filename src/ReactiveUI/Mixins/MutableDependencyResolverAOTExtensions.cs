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
