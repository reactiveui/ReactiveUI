// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

namespace ReactiveUI.Tests.Winforms;

public class TestForm : Form, IActivatableView, ICanActivate
{
    private readonly ReplaySubject<Unit> _activated = new(1);
    private readonly ReplaySubject<Unit> _deactivated = new(1);

    public TestForm()
    {
        this.WhenActivated(d =>
        {
            ////
        });

        _activated.Subscribe();
        _deactivated.Subscribe();
    }

    public TestForm(short activate)
        : this()
    {
        switch (activate)
        {
            case 1:
                _activated.OnNext(Unit.Default);
                break;

            case 2:
                _deactivated.OnNext(Unit.Default);
                break;
        }
    }

    public IObservable<Unit> Deactivated => _deactivated.AsObservable().Publish().RefCount();

    IObservable<Unit> ICanActivate.Activated => _activated.AsObservable().Publish().RefCount();

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
