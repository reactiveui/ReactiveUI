// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using ReactiveUI.Winforms;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>The catalog page: a list box of books, a card per book, and a button that starts a loan.</summary>
[DebuggerDisplay("BookListView")]
public sealed class BookListView : ReactiveUserControl<BookListViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="BookListView"/> class.</summary>
    public BookListView()
    {
        TableLayoutPanel layout = new() { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(BooksListBox, 0, 0);
        layout.Controls.Add(BooksPanel, 0, 1);
        layout.Controls.Add(LoanButton, 0, 2);
        Controls.Add(layout);

        BooksListBox.DisplayMember = nameof(Book.Title);
        BooksListBox.SelectedIndexChanged += (_, _) =>
        {
            if (ViewModel is null)
            {
                return;
            }

            ViewModel.SelectedBook = BooksListBox.SelectedItem as Book;
        };

        _ = this.WhenActivated(d =>
        {
            BooksListBox.Items.Clear();
            BooksListBox.Items.AddRange(ViewModel!.Books.Cast<object>().ToArray());

            _ = this.OneWayBind(ViewModel, static vm => vm.BookCards, static v => v.BooksPanel.Controls)
                .DisposeWith(d);

            _ = this.BindCommand(ViewModel, static vm => vm.LoanSelectedBook, static v => v.LoanButton)
                .DisposeWith(d);
        });
    }

    /// <summary>Gets the list box that shows every book in the catalog.</summary>
    internal ListBox BooksListBox { get; } = new() { Dock = DockStyle.Fill, Name = "BooksListBox" };

    /// <summary>Gets the panel that hosts one read-only card per book.</summary>
    internal Panel BooksPanel { get; } = new() { Dock = DockStyle.Fill, AutoScroll = true, Name = "BooksPanel" };

    /// <summary>Gets the button that starts a loan for <see cref="BooksListBox"/>'s selected book.</summary>
    internal Button LoanButton { get; } = new() { Text = "Loan selected book", Dock = DockStyle.Fill, Name = "LoanButton" };

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            BooksListBox.Dispose();
            BooksPanel.Dispose();
            LoanButton.Dispose();
        }

        base.Dispose(disposing);
    }
}
