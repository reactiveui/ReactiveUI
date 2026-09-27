// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using CoreGraphics;
using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>A plain <see cref="ReactiveCollectionView{TViewModel}"/>: no collection view controller owns it, so
/// <see cref="MembersPlaceholderViewController"/> hosts it directly as the members table's header, listing the same
/// members as a horizontal strip of chips.</summary>
[System.Diagnostics.DebuggerDisplay("MemberChipStripView")]
public sealed class MemberChipStripView : ReactiveCollectionView<MembersViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="MemberChipStripView"/> class.</summary>
    /// <param name="frame">The strip's initial frame.</param>
    /// <param name="layout">The horizontal flow layout the strip scrolls under.</param>
    public MemberChipStripView(CGRect frame, UICollectionViewLayout layout)
        : base(frame, layout)
    {
        BackgroundColor = UIColor.SystemBackground;

        _ = this.WhenActivated(d =>
            d(Signal.Emit<INotifyCollectionChanged>(ViewModel!.Members).BindTo<Member, MemberChipCell>(this)));
    }
}
