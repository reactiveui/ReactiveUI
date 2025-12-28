// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Public AOT-friendly generic registration helpers for IMutableDependencyResolver.
/// These avoid reflection by relying on generic constraints and parameterless constructors.
/// </summary>
public static class MutableDependencyResolverExtensions
{
    /// <summary>
    /// Registers a view for a view model via generics without reflection.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="resolver">Resolver to register into.</param>
    /// <param name="contract">Optional contract.</param>
    /// <returns>The resolver, for chaining.</returns>
#if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Generic registration does not use reflection")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Generic registration does not use dynamic code")]
#endif
    public static IMutableDependencyResolver RegisterViewForViewModel<TView, TViewModel>(this IMutableDependencyResolver resolver, string? contract = null)
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class
    {
        ArgumentExceptionHelper.ThrowIfNull(resolver);
        resolver.Register(static () => new TView(), typeof(IViewFor<TViewModel>), contract ?? string.Empty);
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
#if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Generic registration does not use reflection")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Generic registration does not use dynamic code")]
#endif
    public static IMutableDependencyResolver RegisterSingletonViewForViewModel<TView, TViewModel>(this IMutableDependencyResolver resolver, string? contract = null)
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class
    {
        ArgumentExceptionHelper.ThrowIfNull(resolver);
        resolver.RegisterLazySingleton(static () => new TView(), typeof(IViewFor<TViewModel>), contract ?? string.Empty);
        return resolver;
    }
}
