// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Reflection;

/// <summary>
/// A code-behind that raises activation but is not itself an <see cref="IViewFor"/> — some platforms separate the
/// two, for example a templated container that activates a data-bound content control it does not own. Passing a
/// separate <see cref="IViewFor"/> to <c>WhenActivated</c> covers this case.
/// </summary>
[System.Diagnostics.DebuggerDisplay("MusicPlayerCodeBehind")]
public sealed class MusicPlayerCodeBehind : IActivatableView, ICanActivate, IDisposable
{
    /// <summary>Raised when the control is shown.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>Raised when the control is hidden.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <inheritdoc/>
    public IObservable<RxVoid> Activated => _activated;

    /// <inheritdoc/>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <summary>Shows the control, as a container does when its content becomes visible.</summary>
    public void Show() => _activated.OnNext(RxVoid.Default);

    /// <summary>Hides the control, as a container does when its content is no longer visible.</summary>
    public void Hide() => _deactivated.OnNext(RxVoid.Default);

    /// <inheritdoc/>
    public void Dispose()
    {
        _activated.Dispose();
        _deactivated.Dispose();
    }
}
