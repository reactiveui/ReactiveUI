// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>The recipes the sample app ships with.</summary>
[System.Diagnostics.DebuggerDisplay("RecipeBook")]
public static class RecipeBook
{
    /// <summary>Gets the seeded recipes, oldest first.</summary>
    public static IReadOnlyList<RecipeItem> Recipes { get; } =
    [
        new(1, "Tomato Soup", "Starter"),
        new(2, "Roast Chicken", "Main"),
        new(3, "Apple Crumble", "Dessert"),
    ];
}
