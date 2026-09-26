// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics;
using ReactiveUI.Winforms;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// The library desk: a <see cref="RoutedControlHost"/> navigates between the catalog and the loan form, and a
/// <see cref="ViewModelControlHost"/> shows the card of the member picked during a loan.
/// </summary>
[DebuggerDisplay("LibraryForm")]
public sealed class LibraryForm : Form
{
    /// <summary>The library's catalog. Loaning a book flips its <see cref="Book.IsOnLoan"/> flag in place.</summary>
    private readonly List<Book> _catalog =
    [
        new("Dune", "Frank Herbert"),
        new("Foundation", "Isaac Asimov"),
        new("Neuromancer", "William Gibson"),
    ];

    /// <summary>The members who can borrow a book.</summary>
    private readonly List<Member> _members = [new("M-1", "Ada"), new("M-2", "Grace")];

    /// <summary>The shell that owns the router <see cref="BooksHost"/> follows.</summary>
    private readonly LibraryShellViewModel _shell = new();

    /// <summary>Updates <see cref="MemberCardHost"/> whenever the loan form's selected member changes.</summary>
    private readonly IDisposable _memberCardSubscription;

    /// <summary>Initializes a new instance of the <see cref="LibraryForm"/> class.</summary>
    public LibraryForm()
    {
        Text = "Library Desk";
        ClientSize = new Size(760, 480);
        StartPosition = FormStartPosition.CenterScreen;

        // Subscribe before setting a single property on the host, so the first assignment below is itself observed.
        BooksHost.PropertyChanging += OnBooksHostPropertyChanging;
        BooksHost.PropertyChanged += OnBooksHostPropertyChanged;
        BooksHost.Router = _shell.Router;
        BooksHost.DefaultContent = new Label { Text = "Loading catalog...", AutoSize = true };
        BooksHost.ViewLocator = ViewLocator.GetCurrent();
        BooksHost.ViewContractObservable = Signal.Emit(string.Empty);

        MemberCardHost.PropertyChanging += OnMemberCardHostPropertyChanging;
        MemberCardHost.PropertyChanged += OnMemberCardHostPropertyChanged;
        MemberCardHost.CacheViews = true;
        MemberCardHost.DefaultContent = new Label { Text = "Pick a member during a loan to see their card.", AutoSize = true };

        TableLayoutPanel layout = new() { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        layout.Controls.Add(BooksHost, 0, 0);
        layout.Controls.Add(MemberCardHost, 1, 0);
        Controls.Add(layout);

        _memberCardSubscription = _shell.Router.CurrentViewModel
            .OfType<LoanFormViewModel>()
            .SelectMany(static loan => loan.WhenAnyValue(static x => x.SelectedMember))
            .Subscribe(member => MemberCardHost.ViewModel = member is null ? null : new MemberCardViewModel(member, CountBooksOnLoan()));

        _shell.Router.Navigate.Execute(new BookListViewModel(_shell, _catalog, _members)).Subscribe();
    }

    /// <summary>Gets the host that routes between the catalog and the loan form.</summary>
    internal RoutedControlHost BooksHost { get; } = new() { Dock = DockStyle.Fill, Name = "BooksHost" };

    /// <summary>Gets the host that shows the card of the member currently being lent a book.</summary>
    internal ViewModelControlHost MemberCardHost { get; } = new() { Dock = DockStyle.Fill, Name = "MemberCardHost" };

    /// <summary>Gets the name of the last property <see cref="BooksHost"/> reported changing.</summary>
    internal string? LastBooksHostPropertyChanging { get; private set; }

    /// <summary>Gets the name of the last property <see cref="BooksHost"/> reported changed.</summary>
    internal string? LastBooksHostPropertyChanged { get; private set; }

    /// <summary>Gets the name of the last property <see cref="MemberCardHost"/> reported changing.</summary>
    internal string? LastMemberCardHostPropertyChanging { get; private set; }

    /// <summary>Gets the name of the last property <see cref="MemberCardHost"/> reported changed.</summary>
    internal string? LastMemberCardHostPropertyChanged { get; private set; }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _memberCardSubscription.Dispose();
            BooksHost.Dispose();
            MemberCardHost.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>Counts how many books in the catalog are currently on loan.</summary>
    /// <returns>The number of books on loan.</returns>
    private int CountBooksOnLoan() => _catalog.Count(static book => book.IsOnLoan);

    /// <summary>Records the name of the property <see cref="BooksHost"/> is about to change.</summary>
    /// <param name="sender">The host raising the event.</param>
    /// <param name="e">The name of the property about to change.</param>
    private void OnBooksHostPropertyChanging(object? sender, PropertyChangingEventArgs e) => LastBooksHostPropertyChanging = e.PropertyName;

    /// <summary>Records the name of the property <see cref="BooksHost"/> just changed.</summary>
    /// <param name="sender">The host raising the event.</param>
    /// <param name="e">The name of the property that changed.</param>
    private void OnBooksHostPropertyChanged(object? sender, PropertyChangedEventArgs e) => LastBooksHostPropertyChanged = e.PropertyName;

    /// <summary>Records the name of the property <see cref="MemberCardHost"/> is about to change.</summary>
    /// <param name="sender">The host raising the event.</param>
    /// <param name="e">The name of the property about to change.</param>
    private void OnMemberCardHostPropertyChanging(object? sender, PropertyChangingEventArgs e) => LastMemberCardHostPropertyChanging = e.PropertyName;

    /// <summary>Records the name of the property <see cref="MemberCardHost"/> just changed.</summary>
    /// <param name="sender">The host raising the event.</param>
    /// <param name="e">The name of the property that changed.</param>
    private void OnMemberCardHostPropertyChanged(object? sender, PropertyChangedEventArgs e) => LastMemberCardHostPropertyChanged = e.PropertyName;
}
