// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The loan page: pick a member, then confirm or cancel. Pushed by <see cref="RoutedViewHost"/>.</summary>
[System.Diagnostics.DebuggerDisplay("LoanViewController")]
public sealed class LoanViewController : ReactiveViewController<LoanViewModel>
{
    /// <summary>Names the book being loaned.</summary>
    private readonly UILabel _bookLabel = new() { Font = UIFont.PreferredHeadline! };

    /// <summary>Hosts one button per member.</summary>
    private readonly UIStackView _memberButtons = new() { Axis = UILayoutConstraintAxis.Vertical, Spacing = 8, TranslatesAutoresizingMaskIntoConstraints = false };

    /// <summary>Gets the label naming the picked member. Internal for the binding source generator.</summary>
    internal UILabel SelectedMemberLabel { get; } = new() { TextColor = UIColor.SecondaryLabel };

    /// <summary>Gets the button that confirms the loan. Internal so the binding source generator can observe it.</summary>
    internal UIButton ConfirmButton { get; } = UIButton.FromType(UIButtonType.System);

    /// <summary>Gets the button that cancels the loan. Internal so the binding source generator can observe it.</summary>
    internal UIButton CancelButton { get; } = UIButton.FromType(UIButtonType.System);

    /// <inheritdoc/>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Loan";
        View!.BackgroundColor = UIColor.SystemBackground;
        ConfirmButton.SetTitle("Confirm loan", UIControlState.Normal);
        CancelButton.SetTitle("Cancel", UIControlState.Normal);

        UIStackView layout = new([_bookLabel, _memberButtons, SelectedMemberLabel, ConfirmButton, CancelButton])
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Spacing = 12,
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
            _bookLabel.Text = $"Loaning: {ViewModel!.Book.Title}";

            foreach (Member member in ViewModel.Members)
            {
                UIButton button = UIButton.FromType(UIButtonType.System);
                button.SetTitle(member.Name, UIControlState.Normal);
                button.TouchUpInside += (_, _) => ViewModel.SelectedMember = member;
                _memberButtons.AddArrangedSubview(button);
            }

            d(this.OneWayBind(
                ViewModel,
                static vm => vm.SelectedMember,
                static v => v.SelectedMemberLabel.Text,
                static member => member is null ? "(no member selected)" : $"To: {member.Name}"));
            d(this.BindCommand(ViewModel, static vm => vm.ConfirmLoan, static v => v.ConfirmButton));
            d(this.BindCommand(ViewModel, static vm => vm.Cancel, static v => v.CancelButton));
        });
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _bookLabel.Dispose();
            _memberButtons.Dispose();
            SelectedMemberLabel.Dispose();
            ConfirmButton.Dispose();
            CancelButton.Dispose();
        }

        base.Dispose(disposing);
    }
}
