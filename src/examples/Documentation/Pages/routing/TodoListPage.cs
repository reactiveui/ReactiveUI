// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.Routing;

/// <summary>The page that lists the to-do items. Opening an item navigates to its detail page.</summary>
[System.Diagnostics.DebuggerDisplay("{UrlPathSegment}")]
public sealed class TodoListPage : ReactiveObject, IRoutableViewModel
{
    /// <summary>Initializes a new instance of the <see cref="TodoListPage"/> class.</summary>
    /// <param name="hostScreen">The window the page is shown in.</param>
    /// <param name="items">The items the page lists.</param>
    public TodoListPage(IScreen hostScreen, IReadOnlyList<TodoItem> items)
    {
        HostScreen = hostScreen;
        Items = items;
        Open = ReactiveCommand.CreateFromObservable<TodoItem, IRoutableViewModel>(
            item => HostScreen.Router.Navigate.Execute(new TodoDetailPage(HostScreen, item)));
    }

    /// <inheritdoc/>
    public string UrlPathSegment => "todos";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the items the page lists.</summary>
    public IReadOnlyList<TodoItem> Items { get; }

    /// <summary>Gets the command that opens an item's detail page.</summary>
    public ReactiveCommand<TodoItem, IRoutableViewModel> Open { get; }
}
