// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Widget;

namespace ReactiveUI.Device.Tests;

/// <summary>A <see cref="ReactiveActivity{TViewModel}"/> that counts its activations and wires its layout by name.</summary>
[Activity(Exported = false)]
public class ActivatingActivity : ReactiveActivity<TestViewModel>
{
    /// <summary>The number of times the activity has been activated.</summary>
    private int _activations;

    /// <summary>The number of times the activity has been deactivated.</summary>
    private int _deactivations;

    /// <summary>Initializes a new instance of the <see cref="ActivatingActivity"/> class.</summary>
    public ActivatingActivity() =>
        this.WhenActivated(disposables =>
        {
            _ = Interlocked.Increment(ref _activations);
            disposables(new ActionDisposable(() => Interlocked.Increment(ref _deactivations)));
        });

    /// <summary>Gets the most recently created instance, so a test can reach the instance Android creates when it recreates one.</summary>
    public static ActivatingActivity? LastCreated { get; private set; }

    /// <summary>Gets the number of times the activity has been activated.</summary>
    public int Activations => Volatile.Read(ref _activations);

    /// <summary>Gets a value indicating whether Android passed saved state to <see cref="OnCreate(Bundle?)"/>.</summary>
    public bool CreatedFromSavedState { get; private set; }

    /// <summary>Gets the number of times the activity has been deactivated.</summary>
    public int Deactivations => Volatile.Read(ref _deactivations);

    /// <summary>Gets or sets the title control, wired by name.</summary>
    public TextView? TitleText { get; set; }

    /// <summary>Gets or sets the save button, wired by name.</summary>
    public Button? SaveButton { get; set; }

    /// <summary>Gets the error wiring the controls threw, if it threw.</summary>
    public Exception? WireUpError { get; private set; }

    /// <inheritdoc/>
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        CreatedFromSavedState = savedInstanceState is not null;
        LastCreated = this;
        SetContentView(Resource.Layout.wireup_layout);

        try
        {
            this.WireUpControls();
        }
        catch (MissingFieldException ex)
        {
            WireUpError = ex;
        }
    }
}
