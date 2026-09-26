// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.DataBinding;

/// <summary>Shows the bindings a view makes to its view model: two-way, one-way, one-way with a conversion, and <c>BindTo</c>.</summary>
public static class DataBindingExamples
{
    /// <summary>The title the user types into the new-item box.</summary>
    private const string ElectricianTitle = "Book electrician";

    /// <summary><c>Bind</c> keeps a text box and a view model property in step, whichever side changes.</summary>
    public static void BindTwoWay()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        using TodoListView view = new() { ViewModel = viewModel };

        using IReactiveBinding<TodoListView, BindingChange> binding = view.Bind(viewModel, x => x.NewTitle, v => v.NewTitleBox.Text);

        view.NewTitleBox.Text = ElectricianTitle;
        Console.WriteLine(viewModel.NewTitle);

        viewModel.NewTitle = string.Empty;
        Console.WriteLine($"[{view.NewTitleBox.Text}]");

        // Output:
        // Book electrician
        // []
    }

    /// <summary><c>OneWayBind</c> copies a view model property to the view; the list shows the items the filter lets through.</summary>
    /// <returns>A task that completes when the items are loaded.</returns>
    public static async Task BindOneWay()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        using TodoListView view = new() { ViewModel = viewModel };

        using IReactiveBinding<TodoListView, IReadOnlyList<TodoItem>> binding = view.OneWayBind(viewModel, x => x.Items, v => v.ItemList.Items);

        _ = await viewModel.Load.Execute();
        Console.WriteLine(view.ItemList.Items.Count);

        viewModel.FilterText = "bill";
        Console.WriteLine(view.ItemList.Items[0].Title);

        // Output:
        // 4
        // Pay electricity bill
    }

    /// <summary>A conversion turns the view model's value into what the control shows; here a count becomes a sentence.</summary>
    /// <returns>A task that completes when an item is finished.</returns>
    public static async Task ConvertWhileBinding()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        using TodoListView view = new() { ViewModel = viewModel };

        using IReactiveBinding<TodoListView, string> binding = view.OneWayBind(
            viewModel,
            x => x.RemainingCount,
            v => v.RemainingLabel.Text,
            static count => count == 1 ? "1 item left" : $"{count} items left");

        _ = await viewModel.Load.Execute();
        Console.WriteLine(view.RemainingLabel.Text);

        _ = await viewModel.Complete.Execute(viewModel.Items[0]);
        Console.WriteLine(view.RemainingLabel.Text);

        // Output:
        // 3 items left
        // 2 items left
    }

    /// <summary><c>BindTo</c> writes any observable to a property; here the error label is shown only while there is an error.</summary>
    /// <returns>A task that completes when the refused write has been reported.</returns>
    public static async Task BindAnObservable()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        using TodoListView view = new() { ViewModel = viewModel };

        using IDisposable binding = viewModel.WhenAnyValue(x => x.ErrorMessage)
            .Select(static message => message.Length > 0)
            .BindTo(view, v => v.ErrorLabel.IsVisible);
        Console.WriteLine(view.ErrorLabel.IsVisible);

        _ = await viewModel.Load.Execute();
        viewModel.NewTitle = "Buy groceries";
        try
        {
            _ = await viewModel.Add.Execute();
        }
        catch (TodoStoreException)
        {
            // The store refuses the duplicate; the view model reports it through ErrorMessage.
        }

        Console.WriteLine(view.ErrorLabel.IsVisible);

        // Output:
        // False
        // True
    }
}
