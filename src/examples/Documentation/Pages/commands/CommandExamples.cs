// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.Commands;

/// <summary>Shows how a <see cref="ReactiveCommand"/> is created, when it may run, how it reports errors and how commands combine.</summary>
public static class CommandExamples
{
    /// <summary>The title the user types into the new-item box.</summary>
    private const string ElectricianTitle = "Book electrician";

    /// <summary>A synchronous command runs its action in place; here it clears the filter box.</summary>
    /// <returns>A task that completes when the command has run.</returns>
    public static async Task RunSynchronousCommand()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        viewModel.FilterText = "bill";
        using ReactiveCommand<RxVoid, string>? clearFilter = ReactiveCommand.Create(() => viewModel.FilterText = string.Empty);

        _ = await clearFilter.Execute();

        Console.WriteLine($"[{viewModel.FilterText}]");

        // Output:
        // []
    }

    /// <summary>An asynchronous command runs a task and hands back its result; here it reads the store.</summary>
    /// <returns>A task that completes when the items are loaded.</returns>
    public static async Task RunAsynchronousCommand()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());

        IReadOnlyList<TodoItem> rows = await viewModel.Load.Execute();

        Console.WriteLine(rows.Count);
        Console.WriteLine(viewModel.RemainingCount);

        // Output:
        // 4
        // 3
    }

    /// <summary>The add command may run only while the title is not blank; <c>CanExecute</c> follows the title.</summary>
    public static void ControlExecutability()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        using IDisposable subscription = viewModel.Add.CanExecute.Subscribe(Console.WriteLine);

        viewModel.NewTitle = ElectricianTitle;
        viewModel.NewTitle = "   ";

        // Output:
        // False
        // True
        // False
    }

    /// <summary>A failed execution goes to <c>ThrownExceptions</c>, which the view model turns into a message for the screen.</summary>
    /// <returns>A task that completes when the refused write has been reported.</returns>
    public static async Task HandleErrors()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        _ = await viewModel.Load.Execute();
        viewModel.NewTitle = "Buy groceries";

        try
        {
            _ = await viewModel.Add.Execute();
        }
        catch (TodoStoreException)
        {
            // The awaiting caller sees the error too; the screen shows it through ErrorMessage.
        }

        Console.WriteLine(viewModel.ErrorMessage);
        Console.WriteLine(viewModel.Items.Count);

        // Output:
        // 'Buy groceries' is already on the list.
        // 4
    }

    /// <summary><c>InvokeCommand</c> runs a command from an observable pipeline; here the list loads when the screen starts.</summary>
    /// <returns>A task that completes when the items are loaded.</returns>
    public static async Task InvokeFromPipeline()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        Task<IReadOnlyList<TodoItem>> loaded = viewModel.Load.FirstAsync().ToTask();

        using IDisposable subscription = Signal.Emit(RxVoid.Default).InvokeCommand(viewModel.Load);
        _ = await loaded;

        Console.WriteLine(viewModel.Items.Count);

        // Output:
        // 4
    }

    /// <summary>A combined command runs several commands together; here one refresh reloads the home and the work lists.</summary>
    /// <returns>A task that completes when both lists are loaded.</returns>
    public static async Task CombineCommands()
    {
        using TodoListViewModel home = new(InMemoryTodoStore.CreateSeeded());
        InMemoryTodoStore workStore = new();
        _ = await workStore.AddAsync("Send the quarterly report", CancellationToken.None);
        using TodoListViewModel work = new(workStore);

        using CombinedReactiveCommand<RxVoid, IReadOnlyList<TodoItem>>? refreshAll = ReactiveCommand.CreateCombined([home.Load, work.Load]);
        IList<IReadOnlyList<TodoItem>> results = await refreshAll.Execute();

        Console.WriteLine(results.Count);
        Console.WriteLine(home.Items.Count);
        Console.WriteLine(work.Items[0].Title);

        // Output:
        // 2
        // 4
        // Send the quarterly report
    }
}
