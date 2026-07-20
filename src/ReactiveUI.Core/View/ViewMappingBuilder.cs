// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Splat;

namespace ReactiveUI;

/// <summary>Fluent builder for registering AOT-compatible view-to-viewmodel mappings.</summary>
public sealed class ViewMappingBuilder
{
    /// <summary>The underlying view locator that receives the registered mappings.</summary>
    private readonly DefaultViewLocator _locator;

    /// <summary>Initializes a new instance of the <see cref="ViewMappingBuilder"/> class.</summary>
    /// <param name="locator">The view locator to register mappings with.</param>
    public ViewMappingBuilder(DefaultViewLocator locator)
    {
        ArgumentExceptionHelper.ThrowIfNull(locator);
        _locator = locator;
    }

    /// <summary>
    /// Maps a view model type to a view type with automatic instantiation using the default contract.
    /// The view must have a parameterless constructor.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TView">The view type implementing IViewFor&lt;TViewModel&gt;.</typeparam>
    /// <returns>The builder for chaining.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public ViewMappingBuilder Map<TViewModel, TView>()
        where TViewModel : class
        where TView : class, IViewFor<TViewModel>, new() =>
        Map<TViewModel, TView>((string?)null);

    /// <summary>Maps a view model type to a view type with automatic instantiation. The view must have a parameterless constructor.</summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TView">The view type implementing IViewFor&lt;TViewModel&gt;.</typeparam>
    /// <param name="contract">Optional contract to disambiguate multiple views for the same view model.</param>
    /// <returns>The builder for chaining.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public ViewMappingBuilder Map<TViewModel, TView>(string? contract)
        where TViewModel : class
        where TView : class, IViewFor<TViewModel>, new()
    {
        _ = _locator.Map<TViewModel, TView>(static () => new(), contract);
        return this;
    }

    /// <summary>Maps a view model type to a view type with a custom factory function using the default contract.</summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TView">The view type implementing IViewFor&lt;TViewModel&gt;.</typeparam>
    /// <param name="factory">Factory function that creates the view.</param>
    /// <returns>The builder for chaining.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public ViewMappingBuilder Map<TViewModel, TView>(Func<TView> factory)
        where TViewModel : class
        where TView : class, IViewFor<TViewModel> =>
        Map<TViewModel, TView>(factory, null);

    /// <summary>
    /// Maps a view model type to a view type with a custom factory function.
    /// Use this when the view requires constructor parameters or custom initialization.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TView">The view type implementing IViewFor&lt;TViewModel&gt;.</typeparam>
    /// <param name="factory">Factory function that creates the view.</param>
    /// <param name="contract">Optional contract to disambiguate multiple views for the same view model.</param>
    /// <returns>The builder for chaining.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public ViewMappingBuilder Map<TViewModel, TView>(Func<TView> factory, string? contract)
        where TViewModel : class
        where TView : class, IViewFor<TViewModel>
    {
        ArgumentExceptionHelper.ThrowIfNull(factory);
        _ = _locator.Map<TViewModel, TView>(factory, contract);
        return this;
    }

    /// <summary>Maps a view model type to a view resolved from the service locator using the default contract.</summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TView">The view type implementing IViewFor&lt;TViewModel&gt;.</typeparam>
    /// <returns>The builder for chaining.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public ViewMappingBuilder MapFromServiceLocator<TViewModel, TView>()
        where TViewModel : class
        where TView : class, IViewFor<TViewModel> =>
        MapFromServiceLocator<TViewModel, TView>(null);

    /// <summary>
    /// Maps a view model type to a view resolved from the service locator.
    /// The view must be registered in the dependency injection container.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TView">The view type implementing IViewFor&lt;TViewModel&gt;.</typeparam>
    /// <param name="contract">Optional contract to disambiguate multiple views for the same view model.</param>
    /// <returns>The builder for chaining.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public ViewMappingBuilder MapFromServiceLocator<TViewModel, TView>(string? contract)
        where TViewModel : class
        where TView : class, IViewFor<TViewModel>
    {
        _ = _locator.Map<TViewModel, TView>(
            static () => AppLocator.Current.GetService<TView>() ??
                  throw new InvalidOperationException($"View {nameof(TView)} not registered in service locator"),
            contract);
        return this;
    }
}
