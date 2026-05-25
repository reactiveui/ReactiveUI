// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Forwards the values of every source and completes once all complete (the no-allocation fused replacement for
/// Rx <c>Merge</c>). Shared across the binding internals.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="sources">The sources to merge.</param>
internal sealed class MergeObservable<T>(IObservable<T>[] sources) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        return new Sink(observer, sources);
    }

    /// <summary>Forwards every source value under a gate and completes once all sources complete.</summary>
    private sealed class Sink : IDisposable
    {
        /// <summary>Guards downstream delivery and the completion counter.</summary>
        #if NET9_0_OR_GREATER
        private readonly Lock _gate = new();
        #else
        private readonly object _gate = new();
        #endif

        /// <summary>The observer receiving the merged values.</summary>
        private readonly IObserver<T> _downstream;

        /// <summary>The subscriptions to each source.</summary>
        private readonly IDisposable?[] _subscriptions;

        /// <summary>The number of sources.</summary>
        private readonly int _count;

        /// <summary>The number of sources that have completed.</summary>
        private int _doneCount;

        /// <summary>Whether the downstream has terminated.</summary>
        private bool _stopped;

        /// <summary>Initializes a new instance of the <see cref="Sink"/> class and subscribes to every source.</summary>
        /// <param name="downstream">The observer receiving the merged values.</param>
        /// <param name="sources">The sources to merge.</param>
        public Sink(IObserver<T> downstream, IObservable<T>[] sources)
        {
            _downstream = downstream;
            _count = sources.Length;
            _subscriptions = new IDisposable?[sources.Length];
            for (var i = 0; i < sources.Length; i++)
            {
                _subscriptions[i] = sources[i].Subscribe(new Element(this));
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            for (var i = 0; i < _subscriptions.Length; i++)
            {
                _subscriptions[i]?.Dispose();
            }
        }

        /// <summary>Forwards one source value to the downstream.</summary>
        /// <param name="value">The value to forward.</param>
        private void OnNextAt(T value)
        {
            lock (_gate)
            {
                if (_stopped)
                {
                    return;
                }

                _downstream.OnNext(value);
            }
        }

        /// <summary>Forwards an error from any source.</summary>
        /// <param name="error">The error to forward.</param>
        private void OnErrorAt(Exception error)
        {
            lock (_gate)
            {
                if (_stopped)
                {
                    return;
                }

                _stopped = true;
            }

            _downstream.OnError(error);
        }

        /// <summary>Completes the downstream once every source has completed.</summary>
        private void OnCompletedAt()
        {
            lock (_gate)
            {
                if (_stopped || ++_doneCount < _count)
                {
                    return;
                }

                _stopped = true;
            }

            _downstream.OnCompleted();
        }

        /// <summary>Routes one source's notifications to the parent sink.</summary>
        /// <param name="parent">The owning sink.</param>
        private sealed class Element(Sink parent) : IObserver<T>
        {
            /// <inheritdoc/>
            public void OnNext(T value) => parent.OnNextAt(value);

            /// <inheritdoc/>
            public void OnError(Exception error) => parent.OnErrorAt(error);

            /// <inheritdoc/>
            public void OnCompleted() => parent.OnCompletedAt();
        }
    }
}
