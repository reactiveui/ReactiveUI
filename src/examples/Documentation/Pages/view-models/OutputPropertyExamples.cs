// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.ViewModels;

/// <summary>Shows output properties: <c>ToProperty</c> turns an observable into a read-only property that raises its own changes.</summary>
public static class OutputPropertyExamples
{
    /// <summary>The filtered list is an output property of the loaded items and the filter text.</summary>
    /// <returns>A task that completes when the items are loaded.</returns>
    public static async Task DeriveFilteredList()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        await viewModel.Load.Execute();

        Console.WriteLine(viewModel.Items.Count);

        viewModel.FilterText = "book";

        Console.WriteLine(viewModel.Items.Count);
        Console.WriteLine(viewModel.Items[0].Title);

        // Output:
        // 4
        // 1
        // Book dentist appointment
    }

    /// <summary>The count of what is left follows the list, and raises its own change for the view to pick up.</summary>
    /// <returns>A task that completes when an item is finished.</returns>
    public static async Task CountWhatIsLeft()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        using IDisposable subscription = viewModel.WhenAnyValue(x => x.RemainingCount)
            .Subscribe(static count => Console.WriteLine($"{count} left"));

        await viewModel.Load.Execute();
        await viewModel.Complete.Execute(viewModel.Items[0]);

        // Output:
        // 0 left
        // 3 left
        // 2 left
    }

    /// <summary>A command's <c>IsExecuting</c> as an output property drives a loading indicator.</summary>
    /// <returns>A task that completes when the items are loaded.</returns>
    public static async Task ReportCommandProgress()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        using IDisposable subscription = viewModel.WhenAnyValue(x => x.IsLoading)
            .Subscribe(static loading => Console.WriteLine(loading ? "Loading..." : "Idle"));

        _ = await viewModel.Load.Execute();

        // Output:
        // Idle
        // Loading...
        // Idle
    }
}
