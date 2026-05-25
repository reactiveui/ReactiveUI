// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Invokes a side-effect for each value before forwarding it downstream — a fused replacement for <c>Do(onNext)</c>.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="source">The source observable.</param>
/// <param name="onNext">The side-effect invoked for each value before it is forwarded.</param>
internal sealed class DoObservable<T>(IObservable<T> source, Action<T> onNext) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        return source.Subscribe(new Sink(observer, onNext));
    }

    /// <summary>Runs the side-effect then forwards each value.</summary>
    /// <param name="downstream">The downstream observer.</param>
    /// <param name="onNext">The side-effect.</param>
    private sealed class Sink(IObserver<T> downstream, Action<T> onNext) : IObserver<T>
    {
        /// <inheritdoc/>
        public void OnNext(T value)
        {
            onNext(value);
            downstream.OnNext(value);
        }

        /// <inheritdoc/>
        public void OnError(Exception error) => downstream.OnError(error);

        /// <inheritdoc/>
        public void OnCompleted() => downstream.OnCompleted();
    }
}
