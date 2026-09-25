// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.Routing;

/// <summary>The page that shows one to-do item.</summary>
/// <param name="hostScreen">The window the page is shown in.</param>
/// <param name="item">The item the page shows.</param>
[System.Diagnostics.DebuggerDisplay("{UrlPathSegment}")]
public sealed class TodoDetailPage(IScreen hostScreen, TodoItem item) : ReactiveObject, IRoutableViewModel
{
    /// <inheritdoc/>
    public string UrlPathSegment => $"todos/{Item.Id}";

    /// <inheritdoc/>
    public IScreen HostScreen { get; } = hostScreen;

    /// <summary>Gets the item the page shows.</summary>
    public TodoItem Item { get; } = item;
}
