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

    /// <summary>
    /// <c>ViewContract</c> and <c>ViewContractObservable</c> read back exactly what was assigned to them, but assigning
    /// either one after construction does not re-resolve <c>Content</c>: the host only reacts to the single
    /// <c>ViewContractObservable</c> it captures in its constructor. See the library findings in this page's report
    /// for a minimal repro.
    /// </summary>
    public static void ContractPropertiesDoNotReResolveAfterConstruction()
    {
        AppLocator.CurrentMutable.Register<IViewFor<RecipeListViewModel>>(static () => new RecipeListContentView());
        AppLocator.CurrentMutable.Register<IViewFor<RecipeListViewModel>>(static () => new RecipeListWideContentView(), "Wide");

        ViewModelViewHost host = new() { ViewModel = new RecipeListViewModel(new RecipeBookScreen()) };
        Console.WriteLine(host.Content?.GetType().Name);

        host.ViewContract = "Wide";
        Console.WriteLine(host.ViewContract ?? "(null)");
        Console.WriteLine(host.Content?.GetType().Name);
        Console.WriteLine(ViewModelViewHost.ViewContractObservableProperty.PropertyName);

        // Output:
        // RecipeListContentView
        // (null)
        // RecipeListContentView
        // ViewContractObservable
    }

    /// <summary>
    /// <c>ContractFallbackByPass</c> is a plain bindable property: reading it back gives whatever was last assigned.
    /// It stops <c>ResolveViewForViewModel</c> falling back to the default view when the requested contract has no
    /// registered view, but every resolution after construction runs with a null contract (see
    /// <see cref="ContractPropertiesDoNotReResolveAfterConstruction"/>), so a null contract always finds the default
    /// view and this branch never runs from application code.
    /// </summary>
    public static void ContractFallbackByPassIsABindableProperty()
    {
        ViewModelViewHost host = new() { ContractFallbackByPass = true };

        Console.WriteLine(host.ContractFallbackByPass);
        Console.WriteLine(ViewModelViewHost.ContractFallbackByPassProperty.PropertyName);

        // Output:
        // True
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

        // Output:
        // 3
    }
}
