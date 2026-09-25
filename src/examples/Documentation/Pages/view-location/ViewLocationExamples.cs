// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.GitHub;
using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>Shows how the view locator finds the view for a view model: at compile time, by contract, from a module, or with a locator of your own.</summary>
public static class ViewLocationExamples
{
    /// <summary>
    /// The source generator finds every <see cref="IViewFor{T}"/> in the app, so the to-do list's view is found without
    /// registering it, and without reflection. The locator hands the view its view model.
    /// </summary>
    public static void FindTheViewForAViewModel()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());

        var view = ViewLocator.GetCurrent().ResolveView(viewModel);

        Console.WriteLine(view?.GetType().Name);
        Console.WriteLine(ReferenceEquals(view?.ViewModel, viewModel));

        // Output:
        // TodoListView
        // True
    }

    /// <summary>
    /// A contract picks one of several views of the same view model. A contract with no view finds nothing, and a host
    /// then decides whether to fall back to the view without a contract.
    /// </summary>
    public static void PickAViewByContract()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        var locator = ViewLocator.GetCurrent();

        Console.WriteLine(locator.ResolveView(viewModel, ViewContracts.Compact)?.GetType().Name);
        Console.WriteLine(locator.ResolveView(viewModel, ViewContracts.Print)?.GetType().Name ?? "(none)");

        // Output:
        // CompactTodoListView
        // (none)
    }

    /// <summary>A view module maps the views of one feature; the app adds it with <c>WithViewModule</c> when it starts.</summary>
    public static void MapViewsInAModule()
    {
        using RepositorySearchViewModel viewModel = new(new InMemoryGitHubApi());

        var view = ViewLocator.GetCurrent().ResolveView(viewModel);

        Console.WriteLine(view?.GetType().Name);

        // Output:
        // RepositorySearchView
    }

    /// <summary>A locator of your own can wrap the default one; this one shows a placeholder for screens not built yet.</summary>
    public static void WrapTheViewLocator()
    {
        PlaceholderViewLocator locator = new(ViewLocator.GetCurrent());
        using TodoListViewModel todos = new(InMemoryTodoStore.CreateSeeded());

        Console.WriteLine(locator.ResolveView(todos)?.GetType().Name);
        Console.WriteLine(locator.ResolveView(new SettingsViewModel())?.GetType().Name);

        // Output:
        // TodoListView
        // PlaceholderView
    }
}
