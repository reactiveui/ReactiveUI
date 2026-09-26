// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>The shopping list screen; the app shows the same instance everywhere it is opened from.</summary>
/// <remarks>Excluded from the source generator's automatic view dispatch; see <see cref="RecipeBookView"/>.</remarks>
[ExcludeFromViewRegistration]
[System.Diagnostics.DebuggerDisplay("ShoppingListView ViewModel = {ViewModel}")]
public sealed class ShoppingListView : IViewFor<ShoppingListViewModel>
{
    /// <inheritdoc/>
    public ShoppingListViewModel? ViewModel { get; set; }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ShoppingListViewModel?)value;
    }
}
