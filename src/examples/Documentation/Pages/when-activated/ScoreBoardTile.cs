// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.WhenActivated;

/// <summary>
/// One tile in a scoreboard dashboard that shows several games at once. The dashboard has a single show/hide event
/// of its own, not one per tile, so it forces each tile's activation state directly through
/// <see cref="ICanForceManualActivation"/> instead of waiting for a native event that never comes.
/// </summary>
[System.Diagnostics.DebuggerDisplay("ScoreBoardTile")]
public sealed class ScoreBoardTile : IActivatableView, ICanActivate, ICanForceManualActivation, IDisposable
{
    /// <summary>Raised when the dashboard forces this tile active.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>Raised when the dashboard forces this tile inactive.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <inheritdoc/>
    public IObservable<RxVoid> Activated => _activated;

    /// <inheritdoc/>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <inheritdoc/>
    void ICanForceManualActivation.Activate(bool isActivating)
    {
        if (isActivating)
        {
            _activated.OnNext(RxVoid.Default);
        }
        else
        {
            _deactivated.OnNext(RxVoid.Default);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _activated.Dispose();
        _deactivated.Dispose();
    }
}
