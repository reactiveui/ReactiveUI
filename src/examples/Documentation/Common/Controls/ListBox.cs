// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Controls;

/// <summary>A console stand-in for a list the user can pick one row from.</summary>
/// <typeparam name="T">The type of the rows.</typeparam>
[System.Diagnostics.DebuggerDisplay("ListBox Items = {Items.Count}")]
public sealed class ListBox<T> : ReactiveObject
    where T : class
{
    /// <summary>Gets or sets the rows the list shows.</summary>
    public IReadOnlyList<T> Items
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];

    /// <summary>Gets or sets the picked row, or <see langword="null"/> when none is picked.</summary>
    public T? SelectedItem
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
