// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Fluent builder for registering AOT-compatible view-to-viewmodel mappings.
/// </summary>
public sealed class ViewMappingBuilder
{
    private readonly DefaultViewLocator _locator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewMappingBuilder"/> class.
    /// </summary>
    /// <param name="locator">The view locator to register mappings with.</param>
    internal ViewMappingBuilder(DefaultViewLocator locator)
    {
        ArgumentExceptionHelper.ThrowIfNull(locator);
        _locator = locator;
    }

    /// <summary>
    /// Maps a view model type to a view type with automatic instantiation.
    /// The view must have a parameterless constructor.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TView">The view type implementing IViewFor&lt;TViewModel&gt;.</typeparam>
    /// <param name="contract">Optional contract to disambiguate multiple views for the same view model.</param>
    /// <returns>The builder for chaining.</returns>
    public ViewMappingBuilder Map<TViewModel, TView>(string? contract = null)
        where TViewModel : class
        where TView : class, IViewFor<TViewModel>, new()
    {
        _locator.Map<TViewModel, TView>(() => new TView(), contract);
        return this;
    }

    /// <summary>
    /// Maps a view model type to a view type with a custom factory function.
    /// Use this when the view requires constructor parameters or custom initialization.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TView">The view type implementing IViewFor&lt;TViewModel&gt;.</typeparam>
    /// <param name="factory">Factory function that creates the view.</param>
    /// <param name="contract">Optional contract to disambiguate multiple views for the same view model.</param>
    /// <returns>The builder for chaining.</returns>
    public ViewMappingBuilder Map<TViewModel, TView>(Func<TView> factory, string? contract = null)
        where TViewModel : class
        where TView : class, IViewFor<TViewModel>
    {
        ArgumentExceptionHelper.ThrowIfNull(factory);
        _locator.Map<TViewModel, TView>(factory, contract);
        return this;
    }

    /// <summary>
    /// Maps a view model type to a view resolved from the service locator.
    /// The view must be registered in the dependency injection container.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TView">The view type implementing IViewFor&lt;TViewModel&gt;.</typeparam>
    /// <param name="contract">Optional contract to disambiguate multiple views for the same view model.</param>
    /// <returns>The builder for chaining.</returns>
    public ViewMappingBuilder MapFromServiceLocator<TViewModel, TView>(string? contract = null)
        where TViewModel : class
        where TView : class, IViewFor<TViewModel>
    {
        _locator.Map<TViewModel, TView>(
            () => AppLocator.Current.GetService<TView>() ?? throw new InvalidOperationException($"View {typeof(TView).Name} not registered in service locator"),
            contract);
        return this;
    }
}
