// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Blazor;
using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.PlatformBlazor;

/// <summary>Shows <see cref="TodoScopedComponent"/>, built on <see cref="ReactiveOwningComponentBase{T}"/>.</summary>
public static class OwningComponentBaseExamples
{
    /// <summary>
    /// Blazor sets a <c>[Parameter]</c> through the framework, not a direct assignment, so this example goes through the
    /// explicit <see cref="IViewFor"/> view; setting it still raises <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>.
    /// </summary>
    public static void SetTheViewModelParameter()
    {
        using TodoListViewModel viewModel = new(InMemoryTodoStore.CreateSeeded());
        TodoScopedComponent component = new();
        List<string?> changes = [];
        component.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

        IViewFor componentAsView = component;
        componentAsView.ViewModel = viewModel;

        Console.WriteLine(ReferenceEquals(component.ViewModel, viewModel));
        Console.WriteLine(string.Join(", ", changes));

        ((IDisposable)component).Dispose();

        // Output:
        // True
        // ViewModel
    }

    /// <summary>
    /// Initializing the component fires <see cref="ReactiveOwningComponentBase{T}.Activated"/>; disposing the owning
    /// base class's scope fires <see cref="ReactiveOwningComponentBase{T}.Deactivated"/> first.
    /// </summary>
    public static void ActivateAndDeactivateTheComponent()
    {
        TodoScopedComponent component = new();
        List<string> events = [];
        using IDisposable activatedSubscription = component.Activated.Subscribe(_ => events.Add("activated"));
        using IDisposable deactivatedSubscription = component.Deactivated.Subscribe(_ => events.Add("deactivated"));

        component.Initialize();
        ((IDisposable)component).Dispose();

        Console.WriteLine(string.Join(", ", events));

        // Output:
        // activated, deactivated
    }

    /// <summary>A component calls <c>OnPropertyChanged</c> itself to announce a change to a value it computes.</summary>
    public static void RaiseAPropertyChangeExplicitly()
    {
        TodoScopedComponent component = new();
        List<string?> changes = [];
        component.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

        component.RaiseManualChange();

        Console.WriteLine(changes[0]);

        ((IDisposable)component).Dispose();

        // Output:
        // ViewModel
    }
}
