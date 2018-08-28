// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
using System.Windows.Input;
using Splat;

namespace ReactiveUI.Legacy
{
    /// <summary>
    /// IReactiveNotifyCollectionItemChanged provides notifications for collection item updates, ie when an object in
    /// a collection changes.
    /// </summary>
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    public interface IReactiveNotifyCollectionItemChanged<out TSender>
    {
        /// <summary>
        /// Provides Item Changing notifications for any item in collection that
        /// implements IReactiveNotifyPropertyChanged. This is only enabled when
        /// ChangeTrackingEnabled is set to True.
        /// </summary>
        IObservable<IReactivePropertyChangedEventArgs<TSender>> ItemChanging { get; }

        /// <summary>
        /// Provides Item Changed notifications for any item in collection that
        /// implements IReactiveNotifyPropertyChanged. This is only enabled when
        /// ChangeTrackingEnabled is set to True.
        /// </summary>
        IObservable<IReactivePropertyChangedEventArgs<TSender>> ItemChanged { get; }

        /// <summary>
        /// Enables the ItemChanging and ItemChanged properties; when this is
        /// enabled, whenever a property on any object implementing
        /// IReactiveNotifyPropertyChanged changes, the change will be
        /// rebroadcast through ItemChanging/ItemChanged.
        /// </summary>
        bool ChangeTrackingEnabled { get; set; }
    }

    /// <summary>
    /// IReactiveNotifyCollectionChanged of T provides notifications when the contents
    /// of collection are changed (items are added/removed/moved).
    /// </summary>
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
        /// but fires before the collection is changed
        /// </summary>
        IObservable<NotifyCollectionChangedEventArgs> Changing { get; }

        /// <summary>
        /// This Observable is equivalent to the NotifyCollectionChanged event,
        /// and fires after the collection is changed
        /// </summary>
        IObservable<NotifyCollectionChangedEventArgs> Changed { get; }

        /// <summary>
        /// Fires when the collection count changes, regardless of reason
        /// </summary>
        IObservable<int> CountChanging { get; }

        /// <summary>
        /// Fires when the collection count changes, regardless of reason
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

    /// IReactiveCollection of T represents a collection that can notify when its
    /// contents are changed (either items are added/removed, or the object
    /// itself changes).
    ///
    /// It is important to implement the Changing/Changed from
    /// IReactiveNotifyPropertyChanged semantically as "Fire when *anything* in
    /// the collection or any of its items have changed, in any way".
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    public interface IReactiveCollection<out T> : IReactiveNotifyCollectionChanged<T>, IReactiveNotifyCollectionItemChanged<T>, IEnumerable<T>, INotifyCollectionChanged, INotifyCollectionChanging, IReactiveObject
    {
        /// <summary>
        ///
        /// </summary>
        void Reset();
    }

    /// <summary>
    /// IReadOnlyReactiveCollection of T represents a read-only collection that can notify when its
    /// contents are changed (either items are added/removed, or the object
    /// itself changes).
    ///
    /// It is important to implement the Changing/Changed from
    /// IReactiveNotifyPropertyChanged semantically as "Fire when *anything* in
    /// the collection or any of its items have changed, in any way".
    /// </summary>
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    public interface IReadOnlyReactiveCollection<out T> : IReadOnlyCollection<T>, IReactiveCollection<T>
    {
    }

    /// <summary>
    /// IReadOnlyReactiveList of T represents a read-only list that can notify when its
    /// contents are changed (either items are added/removed, or the object
    /// itself changes).
    ///
    /// It is important to implement the Changing/Changed from
    /// IReactiveNotifyPropertyChanged semantically as "Fire when *anything* in
    /// the collection or any of its items have changed, in any way".
    /// </summary>
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    public interface IReadOnlyReactiveList<out T> : IReadOnlyReactiveCollection<T>, IReadOnlyList<T>
    {
        bool IsEmpty { get; }
    }

    /// <summary>
    /// IReactiveDerivedList represents a collection whose contents will "follow" another
    /// collection; this method is useful for creating ViewModel collections
    /// that are automatically updated when the respective Model collection is updated.
    /// </summary>
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    public interface IReactiveDerivedList<out T> : IReadOnlyReactiveList<T>, IDisposable
    {
    }

    /// <summary>
    /// IReactiveList of T represents a list that can notify when its
    /// contents are changed (either items are added/removed, or the object
    /// itself changes).
    ///
    /// It is important to implement the Changing/Changed from
    /// IReactiveNotifyPropertyChanged semantically as "Fire when *anything* in
    /// the collection or any of its items have changed, in any way".
    /// </summary>
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    public interface IReactiveList<T> : IReactiveCollection<T>, IList<T>
    {
        bool IsEmpty { get; }

        void AddRange(IEnumerable<T> collection);

        void InsertRange(int index, IEnumerable<T> collection);

        void RemoveAll(IEnumerable<T> items);

        void RemoveRange(int index, int count);

        void Sort(IComparer<T> comparer = null);

        void Sort(Comparison<T> comparison);

        void Sort(int index, int count, IComparer<T> comparer);
    }    
}