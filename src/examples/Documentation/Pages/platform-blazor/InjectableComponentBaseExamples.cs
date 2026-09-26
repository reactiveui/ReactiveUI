// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Blazor;
using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.PlatformBlazor;

/// <summary>Shows <see cref="InjectedTodoComponent"/>, built on <see cref="ReactiveInjectableComponentBase{T}"/>.</summary>
public static class InjectableComponentBaseExamples
{
    /// <summary>
    /// A DI container assigns the injected <c>ViewModel</c> the same way any property setter is called; the assignment
    /// still raises <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/> and is visible through
    /// the explicit <see cref="IViewFor"/> view.
    /// </summary>
    public static void SetTheInjectedViewModel()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        using InjectedTodoComponent page = new();
        List<string?> changes = [];
        page.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

        page.ViewModel = viewModel;

        IViewFor pageAsView = page;
        Console.WriteLine(ReferenceEquals(pageAsView.ViewModel, viewModel));
        Console.WriteLine(string.Join(", ", changes));

        // Output:
        // True
        // ViewModel
    }

    /// <summary>
    /// Initializing the component fires <see cref="ReactiveInjectableComponentBase{T}.Activated"/>; disposing it fires
    /// <see cref="ReactiveInjectableComponentBase{T}.Deactivated"/> before the component's own resources go away.
    /// </summary>
    public static void ActivateAndDeactivateTheComponent()
    {
        InjectedTodoComponent page = new();
        List<string> events = [];
        using IDisposable activatedSubscription = page.Activated.Subscribe(_ => events.Add("activated"));
        using IDisposable deactivatedSubscription = page.Deactivated.Subscribe(_ => events.Add("deactivated"));

        page.Initialize();
        page.Dispose();

        Console.WriteLine(string.Join(", ", events));

        // Output:
        // activated, deactivated
    }

    /// <summary>A component calls <c>OnPropertyChanged</c> itself to announce a change to a value it computes.</summary>
    public static void RaiseAPropertyChangeExplicitly()
    {
        using InjectedTodoComponent page = new();
        List<string?> changes = [];
        page.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

        page.RaiseManualChange();

        Console.WriteLine(changes[0]);

        // Output:
        // ViewModel
    }
}
