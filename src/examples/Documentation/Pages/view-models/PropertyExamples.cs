// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.ViewModels;

/// <summary>Shows how a <see cref="ReactiveObject"/> raises changes and how <c>WhenAnyValue</c> observes them.</summary>
public static class PropertyExamples
{
    /// <summary>The title the user types into the new-item box.</summary>
    private const string GroceriesTitle = "Buy groceries";

    /// <summary>A second title the user types.</summary>
    private const string DentistTitle = "Book dentist appointment";

    /// <summary><c>RaiseAndSetIfChanged</c> raises <c>PropertyChanged</c> only when the value really changes.</summary>
    public static void RaiseOnlyWhenTheValueChanges()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        int raised = 0;
        viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(TodoListViewModel.NewTitle))
            {
                return;
            }

            raised++;
        };

        viewModel.NewTitle = GroceriesTitle;
        viewModel.NewTitle = GroceriesTitle;
        viewModel.NewTitle = DentistTitle;

        Console.WriteLine(raised);

        // Output:
        // 2
    }

    /// <summary><c>WhenAnyValue</c> starts with the current value, then reports each change.</summary>
    public static void ObserveOneProperty()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());

        using IDisposable subscription = viewModel.WhenAnyValue(x => x.NewTitle)
            .Subscribe(static title => Console.WriteLine($"[{title}]"));

        viewModel.NewTitle = GroceriesTitle;
        viewModel.NewTitle = DentistTitle;

        // Output:
        // []
        // [Buy groceries]
        // [Book dentist appointment]
    }

    /// <summary>
    /// Skipping the first value reports changes only, without the current value. <c>ObservableForProperty</c> does the
    /// same by reflection, so it is not safe to trim; this form is generated at compile time.
    /// </summary>
    public static void ObserveChangesOnly()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());

        using IDisposable subscription = viewModel.WhenAnyValue(x => x.NewTitle)
            .Skip(1)
            .Subscribe(Console.WriteLine);

        viewModel.NewTitle = GroceriesTitle;

        // Output:
        // Buy groceries
    }

    /// <summary>Combining two properties recomputes whenever either changes, here to show a hint under the add box.</summary>
    public static void CombineTwoProperties()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());

        using IDisposable subscription = viewModel.WhenAnyValue(
                x => x.NewTitle,
                x => x.FilterText,
                static (title, filter) => title.Length > 0 && filter.Length > 0
                    ? "Clear the filter to see the new item"
                    : "Ready")
            .Subscribe(Console.WriteLine);

        viewModel.NewTitle = GroceriesTitle;
        viewModel.FilterText = "bill";
        viewModel.FilterText = string.Empty;

        // Output:
        // Ready
        // Ready
        // Clear the filter to see the new item
        // Ready
    }

    /// <summary>A path through another object follows both: replacing the outer object and changing the inner property both report.</summary>
    public static void ObserveNestedProperty()
    {
        TodoItem groceries = new() { Title = GroceriesTitle };
        TodoItem dentist = new() { Title = DentistTitle };
        SelectionHolder holder = new() { Selected = groceries };

        using IDisposable subscription = holder.WhenAnyValue(x => x.Selected!.Title)
            .Subscribe(Console.WriteLine);

        groceries.Title = "Buy groceries and milk";
        holder.Selected = dentist;

        // Output:
        // Buy groceries
        // Buy groceries and milk
        // Book dentist appointment
    }
}
