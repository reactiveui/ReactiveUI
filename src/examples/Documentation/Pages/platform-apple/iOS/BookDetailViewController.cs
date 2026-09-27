// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>One book's detail: cover, title, author, and a star rating a librarian can adjust.</summary>
[System.Diagnostics.DebuggerDisplay("BookDetailViewController")]
public sealed class BookDetailViewController : ReactiveViewController<Book>
{
    /// <summary>Hosts the cover and rating controls, added once the view model is set on them first.</summary>
    private readonly UIStackView _layout;

    /// <summary>Initializes a new instance of the <see cref="BookDetailViewController"/> class.</summary>
    public BookDetailViewController() =>
        _layout = new UIStackView([TitleLabel, AuthorLabel])
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Alignment = UIStackViewAlignment.Center,
            Spacing = 12,
            TranslatesAutoresizingMaskIntoConstraints = false,
        };

    /// <summary>Gets the label showing the book's title. Internal so the binding source generator can observe it.</summary>
    internal UILabel TitleLabel { get; } = new() { Font = UIFont.PreferredTitle1! };

    /// <summary>Gets the label showing the book's author. Internal so the binding source generator can observe it.</summary>
    internal UILabel AuthorLabel { get; } = new() { Font = UIFont.PreferredBody!, TextColor = UIColor.SecondaryLabel };

    /// <inheritdoc/>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        View!.BackgroundColor = UIColor.SystemBackground;
        View!.AddSubview(_layout);
        NSLayoutConstraint.ActivateConstraints(
        [
            _layout.CenterXAnchor.ConstraintEqualTo(View!.CenterXAnchor),
            _layout.TopAnchor.ConstraintEqualTo(View!.SafeAreaLayoutGuide.TopAnchor, 24),
        ]);

        _ = this.WhenActivated(d =>
        {
            Title = ViewModel!.Title;

            // Setting ViewModel before adding each control to the stack means it already has a book to bind to the
            // moment it gains a superview and its own activation runs.
            BookCoverImageView cover = new() { ViewModel = ViewModel, TranslatesAutoresizingMaskIntoConstraints = false };
            NSLayoutConstraint.ActivateConstraints(
            [
                cover.WidthAnchor.ConstraintEqualTo(120),
                cover.HeightAnchor.ConstraintEqualTo(160),
            ]);
            _layout.InsertArrangedSubview(cover, 0);

            StarRatingControl rating = new() { ViewModel = ViewModel };
            _layout.AddArrangedSubview(rating);

            d(this.OneWayBind(ViewModel, static vm => vm.Title, static v => v.TitleLabel.Text));
            d(this.OneWayBind(ViewModel, static vm => vm.Author, static v => v.AuthorLabel.Text));
            d(new ActionDisposable(() =>
            {
                cover.RemoveFromSuperview();
                cover.Dispose();
                rating.RemoveFromSuperview();
                rating.Dispose();
            }));
        });
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            TitleLabel.Dispose();
            AuthorLabel.Dispose();
            _layout.Dispose();
        }

        base.Dispose(disposing);
    }
}
