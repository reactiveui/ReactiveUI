// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Routing;

/// <summary>The page that shows one recipe, with a command that starts its cooking timer.</summary>
[System.Diagnostics.DebuggerDisplay("{UrlPathSegment}")]
public sealed class RecipeDetailPage : ReactiveObject, IRoutableViewModel
{
    /// <summary>Initializes a new instance of the <see cref="RecipeDetailPage"/> class.</summary>
    /// <param name="hostScreen">The window the page is shown in.</param>
    /// <param name="recipe">The recipe the page shows.</param>
    public RecipeDetailPage(IScreen hostScreen, Recipe recipe)
    {
        HostScreen = hostScreen;
        Recipe = recipe;
        StartCooking = ReactiveCommand.CreateFromObservable(
            () => HostScreen.Router.Navigate.Execute(new CookingTimerPage(HostScreen, recipe)));
    }

    /// <inheritdoc/>
    public string UrlPathSegment => $"recipes/{Recipe.Id}";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the recipe the page shows.</summary>
    public Recipe Recipe { get; }

    /// <summary>Gets the command that opens the cooking timer for this recipe.</summary>
    public ReactiveCommand<RxVoid, IRoutableViewModel> StartCooking { get; }
}
