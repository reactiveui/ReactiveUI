// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AppKit;
using CoreGraphics;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The catalog page: one row per book with a "Select" button, and a button that loans the selected book.</summary>
[System.Diagnostics.DebuggerDisplay("BookListViewController")]
public sealed class BookListViewController : ReactiveViewController<BookCatalogViewModel>
{
    /// <summary>The stack that hosts one row per book. A real catalog would use a reactive table source instead.</summary>
    private readonly NSStackView _rows = new() { Orientation = NSUserInterfaceLayoutOrientation.Vertical, Spacing = 8, TranslatesAutoresizingMaskIntoConstraints = false };

    /// <summary>Gets the button that opens the loan page for the selected book. Internal for the binding source generator.</summary>
    internal NSButton LoanButton { get; } = new() { Title = "Loan selected book", BezelStyle = NSBezelStyle.Rounded };

    /// <inheritdoc/>
    public override void LoadView()
    {
        NSView view = new();
        View = view;

        NSStackView layout = new() { Orientation = NSUserInterfaceLayoutOrientation.Vertical, Spacing = 16, TranslatesAutoresizingMaskIntoConstraints = false };
        layout.AddArrangedSubview(_rows);
        layout.AddArrangedSubview(LoanButton);
        view.AddSubview(layout);
        NSLayoutConstraint.ActivateConstraints(
        [
            layout.LeadingAnchor.ConstraintEqualTo(view.LeadingAnchor, 16),
            layout.TopAnchor.ConstraintEqualTo(view.TopAnchor, 16),
        ]);

        _ = this.WhenActivated(d =>
        {
            foreach (Book book in ViewModel!.Books)
            {
                // The frame constructor pre-sizes the row; Auto Layout resizes it once ViewModel bindings run.
                BookRowView row = new(CGRect.Empty) { ViewModel = book };
                NSButton selectButton = new() { Title = $"Select {book.Title}", BezelStyle = NSBezelStyle.Rounded };
                selectButton.Activated += (_, _) => ViewModel!.SelectedBook = book;

                NSStackView rowLayout = new() { Orientation = NSUserInterfaceLayoutOrientation.Horizontal, Spacing = 8 };
                rowLayout.AddArrangedSubview(row);
                rowLayout.AddArrangedSubview(selectButton);
                _rows.AddArrangedSubview(rowLayout);
            }

            d(this.BindCommand(ViewModel, static vm => vm.OpenLoan, static v => v.LoanButton));
        });
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _rows.Dispose();
            LoanButton.Dispose();
        }

        base.Dispose(disposing);
    }
}
