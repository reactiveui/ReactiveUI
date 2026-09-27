// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The single section header above the book-cover grid, naming how many books it shows.</summary>
[System.Diagnostics.DebuggerDisplay("ShelfHeaderView")]
public sealed class ShelfHeaderView : ReactiveCollectionReusableView<BookCatalogViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="ShelfHeaderView"/> class from a native handle.</summary>
    /// <param name="handle">The native handle UIKit hands the base constructor.</param>
    public ShelfHeaderView(IntPtr handle)
        : base(handle)
    {
        AddSubview(CountLabel);
        CountLabel.TranslatesAutoresizingMaskIntoConstraints = false;
        NSLayoutConstraint.ActivateConstraints(
        [
            CountLabel.LeadingAnchor.ConstraintEqualTo(LeadingAnchor, 16),
            CountLabel.CenterYAnchor.ConstraintEqualTo(CenterYAnchor),
        ]);

        // A supplementary view is dequeued and given its ViewModel before UIKit adds it to the view hierarchy, so it
        // reacts to the property directly rather than through WhenActivated.
        _ = this.WhenAnyValue(static v => v.ViewModel)
            .WhereNotNull()
            .Subscribe(vm => CountLabel.Text = $"{vm.Books.Count} book(s)");
    }

    /// <summary>Gets the label naming how many books the shelf has. Internal so the binding source generator can observe it.</summary>
    internal UILabel CountLabel { get; } = new() { Font = UIFont.PreferredHeadline! };

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CountLabel.Dispose();
        }

        base.Dispose(disposing);
    }
}
