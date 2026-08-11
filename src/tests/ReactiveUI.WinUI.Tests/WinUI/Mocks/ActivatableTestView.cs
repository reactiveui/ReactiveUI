// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Primitives.Signals;

namespace ReactiveUI.Tests.WinUI.Mocks;

/// <summary>A view that declares its own activation lifetime instead of deriving it from the visual tree.</summary>
public sealed class ActivatableTestView : IActivatableView, ICanActivate
{
    /// <summary>The signal raised when the view becomes active.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>The signal raised when the view stops being active.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <inheritdoc/>
    public IObservable<RxVoid> Activated => _activated;

    /// <inheritdoc/>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <summary>Signals that the view has become active.</summary>
    public void Activate() => _activated.OnNext(RxVoid.Default);

    /// <summary>Signals that the view has stopped being active.</summary>
    public void Deactivate() => _deactivated.OnNext(RxVoid.Default);
}
