// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// A test form that supports activation and can activate semantics.
/// </summary>
public class TestForm : Form, IActivatableView, ICanActivate
{
    /// <summary>
    /// The activation state value that signals the form is activated.
    /// </summary>
    private const short ActivatedState = 1;

    /// <summary>
    /// The activation state value that signals the form is deactivated.
    /// </summary>
    private const short DeactivatedState = 2;

    /// <summary>
    /// The subject signalling activation.
    /// </summary>
    private readonly ReplaySubject<Unit> _activated = new(1);

    /// <summary>
    /// The subject signalling deactivation.
    /// </summary>
    private readonly ReplaySubject<Unit> _deactivated = new(1);

    /// <summary>
    /// Initializes a new instance of the <see cref="TestForm"/> class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Reliability",
        "S3366:Don't expose 'this' in constructors",
        Justification = "OAPH/WhenAny initialization requires 'this'; single-threaded test fixture.")]
    public TestForm()
    {
        this.WhenActivated((Action<Action<IDisposable>>)(static _ =>
        {
            ////
        }));

        _activated.Subscribe();
        _deactivated.Subscribe();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestForm"/> class with an initial activation state.
    /// </summary>
    /// <param name="activate">When 1, signals activated; when 2, signals deactivated.</param>
    public TestForm(short activate)
        : this()
    {
        switch (activate)
        {
            case ActivatedState:
            {
                _activated.OnNext(Unit.Default);
                break;
            }

            case DeactivatedState:
            {
                _deactivated.OnNext(Unit.Default);
                break;
            }
        }
    }

    /// <summary>
    /// Gets an observable that signals when the form is deactivated.
    /// </summary>
    public IObservable<Unit> Deactivated => _deactivated.AsObservable().Publish().RefCount();

    /// <inheritdoc/>
    IObservable<Unit> ICanActivate.Activated => _activated.AsObservable().Publish().RefCount();

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
