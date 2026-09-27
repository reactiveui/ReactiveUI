// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The catalog page: every book the library owns, plus the members who can borrow one.</summary>
[System.Diagnostics.DebuggerDisplay("BookCatalogViewModel Books = {Books.Count}")]
public sealed class BookCatalogViewModel : ReactiveObject, IRoutableViewModel, IDisposable
{
    /// <summary>The members who can borrow a book, carried forward to the loan page.</summary>
    private readonly IReadOnlyList<Member> _members;

    /// <summary>Initializes a new instance of the <see cref="BookCatalogViewModel"/> class.</summary>
    /// <param name="hostScreen">The shell that owns the router.</param>
    /// <param name="books">The library's catalog.</param>
    /// <param name="members">The members who can borrow a book.</param>
    public BookCatalogViewModel(IScreen hostScreen, IReadOnlyList<Book> books, IReadOnlyList<Member> members)
    {
        HostScreen = hostScreen;
        Books = books as ObservableCollection<Book> ?? new ObservableCollection<Book>(books);
        _members = members;

        IObservable<bool> canLoan = this.WhenAnyValue(
            static x => x.SelectedBook,
            static book => book is not null && !book.IsOnLoan);

        OpenLoan = ReactiveCommand.CreateFromObservable(
            () => HostScreen.Router.Navigate.Execute(new LoanViewModel(HostScreen, SelectedBook!, _members)),
            canLoan);
    }

    /// <inheritdoc/>
    public string UrlPathSegment => "catalog";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the library's catalog. A reactive table or collection source binds to this directly: because it
    /// implements <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>, the source refreshes the
    /// view whenever a book is added or removed.</summary>
    public ObservableCollection<Book> Books { get; }

    /// <summary>Gets or sets the book the member picked from the catalog.</summary>
    public Book? SelectedBook
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the command that opens the loan page for <see cref="SelectedBook"/>.</summary>
    public ReactiveCommand<RxVoid, IRoutableViewModel> OpenLoan { get; }

    /// <inheritdoc/>
    public void Dispose() => OpenLoan.Dispose();
}
