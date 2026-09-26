// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// Shows the page and view base classes that pair a Microsoft.Maui.Controls type with <see cref="IViewFor{T}"/>. Every
/// one of them exposes the same three members: a <c>ViewModelProperty</c> bindable field, a typed <c>ViewModel</c>
/// property, and an override that copies <c>BindingContext</c> into <c>ViewModel</c> (and back) whenever MAUI sets it.
/// </summary>
public static class PageBasesExamples
{
    /// <summary>
    /// <see cref="ReactiveContentPage{TViewModel}"/> wraps <c>ContentPage</c>. Setting <c>ViewModel</c> updates
    /// <c>BindingContext</c>, and setting <c>BindingContext</c> updates <c>ViewModel</c> back.
    /// </summary>
    public static void ContentPageSyncsViewModelAndBindingContext()
    {
        RecipeListPage page = new();
        RecipeListViewModel first = new(new RecipeBookScreen());
        RecipeListViewModel second = new(new RecipeBookScreen());

        page.ViewModel = first;
        Console.WriteLine(ReferenceEquals(page.BindingContext, first));

        page.BindingContext = second;
        Console.WriteLine(ReferenceEquals(page.ViewModel, second));

        Console.WriteLine(ReactiveContentPage<RecipeListViewModel>.ViewModelProperty.PropertyName);

        // Output:
        // True
        // True
        // ViewModel
    }

    /// <summary>
    /// <see cref="ReactiveNavigationPage{TViewModel}"/>, <see cref="ReactiveFlyoutPage{TViewModel}"/>,
    /// <see cref="ReactiveMasterDetailPage{TViewModel}"/>, <see cref="ReactiveCarouselView{TViewModel}"/> and
    /// <see cref="ReactiveContentView{TViewModel}"/> follow the same pattern as
    /// <see cref="ContentPageSyncsViewModelAndBindingContext"/>, over <c>NavigationPage</c>, <c>FlyoutPage</c> (twice,
    /// under its old and new names) <c>CarouselView</c> and <c>ContentView</c>.
    /// </summary>
    public static void OtherPageAndViewBasesFollowTheSamePattern()
    {
        RecipeListViewModel recipes = new(new RecipeBookScreen());
        RecipeBookScreen screen = new();

        RecipeNavigationPage navigationPage = new() { BindingContext = recipes };
        Console.WriteLine($"{nameof(RecipeNavigationPage)}: {navigationPage.ViewModel == recipes}");

        RecipeFlyoutPage flyoutPage = new() { BindingContext = screen };
        Console.WriteLine($"{nameof(RecipeFlyoutPage)}: {flyoutPage.ViewModel == screen}");

        RecipeMasterDetailPage masterDetailPage = new() { BindingContext = screen };
        Console.WriteLine($"{nameof(RecipeMasterDetailPage)}: {masterDetailPage.ViewModel == screen}");

        RecipeCarouselView carouselView = new() { BindingContext = recipes };
        Console.WriteLine($"{nameof(RecipeCarouselView)}: {carouselView.ViewModel == recipes}");

        RecipeSummaryView summaryView = new() { BindingContext = RecipeBook.Recipes[0] };
        Console.WriteLine($"{nameof(RecipeSummaryView)}: {summaryView.ViewModel == RecipeBook.Recipes[0]}");

        // Output:
        // RecipeNavigationPage: True
        // RecipeFlyoutPage: True
        // RecipeMasterDetailPage: True
        // RecipeCarouselView: True
        // RecipeSummaryView: True
    }

    /// <summary>
    /// <see cref="ReactiveWindow{TViewModel}"/> and <see cref="ReactiveTitleBar{TViewModel}"/> wrap the desktop-only
    /// <c>Window</c> and <c>TitleBar</c> types, but follow the same <c>ViewModel</c>/<c>BindingContext</c> pattern.
    /// </summary>
    public static void WindowAndTitleBarFollowTheSamePattern()
    {
        RecipeBookScreen screen = new();

        RecipeWindow window = new() { BindingContext = screen };
        Console.WriteLine($"{nameof(RecipeWindow)}: {window.ViewModel == screen}");

        RecipeTitleBar titleBar = new() { BindingContext = screen };
        Console.WriteLine($"{nameof(RecipeTitleBar)}: {titleBar.ViewModel == screen}");

        // Output:
        // RecipeWindow: True
        // RecipeTitleBar: True
    }

    /// <summary>
    /// <see cref="ReactivePage{TViewModel}"/> is the older <c>Page</c> wrapper <see cref="ReactiveContentPage{TViewModel}"/>
    /// replaced. It follows the same pattern, and adds <c>BindingRoot</c>, an alias for <c>ViewModel</c> that XAML binds
    /// against.
    /// </summary>
    public static void ReactivePageAddsBindingRoot()
    {
        RecipePage page = new() { BindingContext = new RecipeListViewModel(new RecipeBookScreen()) };

        Console.WriteLine(ReferenceEquals(page.BindingRoot, page.ViewModel));
        Console.WriteLine(ReactivePage<RecipeListViewModel>.ViewModelProperty.PropertyName);

        // Output:
        // True
        // ViewModel
    }

    /// <summary><see cref="ReactiveShell{TViewModel}"/> wraps MAUI's app-shell root, following the same pattern.</summary>
    public static void ShellFollowsTheSamePattern()
    {
        RecipeBookScreen screen = new();
        RecipeShell shell = new() { BindingContext = screen };

        Console.WriteLine(shell.ViewModel == screen);

        // Output:
        // True
    }
}
