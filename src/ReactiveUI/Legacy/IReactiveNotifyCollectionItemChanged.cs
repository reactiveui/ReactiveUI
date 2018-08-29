// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ReactiveUI.Legacy
{
    /// <summary>
    /// IReactiveNotifyCollectionItemChanged provides notifications for collection item updates, ie when an object in
    /// a collection changes.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
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
}
