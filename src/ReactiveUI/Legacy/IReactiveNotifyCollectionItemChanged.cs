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
        /// Gets an observable that signals when item changing notifications have occurred for any item in collection that
        /// implements IReactiveNotifyPropertyChanged. This is only enabled when
        /// ChangeTrackingEnabled is set to True.
        /// </summary>
        IObservable<IReactivePropertyChangedEventArgs<TSender>> ItemChanging { get; }

        /// <summary>
        /// Gets an observable that signals when item change notifications have occurred Item Changed notifications for any item in collection that
        /// implements IReactiveNotifyPropertyChanged. This is only enabled when
        /// ChangeTrackingEnabled is set to True.
        /// </summary>
        IObservable<IReactivePropertyChangedEventArgs<TSender>> ItemChanged { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to track if contained items INotifyPropertyChanged events have been triggered.
        /// </summary>
        bool ChangeTrackingEnabled { get; set; }
    }
}
