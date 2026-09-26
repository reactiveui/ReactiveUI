// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;
using Android.Widget;
using ReactiveUI.AndroidX;

namespace ReactiveUI.Device.Tests;

/// <summary>A <see cref="ReactiveAppCompatActivity{TViewModel}"/> that hosts a <see cref="WiredFragment"/>.</summary>
[Activity(Exported = false)]
public class CompatHostActivity : ReactiveAppCompatActivity<TestViewModel>
{
    /// <summary>The number of times the activity has been activated.</summary>
    private int _activations;

    /// <summary>Initializes a new instance of the <see cref="CompatHostActivity"/> class.</summary>
    public CompatHostActivity() =>
        this.WhenActivated((Action<IDisposable> _) => Interlocked.Increment(ref _activations));

    /// <summary>Gets the number of times the activity has been activated.</summary>
    public int Activations => Volatile.Read(ref _activations);

    /// <summary>Gets the hosted fragment.</summary>
    public WiredFragment Fragment { get; } = new();

    /// <inheritdoc/>
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        var container = new FrameLayout(this) { Id = View.GenerateViewId() };
        SetContentView(container);
        SupportFragmentManager!.BeginTransaction()!.Add(container.Id, Fragment)!.CommitNow();
    }
}
