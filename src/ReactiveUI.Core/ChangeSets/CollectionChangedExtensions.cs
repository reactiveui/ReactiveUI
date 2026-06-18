// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;

namespace ReactiveUI;

/// <summary>
/// Extensions that expose <see cref="INotifyCollectionChanged.CollectionChanged"/> as an observable, replacing
/// DynamicData's <c>ObserveCollectionChanges</c> with a single tailored event-bridge layer.
/// </summary>
public static class CollectionChangedExtensions
{
    /// <summary>Provides collection-change observation extension members for <see cref="INotifyCollectionChanged"/>.</summary>
    /// <param name="source">The collection to observe.</param>
    extension(INotifyCollectionChanged source)
    {
        /// <summary>Observes a collection's <see cref="INotifyCollectionChanged.CollectionChanged"/> event as a stream of <see cref="CollectionChanged"/> notifications.</summary>
        /// <returns>A stream of collection-changed notifications.</returns>
        public IObservable<CollectionChanged> ObserveCollectionChanges()
        {
            ArgumentExceptionHelper.ThrowIfNull(source);
            return new CollectionChangedObservable(source);
        }
    }

    /// <summary>Forwards each collection-changed event to subscribers for the lifetime of the subscription.</summary>
    /// <param name="source">The collection to observe.</param>
    private sealed class CollectionChangedObservable(INotifyCollectionChanged source) : IObservable<CollectionChanged>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<CollectionChanged> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Subscription(source, observer);
        }

        /// <summary>Attaches the collection-changed handler and detaches it on dispose.</summary>
        private sealed class Subscription : IDisposable
        {
            /// <summary>The observed collection.</summary>
            private readonly INotifyCollectionChanged _source;

            /// <summary>The observer receiving notifications.</summary>
            private readonly IObserver<CollectionChanged> _observer;

            /// <summary>Initializes a new instance of the <see cref="Subscription"/> class and hooks the event.</summary>
            /// <param name="source">The collection to observe.</param>
            /// <param name="observer">The observer receiving notifications.</param>
            public Subscription(INotifyCollectionChanged source, IObserver<CollectionChanged> observer)
            {
                _source = source;
                _observer = observer;
                _source.CollectionChanged += OnCollectionChanged;
            }

            /// <inheritdoc/>
            public void Dispose() => _source.CollectionChanged -= OnCollectionChanged;

            /// <summary>Forwards a collection-changed event to the observer.</summary>
            /// <param name="sender">The collection that raised the event.</param>
            /// <param name="e">The collection-changed event arguments.</param>
            private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
                _observer.OnNext(new(sender, e));
        }
    }
}
