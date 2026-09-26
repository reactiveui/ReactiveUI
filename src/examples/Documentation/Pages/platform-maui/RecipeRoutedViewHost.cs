// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;
using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// A <see cref="RoutedViewHost"/> that exposes its protected members so the examples can call them directly, the way
/// an app overriding <c>PageForViewModel</c> or <c>PagesForViewModel</c> for a custom page-resolution policy would.
/// </summary>
[System.Diagnostics.DebuggerDisplay("RecipeRoutedViewHost")]
public sealed class RecipeRoutedViewHost : RoutedViewHost
{
    /// <summary>Resolves the page for a view model, the same way <c>SyncNavigationStacksAsync</c> does.</summary>
    /// <param name="viewModel">The view model to resolve a page for.</param>
    /// <returns>The resolved page.</returns>
    public Page ResolvePage(IRoutableViewModel viewModel) => PageForViewModel(viewModel);

    /// <summary>Resolves the page for a view model as an observable, or an empty one when <paramref name="viewModel"/> is <see langword="null"/>.</summary>
    /// <param name="viewModel">The view model to resolve a page for, or <see langword="null"/>.</param>
    /// <returns>An observable of the resolved page.</returns>
    public IObservable<Page> ResolvePages(IRoutableViewModel? viewModel) => PagesForViewModel(viewModel);

    /// <summary>Pushes the router's stack onto the page's own navigation stack.</summary>
    /// <returns>A task that completes once every page is pushed.</returns>
    public Task SyncAsync() => SyncNavigationStacksAsync();

    /// <summary>Reassigns the current page's view model from the router's current view model.</summary>
    public void RefreshCurrentViewModel() => InvalidateCurrentViewModel();
}
