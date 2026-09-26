// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;
using ReactiveUI.Maui;
using Splat;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// Shows <see cref="ViewModelViewHost"/>, a <c>ContentView</c> that shows the view registered for whatever view model
/// is assigned to it. Unlike <see cref="RoutedViewHost"/> it keeps no navigation stack: it always shows one view model.
/// </summary>
public static class ViewModelViewHostExamples
{
    /// <summary>Setting <c>ViewModel</c> resolves the registered view and shows it as <c>Content</c>.</summary>
    public static void ViewModelResolvesTheRegisteredView()
    {
        AppLocator.CurrentMutable.Register<IViewFor<RecipeListViewModel>>(static () => new RecipeListContentView());

        ViewModelViewHost host = new();
        RecipeListViewModel recipes = new(new RecipeBookScreen());

        host.ViewModel = recipes;

        Console.WriteLine(host.Content?.GetType().Name);
        Console.WriteLine(ReferenceEquals(((IViewFor)host.Content!).ViewModel, recipes));
        Console.WriteLine(ViewModelViewHost.ViewModelProperty.PropertyName);

        // Output:
        // RecipeListContentView
        // True
        // ViewModel
    }

    /// <summary>With no view model, the host shows <c>DefaultContent</c> instead.</summary>
    public static void NoViewModelShowsTheDefaultContent()
    {
        ViewModelViewHost host = new() { DefaultContent = new Label { Text = "Pick a recipe" } };

        host.ViewModel = new RecipeListViewModel(new RecipeBookScreen());
        host.ViewModel = null;

        Console.WriteLine(((Label)host.Content).Text);
        Console.WriteLine(ViewModelViewHost.DefaultContentProperty.PropertyName);

        // Output:
        // Pick a recipe
        // DefaultContent
    }

    /// <summary>Setting <c>ViewContract</c> resolves the view marked with that contract and shows it.</summary>
    public static void ViewContractPicksTheViewForTheContract()
    {
        AppLocator.CurrentMutable.Register<IViewFor<RecipeListViewModel>>(static () => new RecipeListContentView());

        ViewModelViewHost host = new() { ViewModel = new RecipeListViewModel(new RecipeBookScreen()) };
        Console.WriteLine(host.Content?.GetType().Name);

        host.ViewContract = "Wide";
        Console.WriteLine(host.ViewContract);
        Console.WriteLine(host.Content?.GetType().Name);

        // Output:
        // RecipeListContentView
        // Wide
        // RecipeListWideContentView
    }

    /// <summary>Each contract <c>ViewContractObservable</c> emits resolves the view again, so a layout stream can switch views as it changes.</summary>
    public static void ViewContractObservableSwitchesTheView()
    {
        AppLocator.CurrentMutable.Register<IViewFor<RecipeListViewModel>>(static () => new RecipeListContentView());

        using Signal<string?> layout = new();
        ViewModelViewHost host = new()
        {
            ViewModel = new RecipeListViewModel(new RecipeBookScreen()),
            ViewContractObservable = layout,
        };

        layout.OnNext("Wide");
        Console.WriteLine(host.Content?.GetType().Name);

        layout.OnNext(null);
        Console.WriteLine(host.Content?.GetType().Name);
        Console.WriteLine(ViewModelViewHost.ViewContractObservableProperty.PropertyName);

        // Output:
        // RecipeListWideContentView
        // RecipeListContentView
        // ViewContractObservable
    }

    /// <summary>
    /// A contract with no registered view falls back to the default view. With <c>ContractFallbackByPass</c> set,
    /// the host throws instead.
    /// </summary>
    public static void ContractFallbackByPassStopsTheFallbackToTheDefaultView()
    {
        AppLocator.CurrentMutable.Register<IViewFor<RecipeListViewModel>>(static () => new RecipeListContentView());

        ViewModelViewHost fallingBack = new() { ViewModel = new RecipeListViewModel(new RecipeBookScreen()) };
        fallingBack.ViewContract = "Print";
        Console.WriteLine(fallingBack.Content?.GetType().Name);

        ViewModelViewHost strict = new()
        {
            ContractFallbackByPass = true,
            ViewModel = new RecipeListViewModel(new RecipeBookScreen()),
        };

        try
        {
            strict.ViewContract = "Print";
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine(exception.GetType().Name);
        }

        Console.WriteLine(ViewModelViewHost.ContractFallbackByPassProperty.PropertyName);

        // Output:
        // RecipeListContentView
        // InvalidOperationException
        // ContractFallbackByPass
    }

    /// <summary>Setting <c>ViewLocator</c> overrides the service-located locator for this host alone.</summary>
    public static void ViewLocatorOverridesTheDefaultLocator()
    {
        ViewModelViewHost host = new() { ViewLocator = new AlwaysWideViewLocator() };

        host.ViewModel = new RecipeListViewModel(new RecipeBookScreen());

        Console.WriteLine(host.Content?.GetType().Name);

        // Output:
        // RecipeListWideContentView
    }

    /// <summary><see cref="ViewModelViewHost{TViewModel}"/> gives <c>ViewModel</c> its view model's own type, so no cast is needed to read it back.</summary>
    public static void GenericHostTypesTheViewModelProperty()
    {
        AppLocator.CurrentMutable.Register<IViewFor<RecipeListViewModel>>(static () => new RecipeListContentView());

        ViewModelViewHost<RecipeListViewModel> host = new();
        RecipeListViewModel recipes = new(new RecipeBookScreen());

        host.ViewModel = recipes;

        Console.WriteLine(host.ViewModel.Recipes.Count);
        Console.WriteLine(ReferenceEquals(((IViewFor)host).ViewModel, recipes));

        // Output:
        // 3
        // True
    }
}
