// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Commands;

/// <summary>
/// A library loans desk: it searches the catalogue, lends and renews books, takes every book back at closing, and
/// syncs its catalogue with the library network's central server.
/// </summary>
[System.Diagnostics.DebuggerDisplay("LoanCount = {LoanCount}")]
public sealed class LibraryDesk : ReactiveObject
{
    /// <summary>The books the desk knows about.</summary>
    private static readonly Book[] _catalogue =
    [
        new Book(1, "Clean Code", "Robert C. Martin"),
        new Book(2, "The Pragmatic Programmer", "Andrew Hunt"),
        new Book(3, "Design Patterns", "Erich Gamma"),
        new Book(4, "Refactoring", "Martin Fowler"),
    ];

    /// <summary>The clock the desk reads the current time from.</summary>
    private readonly TimeProvider _timeProvider = TimeProvider.System;

    /// <summary>The due date of every book on loan, keyed by book id.</summary>
    private readonly Dictionary<int, DateTimeOffset> _loans = [];

    /// <summary>The id of every book a renewal receipt was sent for.</summary>
    private readonly List<int> _renewalReceipts = [];

    /// <summary>Gets or sets a value indicating whether the desk is open for lending.</summary>
    public bool IsOpen
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    /// <summary>Gets the number of books currently on loan.</summary>
    public int LoanCount => _loans.Count;

    /// <summary>Gets the id of every book a renewal receipt was sent for, in the order they were sent.</summary>
    public IReadOnlyList<int> RenewalReceipts => _renewalReceipts;

    /// <summary>Finds every book whose title or author contains a piece of text.</summary>
    /// <param name="term">The text to look for.</param>
    /// <returns>The matching books.</returns>
    public IReadOnlyList<Book> Search(string term) =>
        [.. _catalogue.Where(book => book.Title.Contains(term, StringComparison.OrdinalIgnoreCase) || book.Author.Contains(term, StringComparison.OrdinalIgnoreCase))];

    /// <summary>Lends a book to a member for two weeks.</summary>
    /// <param name="bookId">The catalogue number of the book to lend.</param>
    /// <returns>The book that was lent.</returns>
    public Book Borrow(int bookId)
    {
        Book book = _catalogue.First(candidate => candidate.Id == bookId);
        _loans[bookId] = _timeProvider.GetUtcNow().AddDays(14);
        return book;
    }

    /// <summary>Takes every book on loan back, as the desk does at closing.</summary>
    /// <returns>The number of books returned.</returns>
    public int ReturnAll()
    {
        int returned = _loans.Count;
        _loans.Clear();
        return returned;
    }

    /// <summary>Downloads the latest catalogue from the library network's central server.</summary>
    /// <param name="cancellationToken">A token that cancels the sync.</param>
    /// <returns>The number of books the central catalogue holds.</returns>
    public async Task<int> SyncWithCentralCatalogueAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken).ConfigureAwait(false);
        return _catalogue.Length;
    }

    /// <summary>Refreshes the desk's local shelf cache from the library network, without reporting a count back.</summary>
    /// <param name="cancellationToken">A token that cancels the refresh.</param>
    /// <returns>A task that completes once the cache is refreshed.</returns>
    public Task RefreshLocalCacheAsync(CancellationToken cancellationToken) =>
        Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken);

    /// <summary>Takes every book on loan back, after writing the returns to the desk's ledger.</summary>
    /// <returns>A task that completes once every book has been returned.</returns>
    public async Task ReturnAllBooksAsync()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
        _ = ReturnAll();
    }

    /// <summary>Takes every book on loan back, after writing the returns to the desk's ledger.</summary>
    /// <returns>A task producing the number of books returned.</returns>
    public async Task<int> ReturnAllBooksWithCountAsync()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
        return ReturnAll();
    }

    /// <summary>Extends a loan by two more weeks, after writing the renewal to the desk's ledger.</summary>
    /// <param name="bookId">The catalogue number of the book on loan.</param>
    /// <returns>A task producing the new due date.</returns>
    public async Task<DateTimeOffset> RenewLoanAsync(int bookId)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
        DateTimeOffset dueDate = _loans[bookId].AddDays(14);
        _loans[bookId] = dueDate;
        return dueDate;
    }

    /// <summary>Sends a renewal receipt for a book on loan.</summary>
    /// <param name="bookId">The catalogue number of the book that was renewed.</param>
    /// <returns>A task that completes once the receipt has been sent.</returns>
    public async Task SendRenewalReceiptAsync(int bookId)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
        _renewalReceipts.Add(bookId);
    }

    /// <summary>Sends a renewal receipt for a book on loan.</summary>
    /// <param name="bookId">The catalogue number of the book that was renewed.</param>
    /// <param name="cancellationToken">A token that cancels sending the receipt.</param>
    /// <returns>A task that completes once the receipt has been sent.</returns>
    public async Task SendRenewalReceiptAsync(int bookId, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken).ConfigureAwait(false);
        _renewalReceipts.Add(bookId);
    }

    /// <summary>Lends a book to a member, after checking it in with the library network.</summary>
    /// <param name="bookId">The catalogue number of the book to lend.</param>
    /// <param name="cancellationToken">A token that cancels the check.</param>
    /// <returns>A task producing the book that was lent.</returns>
    public async Task<Book> BorrowAsync(int bookId, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken).ConfigureAwait(false);
        return Borrow(bookId);
    }
}
