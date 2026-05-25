// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Forwards only values that satisfy a predicate — a fused, allocation-light replacement for <c>Where</c>. It adds a
/// single observer layer with no operator pipeline, forwarding errors and completion unchanged.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="source">The source to filter.</param>
/// <param name="predicate">Returns true for values that should be forwarded.</param>
internal sealed class WhereObservable<T>(IObservable<T> source, Func<T, bool> predicate) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        return source.Subscribe(new Sink(observer, predicate));
    }

    /// <summary>Forwards values that satisfy the predicate.</summary>
    /// <param name="downstream">The downstream observer.</param>
    /// <param name="predicate">The filter predicate.</param>
    private sealed class Sink(IObserver<T> downstream, Func<T, bool> predicate) : IObserver<T>
    {
        /// <inheritdoc/>
        public void OnNext(T value)
        {
            if (!predicate(value))
            {
                return;
            }

            downstream.OnNext(value);
        }

        /// <inheritdoc/>
        public void OnError(Exception error) => downstream.OnError(error);

        /// <inheritdoc/>
        public void OnCompleted() => downstream.OnCompleted();
    }
}
