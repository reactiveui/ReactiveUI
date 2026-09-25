// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.ViewModels;

/// <summary>Holds the item the user has selected, so a path can run through it.</summary>
[System.Diagnostics.DebuggerDisplay("Selected = {Selected}")]
public sealed class SelectionHolder : ReactiveObject
{
    /// <summary>Gets or sets the selected item.</summary>
    public TodoItem? Selected
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
