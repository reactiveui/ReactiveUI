// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Projects each source value to an inner observable and merges every inner observable's values downstream — a fused,
/// allocation-light replacement for <c>SelectMany</c> (merge semantics). Per-source completion is ignored; the sink
/// completes only via dispose, and disposing tears down the source and every live inner subscription.
/// </summary>
/// <typeparam name="TSource">The source element type.</typeparam>
/// <typeparam name="TResult">The merged result element type.</typeparam>
/// <param name="source">The source whose values select inner observables.</param>
/// <param name="selector">Projects each source value into an inner observable to merge.</param>
internal sealed class SelectManyObservable<TSource, TResult>(
    IObservable<TSource> source,
    Func<TSource, IObservable<TResult>> selector) : IObservable<TResult>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<TResult> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        var sink = new Sink(observer, selector);
        sink.Run(source);
        return sink;
    }

    /// <summary>Subscribes to each selected inner observable and merges their values downstream.</summary>
    /// <param name="downstream">The downstream observer.</param>
    /// <param name="selector">Projects each source value into an inner observable.</param>
    private sealed class Sink(IObserver<TResult> downstream, Func<TSource, IObservable<TResult>> selector)
        : IObserver<TSource>, IDisposable
    {
        /// <summary>Serializes inner subscription bookkeeping and downstream forwarding.</summary>
#if NET9_0_OR_GREATER
        private readonly Lock _gate = new();
#else
        private readonly object _gate = new();
#endif

        /// <summary>The live inner subscriptions, torn down on dispose or as each inner completes.</summary>
        private readonly List<IDisposable> _innerSubscriptions = [];

        /// <summary>The source subscription, torn down on dispose.</summary>
        private IDisposable? _sourceSubscription;

        /// <summary>Whether the sink has been disposed.</summary>
        private bool _disposed;

        /// <summary>Subscribes to the source.</summary>
        /// <param name="source">The source.</param>
        public void Run(IObservable<TSource> source) => _sourceSubscription = source.Subscribe(this);

        /// <inheritdoc/>
        public void OnNext(TSource value)
        {
            var inner = selector(value);
            var innerObserver = new InnerObserver(this);
            lock (_gate)
            {
                if (_disposed)
                {
                    return;
                }

                var subscription = inner.Subscribe(innerObserver);
                innerObserver.Subscription = subscription;
                _innerSubscriptions.Add(subscription);
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
                _disposed = true;
                _sourceSubscription?.Dispose();
                foreach (var subscription in _innerSubscriptions)
                {
                    subscription.Dispose();
                }

                _innerSubscriptions.Clear();
            }
        }

        /// <summary>Forwards a value from an inner observable.</summary>
        /// <param name="value">The inner value.</param>
        private void InnerNext(TResult value)
        {
            lock (_gate)
            {
                downstream.OnNext(value);
            }
        }

        /// <summary>Forwards an inner error downstream.</summary>
        /// <param name="error">The error.</param>
        private void InnerError(Exception error)
        {
            lock (_gate)
            {
                downstream.OnError(error);
            }
        }

        /// <summary>Removes and disposes a completed inner subscription.</summary>
        /// <param name="observer">The completed inner observer.</param>
        private void InnerCompleted(InnerObserver observer)
        {
            lock (_gate)
            {
                if (observer.Subscription is null)
                {
                    return;
                }

                _innerSubscriptions.Remove(observer.Subscription);
                observer.Subscription.Dispose();
            }
        }

        /// <summary>Observes a single inner observable.</summary>
        /// <param name="parent">The owning sink.</param>
        private sealed class InnerObserver(Sink parent) : IObserver<TResult>
        {
            /// <summary>Gets or sets the subscription for this inner observable.</summary>
            public IDisposable? Subscription { get; set; }

            /// <inheritdoc/>
            public void OnNext(TResult value) => parent.InnerNext(value);

            /// <inheritdoc/>
            public void OnError(Exception error) => parent.InnerError(error);

            /// <inheritdoc/>
            public void OnCompleted() => parent.InnerCompleted(this);
        }
    }
}
