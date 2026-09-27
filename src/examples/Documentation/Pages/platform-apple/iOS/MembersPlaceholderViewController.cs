// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The members tab: counts members. Listing each in a reactive table is a different chunk of the surface.</summary>
[System.Diagnostics.DebuggerDisplay("MembersPlaceholderViewController")]
public sealed class MembersPlaceholderViewController : ReactiveViewController<MembersViewModel>
{
    /// <summary>Gets the label naming how many members the library has. Internal for the binding source generator.</summary>
    internal UILabel CountLabel { get; } = new() { Font = UIFont.PreferredBody! };

    /// <inheritdoc/>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Members";
        View!.BackgroundColor = UIColor.SystemBackground;
        View!.AddSubview(CountLabel);
        CountLabel.TranslatesAutoresizingMaskIntoConstraints = false;
        NSLayoutConstraint.ActivateConstraints(
        [
            CountLabel.CenterXAnchor.ConstraintEqualTo(View!.CenterXAnchor),
            CountLabel.TopAnchor.ConstraintEqualTo(View!.SafeAreaLayoutGuide.TopAnchor, 24),
        ]);

        _ = this.WhenActivated(d =>
            d(this.OneWayBind(ViewModel, static vm => vm.MemberCount, static v => v.CountLabel.Text, static count => $"{count} member(s)")));
    }

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
