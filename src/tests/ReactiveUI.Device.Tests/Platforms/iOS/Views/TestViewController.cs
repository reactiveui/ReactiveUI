// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Device.Tests;

/// <summary>A reactive view controller for <see cref="TestViewModel"/> that counts its activations.</summary>
public sealed class TestViewController : ReactiveViewController<TestViewModel>
{
    /// <summary>The number of times the controller has been activated.</summary>
    private int _activations;

    /// <summary>The number of times the controller has been deactivated.</summary>
    private int _deactivations;

    /// <summary>Initializes a new instance of the <see cref="TestViewController"/> class.</summary>
    public TestViewController() =>
        this.WhenActivated(disposables =>
        {
            _ = Interlocked.Increment(ref _activations);
            disposables(new ActionDisposable(() => Interlocked.Increment(ref _deactivations)));
        });

    /// <summary>Gets the number of times the controller has been activated.</summary>
    public int Activations => Volatile.Read(ref _activations);

    /// <summary>Gets the number of times the controller has been deactivated.</summary>
    public int Deactivations => Volatile.Read(ref _deactivations);
}
