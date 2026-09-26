// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;
using ReactiveUI.AndroidX;

namespace ReactiveUI.Device.Tests;

/// <summary>A reactive view holder that counts its activations.</summary>
public sealed class TestViewHolder : ReactiveRecyclerViewViewHolder<TestViewModel>
{
    /// <summary>The number of times the holder has been activated.</summary>
    private int _activations;

    /// <summary>The number of times the holder has been deactivated.</summary>
    private int _deactivations;

    /// <summary>Initializes a new instance of the <see cref="TestViewHolder"/> class.</summary>
    /// <param name="view">The row view.</param>
    public TestViewHolder(View view)
        : base(view) =>
        this.WhenActivated(disposables =>
        {
            _ = Interlocked.Increment(ref _activations);
            disposables(new ActionDisposable(() => Interlocked.Increment(ref _deactivations)));
        });

    /// <summary>Gets the number of times the holder has been activated.</summary>
    public int Activations => Volatile.Read(ref _activations);

    /// <summary>Gets the number of times the holder has been deactivated.</summary>
    public int Deactivations => Volatile.Read(ref _deactivations);
}
