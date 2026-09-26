// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Blazor;
using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.PlatformBlazor;

/// <summary>Shows <see cref="TodoShellLayoutComponent"/>, a layout built on <see cref="ReactiveLayoutComponentBase{T}"/>.</summary>
public static class LayoutComponentBaseExamples
{
    /// <summary>
    /// Blazor sets a <c>[Parameter]</c> through the framework, not a direct assignment, so this example goes through the
    /// explicit <see cref="IViewFor"/> view; setting it still raises <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>.
    /// </summary>
    public static void SetTheViewModelParameter()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        using TodoShellLayoutComponent shell = new();
        List<string?> changes = [];
        shell.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

        IViewFor shellAsView = shell;
        shellAsView.ViewModel = viewModel;

        Console.WriteLine(ReferenceEquals(shell.ViewModel, viewModel));
        Console.WriteLine(string.Join(", ", changes));

        // Output:
        // True
        // ViewModel
    }

    /// <summary>
    /// Initializing the layout fires <see cref="ReactiveLayoutComponentBase{T}.Activated"/>; disposing it fires
    /// <see cref="ReactiveLayoutComponentBase{T}.Deactivated"/> before the layout's own resources go away.
    /// </summary>
    public static void ActivateAndDeactivateTheComponent()
    {
        TodoShellLayoutComponent shell = new();
        List<string> events = [];
        using IDisposable activatedSubscription = shell.Activated.Subscribe(_ => events.Add("activated"));
        using IDisposable deactivatedSubscription = shell.Deactivated.Subscribe(_ => events.Add("deactivated"));

        shell.Initialize();
        shell.Dispose();

        Console.WriteLine(string.Join(", ", events));

        // Output:
        // activated, deactivated
    }

    /// <summary>A layout calls <c>OnPropertyChanged</c> itself to announce a change to a value it computes.</summary>
    public static void RaiseAPropertyChangeExplicitly()
    {
        using TodoShellLayoutComponent shell = new();
        List<string?> changes = [];
        shell.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

        shell.RaiseManualChange();

        Console.WriteLine(changes[0]);

        // Output:
        // ViewModel
    }
}
