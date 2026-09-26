// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;
using Splat;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// Shows the two ways to host a view registered only with the service locator: the Unsafe twin of each host, or a
/// <c>MapFromServiceLocator</c> entry that the default hosts read.
/// </summary>
public static class ServiceLocatorViewExamples
{
    /// <summary>The default <see cref="ViewModelViewHost"/> throws for this view; <see cref="ViewModelViewHostUnsafe"/> finds it.</summary>
    public static void ViewModelViewHostUnsafeFindsTheView()
    {
        AppLocator.CurrentMutable.Register<IViewFor<ShoppingListViewModel>>(static () => new ShoppingListView());
        ShoppingListViewModel shopping = new(new RecipeBookScreen());

        ViewModelViewHost host = new();
        try
        {
            host.ViewModel = shopping;
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine(exception.GetType().Name);
        }

        ViewModelViewHostUnsafe unsafeHost = new() { ViewModel = shopping };
        Console.WriteLine(unsafeHost.Content?.GetType().Name);

        // Output:
        // InvalidOperationException
        // ShoppingListView
    }

    /// <summary><see cref="RoutedViewHostUnsafe"/> pushes a page registered only with the service locator.</summary>
    /// <returns>A task that completes once the navigation has run.</returns>
    public static async Task RoutedViewHostUnsafePushesThePage()
    {
        RecipeBookScreen screen = new();
        AppLocator.CurrentMutable.RegisterConstant<IScreen>(screen);
        AppLocator.CurrentMutable.Register<IViewFor<ShoppingListViewModel>>(static () => new ShoppingListPage());

        RoutedViewHostUnsafe host = new();
        _ = await screen.Router.Navigate.Execute(new ShoppingListViewModel(screen));

        Console.WriteLine(host.Navigation.NavigationStack[0].GetType().Name);

        // Output:
        // ShoppingListPage
    }

    /// <summary>
    /// <c>MapFromServiceLocator</c> adds the service locator's view to the view locator's <c>Map</c> entries, so the
    /// default <see cref="ViewModelViewHost"/> finds it and the app stays safe to compile ahead of time.
    /// </summary>
    public static void MapFromServiceLocatorLetsTheDefaultHostFindTheView()
    {
        AppLocator.CurrentMutable.Register<IViewFor<ShoppingListViewModel>>(static () => new ShoppingListView());
        DefaultViewLocator locator = new();
        _ = locator.CreateMappingBuilder().MapFromServiceLocator<ShoppingListViewModel, IViewFor<ShoppingListViewModel>>();

        ViewModelViewHost host = new() { ViewLocator = locator, ViewModel = new ShoppingListViewModel(new RecipeBookScreen()) };

        Console.WriteLine(host.Content?.GetType().Name);

        // Output:
        // ShoppingListView
    }
}
