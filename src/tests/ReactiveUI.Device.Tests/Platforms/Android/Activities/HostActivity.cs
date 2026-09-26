// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Widget;

namespace ReactiveUI.Device.Tests;

/// <summary>A plain activity whose content view the tests add views to, so the views attach to a real window.</summary>
[Activity(Exported = false)]
public class HostActivity : Activity
{
    /// <summary>Gets the container the tests add views to.</summary>
    public FrameLayout Container { get; private set; } = null!;

    /// <inheritdoc/>
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Container = new(this);
        SetContentView(Container);
    }
}
