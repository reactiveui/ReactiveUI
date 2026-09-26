// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;
using Android.Widget;
using static ReactiveUI.ControlFetcherMixins;

namespace ReactiveUI.Device.Tests;

/// <summary>A <see cref="ReactiveViewHostUnsafe{TViewModel}"/> over <c>wireup_layout</c> that wires its controls by reflection.</summary>
public sealed class UnsafeTestViewHost : ReactiveViewHostUnsafe<TestViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="UnsafeTestViewHost"/> class, wiring by reflection.</summary>
    /// <param name="context">The Android context.</param>
    /// <param name="parent">The parent the layout is inflated against.</param>
    /// <param name="strategy">The wire-up strategy.</param>
    public UnsafeTestViewHost(Context context, ViewGroup parent, ResolveStrategy strategy)
        : base(context, Resource.Layout.wireup_layout, parent, false, true, strategy)
    {
    }

    /// <summary>Gets or sets the title control; wired by its property name.</summary>
    public TextView? TitleText { get; set; }

    /// <summary>Gets a value indicating whether the reflection constructor prepared the legacy property metadata.</summary>
    public bool HasLegacyPropertyMetadata => AllPublicProperties is not null;
}
