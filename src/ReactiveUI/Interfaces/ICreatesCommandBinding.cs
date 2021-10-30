// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows.Input;

namespace ReactiveUI;

/// <summary>
/// Classes that implement this interface and registered inside Splat will be
/// used to potentially provide binding to a ICommand in the ViewModel to a Control
/// in the View.
/// </summary>
public interface ICreatesCommandBinding
{
    /// <summary>
    /// Returns a positive integer when this class supports
    /// BindCommandToObject for this particular Type. If the method
    /// isn't supported at all, return a non-positive integer. When multiple
    /// implementations return a positive value, the host will use the one
    /// which returns the highest value. When in doubt, return '2' or '0'.
    /// </summary>
    /// <param name="type">The type to query for.</param>
    /// <param name="hasEventTarget">If true, the host intends to use a custom
    /// event target.</param>
    /// <returns>A positive integer if BCTO is supported, zero or a negative
    /// value otherwise.</returns>
    int GetAffinityForObject(Type type, bool hasEventTarget);

    /// <summary>
    /// Bind an ICommand to a UI object, in the "default" way. The meaning
    /// of this is dependent on the implementation. Implement this if you
    /// have a new type of UI control that doesn't have
    /// Command/CommandParameter like WPF or has a non-standard event name
    /// for "Invoke".
    /// </summary>
    /// <param name="command">The command to bind.</param>
    /// <param name="target">The target object, usually a UI control of
    ///     some kind.</param>
    /// <param name="commandParameter">An IObservable source whose latest
    ///     value will be passed as the command parameter to the command. Hosts
    ///     will always pass a valid IObservable, but this may be
    ///     Observable.Empty.</param>
    /// <returns>An IDisposable which will disconnect the binding when
    /// disposed.</returns>
    IDisposable? BindCommandToObject(ICommand? command, object? target, IObservable<object?> commandParameter);

    /// <summary>
    /// Bind an ICommand to a UI object to a specific event. This event may
    /// be a standard .NET event, or it could be an event derived in another
    /// manner (i.e. in MonoTouch).
    /// </summary>
    /// <typeparam name="TEventArgs">The event argument type.</typeparam>
    /// <param name="command">The command to bind.</param>
    /// <param name="target">The target object, usually a UI control of
    /// some kind.</param>
    /// <param name="commandParameter">An IObservable source whose latest
    /// value will be passed as the command parameter to the command. Hosts
    /// will always pass a valid IObservable, but this may be
    /// Observable.Empty.</param>
    /// <param name="eventName">The event to bind to.</param>
    /// <returns>An IDisposable which will disconnect the binding when disposed.</returns>
    IDisposable? BindCommandToObject<TEventArgs>(ICommand? command, object? target, IObservable<object?> commandParameter, string eventName)
#if MONO
        where TEventArgs : EventArgs
#endif
    ;
}