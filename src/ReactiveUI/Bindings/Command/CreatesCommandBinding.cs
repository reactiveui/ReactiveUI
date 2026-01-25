// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace ReactiveUI;

/// <summary>
/// AOT-compatible command binding helper that uses generic type parameters instead of reflection.
/// </summary>
internal static class CreatesCommandBinding
{
    /// <summary>
    /// Binds an ICommand to the specified target object, enabling the command to be executed in response to events on
    /// the target. Fully AOT-compatible.
    /// </summary>
    /// <remarks>This method uses reflection to bind the command to the target object's events and properties.
    /// Trimming tools may remove required members, so use caution when linking with trimming enabled.</remarks>
    /// <typeparam name="TControl">The type of the target object to which the command will be bound. Must be a class with accessible events and
    /// properties.</typeparam>
    /// <param name="command">The command to bind to the target object. Can be null to remove an existing binding.</param>
    /// <param name="target">The object whose events will trigger the command. Can be null if no binding should be established.</param>
    /// <param name="commandParameter">An observable sequence that provides the parameter to pass to the command when it is executed.</param>
    /// <returns>An IDisposable that can be used to unbind the command from the target object.</returns>
    /// <exception cref="Exception">Thrown if a suitable command binder cannot be found for the specified target type.</exception>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    public static IDisposable BindCommandToObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] TControl>(ICommand? command, TControl? target, IObservable<object?> commandParameter)
        where TControl : class
    {
        var binder = GetBinder<TControl>(hasEventTarget: false);
        var ret = binder.BindCommandToObject(command, target, commandParameter)
            ?? throw new Exception($"Couldn't bind Command Binder for {typeof(TControl).FullName}");
        return ret;
    }

    /// <summary>
    /// Binds an ICommand to a specified event on a target object, enabling command execution in response to the event. Fully AOT-compatible.
    /// </summary>
    /// <remarks>This method uses reflection to bind the command to the specified event. When the event is
    /// raised, the command is executed with the latest value from the commandParameter observable. The returned
    /// IDisposable should be disposed to detach the event handler and prevent memory leaks. This method may not be
    /// compatible with all trimming scenarios due to its use of reflection.</remarks>
    /// <typeparam name="TControl">The type of the target object to which the command is bound. Must be a class with accessible events and
    /// properties.</typeparam>
    /// <typeparam name="TEventArgs">The type of the event arguments associated with the event to bind.</typeparam>
    /// <param name="command">The command to execute when the specified event is raised. Can be null to unbind any existing command.</param>
    /// <param name="target">The object that exposes the event to which the command will be bound. Must not be null.</param>
    /// <param name="commandParameter">An observable sequence that provides the parameter to pass to the command when it is executed.</param>
    /// <param name="eventName">The name of the event on the target object that triggers command execution. Must correspond to an event defined
    /// on the target.</param>
    /// <returns>An IDisposable that can be used to unbind the command from the event.</returns>
    /// <exception cref="Exception">Thrown if a suitable command binder cannot be found for the specified target type and event name.</exception>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    public static IDisposable BindCommandToObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] TControl, TEventArgs>(
        ICommand? command,
        TControl? target,
        IObservable<object?> commandParameter,
        string eventName)
        where TControl : class
    {
        var binder = GetBinder<TControl>(hasEventTarget: true);
        var ret = binder.BindCommandToObject<TControl, TEventArgs>(command, target, commandParameter, eventName)
            ?? throw new Exception($"Couldn't bind Command Binder for {typeof(TControl).FullName} and event {eventName}");
        return ret;
    }

    /// <summary>
    /// Selects the most suitable command binding provider for the specified target type.
    /// </summary>
    /// <typeparam name="T">The type of the target object for which a command binding provider is required. The type must have accessible
    /// events or properties as determined by the binding providers.</typeparam>
    /// <param name="hasEventTarget">true if the target object exposes an event to bind to; otherwise, false.</param>
    /// <returns>An instance of ICreatesCommandBinding that is best suited for the specified target type.</returns>
    /// <exception cref="Exception">Thrown if no suitable command binding provider can be found for the specified target type.</exception>
    private static ICreatesCommandBinding GetBinder<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] T>(bool hasEventTarget)
    {
        var binder = AppLocator.Current.GetServices<ICreatesCommandBinding>()
            .Aggregate((score: 0, binding: (ICreatesCommandBinding?)null), (acc, x) =>
            {
                var score = x.GetAffinityForObject<T>(hasEventTarget);
                return (score > acc.score) ? (score, x) : acc;
            }).binding;

        return binder ?? throw new Exception($"Couldn't find a Command Binder for {typeof(T).FullName}");
    }
}
