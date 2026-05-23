// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>
/// An <see cref="IDisposable"/> that runs the supplied <see cref="Action"/> exactly once on
/// <see cref="Dispose"/>. Replaces <c>Disposable.Create(Action)</c>.
/// </summary>
internal sealed class ActionDisposable : IDisposable
{
    /// <summary>The action to run once on dispose; nulled out after it runs.</summary>
    private Action? _action;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionDisposable"/> class.
    /// </summary>
    /// <param name="action">The action to invoke once on dispose.</param>
    public ActionDisposable(Action action) => _action = action;

    /// <inheritdoc/>
    public void Dispose() => Interlocked.Exchange(ref _action, null)?.Invoke();
}
