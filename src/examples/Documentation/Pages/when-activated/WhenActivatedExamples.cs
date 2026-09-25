// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.WhenActivated;

/// <summary>Shows <c>WhenActivated</c>: work that starts when a screen is shown and is cleaned up when it is hidden.</summary>
public static class WhenActivatedExamples
{
    /// <summary>The title the user types into the new-item box.</summary>
    private const string ElectricianTitle = "Book electrician";

    /// <summary>A view model's <c>WhenActivated</c> block runs when its activator is activated; the to-do list loads itself.</summary>
    public static void ActivateAViewModel()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        Console.WriteLine(viewModel.Items.Count);

        using (viewModel.Activator.Activate())
        {
            Console.WriteLine(viewModel.Items.Count);
        }

        // Output:
        // 0
        // 4
    }

    /// <summary>
    /// Showing the view runs its <c>WhenActivated</c> block, which binds the controls, and activates its view model, which
    /// loads the list. Hiding it drops the bindings, so typing into the hidden screen no longer reaches the view model.
    /// </summary>
    public static void ActivateAView()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        using TodoListView view = new() { ViewModel = viewModel };

        view.Show();
        Console.WriteLine(view.RemainingLabel.Text);

        view.NewTitleBox.Text = ElectricianTitle;
        Console.WriteLine(viewModel.NewTitle);

        view.Hide();
        view.NewTitleBox.Text = "Book plumber";
        Console.WriteLine(viewModel.NewTitle);

        // Output:
        // 3 left
        // Book electrician
        // Book electrician
    }

    /// <summary>Each time the screen is shown again, its view model is activated again and reloads what changed while it was away.</summary>
    /// <returns>A task that completes when the store has been changed behind the screen.</returns>
    public static async Task ReactivateToRefresh()
    {
        var store = InMemoryTodoStore.CreateSeeded();
        using TodoListViewModel viewModel = new(store);
        using TodoListView view = new() { ViewModel = viewModel };

        view.Show();
        Console.WriteLine(view.RemainingLabel.Text);
        view.Hide();

        _ = await store.AddAsync("Call the plumber", CancellationToken.None);

        view.Show();
        Console.WriteLine(view.RemainingLabel.Text);
        view.Hide();

        // Output:
        // 3 left
        // 4 left
    }
}
