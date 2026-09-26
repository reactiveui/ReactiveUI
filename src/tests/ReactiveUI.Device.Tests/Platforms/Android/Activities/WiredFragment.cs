// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;
using Android.Widget;
using ReactiveUI.AndroidX;

namespace ReactiveUI.Device.Tests;

/// <summary>An AndroidX <see cref="ReactiveFragment{TViewModel}"/> that wires its layout by name and counts activations.</summary>
public class WiredFragment : ReactiveUI.AndroidX.ReactiveFragment<TestViewModel>
{
    /// <summary>The number of times the fragment has been activated.</summary>
    private int _activations;

    /// <summary>The number of times the fragment has been deactivated.</summary>
    private int _deactivations;

    /// <summary>Initializes a new instance of the <see cref="WiredFragment"/> class.</summary>
    public WiredFragment() =>
        this.WhenActivated(disposables =>
        {
            _ = Interlocked.Increment(ref _activations);
            disposables(new ActionDisposable(() => Interlocked.Increment(ref _deactivations)));
        });

    /// <summary>Gets the number of times the fragment has been activated.</summary>
    public int Activations => Volatile.Read(ref _activations);

    /// <summary>Gets the number of times the fragment has been deactivated.</summary>
    public int Deactivations => Volatile.Read(ref _deactivations);

    /// <summary>Gets or sets the title control, wired by name.</summary>
    public TextView? TitleText { get; set; }

    /// <summary>Gets or sets the renamed input, wired through its resource-name override.</summary>
    [WireUpResource("renamed_input")]
    public EditText? Input { get; set; }

    /// <inheritdoc/>
    public override View? OnCreateView(LayoutInflater? inflater, ViewGroup? container, Bundle? savedInstanceState)
    {
        if (inflater?.Inflate(Resource.Layout.wireup_layout, container, false) is not { } view)
        {
            return null;
        }

        this.WireUpControls(view);
        return view;
    }
}
