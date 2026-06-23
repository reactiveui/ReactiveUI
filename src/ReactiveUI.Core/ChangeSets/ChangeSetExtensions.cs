// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Extensions that turn observable collections into a stream of <see cref="IReactiveChangeSet{T}"/> batches, replacing
/// the DynamicData change-set surface used inside ReactiveUI with a tailored, allocation-light layer.
/// </summary>
public static class ChangeSetExtensions
{
    /// <summary>Provides change-set observation extension members for <see cref="ObservableCollection{T}"/>.</summary>
    /// <typeparam name="T">The collection item type.</typeparam>
    /// <param name="collection">The collection to observe.</param>
    extension<T>(ObservableCollection<T> collection)
    {
        /// <summary>
        /// Observes an <see cref="ObservableCollection{T}"/> as a change-set stream, emitting an initial batch for the
        /// current items and then one batch per collection change.
        /// </summary>
        /// <returns>A change-set stream.</returns>
        public IObservable<IReactiveChangeSet<T>> ToReactiveChangeSet()
        {
            ArgumentExceptionHelper.ThrowIfNull(collection);
            return new ChangeSetObservable<ObservableCollection<T>, T>(collection);
        }
    }

    /// <summary>Provides change-set observation extension members for collections that raise <see cref="INotifyCollectionChanged"/>.</summary>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="T">The collection item type.</typeparam>
    /// <param name="collection">The collection to observe.</param>
    extension<TCollection, T>(TCollection collection)
        where TCollection : INotifyCollectionChanged, IEnumerable<T>
    {
        /// <summary>
        /// Observes a collection that raises <see cref="INotifyCollectionChanged"/> as a change-set stream, emitting an
        /// initial batch for the current items and then one batch per collection change.
        /// </summary>
        /// <returns>A change-set stream.</returns>
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "T is the element type and is supplied explicitly by callers; it cannot be inferred from the collection parameter.")]
        public IObservable<IReactiveChangeSet<T>> ToReactiveChangeSet()
        {
            ArgumentExceptionHelper.ThrowIfNull(collection);
            return new ChangeSetObservable<TCollection, T>(collection);
        }
    }

    /// <summary>
    /// Observes a notifying collection and translates each <see cref="NotifyCollectionChangedEventArgs"/> into a
    /// flattened per-item change set, maintaining a shadow copy to service the initial batch and reset translation.
    /// </summary>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="T">The collection item type.</typeparam>
    /// <param name="collection">The collection to observe.</param>
    private sealed class ChangeSetObservable<TCollection, T>(TCollection collection) : IObservable<IReactiveChangeSet<T>>
        where TCollection : INotifyCollectionChanged, IEnumerable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IReactiveChangeSet<T>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            var subscription = new Subscription(collection, observer);
            subscription.Start();
            return subscription;
        }

        /// <summary>Hooks the collection, emits the initial batch, and translates subsequent changes.</summary>
        /// <param name="collection">The collection to observe.</param>
        /// <param name="observer">The observer receiving change sets.</param>
        private sealed class Subscription(TCollection collection, IObserver<IReactiveChangeSet<T>> observer) : IDisposable
        {
            /// <summary>Floor capacity for a per-event change list so the first <c>Add</c> doesn't immediately
            /// reallocate the backing array when the change count is unknown or trivial.</summary>
            private const int DefaultChangeCapacity = 4;

            /// <summary>Shadow copy of the collection, kept in sync to service resets and indices.</summary>
            private readonly List<T> _shadow = [];

            /// <summary>Emits the initial batch for the current items and begins observing changes.</summary>
            public void Start()
            {
                _shadow.AddRange(collection);

                // Emit an initial batch on subscribe even when the collection is empty, matching the
                // DynamicData ToObservableChangeSet() behaviour the change layer replaced. Downstream projections
                // such as RoutingState.CurrentViewModel rely on this seed emission to surface the current value
                // (e.g. a null "no current view model") immediately on subscription. The empty change set carries
                // no adds/removes, so count-gated consumers (WhenCountChanged) correctly ignore it.
                var initial = new List<ReactiveChange<T>>(_shadow.Count);
                for (var i = 0; i < _shadow.Count; i++)
                {
                    initial.Add(new(ReactiveChangeReason.Add, _shadow[i], default, i, -1));
                }

                observer.OnNext(new ReactiveChangeSet<T>(initial));

                collection.CollectionChanged += OnCollectionChanged;
            }

            /// <inheritdoc/>
            public void Dispose() => collection.CollectionChanged -= OnCollectionChanged;

            /// <summary>Translates a collection-changed event into a change set and forwards it.</summary>
            /// <param name="sender">The collection.</param>
            /// <param name="e">The collection-changed event arguments.</param>
            private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                // Pre-size the change list from the known item count so the per-change Add calls don't trigger
                // List growth/resize churn.
                var changes = new List<ReactiveChange<T>>(GetChangeCapacity(e));
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            ApplyAdd(e, changes);
                            break;
                        }

                    case NotifyCollectionChangedAction.Remove:
                        {
                            ApplyRemove(e, changes);
                            break;
                        }

                    case NotifyCollectionChangedAction.Replace:
                        {
                            ApplyReplace(e, changes);
                            break;
                        }

                    case NotifyCollectionChangedAction.Move:
                        {
                            ApplyMove(e, changes);
                            break;
                        }

                    case NotifyCollectionChangedAction.Reset:
                        {
                            ApplyReset(changes);
                            break;
                        }
                }

                if (changes.Count == 0)
                {
                    return;
                }

                observer.OnNext(new ReactiveChangeSet<T>(changes));
            }

            /// <summary>Computes a starting capacity for the per-event change list from the known item count.</summary>
            /// <param name="e">The collection-changed event arguments.</param>
            /// <returns>A capacity of at least <see cref="DefaultChangeCapacity"/>.</returns>
            /// <remarks>A reset emits a remove for every prior item then an add for every surviving one, so its
            /// worst case is twice the current shadow count.</remarks>
            private int GetChangeCapacity(NotifyCollectionChangedEventArgs e)
            {
                var added = e.NewItems?.Count ?? 0;
                var removed = e.OldItems?.Count ?? 0;
                var count = e.Action == NotifyCollectionChangedAction.Reset ? _shadow.Count * 2 : added + removed;
                return count < DefaultChangeCapacity ? DefaultChangeCapacity : count;
            }

            /// <summary>Records added items into the shadow and the change list.</summary>
            /// <param name="e">The collection-changed event arguments.</param>
            /// <param name="changes">The change list being built.</param>
            private void ApplyAdd(NotifyCollectionChangedEventArgs e, List<ReactiveChange<T>> changes)
            {
                if (e.NewItems is null)
                {
                    return;
                }

                var start = e.NewStartingIndex;
                for (var i = 0; i < e.NewItems.Count; i++)
                {
                    var item = (T)e.NewItems[i]!;
                    var index = start < 0 ? _shadow.Count : start + i;
                    _shadow.Insert(index, item);
                    changes.Add(new(ReactiveChangeReason.Add, item, default, index, -1));
                }
            }

            /// <summary>Records removed items out of the shadow and into the change list.</summary>
            /// <param name="e">The collection-changed event arguments.</param>
            /// <param name="changes">The change list being built.</param>
            private void ApplyRemove(NotifyCollectionChangedEventArgs e, List<ReactiveChange<T>> changes)
            {
                if (e.OldItems is null)
                {
                    return;
                }

                var start = e.OldStartingIndex;
                for (var i = 0; i < e.OldItems.Count; i++)
                {
                    var item = (T)e.OldItems[i]!;
                    var index = start < 0 ? -1 : start;
                    RemoveFromShadow(index, item);
                    changes.Add(new(ReactiveChangeReason.Remove, item, default, index, -1));
                }
            }

            /// <summary>Records replaced items in the shadow and the change list.</summary>
            /// <param name="e">The collection-changed event arguments.</param>
            /// <param name="changes">The change list being built.</param>
            private void ApplyReplace(NotifyCollectionChangedEventArgs e, List<ReactiveChange<T>> changes)
            {
                if (e.NewItems is null || e.OldItems is null)
                {
                    return;
                }

                var start = e.NewStartingIndex;
                for (var i = 0; i < e.NewItems.Count; i++)
                {
                    var current = (T)e.NewItems[i]!;
                    var previous = (T)e.OldItems[i]!;
                    var index = start < 0 ? -1 : start + i;
                    if (index >= 0 && index < _shadow.Count)
                    {
                        _shadow[index] = current;
                    }

                    changes.Add(new(ReactiveChangeReason.Replace, current, previous, index, -1));
                }
            }

            /// <summary>Records a moved item in the shadow and the change list.</summary>
            /// <param name="e">The collection-changed event arguments.</param>
            /// <param name="changes">The change list being built.</param>
            private void ApplyMove(NotifyCollectionChangedEventArgs e, List<ReactiveChange<T>> changes)
            {
                if (e.NewItems is null)
                {
                    return;
                }

                var item = (T)e.NewItems[0]!;
                if (e.OldStartingIndex >= 0 && e.OldStartingIndex < _shadow.Count)
                {
                    _shadow.RemoveAt(e.OldStartingIndex);
                }

                var index = e.NewStartingIndex < 0 ? _shadow.Count : e.NewStartingIndex;
                _shadow.Insert(index, item);
                changes.Add(new(ReactiveChangeReason.Move, item, default, index, e.OldStartingIndex));
            }

            /// <summary>Re-syncs the shadow against the collection's current contents on a reset.</summary>
            /// <param name="changes">The change list being built.</param>
            /// <remarks>A reset means "re-read the collection from scratch": some sources (e.g. DynamicData's
            /// <c>ObservableCollectionExtended</c> resuming suspended notifications) raise a reset while the items
            /// remain present. Emit a remove for every prior item, then an add for every item still present, so
            /// surviving items are re-subscribed rather than dropped.</remarks>
            private void ApplyReset(List<ReactiveChange<T>> changes)
            {
                for (var i = 0; i < _shadow.Count; i++)
                {
                    changes.Add(new(ReactiveChangeReason.Remove, _shadow[i], default, i, -1));
                }

                _shadow.Clear();
                _shadow.AddRange(collection);

                for (var i = 0; i < _shadow.Count; i++)
                {
                    changes.Add(new(ReactiveChangeReason.Add, _shadow[i], default, i, -1));
                }
            }

            /// <summary>Removes an item from the shadow by index when valid, otherwise by value.</summary>
            /// <param name="index">The index to remove at, or -1 to remove by value.</param>
            /// <param name="item">The item to remove by value when the index is unusable.</param>
            private void RemoveFromShadow(int index, T item)
            {
                if (index >= 0 && index < _shadow.Count)
                {
                    _shadow.RemoveAt(index);
                    return;
                }

                _ = _shadow.Remove(item);
            }
        }
    }
}
