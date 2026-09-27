// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>Confirms a book's return at the library desk, including whether it came back damaged.</summary>
/// <param name="bookTitle">The title of the book being returned.</param>
[System.Diagnostics.DebuggerDisplay("BookReturnViewModel BookTitle = {BookTitle}")]
public sealed class BookReturnViewModel(string bookTitle) : ReactiveObject
{
    /// <summary>Gets the title of the book being returned.</summary>
    public string BookTitle => bookTitle;

    /// <summary>Gets or sets a value indicating whether the returned book is damaged.</summary>
    public bool IsDamaged
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
