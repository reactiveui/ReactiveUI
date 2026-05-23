// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Contains the state information about the current status of a Reactive Object.
/// </summary>
/// <typeparam name="TSender">The type of the sender of the property changes.</typeparam>
internal interface IExtensionState<out TSender>
    where TSender : IReactiveObject
{
    /// <summary>
    /// Gets an observable for when a property is changing.
    /// </summary>
    IObservable<IReactivePropertyChangedEventArgs<TSender>> Changing { get; }

    /// <summary>
    /// Gets an observable for when the property has changed.
    /// </summary>
    IObservable<IReactivePropertyChangedEventArgs<TSender>> Changed { get; }

    /// <summary>
    /// Gets a observable for when an exception is thrown.
    /// </summary>
    IObservable<Exception> ThrownExceptions { get; }

    /// <summary>
    /// Subscribe raise property changing events to a property changing
    /// observable. Must be called before raising property changing events.
    /// </summary>
    void SubscribeChanging();

    /// <summary>
    /// Raises a property changing event.
    /// </summary>
    /// <param name="propertyName">The name of the property that is changing.</param>
    void RaiseChanging(string propertyName);

    /// <summary>
    /// Subscribe raise property changed events to a property changed
    /// observable. Must be called before raising property changed events.
    /// </summary>
    void SubscribeChanged();

    /// <summary>
    /// Raises a property changed event.
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed.</param>
    void RaiseChanged(string propertyName);

    /// <summary>
    /// Indicates if we are currently sending change notifications.
    /// </summary>
    /// <returns>If change notifications are being sent.</returns>
    bool NotificationsEnabled();

    /// <summary>
    /// Suppress change notifications until the return value is disposed.
    /// </summary>
    /// <returns>A IDisposable which when disposed will re-enable change notifications.</returns>
    IDisposable Suppress();

    /// <summary>
    /// Are change notifications currently delayed. Used for Observables change notifications only.
    /// </summary>
    /// <returns>If the change notifications are delayed.</returns>
    bool AreChangeNotificationsDelayed();

    /// <summary>
    /// Delay change notifications until the return value is disposed.
    /// </summary>
    /// <returns>A IDisposable which when disposed will re-enable change notifications.</returns>
    IDisposable Delay();
}
