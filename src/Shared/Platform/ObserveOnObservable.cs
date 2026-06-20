// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>
/// Delivers each notification to the downstream observer on a scheduler — a fused replacement for
/// <c>ObserveOn(scheduler)</c>. It wraps the observer rather than building an operator pipeline.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="source">The source observable.</param>
/// <param name="scheduler">The scheduler each notification is delivered on.</param>
internal sealed class ObserveOnObservable<T>(IObservable<T> source, ISequencer scheduler) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        return source.Subscribe(new Sink(observer, scheduler));
    }

    /// <summary>Reschedules each notification onto the scheduler.</summary>
    /// <param name="downstream">The downstream observer.</param>
    /// <param name="scheduler">The scheduler each notification is delivered on.</param>
    private sealed class Sink(IObserver<T> downstream, ISequencer scheduler) : IObserver<T>
    {
        /// <inheritdoc/>
        public void OnNext(T value) =>
            scheduler.Schedule((Downstream: downstream, Value: value), static (_, state) =>
            {
                state.Downstream.OnNext(state.Value);
                return EmptyDisposable.Instance;
            });

        /// <inheritdoc/>
        public void OnError(Exception error) =>
            scheduler.Schedule((Downstream: downstream, Error: error), static (_, state) =>
            {
                state.Downstream.OnError(state.Error);
                return EmptyDisposable.Instance;
            });

        /// <inheritdoc/>
        public void OnCompleted() =>
            scheduler.Schedule(downstream, static (_, observer) =>
            {
                observer.OnCompleted();
                return EmptyDisposable.Instance;
            });
    }
}
