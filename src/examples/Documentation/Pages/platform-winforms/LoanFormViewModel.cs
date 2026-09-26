// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>The loan page: pick a member for <see cref="Book"/>, then confirm or cancel.</summary>
[DebuggerDisplay("LoanFormViewModel Book = {Book.Title}")]
public sealed class LoanFormViewModel : ReactiveObject, IRoutableViewModel, IDisposable
{
    /// <summary>Initializes a new instance of the <see cref="LoanFormViewModel"/> class.</summary>
    /// <param name="hostScreen">The shell that owns the router.</param>
    /// <param name="book">The book being loaned.</param>
    /// <param name="members">The members who can borrow the book.</param>
    public LoanFormViewModel(IScreen hostScreen, Book book, IReadOnlyList<Member> members)
    {
        HostScreen = hostScreen;
        Book = book;

        List<Button> buttons = [];
        foreach (Member member in members)
        {
            Button button = new() { Text = member.Name, AutoSize = true, Tag = member };
            button.Click += (_, _) => SelectedMember = member;
            buttons.Add(button);
        }

        MemberButtons = buttons;

        IObservable<bool> canConfirm = this.WhenAnyValue(static x => x.SelectedMember).Select(static member => member is not null);

        ConfirmLoan = ReactiveCommand.CreateFromObservable(
            () =>
            {
                Book.IsOnLoan = true;
                return HostScreen.Router.NavigateBack.Execute();
            },
            canConfirm);

        Cancel = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.NavigateBack.Execute());
    }

    /// <inheritdoc/>
    public string UrlPathSegment => "loan";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the book being loaned.</summary>
    public Book Book { get; }

    /// <summary>Gets a <see cref="Button"/> per member, bound into a <see cref="TableLayoutPanel"/>.</summary>
    public IReadOnlyList<Button> MemberButtons { get; }

    /// <summary>Gets or sets the member the librarian picked by clicking one of the <see cref="MemberButtons"/>.</summary>
    public Member? SelectedMember
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the command that marks the book on loan and returns to the catalog.</summary>
    public ReactiveCommand<RxVoid, IRoutableViewModel> ConfirmLoan { get; }

    /// <summary>Gets the command that returns to the catalog without loaning the book.</summary>
    public ReactiveCommand<RxVoid, IRoutableViewModel> Cancel { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        ConfirmLoan.Dispose();
        Cancel.Dispose();
        foreach (Button button in MemberButtons)
        {
            button.Dispose();
        }
    }
}
