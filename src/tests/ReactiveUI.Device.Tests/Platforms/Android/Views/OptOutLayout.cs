// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;
using Android.Widget;

namespace ReactiveUI.Device.Tests;

/// <summary>A linear layout holding the <c>wireup_layout</c> controls, with one property that opts out of wire-up.</summary>
public sealed class OptOutLayout : LinearLayout
{
    /// <summary>Initializes a new instance of the <see cref="OptOutLayout"/> class.</summary>
    /// <param name="context">The Android context.</param>
    private OptOutLayout(Context context)
        : base(context)
    {
    }

    /// <summary>Gets or sets the title control.</summary>
    public TextView? TitleText { get; set; }

    /// <summary>Gets or sets the input; wired through a resource-name override.</summary>
    [WireUpResource("renamed_input")]
    public EditText? Input { get; set; }

    /// <summary>Gets or sets a control with no matching id that opts out of wire-up.</summary>
    [IgnoreResource]
    public TextView? Ignored { get; set; }

    /// <summary>Creates a layout and inflates <c>wireup_layout</c> into it.</summary>
    /// <param name="context">The Android context.</param>
    /// <returns>The populated layout.</returns>
    public static OptOutLayout Create(Context context)
    {
        var layout = new OptOutLayout(context);
        _ = LayoutInflater.From(context)!.Inflate(Resource.Layout.wireup_layout, layout, true);
        return layout;
    }
}
