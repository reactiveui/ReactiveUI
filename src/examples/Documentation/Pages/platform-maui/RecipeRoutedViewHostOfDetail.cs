// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;
using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// A <see cref="RoutedViewHost{TViewModel}"/> fixed to <see cref="RecipeDetailViewModel"/>. Resolving its page never
/// uses reflection, unlike the non-generic <see cref="RoutedViewHost"/>, so it is safe to trim and to publish as
/// NativeAOT.
/// </summary>
[System.Diagnostics.DebuggerDisplay("RecipeRoutedViewHostOfDetail")]
public sealed class RecipeRoutedViewHostOfDetail : RoutedViewHost<RecipeDetailViewModel>
{
    /// <summary>Resolves the page for the detail view model without reflection.</summary>
    /// <param name="viewModel">The view model to resolve a page for.</param>
    /// <returns>The resolved page.</returns>
    public Page ResolvePage(RecipeDetailViewModel viewModel) => PageForViewModel(viewModel);

    /// <summary>Resolves the page for the detail view model as an observable, the same way the router's own navigation does.</summary>
    /// <param name="viewModel">The view model to resolve a page for, or <see langword="null"/>.</param>
    /// <returns>An observable of the resolved page.</returns>
    public IObservable<Page> ResolvePages(RecipeDetailViewModel? viewModel) => PagesForViewModel(viewModel);
}
