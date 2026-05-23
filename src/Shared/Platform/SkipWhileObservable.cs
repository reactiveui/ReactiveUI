// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Suppresses values from a source while a predicate holds, then forwards every value once the predicate first fails —
/// a fused, allocation-light replacement for <c>SkipWhile</c>.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="source">The source to filter.</param>
/// <param name="predicate">While this returns true the value is skipped; once it returns false all subsequent values flow.</param>
internal sealed class SkipWhileObservable<T>(IObservable<T> source, Func<T, bool> predicate) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        return source.Subscribe(new Sink(observer, predicate));
    }

    /// <summary>Skips leading values while the predicate holds, then forwards the rest.</summary>
    /// <param name="downstream">The downstream observer.</param>
    /// <param name="predicate">The skip predicate.</param>
    private sealed class Sink(IObserver<T> downstream, Func<T, bool> predicate) : IObserver<T>
    {
        /// <summary>Whether values are still being skipped.</summary>
        private bool _skipping = true;

        /// <inheritdoc/>
        public void OnNext(T value)
        {
            if (_skipping)
            {
                if (predicate(value))
                {
                    return;
                }

                _skipping = false;
            }

            downstream.OnNext(value);
        }

        /// <inheritdoc/>
        public void OnError(Exception error) => downstream.OnError(error);

        /// <inheritdoc/>
        public void OnCompleted() => downstream.OnCompleted();
    }
}
