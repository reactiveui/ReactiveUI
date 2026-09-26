// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;
using Android.Widget;

namespace ReactiveUI.Device.Tests;

/// <summary>A layout host that inflates <c>wireup_layout</c> and wires it through an explicit bind callback.</summary>
public sealed class BoundHost : LayoutViewHost
{
    /// <summary>Initializes a new instance of the <see cref="BoundHost"/> class.</summary>
    /// <param name="context">The Android context.</param>
    /// <param name="parent">The parent the layout is inflated against.</param>
    /// <param name="attachToRoot">Whether the inflated layout is attached to <paramref name="parent"/>.</param>
    public BoundHost(Context context, ViewGroup parent, bool attachToRoot)
        : base(context, Resource.Layout.wireup_layout, parent, attachToRoot, static (host, view) => ((BoundHost)host).Bind(view))
    {
    }

    /// <summary>Gets the view the bind callback received.</summary>
    public View? BoundView { get; private set; }

    /// <summary>Gets the title control found by the bind callback.</summary>
    public TextView? TitleText { get; private set; }

    /// <summary>Records the bound view and finds the title control.</summary>
    /// <param name="view">The inflated view.</param>
    private void Bind(View view)
    {
        BoundView = view;
        TitleText = view.FindViewById<TextView>(Resource.Id.TitleText);
    }
}
