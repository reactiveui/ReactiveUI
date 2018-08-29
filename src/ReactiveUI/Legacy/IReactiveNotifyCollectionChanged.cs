// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;
using System.Reactive;

#pragma warning disable SA1600 // Elements should be documented -- not used for legacy

namespace ReactiveUI.Legacy
{
    /// <summary>
    /// IReactiveNotifyCollectionChanged of T provides notifications when the contents
    /// of collection are changed (items are added/removed/moved).
    /// </summary>
    /// <typeparam name="T">The list type.</typeparam>
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    public interface IReactiveNotifyCollectionChanged<out T>
    {
        /// <summary>
        /// Fires when items are added to the collection, once per item added.
        /// Functions that add multiple items such AddRange should fire this
        /// multiple times. The object provided is the item that was added.
        /// </summary>
        IObservable<T> ItemsAdded { get; }

        /// <summary>
        /// Fires before an item is going to be added to the collection.
        /// </summary>
        IObservable<T> BeforeItemsAdded { get; }

        /// <summary>
        /// Fires once an item has been removed from a collection, providing the
        /// item that was removed.
        /// </summary>
        IObservable<T> ItemsRemoved { get; }

        /// <summary>
        /// Fires before an item will be removed from a collection, providing
        /// the item that will be removed.
        /// </summary>
        IObservable<T> BeforeItemsRemoved { get; }

        /// <summary>
        /// Fires before an items moves from one position in the collection to
        /// another, providing the item(s) to be moved as well as source and destination
        /// indices.
        /// </summary>
        IObservable<IMoveInfo<T>> BeforeItemsMoved { get; }

        /// <summary>
        /// Fires once one or more items moves from one position in the collection to
        /// another, providing the item(s) that was moved as well as source and destination
        /// indices.
        /// </summary>
        IObservable<IMoveInfo<T>> ItemsMoved { get; }

        /// <summary>
        /// This Observable is equivalent to the NotifyCollectionChanged event,
        /// but fires before the collection is changed.
        /// </summary>
        IObservable<NotifyCollectionChangedEventArgs> Changing { get; }

        /// <summary>
        /// This Observable is equivalent to the NotifyCollectionChanged event,
        /// and fires after the collection is changed.
        /// </summary>
        IObservable<NotifyCollectionChangedEventArgs> Changed { get; }

        /// <summary>
        /// Fires when the collection count changes, regardless of reason.
        /// </summary>
        IObservable<int> CountChanging { get; }

        /// <summary>
        /// Fires when the collection count changes, regardless of reason.
        /// </summary>
        IObservable<int> CountChanged { get; }

        IObservable<bool> IsEmptyChanged { get; }

        /// <summary>
        /// This Observable is fired when a ShouldReset fires on the collection. This
        /// means that you should forget your previous knowledge of the state
        /// of the collection and reread it.
        ///
        /// This does *not* mean Clear, and if you interpret it as such, you are
        /// Doing It Wrong.
        /// </summary>
        IObservable<Unit> ShouldReset { get; }

        IDisposable SuppressChangeNotifications();
    }
}
