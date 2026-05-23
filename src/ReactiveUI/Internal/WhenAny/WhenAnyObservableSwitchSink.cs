// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// The arity-1 WhenAnyObservable sink: observes an outer stream of inner observables and
/// forwards values from the most recent inner, unsubscribing from the previous inner each
/// time a new one arrives. The switch logic is written inline; there is no operator call.
/// </summary>
/// <typeparam name="TResult">The element type produced by the inner observables.</typeparam>
/// <param name="sources">The outer observable whose latest inner observable is subscribed.</param>
internal sealed class WhenAnyObservableSwitchSink<TResult>(IObservable<IObservable<TResult>> sources) : IObservable<TResult>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<TResult> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        var sink = new Sink(observer);
        sink.Run(sources);
        return sink;
    }

    /// <summary>
    /// Tracks the active inner subscription and forwards only the latest inner's values.
    /// </summary>
    /// <param name="downstream">The observer receiving the switched values.</param>
    private sealed class Sink(IObserver<TResult> downstream) : IDisposable
    {
#if NET9_0_OR_GREATER
        /// <summary>Serializes outer and inner notifications.</summary>
        private readonly Lock _gate = new();
#else
        /// <summary>Serializes outer and inner notifications.</summary>
        private readonly object _gate = new();
#endif

        /// <summary>Holds the current inner subscription; disposed when a new inner arrives.</summary>
        private readonly SwapDisposable _inner = new();

        /// <summary>The subscription to the outer observable.</summary>
        private IDisposable? _outer;

        /// <summary>Monotonic id of the current inner; notifications from older inners are ignored.</summary>
        private int _index;

        /// <summary>Whether an inner observable is currently active.</summary>
        private bool _hasCurrent;

        /// <summary>Whether the outer observable has completed.</summary>
        private bool _outerDone;

        /// <summary>Subscribes to the outer observable.</summary>
        /// <param name="source">The outer observable of inner observables.</param>
        public void Run(IObservable<IObservable<TResult>> source) =>
            _outer = source.Subscribe(new DelegateObserver<IObservable<TResult>>(OnNextOuter, OnError, OnOuterCompleted));

        /// <summary>Forwards an error from the outer or current inner and tears down.</summary>
        /// <param name="error">The error to forward.</param>
        public void OnError(Exception error)
        {
            lock (_gate)
            {
                downstream.OnError(error);
            }

            Dispose();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _outer?.Dispose();
            _inner.Dispose();
        }

        /// <summary>Switches to a newly produced inner observable, dropping the previous one.</summary>
        /// <param name="inner">The new inner observable to subscribe.</param>
        private void OnNextOuter(IObservable<TResult> inner)
        {
            int id;
            lock (_gate)
            {
                id = ++_index;
                _hasCurrent = true;
            }

            _inner.Disposable = inner.Subscribe(new InnerObserver(this, id));
        }

        /// <summary>Records outer completion and completes the result if no inner is active.</summary>
        private void OnOuterCompleted()
        {
            lock (_gate)
            {
                _outerDone = true;
                if (!_hasCurrent)
                {
                    downstream.OnCompleted();
                }
            }
        }

        /// <summary>Forwards a value from the current inner, ignoring stale inners.</summary>
        /// <param name="id">The id of the inner that produced the value.</param>
        /// <param name="value">The value to forward.</param>
        private void OnNextInner(int id, TResult value)
        {
            lock (_gate)
            {
                if (id != _index)
                {
                    return;
                }

                downstream.OnNext(value);
            }
        }

        /// <summary>Handles completion of the current inner, completing the result if the outer is done.</summary>
        /// <param name="id">The id of the inner that completed.</param>
        private void OnInnerCompleted(int id)
        {
            lock (_gate)
            {
                if (id != _index)
                {
                    return;
                }

                _hasCurrent = false;
                if (_outerDone)
                {
                    downstream.OnCompleted();
                }
            }
        }

        /// <summary>Routes one inner observable's notifications back to the sink, tagged with its id.</summary>
        /// <param name="parent">The owning sink.</param>
        /// <param name="id">The id identifying this inner subscription.</param>
        private sealed class InnerObserver(Sink parent, int id) : IObserver<TResult>
        {
            /// <inheritdoc/>
            public void OnNext(TResult value) => parent.OnNextInner(id, value);

            /// <inheritdoc/>
            public void OnError(Exception error) => parent.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => parent.OnInnerCompleted(id);
        }
    }
}
