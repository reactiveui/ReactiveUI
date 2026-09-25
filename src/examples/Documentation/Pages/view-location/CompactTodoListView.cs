// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>A one-line summary of the to-do list for a narrow window or a home-screen widget, found by its contract.</summary>
[ViewContract(ViewContracts.Compact)]
[System.Diagnostics.DebuggerDisplay("CompactTodoListView ViewModel = {ViewModel}")]
public sealed class CompactTodoListView : ReactiveObject, IViewFor<TodoListViewModel>
{
    /// <summary>Gets or sets the view model the summary shows.</summary>
    public TodoListViewModel? ViewModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TodoListViewModel?)value;
    }
}
