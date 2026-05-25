// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Flattens a stream of observables by always forwarding values from the most recent inner observable, disposing the
/// previous inner subscription whenever a new one arrives — a fused, allocation-light replacement for <c>Switch</c>.
/// Per-source completion is ignored; the sink completes only via dispose.
/// </summary>
/// <typeparam name="T">The element type produced by the inner observables.</typeparam>
/// <param name="sources">The stream of inner observables to switch between.</param>
internal sealed class SwitchObservable<T>(IObservable<IObservable<T>> sources) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        var sink = new Sink(observer);
        sink.Run(sources);
        return sink;
    }

    /// <summary>Tracks the current inner subscription and forwards its values downstream.</summary>
    /// <param name="downstream">The downstream observer.</param>
    private sealed class Sink(IObserver<T> downstream) : IObserver<IObservable<T>>, IDisposable
    {
        /// <summary>Serializes switching the inner subscription and forwarding downstream.</summary>
#if NET9_0_OR_GREATER
        private readonly Lock _gate = new();
#else
        private readonly object _gate = new();
#endif

        /// <summary>The outer subscription, torn down on dispose.</summary>
        private IDisposable? _outerSubscription;

        /// <summary>The current inner subscription, replaced on each new inner observable and torn down on dispose.</summary>
        private IDisposable? _innerSubscription;

        /// <summary>Subscribes to the outer stream of observables.</summary>
        /// <param name="sources">The stream of inner observables.</param>
        public void Run(IObservable<IObservable<T>> sources) => _outerSubscription = sources.Subscribe(this);

        /// <inheritdoc/>
        public void OnNext(IObservable<T> inner)
        {
            var innerObserver = new InnerObserver(this);
            lock (_gate)
            {
                _innerSubscription?.Dispose();
                _innerSubscription = inner.Subscribe(innerObserver);
            }
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
            lock (_gate)
            {
                downstream.OnError(error);
            }
        }

        /// <inheritdoc/>
        public void OnCompleted()
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_gate)
            {
                _innerSubscription?.Dispose();
                _outerSubscription?.Dispose();
            }
        }

        /// <summary>Forwards a value from the current inner observable.</summary>
        /// <param name="value">The inner value.</param>
        private void ForwardNext(T value)
        {
            lock (_gate)
            {
                downstream.OnNext(value);
            }
        }

        /// <summary>Observes the current inner observable.</summary>
        /// <param name="parent">The owning sink.</param>
        private sealed class InnerObserver(Sink parent) : IObserver<T>
        {
            /// <inheritdoc/>
            public void OnNext(T value) => parent.ForwardNext(value);

            /// <inheritdoc/>
            public void OnError(Exception error) => parent.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted()
            {
            }
        }
    }
}
