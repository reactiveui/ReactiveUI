// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.OS;
using Android.Widget;
using SgAndroid = ReactiveUI.SourceGenerators.Android;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>Shows compile-time Android view wire-up via <see cref="SgAndroid.WireUpControlsAttribute"/> without reflection.</summary>
[Activity(Label = "Report absence (SourceGen)")]
[SgAndroid.WireUpControls]
[System.Diagnostics.DebuggerDisplay("{ViewModel}")]
public sealed partial class GeneratedAbsenceActivity : AndroidX.ReactiveAppCompatActivity<AbsenceViewModel>
{
    /// <summary>Gets or sets the status label wired at compile time to <c>Resource.Id.statusLabel</c>.</summary>
    [SgAndroid.WireUpResource("statusLabel")]
    public TextView? StatusText { get; set; }

    /// <summary>Gets or sets the compound view wired at compile time to <c>Resource.Id.absenceStatusView</c>.</summary>
    [SgAndroid.WireUpResource("absenceStatusView")]
    public AbsenceStatusView? StatusView { get; set; }

    /// <summary>Gets or sets a label excluded from generated wire-up via <c>[IgnoreResource]</c>.</summary>
    [SgAndroid.IgnoreResource]
    public TextView? ScratchLabel { get; set; }

    /// <inheritdoc/>
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_absence);

        // Generated private method calling FindViewById<T>(Resource.Id.*) with zero reflection.
        WireUpControls();
    }
}
