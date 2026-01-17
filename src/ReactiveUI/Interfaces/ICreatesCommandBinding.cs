// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace ReactiveUI;

/// <summary>
/// Classes that implement this interface and registered inside Splat will be
/// used to potentially provide binding to a ICommand in the ViewModel to a Control
/// in the View. This interface is fully AOT-compatible using generic type parameters.
/// </summary>
public interface ICreatesCommandBinding
{
    /// <summary>
    /// Returns a positive integer when this class supports binding a command
    /// to an object of the specified type. If the binding is not supported,
    /// the method will return a non-positive integer. In cases where multiple
    /// implementations return positive values, the one with the highest value will
    /// be chosen. Default values are typically '2' or '0'.
    /// </summary>
    /// <param name="hasEventTarget">Determines if the host intends to use a custom event target.</param>
    /// <typeparam name="T">The type of the object to query for compatibility with command binding.</typeparam>
    /// <returns>A positive integer if binding is supported, or zero/a negative value if not supported.</returns>
    int GetAffinityForObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] T>(bool hasEventTarget);

    /// <summary>
    /// Bind an ICommand to a UI object, in the "default" way. The meaning
    /// of this is dependent on the implementation. This method will discover
    /// which event to bind to (e.g., Click, MouseUp) based on the control type.
    /// Implement this if you have a new type of UI control that doesn't have
    /// Command/CommandParameter like WPF or has a non-standard event name
    /// for "Invoke".
    /// </summary>
    /// <typeparam name="T">The type of the target object to which the command is bound. Must be a reference type.</typeparam>
    /// <param name="command">The command to bind. Can be null.</param>
    /// <param name="target">The target object, usually a UI control of some kind. Can be null.</param>
    /// <param name="commandParameter">An IObservable source whose latest
    ///     value will be passed as the command parameter to the command. Hosts
    ///     will always pass a valid IObservable, but this may be
    ///     Observable.Empty.</param>
    /// <returns>An IDisposable which will disconnect the binding when disposed, or null if no binding was created.</returns>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    IDisposable? BindCommandToObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] T>(
            ICommand? command,
            T? target,
            IObservable<object?> commandParameter)
        where T : class;

    /// <summary>
    /// Bind an ICommand to a UI object to a specific event. This event may
    /// be a standard .NET event, or it could be an event derived in another
    /// manner (i.e. in MonoTouch). This method is fully AOT-compatible as it
    /// uses generic type parameters instead of reflection.
    /// </summary>
    /// <typeparam name="T">The type of the target object to which the command is bound. Must be a reference type.</typeparam>
    /// <typeparam name="TEventArgs">The event argument type.</typeparam>
    /// <param name="command">The command to bind. Can be null.</param>
    /// <param name="target">The target object, usually a UI control of some kind. Can be null.</param>
    /// <param name="commandParameter">An IObservable source whose latest
    ///     value will be passed as the command parameter to the command. Hosts
    ///     will always pass a valid IObservable, but this may be
    ///     Observable.Empty.</param>
    /// <param name="eventName">The event to bind to.</param>
    /// <returns>An IDisposable which will disconnect the binding when disposed, or null if no binding was created.</returns>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    IDisposable? BindCommandToObject<T, TEventArgs>(
            ICommand? command,
            T? target,
            IObservable<object?> commandParameter,
            string eventName)
        where T : class;

    /// <summary>
    /// Binds a command to a specific event on a target object using explicit add/remove handler delegates.
    /// </summary>
    /// <typeparam name="T">The type of the target object.</typeparam>
    /// <typeparam name="TEventArgs">The event arguments type.</typeparam>
    /// <param name="command">The command to bind. If <see langword="null"/>, no binding is created.</param>
    /// <param name="target">The target object.</param>
    /// <param name="commandParameter">An observable that supplies command parameter values.</param>
    /// <param name="addHandler">Adds the handler to the target event.</param>
    /// <param name="removeHandler">Removes the handler from the target event.</param>
    /// <returns>A disposable that unbinds the command.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="target"/>, <paramref name="addHandler"/>, or <paramref name="removeHandler"/> is <see langword="null"/>.
    /// </exception>
    IDisposable? BindCommandToObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] T, TEventArgs>(
        ICommand? command,
        T? target,
        IObservable<object?> commandParameter,
        Action<EventHandler<TEventArgs>> addHandler,
        Action<EventHandler<TEventArgs>> removeHandler)
        where T : class
        where TEventArgs : EventArgs;
}
