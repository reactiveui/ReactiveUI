// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>One row in the on-loan board: a book's title.</summary>
[System.Diagnostics.DebuggerDisplay("LoanedBookCell")]
public sealed class LoanedBookCell : ReactiveTableViewCell<Book>
{
    /// <summary>Initializes a new instance of the <see cref="LoanedBookCell"/> class from a native handle.</summary>
    /// <param name="handle">The native handle UIKit hands the base constructor.</param>
    public LoanedBookCell(IntPtr handle)
        : base(handle)
    {
        ContentView.AddSubview(TitleLabel);
        TitleLabel.TranslatesAutoresizingMaskIntoConstraints = false;
        NSLayoutConstraint.ActivateConstraints(
        [
            TitleLabel.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 16),
            TitleLabel.CenterYAnchor.ConstraintEqualTo(ContentView.CenterYAnchor),
        ]);

        _ = this.WhenActivated(d =>
            d(this.OneWayBind(ViewModel, static vm => vm.Title, static v => v.TitleLabel.Text)));
    }

    /// <summary>Gets the book's title. Internal so the binding source generator can observe it.</summary>
    internal UILabel TitleLabel { get; } = new() { Font = UIFont.PreferredBody! };

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            TitleLabel.Dispose();
        }

        base.Dispose(disposing);
    }
}
