// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Routing;

/// <summary>The page that lists the recipes. Opening one navigates to its detail page.</summary>
[System.Diagnostics.DebuggerDisplay("{UrlPathSegment}")]
public sealed class RecipeListPage : ReactiveObject, IRoutableViewModel
{
    /// <summary>Initializes a new instance of the <see cref="RecipeListPage"/> class.</summary>
    /// <param name="hostScreen">The window the page is shown in.</param>
    /// <param name="recipes">The recipes the page lists.</param>
    public RecipeListPage(IScreen hostScreen, IReadOnlyList<Recipe> recipes)
    {
        HostScreen = hostScreen;
        Recipes = recipes;
        Open = ReactiveCommand.CreateFromObservable<Recipe, IRoutableViewModel>(
            recipe => HostScreen.Router.Navigate.Execute(new RecipeDetailPage(HostScreen, recipe)));
    }

    /// <inheritdoc/>
    public string UrlPathSegment => "recipes";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the recipes the page lists.</summary>
    public IReadOnlyList<Recipe> Recipes { get; }

    /// <summary>Gets the command that opens a recipe's detail page.</summary>
    public ReactiveCommand<Recipe, IRoutableViewModel> Open { get; }
}
