// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>
/// Shared lock-free slot mechanics for the single-slot reassignable disposables
/// (<see cref="MutableDisposable"/> and <see cref="SwapDisposable"/>). The disposed
/// flag is an <see cref="int"/> (0 = open, 1 = disposed) driven through
/// <see cref="Interlocked"/> so a race between assignment and disposal always leaves
/// the most recently assigned value disposed.
/// </summary>
internal static class DisposableSlotHelper
{
    /// <summary>
    /// Assigns <paramref name="value"/> into the slot without disposing the previous value
    /// (multiple-assignment semantics). If the slot is already disposed, <paramref name="value"/>
    /// is disposed immediately.
    /// </summary>
    /// <param name="current">The slot field.</param>
    /// <param name="disposed">The disposed flag field (0 = open, 1 = disposed).</param>
    /// <param name="value">The disposable to assign.</param>
    public static void AssignWithoutDisposingPrevious(ref IDisposable? current, ref int disposed, IDisposable? value)
    {
        if (Volatile.Read(ref disposed) != 0)
        {
            value?.Dispose();
            return;
        }

        Volatile.Write(ref current, value);

        if (Volatile.Read(ref disposed) == 0)
        {
            return;
        }

        if (!ReferenceEquals(Interlocked.Exchange(ref current, null), value))
        {
            return;
        }

        value?.Dispose();
    }

    /// <summary>
    /// Swaps <paramref name="value"/> into the slot and disposes the value it replaced
    /// (serial semantics). If the slot is already disposed, <paramref name="value"/> is
    /// disposed immediately.
    /// </summary>
    /// <param name="current">The slot field.</param>
    /// <param name="disposed">The disposed flag field (0 = open, 1 = disposed).</param>
    /// <param name="value">The disposable to assign.</param>
    public static void SwapAndDisposePrevious(ref IDisposable? current, ref int disposed, IDisposable? value)
    {
        if (Volatile.Read(ref disposed) != 0)
        {
            value?.Dispose();
            return;
        }

        var previous = Interlocked.Exchange(ref current, value);
        previous?.Dispose();

        if (Volatile.Read(ref disposed) == 0)
        {
            return;
        }

        if (!ReferenceEquals(Interlocked.Exchange(ref current, null), value))
        {
            return;
        }

        value?.Dispose();
    }

    /// <summary>
    /// Marks the slot disposed exactly once and disposes the current value.
    /// </summary>
    /// <param name="current">The slot field.</param>
    /// <param name="disposed">The disposed flag field (0 = open, 1 = disposed).</param>
    public static void TryDispose(ref IDisposable? current, ref int disposed)
    {
        if (Interlocked.Exchange(ref disposed, 1) != 0)
        {
            return;
        }

        Interlocked.Exchange(ref current, null)?.Dispose();
    }
}
