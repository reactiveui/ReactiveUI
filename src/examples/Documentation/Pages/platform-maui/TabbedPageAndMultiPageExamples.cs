// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// Shows <see cref="ReactiveTabbedPage{TViewModel}"/> and <see cref="ReactiveMultiPage{TPage, TViewModel}"/>, which
/// otherwise follow the same <c>ViewModel</c>/<c>BindingContext</c> pattern as every other page base. Building either
/// one asks the constructing thread for a MAUI dispatcher, which only exists once a MAUI app is running, so this
/// method builds but the page never calls it.
/// </summary>
public static class TabbedPageAndMultiPageExamples
{
    /// <summary>Building a tabbed page, and the concrete <see cref="RecipeMultiPage"/> it is built on, needs a running app.</summary>
    public static void TabbedPageAndMultiPageNeedARunningApp()
    {
        RecipeBookScreen screen = new();

        RecipeListViewModel recipes = new(screen);
        RecipeTabbedPage tabbedPage = new() { BindingContext = recipes };
        Console.WriteLine(ReferenceEquals(tabbedPage.ViewModel, recipes));

        RecipeMultiPage multiPage = new() { BindingContext = recipes };
        Console.WriteLine(ReferenceEquals(multiPage.ViewModel, recipes));
    }
}
