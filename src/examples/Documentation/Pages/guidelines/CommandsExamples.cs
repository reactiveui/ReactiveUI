// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Controls;
using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>Shows binding a button to a command instead of wiring its click event to a method by naming convention.</summary>
public static class CommandsExamples
{
    /// <summary>The button's click event calls a method directly. Nothing disables it once there is nothing left to submit, so a second click still reaches the method.</summary>
    public static void AvoidWiringAClickHandlerDirectly()
    {
        GradeRepository repository = new();
        Button submitButton = new();
        submitButton.Click += (_, _) => repository.Submit();

        submitButton.PerformClick();
        submitButton.PerformClick();

        // Output:
        // Submitted Ada: 92
        // Nothing to submit, but the button let the click through anyway.
    }

    /// <summary><c>BindCommand</c> disables the button through the command's <c>CanExecute</c>, so a click while the command cannot run does nothing.</summary>
    /// <returns>A task that completes once the item has been added.</returns>
    public static async Task PreferBindingToACommand()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        _ = await viewModel.Load.Execute();
        using TodoListView view = new() { ViewModel = viewModel };
        using IDisposable binding = view.BindCommand(viewModel, x => x.Add, v => v.AddButton);

        view.AddButton.PerformClick(); // NewTitle is blank: CanExecute is false, so the button is disabled and nothing happens.
        Console.WriteLine(viewModel.Items.Count);

        viewModel.NewTitle = "Submit Ada's grade";
        Task<TodoItem> added = viewModel.Add.FirstAsync().ToTask();
        view.AddButton.PerformClick();
        TodoItem item = await added;

        Console.WriteLine(item.Title);
        Console.WriteLine(viewModel.Items.Count);

        // Output:
        // 4
        // Submit Ada's grade
        // 5
    }
}
