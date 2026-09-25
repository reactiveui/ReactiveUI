// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.Interactions;

/// <summary>
/// Shows <see cref="Interaction{TInput, TOutput}"/>: the view model asks a question, such as "delete this item?", and the view
/// answers it, without the view model knowing how the question is put to the user.
/// </summary>
public static class InteractionExamples
{
    /// <summary>The view answers the delete confirmation; a "yes" deletes the item, a "no" keeps it.</summary>
    /// <returns>A task that completes when both deletes have been answered.</returns>
    public static async Task ConfirmADelete()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        _ = await viewModel.Load.Execute();
        var answer = false;

        using var handler = viewModel.ConfirmDelete.RegisterHandler(context =>
        {
            Console.WriteLine($"Delete '{context.Input.Title}'?");
            context.SetOutput(answer);
        });

        Console.WriteLine(await viewModel.Delete.Execute(viewModel.Items[0]));

        answer = true;
        Console.WriteLine(await viewModel.Delete.Execute(viewModel.Items[0]));
        Console.WriteLine(viewModel.Items.Count);

        // Output:
        // Delete 'Buy groceries'?
        // False
        // Delete 'Buy groceries'?
        // True
        // 3
    }

    /// <summary>
    /// Handlers run newest first. A screen can register a handler over an app-wide one; a handler that does not answer
    /// passes the question to the one registered before it.
    /// </summary>
    /// <returns>A task that completes when both deletes have been answered.</returns>
    public static async Task LetTheNewestHandlerAnswer()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        _ = await viewModel.Load.Execute();

        using var appWide = viewModel.ConfirmDelete.RegisterHandler(static context =>
        {
            Console.WriteLine("App-wide dialog");
            context.SetOutput(true);
        });
        using var finishedItemsOnly = viewModel.ConfirmDelete.RegisterHandler(static context =>
        {
            if (!context.Input.IsDone)
            {
                return;
            }

            Console.WriteLine("Finished items are deleted without asking");
            context.SetOutput(true);
        });

        var billPaid = viewModel.Items[1];
        _ = await viewModel.Delete.Execute(billPaid);
        _ = await viewModel.Delete.Execute(viewModel.Items[0]);

        // Output:
        // Finished items are deleted without asking
        // App-wide dialog
    }

    /// <summary>A question nobody answers fails the command with <c>UnhandledInteractionException</c>, so a missing dialog is found in testing.</summary>
    /// <returns>A task that completes when the delete has failed.</returns>
    public static async Task FailWhenNobodyAnswers()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        _ = await viewModel.Load.Execute();

        try
        {
            _ = await viewModel.Delete.Execute(viewModel.Items[0]);
        }
        catch (UnhandledInteractionException<TodoItem, bool> ex)
        {
            Console.WriteLine(ex.Input.Title);
        }

        Console.WriteLine(viewModel.Items.Count);

        // Output:
        // Buy groceries
        // 4
    }

    /// <summary>
    /// <c>BindInteraction</c> registers the view's handler on whatever interaction the current view model holds, and moves
    /// it when the view is given another view model.
    /// </summary>
    /// <returns>A task that completes when the delete has been answered.</returns>
    public static async Task BindTheViewsHandler()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        _ = await viewModel.Load.Execute();
        using TodoListView view = new() { ViewModel = viewModel };

        using var binding = view.BindInteraction(viewModel, x => x.ConfirmDelete, static context =>
        {
            Console.WriteLine($"The view asks: delete '{context.Input.Title}'?");
            context.SetOutput(true);
            return Task.CompletedTask;
        });

        _ = await viewModel.Delete.Execute(viewModel.Items[0]);
        Console.WriteLine(viewModel.Items.Count);

        // Output:
        // The view asks: delete 'Buy groceries'?
        // 3
    }
}
