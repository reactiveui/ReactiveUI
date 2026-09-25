// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.Commands;

/// <summary>Shows <c>BindCommand</c>: a button on the view runs a view model command when it is clicked.</summary>
public static class BindCommandExamples
{
    /// <summary>
    /// The add button runs the add command through its <c>Click</c> event. A click while the title is blank does nothing,
    /// because the command may not run; once a title is typed, a click stores it.
    /// </summary>
    /// <returns>A task that completes when the item is stored.</returns>
    public static async Task BindAddButton()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        _ = await viewModel.Load.Execute();
        using TodoListView view = new() { ViewModel = viewModel };

        using var binding = view.BindCommand(viewModel, x => x.Add, v => v.AddButton);

        view.AddButton.PerformClick();
        Console.WriteLine(viewModel.Items.Count);

        viewModel.NewTitle = "Book electrician";
        var added = viewModel.Add.FirstAsync().ToTask();
        view.AddButton.PerformClick();
        var item = await added;

        Console.WriteLine(item.Title);
        Console.WriteLine(viewModel.Items.Count);

        // Output:
        // 4
        // Book electrician
        // 5
    }

    /// <summary>A parameter observable hands the command its argument; here the complete button finishes the item selected in the list.</summary>
    /// <returns>A task that completes when the item is finished.</returns>
    public static async Task PassTheSelectedItem()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        _ = await viewModel.Load.Execute();
        using TodoListView view = new() { ViewModel = viewModel };
        view.ItemList.Items = viewModel.Items;

        using var binding = view.BindCommand(
            viewModel,
            x => x.Complete,
            v => v.CompleteButton,
            view.WhenAnyValue(v => v.ItemList.SelectedItem).WhereNotNull());

        view.ItemList.SelectedItem = viewModel.Items[0];
        var completed = viewModel.Complete.FirstAsync().ToTask();
        view.CompleteButton.PerformClick();
        var item = await completed;

        Console.WriteLine($"{item.Title}: {item.IsDone}");
        Console.WriteLine(viewModel.RemainingCount);

        // Output:
        // Buy groceries: True
        // 2
    }
}
