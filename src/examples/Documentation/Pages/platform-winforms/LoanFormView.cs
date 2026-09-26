// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using ReactiveUI.Winforms;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>The loan page: the book being loaned, a table of member buttons, and confirm/cancel buttons.</summary>
[DebuggerDisplay("LoanFormView")]
public sealed class LoanFormView : ReactiveUserControl<LoanFormViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="LoanFormView"/> class.</summary>
    public LoanFormView()
    {
        TableLayoutPanel layout = new() { Dock = DockStyle.Fill, RowCount = 5, ColumnCount = 1 };
        layout.Controls.Add(BookLabel, 0, 0);
        layout.Controls.Add(MembersTable, 0, 1);
        layout.Controls.Add(SelectedMemberLabel, 0, 2);
        layout.Controls.Add(ConfirmButton, 0, 3);
        layout.Controls.Add(CancelButton, 0, 4);
        Controls.Add(layout);

        _ = this.WhenActivated(d =>
        {
            BookLabel.Text = $"Loaning: {ViewModel!.Book.Title}";

            _ = ViewModel!.WhenAnyValue(static vm => vm.Members)
                .Select(CreateMemberButtons)
                .BindTo(this, static v => v.MembersTable.Controls)
                .DisposeWith(d);

            _ = this.OneWayBind(
                    ViewModel,
                    static vm => vm.SelectedMember,
                    static v => v.SelectedMemberLabel.Text,
                    static member => member is null ? "(no member selected)" : $"To: {member.Name}")
                .DisposeWith(d);

            _ = this.BindCommand(ViewModel, static vm => vm.ConfirmLoan, static v => v.ConfirmButton)
                .DisposeWith(d);

            _ = this.BindCommand(ViewModel, static vm => vm.Cancel, static v => v.CancelButton)
                .DisposeWith(d);
        });
    }

    /// <summary>Gets the label that names the book being loaned.</summary>
    internal Label BookLabel { get; } = new() { AutoSize = true, Name = "BookLabel" };

    /// <summary>Gets the table that hosts one button per member.</summary>
    internal TableLayoutPanel MembersTable { get; } = new() { Dock = DockStyle.Fill, AutoScroll = true, Name = "MembersTable" };

    /// <summary>Gets the label that shows the member picked from <see cref="MembersTable"/>.</summary>
    internal Label SelectedMemberLabel { get; } = new() { AutoSize = true, Name = "SelectedMemberLabel" };

    /// <summary>Gets the button that confirms the loan.</summary>
    internal Button ConfirmButton { get; } = new() { Text = "Confirm loan", AutoSize = true, Name = "ConfirmButton" };

    /// <summary>Gets the button that cancels the loan and returns to the catalog.</summary>
    internal Button CancelButton { get; } = new() { Text = "Cancel", AutoSize = true, Name = "CancelButton" };

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            BookLabel.Dispose();
            MembersTable.Dispose();
            SelectedMemberLabel.Dispose();
            ConfirmButton.Dispose();
            CancelButton.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Builds one <see cref="Button"/> per member. The buttons belong to this view: once they are in
    /// <see cref="MembersTable"/>, disposing the view disposes them with the table. A click hands the member to the
    /// view model, which holds data only.
    /// </summary>
    /// <param name="members">The members the view model exposes.</param>
    /// <returns>A button per member.</returns>
    private List<Button> CreateMemberButtons(IReadOnlyList<Member> members)
    {
        List<Button> buttons = [];
        foreach (Member member in members)
        {
            Button button = new() { Text = member.Name, AutoSize = true, Tag = member };
            button.Click += (_, _) =>
            {
                if (ViewModel is null)
                {
                    return;
                }

                ViewModel.SelectedMember = member;
            };
            buttons.Add(button);
        }

        return buttons;
    }
}
