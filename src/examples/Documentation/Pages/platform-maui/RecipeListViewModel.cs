// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>The page that lists the recipes. Opening one navigates to its detail page.</summary>
[System.Diagnostics.DebuggerDisplay("{UrlPathSegment}")]
public sealed class RecipeListViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>Initializes a new instance of the <see cref="RecipeListViewModel"/> class.</summary>
    /// <param name="hostScreen">The window the page is shown in.</param>
    public RecipeListViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
        Open = ReactiveCommand.CreateFromObservable<RecipeItem, IRoutableViewModel>(
            recipe => HostScreen.Router.Navigate.Execute(new RecipeDetailViewModel(HostScreen, recipe)));
    }

    /// <inheritdoc/>
    public string UrlPathSegment => "recipes";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the recipes the page lists.</summary>
    public IReadOnlyList<RecipeItem> Recipes => RecipeBook.Recipes;

    /// <summary>Gets the command that opens a recipe's detail page.</summary>
    public ReactiveCommand<RecipeItem, IRoutableViewModel> Open { get; }
}
