// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Todo;

/// <summary>One task on the to-do list.</summary>
[System.Diagnostics.DebuggerDisplay("{Id}: {Title}, IsDone = {IsDone}")]
public sealed class TodoItem : ReactiveObject
{
    /// <summary>Gets or sets the identifier the store assigned; zero until the item is stored.</summary>
    public int Id
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the short summary of the task.</summary>
    public string Title
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets or sets a value indicating whether the task is finished.</summary>
    public bool IsDone
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Creates an independent copy, as a database returns a new row each time it is read.</summary>
    /// <returns>A copy with the same values.</returns>
    public TodoItem Clone() => new() { Id = Id, Title = Title, IsDone = IsDone };
}
