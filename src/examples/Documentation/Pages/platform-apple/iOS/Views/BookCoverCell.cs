// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>One cover tile in the book-cover grid: title and loan status.</summary>
[System.Diagnostics.DebuggerDisplay("BookCoverCell")]
public sealed class BookCoverCell : ReactiveCollectionViewCell<Book>
{
    /// <summary>Initializes a new instance of the <see cref="BookCoverCell"/> class from a native handle.</summary>
    /// <param name="handle">The native handle UIKit hands the base constructor.</param>
    public BookCoverCell(IntPtr handle)
        : base(handle)
    {
        ContentView.BackgroundColor = UIColor.SecondarySystemBackground;

        UIStackView stack = new([TitleLabel, StatusLabel])
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Spacing = 2,
            TranslatesAutoresizingMaskIntoConstraints = false,
        };
        ContentView.AddSubview(stack);
        NSLayoutConstraint.ActivateConstraints(
        [
            stack.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 8),
            stack.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor, -8),
            stack.CenterYAnchor.ConstraintEqualTo(ContentView.CenterYAnchor),
        ]);

        _ = this.WhenActivated(d =>
        {
            d(this.OneWayBind(ViewModel, static vm => vm.Title, static v => v.TitleLabel.Text));
            d(this.OneWayBind(
                ViewModel,
                static vm => vm.IsOnLoan,
                static v => v.StatusLabel.Text,
                static onLoan => onLoan ? "On loan" : "Available"));
        });
    }

    /// <summary>Gets the book's title. Internal so the binding source generator can observe it.</summary>
    internal UILabel TitleLabel { get; } = new() { Font = UIFont.PreferredCaption1!, Lines = 2 };

    /// <summary>Gets the book's loan status. Internal so the binding source generator can observe it.</summary>
    internal UILabel StatusLabel { get; } = new() { Font = UIFont.PreferredCaption2!, TextColor = UIColor.SecondaryLabel };

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            TitleLabel.Dispose();
            StatusLabel.Dispose();
        }

        base.Dispose(disposing);
    }
}
