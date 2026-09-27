// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CoreGraphics;
using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The catalog page: one row per book, and a button that starts a loan for the selected one.</summary>
[System.Diagnostics.DebuggerDisplay("BookListViewController")]
public sealed class BookListViewController : ReactiveViewController<BookCatalogViewModel>
{
    /// <summary>The stack that hosts one <see cref="BookRowView"/> per book. A real catalog would use a reactive table source instead.</summary>
    private readonly UIStackView _rows = new() { Axis = UILayoutConstraintAxis.Vertical, Spacing = 8, TranslatesAutoresizingMaskIntoConstraints = false };

    /// <summary>Gets the button that opens the loan page for the selected book. Internal for the binding source generator.</summary>
    internal UIButton LoanButton { get; } = UIButton.FromType(UIButtonType.System);

    /// <inheritdoc/>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Catalog";
        View!.BackgroundColor = UIColor.SystemBackground;
        LoanButton.SetTitle("Loan selected book", UIControlState.Normal);

        UIStackView layout = new([_rows, LoanButton])
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Spacing = 16,
            TranslatesAutoresizingMaskIntoConstraints = false,
        };
        View!.AddSubview(layout);
        NSLayoutConstraint.ActivateConstraints(
        [
            layout.LeadingAnchor.ConstraintEqualTo(View!.SafeAreaLayoutGuide.LeadingAnchor, 16),
            layout.TrailingAnchor.ConstraintEqualTo(View!.SafeAreaLayoutGuide.TrailingAnchor, -16),
            layout.TopAnchor.ConstraintEqualTo(View!.SafeAreaLayoutGuide.TopAnchor, 16),
        ]);

        _ = this.WhenActivated(d =>
        {
            foreach (Book book in ViewModel!.Books)
            {
                // The frame constructor pre-sizes the row; Auto Layout resizes it once ViewModel bindings run.
                BookRowView row = new(CGRect.Empty) { ViewModel = book };
                UITapGestureRecognizer tap = new(() => ViewModel!.SelectedBook = book);
                row.AddGestureRecognizer(tap);
                _rows.AddArrangedSubview(row);
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
