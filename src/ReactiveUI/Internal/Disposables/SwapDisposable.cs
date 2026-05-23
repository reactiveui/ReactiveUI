// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>
/// A disposable holder whose inner disposable can be re-assigned. The previous inner
/// disposable is disposed when replaced (in contrast to <see cref="MutableDisposable"/>).
/// Once this object is disposed, any subsequently assigned inner disposable is disposed
/// immediately. Replaces <c>SerialDisposable</c>.
/// </summary>
internal sealed class SwapDisposable : IDisposable
{
    /// <summary>The current inner disposable.</summary>
    private IDisposable? _current;

    /// <summary>Disposed flag (0 = open, 1 = disposed), driven through <see cref="Interlocked"/>.</summary>
    private int _disposed;

    /// <summary>
    /// Gets or sets the current inner disposable. Setting disposes the previous value.
    /// </summary>
    public IDisposable? Disposable
    {
        get => Volatile.Read(ref _current);
        set => DisposableSlotHelper.SwapAndDisposePrevious(ref _current, ref _disposed, value);
    }

    /// <inheritdoc/>
    public void Dispose() => DisposableSlotHelper.TryDispose(ref _current, ref _disposed);
}
