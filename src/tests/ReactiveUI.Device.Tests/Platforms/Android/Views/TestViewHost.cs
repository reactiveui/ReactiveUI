// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;
using Android.Widget;

namespace ReactiveUI.Device.Tests;

/// <summary>A <see cref="ReactiveViewHost{TViewModel}"/> over <c>wireup_layout</c>.</summary>
public sealed class TestViewHost : ReactiveViewHost<TestViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="TestViewHost"/> class without wiring any control.</summary>
    /// <param name="context">The Android context.</param>
    /// <param name="parent">The parent the layout is inflated against.</param>
    public TestViewHost(Context context, ViewGroup parent)
        : base(context, Resource.Layout.wireup_layout, parent)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TestViewHost"/> class, wiring the title through a bind callback.</summary>
    /// <param name="context">The Android context.</param>
    /// <param name="parent">The parent the layout is inflated against.</param>
    /// <param name="bindTitle">Unused marker selecting the bind-callback constructor.</param>
    public TestViewHost(Context context, ViewGroup parent, bool bindTitle)
        : base(
            context,
            Resource.Layout.wireup_layout,
            parent,
            false,
            static (host, view) => ((TestViewHost)host).TitleText = view.FindViewById<TextView>(Resource.Id.TitleText)) =>
        BoundByCallback = bindTitle;

    /// <summary>Gets or sets the title control.</summary>
    public TextView? TitleText { get; set; }

    /// <summary>Gets a value indicating whether the bind-callback constructor built this host.</summary>
    public bool BoundByCallback { get; }

    /// <summary>Gets a value indicating whether the legacy reflection metadata was prepared (only the Unsafe host does).</summary>
    public bool HasLegacyPropertyMetadata => AllPublicProperties is not null;
}
