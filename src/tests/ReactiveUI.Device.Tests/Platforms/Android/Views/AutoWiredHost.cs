// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;
using Android.Widget;
using static ReactiveUI.ControlFetcherMixins;

namespace ReactiveUI.Device.Tests;

/// <summary>A layout host that inflates <c>wireup_layout</c> and wires it by reflection with a chosen strategy.</summary>
public sealed class AutoWiredHost : LayoutViewHostUnsafe
{
    /// <summary>Initializes a new instance of the <see cref="AutoWiredHost"/> class.</summary>
    /// <param name="context">The Android context.</param>
    /// <param name="parent">The parent the layout is inflated against.</param>
    /// <param name="strategy">The wire-up strategy.</param>
    public AutoWiredHost(Context context, ViewGroup parent, ResolveStrategy strategy)
        : base(context, Resource.Layout.wireup_layout, parent, false, true, strategy)
    {
    }

    /// <summary>Gets or sets the title control; wired by its property name.</summary>
    public TextView? TitleText { get; set; }

    /// <summary>Gets or sets the save button; opted in explicitly.</summary>
    [WireUpResource]
    public Button? SaveButton { get; set; }

    /// <summary>Gets or sets the input; wired through a resource-name override.</summary>
    [WireUpResource("renamed_input")]
    public EditText? Input { get; set; }

    /// <summary>Gets or sets a control that opts out of wire-up.</summary>
    [IgnoreResource]
    public TextView? Ignored { get; set; }
}
