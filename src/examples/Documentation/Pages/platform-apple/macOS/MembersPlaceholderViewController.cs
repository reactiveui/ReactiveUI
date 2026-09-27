// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AppKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The members placeholder: counts members. Listing each in a reactive table is a different chunk of the surface.</summary>
[System.Diagnostics.DebuggerDisplay("MembersPlaceholderViewController")]
public sealed class MembersPlaceholderViewController : ReactiveViewController<MembersViewModel>
{
    /// <summary>Gets the label naming how many members the library has. Internal for the binding source generator.</summary>
    internal NSTextField CountLabel { get; } = NSTextField.CreateLabel(string.Empty);

    /// <inheritdoc/>
    public override void LoadView()
    {
        NSView view = new();
        View = view;
        view.AddSubview(CountLabel);
        CountLabel.TranslatesAutoresizingMaskIntoConstraints = false;
        NSLayoutConstraint.ActivateConstraints(
        [
            CountLabel.CenterXAnchor.ConstraintEqualTo(view.CenterXAnchor),
            CountLabel.TopAnchor.ConstraintEqualTo(view.TopAnchor, 24),
        ]);

        _ = this.WhenActivated(d =>
            d(this.OneWayBind(ViewModel, static vm => vm.MemberCount, static v => v.CountLabel.StringValue, static count => $"{count} member(s)")));
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
