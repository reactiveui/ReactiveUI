// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>A test form that supports activation and can activate semantics.</summary>
public class TestForm : Form, IActivatableView, ICanActivate
{
    /// <summary>The activation state value that signals the form is activated.</summary>
    private const short ActivatedState = 1;

    /// <summary>The activation state value that signals the form is deactivated.</summary>
    private const short DeactivatedState = 2;

    /// <summary>The subject signalling activation.</summary>
    private readonly ReplaySignal<RxVoid> _activated = new(1);

    /// <summary>The subject signalling deactivation.</summary>
    private readonly ReplaySignal<RxVoid> _deactivated = new(1);

    /// <summary>Initializes a new instance of the <see cref="TestForm"/> class.</summary>
    public TestForm()
    {
        _ = this.WhenActivated((Action<Action<IDisposable>>)(static _ =>
        {
            ////
        }));

        _ = _activated.Subscribe();
        _ = _deactivated.Subscribe();
    }

    /// <summary>Initializes a new instance of the <see cref="TestForm"/> class with an initial activation state.</summary>
    /// <param name="activate">When 1, signals activated; when 2, signals deactivated.</param>
    public TestForm(short activate)
        : this()
    {
        switch (activate)
        {
            case ActivatedState:
            {
                _activated.OnNext(RxVoid.Default);
                break;
            }

            case DeactivatedState:
            {
                _deactivated.OnNext(RxVoid.Default);
                break;
            }
        }
    }

    /// <summary>Gets an observable that signals when the form is deactivated.</summary>
    public IObservable<RxVoid> Deactivated => _deactivated.AsObservable();

    /// <inheritdoc/>
    IObservable<RxVoid> ICanActivate.Activated => _activated.AsObservable();

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _activated.Dispose();
            _deactivated.Dispose();
        }

        base.Dispose(disposing);
    }
}
