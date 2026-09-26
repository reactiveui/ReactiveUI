// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>The catalog page: a list of books, plus a read-only card per book for the panel.</summary>
[DebuggerDisplay("BookListViewModel Books = {Books.Count}")]
public sealed class BookListViewModel : ReactiveObject, IRoutableViewModel, IDisposable
{
    /// <summary>The members who can borrow a book, carried forward to the loan form.</summary>
    private readonly IReadOnlyList<Member> _members;

    /// <summary>Initializes a new instance of the <see cref="BookListViewModel"/> class.</summary>
    /// <param name="hostScreen">The shell that owns the router.</param>
    /// <param name="books">The library's catalog.</param>
    /// <param name="members">The members who can borrow a book.</param>
    public BookListViewModel(IScreen hostScreen, IReadOnlyList<Book> books, IReadOnlyList<Member> members)
    {
        HostScreen = hostScreen;
        Books = books;
        _members = members;

        List<Label> cards = [];
        foreach (Book book in books)
        {
            string status = book.IsOnLoan ? " (on loan)" : string.Empty;
            cards.Add(new Label { Text = $"{book.Title} — {book.Author}{status}", AutoSize = true });
        }

        BookCards = cards;

        IObservable<bool> canLoan = this.WhenAnyValue(
            static x => x.SelectedBook,
            static book => book is not null && !book.IsOnLoan);

        LoanSelectedBook = ReactiveCommand.CreateFromObservable(
            () => HostScreen.Router.Navigate.Execute(new LoanFormViewModel(HostScreen, SelectedBook!, _members)),
            canLoan);
    }

    /// <inheritdoc/>
    public string UrlPathSegment => "books";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the library's catalog.</summary>
    public IReadOnlyList<Book> Books { get; }

    /// <summary>Gets a read-only <see cref="Label"/> per book, bound into a <see cref="Panel"/>.</summary>
    public IReadOnlyList<Label> BookCards { get; }

    /// <summary>Gets or sets the book the member picked from the list.</summary>
    public Book? SelectedBook
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the command that opens the loan form for <see cref="SelectedBook"/>.</summary>
    public ReactiveCommand<RxVoid, IRoutableViewModel> LoanSelectedBook { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        LoanSelectedBook.Dispose();
        foreach (Label card in BookCards)
        {
            card.Dispose();
        }
    }
}
