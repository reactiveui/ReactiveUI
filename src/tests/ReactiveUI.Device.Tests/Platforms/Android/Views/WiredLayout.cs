// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;
using Android.Widget;

namespace ReactiveUI.Device.Tests;

/// <summary>A linear layout holding the <c>wireup_layout</c> controls, with properties to wire them to.</summary>
public sealed class WiredLayout : LinearLayout
{
    /// <summary>Initializes a new instance of the <see cref="WiredLayout"/> class.</summary>
    /// <param name="context">The Android context.</param>
    private WiredLayout(Context context)
        : base(context)
    {
    }

    /// <summary>Gets or sets the title control.</summary>
    public TextView? TitleText { get; set; }

    /// <summary>Gets or sets the save button.</summary>
    public Button? SaveButton { get; set; }

    /// <summary>Creates a layout and inflates <c>wireup_layout</c> into it.</summary>
    /// <param name="context">The Android context.</param>
    /// <returns>The populated layout.</returns>
    public static WiredLayout Create(Context context)
    {
        var layout = new WiredLayout(context);
        _ = LayoutInflater.From(context)!.Inflate(Resource.Layout.wireup_layout, layout, true);
        return layout;
    }
}
