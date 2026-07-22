// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Internal;
#else
namespace ReactiveUI.Internal;
#endif
/// <summary>
/// The arity-N (no-selector) WhenAnyObservable sink: each time the observed properties
/// produce a new set of inner observables, it unsubscribes from the previous set and
/// merges the new set, forwarding every value from every current inner. The switch and
/// merge logic is written inline; there is no operator call.
/// </summary>
/// <typeparam name="TResult">The element type produced by the inner observables.</typeparam>
/// <param name="sources">The outer observable; each emission is the current set of inner observables.</param>
internal sealed class WhenAnyObservableMergeSink<TResult>(IObservable<IObservable<TResult>[]> sources) : IObservable<TResult>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<TResult> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        var sink = new Sink(observer);
        sink.Run(sources);
        return sink;
    }

    /// <summary>Tracks the active set of inner subscriptions and forwards values from all of them.</summary>
    /// <param name="downstream">The observer receiving the merged values.</param>
    private sealed class Sink(IObserver<TResult> downstream) : IDisposable
    {
#if NET9_0_OR_GREATER
        /// <summary>Serializes outer and inner notifications.</summary>
        private readonly Lock _gate = new();
#else
        /// <summary>Serializes outer and inner notifications.</summary>
        private readonly object _gate = new();
#endif

        /// <summary>Holds the current generation's inner subscriptions; disposed when a new set arrives.</summary>
        private readonly SwapDisposable _innerGroup = new();

        /// <summary>The subscription to the outer observable.</summary>
        private IDisposable? _outer;

        /// <summary>Monotonic id of the current inner set; notifications from older sets are ignored.</summary>
        private int _index;

        /// <summary>The number of current-generation inners that have not yet completed.</summary>
        private int _activeInners;

        /// <summary>Whether the current generation has any active inners.</summary>
        private bool _hasCurrent;

        /// <summary>Whether the outer observable has completed.</summary>
        private bool _outerDone;

        /// <summary>Subscribes to the outer observable.</summary>
        /// <param name="source">The outer observable of inner-observable sets.</param>
        public void Run(IObservable<IObservable<TResult>[]> source) =>
            _outer = source.Subscribe(new DelegateObserver<IObservable<TResult>[]>(OnNextOuter, OnError, OnOuterCompleted));

        /// <inheritdoc/>
        public void Dispose()
        {
            _outer?.Dispose();
            _innerGroup.Dispose();
        }

        /// <summary>Forwards an error from the outer or any current inner and tears down.</summary>
        /// <param name="error">The error to forward.</param>
        private void OnError(Exception error)
        {
            lock (_gate)
            {
                downstream.OnError(error);
            }

            Dispose();
        }

        /// <summary>Switches to a new set of inner observables, dropping the previous set, and merges it.</summary>
        /// <param name="inners">The new set of inner observables.</param>
        private void OnNextOuter(IObservable<TResult>[] inners)
        {
            int id;
            lock (_gate)
            {
                id = ++_index;
                _activeInners = inners.Length;
                _hasCurrent = inners.Length > 0;
            }

            var bag = new DisposableBag();
            for (var i = 0; i < inners.Length; i++)
            {
                bag.Add(inners[i].Subscribe(new InnerObserver(this, id)));
            }

            _innerGroup.Disposable = bag;
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

        /// <summary>Forwards a value from a current-generation inner, ignoring stale generations.</summary>
        /// <param name="id">The id of the inner set that produced the value.</param>
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

        /// <summary>Handles inner completion, completing the result when the current set and outer are done.</summary>
        /// <param name="id">The id of the inner set whose member completed.</param>
        private void OnInnerCompleted(int id)
        {
            lock (_gate)
            {
                if (id != _index)
                {
                    return;
                }

                _activeInners--;
                if (_activeInners != 0)
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

        /// <summary>Routes one inner observable's notifications back to the sink, tagged with its set id.</summary>
        /// <param name="parent">The owning sink.</param>
        /// <param name="id">The id identifying this inner subscription's generation.</param>
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
