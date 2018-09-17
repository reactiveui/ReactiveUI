// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace ReactiveUI.Legacy
{
    /// <summary>
    /// Extension methods to create collections that "follow" other collections.
    /// </summary>
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    public static class ObservableCollectionMixin
    {
        /// <summary>
        /// Creates a collection whose contents will "follow" another
        /// collection; this method is useful for creating ViewModel collections
        /// that are automatically updated when the respective Model collection
        /// is updated.
        ///
        /// Note that even though this method attaches itself to any
        /// IEnumerable, it will only detect changes from objects implementing
        /// <see cref="INotifyCollectionChanged"/> (like <see cref="ReactiveList{T}"/>).
        /// If your source collection doesn't implement this, <paramref name="signalReset"/>
        /// is the way to signal the derived collection to reorder/refilter itself.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TNew">The new type.</typeparam>
        /// <typeparam name="TDontCare">The signal type.</typeparam>
        /// <param name="this">
        /// The source <see cref="IEnumerable{T}"/> to track.
        /// </param>
        /// <param name="selector">
        /// A Select function that will be run on each item.
        /// </param>
        /// <param name="onRemoved">
        /// An action that is called on each item when it is removed.
        /// </param>
        /// <param name="filter">
        /// A filter to determine whether to exclude items in the derived collection.
        /// </param>
        /// <param name="orderer">
        /// A comparator method to determine the ordering of the resulting collection.
        /// </param>
        /// <param name="signalReset">
        /// When this Observable is signalled, the derived collection will be manually
        /// reordered/refiltered.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler used to dispatch change notifications.
        /// </param>
        /// <returns>
        /// A new collection whose items are equivalent to
        /// <c>Collection.Select().Where().OrderBy()</c> and will mirror changes
        /// in the initial collection.
        /// </returns>
        public static IReactiveDerivedList<TNew> CreateDerivedCollection<T, TNew, TDontCare>(
            this IEnumerable<T> @this,
            Func<T, TNew> selector,
            Action<TNew> onRemoved,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null,
            IObservable<TDontCare> signalReset = null,
            IScheduler scheduler = null)
        {
            Contract.Requires(selector != null);

            IObservable<Unit> reset = null;

            if (signalReset != null)
            {
                reset = signalReset.Select(_ => Unit.Default);
            }

            if (scheduler == null)
            {
                scheduler = Scheduler.Immediate;
            }

            return new ReactiveDerivedCollection<T, TNew>(@this, selector, filter, orderer, onRemoved, reset, scheduler);
        }

        /// <summary>
        /// Creates a collection whose contents will "follow" another
        /// collection; this method is useful for creating ViewModel collections
        /// that are automatically updated when the respective Model collection
        /// is updated.
        ///
        /// Note that even though this method attaches itself to any
        /// IEnumerable, it will only detect changes from objects implementing
        /// <see cref="INotifyCollectionChanged"/> (like <see cref="ReactiveList{T}"/>).
        /// If your source collection doesn't implement this, <paramref name="signalReset"/>
        /// is the way to signal the derived collection to reorder/refilter itself.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TNew">The new type.</typeparam>
        /// <typeparam name="TDontCare">The signal type.</typeparam>
        /// <param name="this">
        /// The source <see cref="IEnumerable{T}"/> to track.
        /// </param>
        /// <param name="selector">
        /// A Select function that will be run on each item.
        /// </param>
        /// <param name="filter">
        /// A filter to determine whether to exclude items in the derived collection.
        /// </param>
        /// <param name="orderer">
        /// A comparator method to determine the ordering of the resulting collection.
        /// </param>
        /// <param name="signalReset">
        /// When this Observable is signalled, the derived collection will be manually
        /// reordered/refiltered.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler used to dispatch change notifications.
        /// </param>
        /// <returns>
        /// A new collection whose items are equivalent to
        /// <c>Collection.Select().Where().OrderBy()</c> and will mirror changes
        /// in the initial collection.
        /// </returns>
        public static IReactiveDerivedList<TNew> CreateDerivedCollection<T, TNew, TDontCare>(
            this IEnumerable<T> @this,
            Func<T, TNew> selector,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null,
            IObservable<TDontCare> signalReset = null,
            IScheduler scheduler = null)
        {
            return @this.CreateDerivedCollection(selector, (Action<TNew>)null, filter, orderer, signalReset, scheduler);
        }

        /// <summary>
        /// Creates a collection whose contents will "follow" another
        /// collection; this method is useful for creating ViewModel collections
        /// that are automatically updated when the respective Model collection
        /// is updated.
        ///
        /// Be aware that this overload will result in a collection that *only*
        /// updates if the source implements INotifyCollectionChanged. If your
        /// list changes but isn't a ReactiveList/ObservableCollection,
        /// you probably want to use the other overload.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TNew">The new type.</typeparam>
        /// <param name="this">
        /// The source <see cref="IEnumerable{T}"/> to track.
        /// </param>
        /// <param name="selector">
        /// A Select function that will be run on each item.
        /// </param>
        /// <param name="onRemoved">
        /// An action that is called on each item when it is removed.
        /// </param>
        /// <param name="filter">
        /// A filter to determine whether to exclude items in the derived collection.
        /// </param>
        /// <param name="orderer">
        /// A comparator method to determine the ordering of the resulting collection.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler used to dispatch change notifications.
        /// </param>
        /// <returns>
        /// A new collection whose items are equivalent to
        /// <c>Collection.Select().Where().OrderBy()</c> and will mirror changes
        /// in the initial collection.
        /// </returns>
        public static IReactiveDerivedList<TNew> CreateDerivedCollection<T, TNew>(
            this IEnumerable<T> @this,
            Func<T, TNew> selector,
            Action<TNew> onRemoved,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null,
            IScheduler scheduler = null)
        {
            return @this.CreateDerivedCollection(selector, onRemoved, filter, orderer, (IObservable<Unit>)null, scheduler);
        }

        /// <summary>
        /// Creates a collection whose contents will "follow" another
        /// collection; this method is useful for creating ViewModel collections
        /// that are automatically updated when the respective Model collection
        /// is updated.
        ///
        /// Be aware that this overload will result in a collection that *only*
        /// updates if the source implements INotifyCollectionChanged. If your
        /// list changes but isn't a ReactiveList/ObservableCollection,
        /// you probably want to use the other overload.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TNew">The new type.</typeparam>
        /// <param name="this">
        /// The source <see cref="IEnumerable{T}"/> to track.
        /// </param>
        /// <param name="selector">
        /// A Select function that will be run on each item.
        /// </param>
        /// <param name="filter">
        /// A filter to determine whether to exclude items in the derived collection.
        /// </param>
        /// <param name="orderer">
        /// A comparator method to determine the ordering of the resulting collection.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler used to dispatch change notifications.
        /// </param>
        /// <returns>
        /// A new collection whose items are equivalent to
        /// <c>Collection.Select().Where().OrderBy()</c> and will mirror changes
        /// in the initial collection.
        /// </returns>
        public static IReactiveDerivedList<TNew> CreateDerivedCollection<T, TNew>(
            this IEnumerable<T> @this,
            Func<T, TNew> selector,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null,
            IScheduler scheduler = null)
        {
            return @this.CreateDerivedCollection(selector, default(Action<TNew>), filter, orderer, (IObservable<Unit>)null, scheduler);
        }
    }
}
