// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>The page that shows one recipe.</summary>
/// <param name="hostScreen">The window the page is shown in.</param>
/// <param name="recipe">The recipe the page shows.</param>
[System.Diagnostics.DebuggerDisplay("{UrlPathSegment}")]
public sealed class RecipeDetailViewModel(IScreen hostScreen, RecipeItem recipe) : ReactiveObject, IRoutableViewModel
{
    /// <inheritdoc/>
    public string UrlPathSegment => $"recipes/{Recipe.Id}";

    /// <inheritdoc/>
    public IScreen HostScreen { get; } = hostScreen;

    /// <summary>Gets the recipe the page shows.</summary>
    public RecipeItem Recipe { get; } = recipe;
}
