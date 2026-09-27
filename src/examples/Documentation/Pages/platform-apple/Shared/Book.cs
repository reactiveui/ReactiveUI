// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>A book the library owns. Bound labels and controls refresh whenever <see cref="IsOnLoan"/> or <see cref="Rating"/> changes.</summary>
[System.Diagnostics.DebuggerDisplay("Book {Title} IsOnLoan = {IsOnLoan}")]
public sealed class Book : ReactiveObject
{
    /// <summary>Initializes a new instance of the <see cref="Book"/> class.</summary>
    /// <param name="title">The book's title.</param>
    /// <param name="author">The book's author.</param>
    public Book(string title, string author)
    {
        Title = title;
        Author = author;
    }

    /// <summary>Gets the book's title.</summary>
    public string Title { get; }

    /// <summary>Gets the book's author.</summary>
    public string Author { get; }

    /// <summary>Gets or sets a value indicating whether a member currently has the book on loan.</summary>
    public bool IsOnLoan
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the book's star rating, from 0 to 5.</summary>
    public int Rating
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
