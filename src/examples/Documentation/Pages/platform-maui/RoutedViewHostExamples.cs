// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;
using Splat;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// Shows <see cref="RoutedViewHost"/>, the <c>NavigationPage</c> that keeps its own navigation stack in step with a
/// <see cref="RoutingState"/>. Everyday navigation through <c>Router.Navigate</c> and <c>Router.NavigateBack</c> is
/// covered on the routing page, using the platform-independent router; see that page's <c>RoutingExamples</c>. This
/// page instead shows the members <see cref="RoutedViewHost"/> itself adds: the constructor's screen requirement, its
/// two bindable properties, and the protected page-resolution members an app overrides for a custom policy.
/// </summary>
public static class RoutedViewHostExamples
{
    /// <summary>A <see cref="RoutedViewHost"/> needs a registered <see cref="IScreen"/> before it can find the app's router.</summary>
    public static void ConstructingWithoutAScreenThrows()
    {
        try
        {
            _ = new RoutedViewHost();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }

        // Output:
        // You *must* register an IScreen class representing your App's main Screen
    }

    /// <summary><c>Router</c> and <c>SetTitleOnNavigate</c> are bindable properties, set from the registered screen and from the host itself.</summary>
    public static void RouterAndSetTitleOnNavigateAreBindableProperties()
    {
        RecipeBookScreen screen = new();
        AppLocator.CurrentMutable.RegisterConstant<IScreen>(screen);

        RoutedViewHost host = new() { SetTitleOnNavigate = true };

        Console.WriteLine(ReferenceEquals(host.Router, screen.Router));
        Console.WriteLine(host.SetTitleOnNavigate);
        Console.WriteLine(RoutedViewHost.RouterProperty.PropertyName);
        Console.WriteLine(RoutedViewHost.SetTitleOnNavigateProperty.PropertyName);

        // Output:
        // True
        // True
        // Router
        // SetTitleOnNavigate
    }

    /// <summary>
    /// <c>SyncNavigationStacksAsync</c> pushes the router's stack onto the host's own <c>Navigation.NavigationStack</c>.
    /// Call it after changing <c>Router.NavigationStack</c> directly, such as restoring a saved stack.
    /// </summary>
    /// <returns>A task that completes once every page is pushed.</returns>
    public static async Task SyncingPushesTheRouterStackOntoTheNavigationPage()
    {
        RecipeBookScreen screen = new();
        AppLocator.CurrentMutable.RegisterConstant<IScreen>(screen);
        AppLocator.CurrentMutable.Register<IViewFor<RecipeListViewModel>>(static () => new RecipeListPage());

        RecipeRoutedViewHost host = new();
        screen.Router.NavigationStack.Add(new RecipeListViewModel(screen));

        await host.SyncAsync();

        Console.WriteLine(host.Navigation.NavigationStack.Count);
        Console.WriteLine(host.Navigation.NavigationStack[0].GetType().Name);

        // Output:
        // 1
        // RecipeListPage
    }

    /// <summary><c>PageForViewModel</c> and <c>PagesForViewModel</c> resolve the page for one view model, applying the title when <c>SetTitleOnNavigate</c> is set.</summary>
    public static void ResolvingAPageForAViewModel()
    {
        RecipeBookScreen screen = new();
        AppLocator.CurrentMutable.RegisterConstant<IScreen>(screen);
        AppLocator.CurrentMutable.Register<IViewFor<RecipeDetailViewModel>>(static () => new RecipeDetailPage());

        RecipeRoutedViewHost host = new() { SetTitleOnNavigate = true };
        RecipeDetailViewModel soup = new(screen, RecipeBook.Recipes[0]);

        Console.WriteLine(host.ResolvePage(soup).GetType().Name);

        List<string> pageNames = [];
        using IDisposable subscription = host.ResolvePages(soup).Subscribe(page => pageNames.Add(page.GetType().Name));
        Console.WriteLine(string.Join(",", pageNames));

        List<string> emptyNames = [];
        using IDisposable emptySubscription = host.ResolvePages(null).Subscribe(page => emptyNames.Add(page.GetType().Name));
        Console.WriteLine(emptyNames.Count);

        // Output:
        // RecipeDetailPage
        // RecipeDetailPage
        // 0
    }

    /// <summary><see cref="RoutedViewHost{TViewModel}"/> resolves through the compile-time view model type, with no reflection.</summary>
    public static void GenericHostResolvesWithoutReflection()
    {
        RecipeBookScreen screen = new();
        AppLocator.CurrentMutable.RegisterConstant<IScreen>(screen);
        AppLocator.CurrentMutable.Register<IViewFor<RecipeDetailViewModel>>(static () => new RecipeDetailPage());

        RecipeRoutedViewHostOfDetail host = new();
        RecipeDetailViewModel chicken = new(screen, RecipeBook.Recipes[1]);

        Console.WriteLine(host.ResolvePage(chicken).GetType().Name);

        // Output:
        // RecipeDetailPage
    }

    /// <summary><c>InvalidateCurrentViewModel</c> reassigns the current page's view model from the router, when the two are the same type.</summary>
    /// <returns>A task that completes once the stack is synced and the view model refreshed.</returns>
    public static async Task InvalidatingRefreshesTheCurrentPagesViewModel()
    {
        RecipeBookScreen screen = new();
        AppLocator.CurrentMutable.RegisterConstant<IScreen>(screen);
        AppLocator.CurrentMutable.Register<IViewFor<RecipeListViewModel>>(static () => new RecipeListPage());

        RecipeRoutedViewHost host = new();
        RecipeListViewModel first = new(screen);
        screen.Router.NavigationStack.Add(first);
        await host.SyncAsync();

        RecipeListViewModel second = new(screen);
        screen.Router.NavigationStack[0] = second;
        host.RefreshCurrentViewModel();

        RecipeListPage page = (RecipeListPage)host.Navigation.NavigationStack[0];
        Console.WriteLine(ReferenceEquals(page.ViewModel, second));

        // Output:
        // True
    }
}
