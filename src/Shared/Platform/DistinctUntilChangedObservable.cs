// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Forwards only values that differ from the previously forwarded one — a fused, allocation-light replacement for
/// <c>DistinctUntilChanged()</c>. It is a single pass-through observer with no buffering and relies on the Rx
/// single-threaded delivery contract, so no locking is required.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="source">The source observable.</param>
internal sealed class DistinctUntilChangedObservable<T>(IObservable<T> source) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        return source.Subscribe(new Sink(observer));
    }

    /// <summary>Suppresses consecutive duplicate values.</summary>
    /// <param name="downstream">The downstream observer.</param>
    private sealed class Sink(IObserver<T> downstream) : IObserver<T>
    {
        /// <summary>The previously forwarded value (valid only once <see cref="_hasLast"/> is set).</summary>
        private T _last = default!;

        /// <summary>Whether a value has been forwarded yet.</summary>
        private bool _hasLast;

        /// <inheritdoc/>
        public void OnNext(T value)
        {
            if (_hasLast && EqualityComparer<T>.Default.Equals(_last, value))
            {
                return;
            }

            _last = value;
            _hasLast = true;
            downstream.OnNext(value);
        }

        /// <inheritdoc/>
        public void OnError(Exception error) => downstream.OnError(error);

        /// <inheritdoc/>
        public void OnCompleted() => downstream.OnCompleted();
    }
}
