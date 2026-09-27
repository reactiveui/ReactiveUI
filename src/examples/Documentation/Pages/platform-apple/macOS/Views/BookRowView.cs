// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AppKit;
using CoreGraphics;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>One row in the catalog: title, author, and loan status. A real catalog would use a reactive table source instead.</summary>
[System.Diagnostics.DebuggerDisplay("BookRowView")]
public sealed class BookRowView : ReactiveView<Book>
{
    /// <summary>Initializes a new instance of the <see cref="BookRowView"/> class.</summary>
    public BookRowView() => Initialize();

    /// <summary>Initializes a new instance of the <see cref="BookRowView"/> class with an explicit frame.</summary>
    /// <param name="frame">The row's initial frame.</param>
    public BookRowView(CGRect frame)
        : base(frame) =>
        Initialize();

    /// <summary>Gets the book's title label. Internal so the binding source generator can observe it.</summary>
    internal NSTextField TitleLabel { get; } = NSTextField.CreateLabel(string.Empty);

    /// <summary>Gets the book's loan status label. Internal so the binding source generator can observe it.</summary>
    internal NSTextField StatusLabel { get; } = NSTextField.CreateLabel(string.Empty);

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

    /// <summary>Lays out the labels and binds them to the book set as the view model before this row is added to a superview.</summary>
    private void Initialize()
    {
        NSStackView stack = new()
        {
            Orientation = NSUserInterfaceLayoutOrientation.Vertical,
            Spacing = 2,
            TranslatesAutoresizingMaskIntoConstraints = false,
        };
        stack.AddArrangedSubview(TitleLabel);
        stack.AddArrangedSubview(StatusLabel);
        AddSubview(stack);
        NSLayoutConstraint.ActivateConstraints(
        [
            stack.LeadingAnchor.ConstraintEqualTo(LeadingAnchor),
            stack.TrailingAnchor.ConstraintEqualTo(TrailingAnchor),
            stack.TopAnchor.ConstraintEqualTo(TopAnchor),
            stack.BottomAnchor.ConstraintEqualTo(BottomAnchor),
        ]);

        _ = this.WhenActivated(d =>
        {
            d(this.OneWayBind(ViewModel, static vm => vm.Title, static v => v.TitleLabel.StringValue));
            d(this.OneWayBind(
                ViewModel,
                static vm => vm.IsOnLoan,
                static v => v.StatusLabel.StringValue,
                static onLoan => onLoan ? "On loan" : "Available"));
            d(Activated.Subscribe(static _ => Console.WriteLine("BookRowView activated.")));
            d(Deactivated.Subscribe(static _ => Console.WriteLine("BookRowView deactivated.")));
        });
    }
}
