// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CoreGraphics;
using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>One chip in the horizontal member strip: the member's initial in a circle.</summary>
[System.Diagnostics.DebuggerDisplay("MemberChipCell")]
public sealed class MemberChipCell : ReactiveCollectionViewCell<Member>
{
    /// <summary>Initializes a new instance of the <see cref="MemberChipCell"/> class from a native handle.</summary>
    /// <param name="handle">The native handle UIKit hands the base constructor.</param>
    public MemberChipCell(IntPtr handle)
        : base(handle)
    {
        ContentView.BackgroundColor = UIColor.SystemGray5;
        ContentView.Layer.CornerRadius = 20;
        ContentView.AddSubview(InitialLabel);
        InitialLabel.Frame = new CGRect(0, 0, 40, 40);
        InitialLabel.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

        // Member is an immutable record, not a ReactiveObject, so the label follows ViewModel itself changing rather
        // than a OneWayBind on one of its properties.
        _ = this.WhenActivated(d =>
            d(this.WhenAnyValue(static v => v.ViewModel)
                .Subscribe(member => InitialLabel.Text = member?.Name[..1].ToUpperInvariant())));
    }

    /// <summary>Gets the member's initial. Internal so the binding source generator can observe it.</summary>
    internal UILabel InitialLabel { get; } = new() { TextAlignment = UITextAlignment.Center, Font = UIFont.PreferredHeadline! };

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            InitialLabel.Dispose();
        }

        base.Dispose(disposing);
    }
}
