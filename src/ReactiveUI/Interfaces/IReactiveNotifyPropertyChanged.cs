// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// IReactiveNotifyPropertyChanged represents an extended version of
/// INotifyPropertyChanged that also exposes typed Observables.
/// </summary>
/// <typeparam name="TSender">The sender type.</typeparam>
public interface IReactiveNotifyPropertyChanged<out TSender>
{
    /// <summary>
    /// Gets an observable that fires *before* a property is about to
    /// be changed. Note that this should not fire duplicate change notifications if a
    /// property is set to the same value multiple times.
    /// </summary>
    IObservable<IReactivePropertyChangedEventArgs<TSender>> Changing { get; }

    /// <summary>
    /// Gets an Observable that fires *after* a property has changed.
    /// Note that this should not fire duplicate change notifications if a
    /// property is set to the same value multiple times.
    /// </summary>
    IObservable<IReactivePropertyChangedEventArgs<TSender>> Changed { get; }

    /// <summary>
    /// When this method is called, an object will not fire change
    /// notifications (neither traditional nor Observable notifications)
    /// until the return value is disposed.
    /// </summary>
    /// <returns>An object that, when disposed, reenables change
    /// notifications.</returns>
    IDisposable SuppressChangeNotifications();
}