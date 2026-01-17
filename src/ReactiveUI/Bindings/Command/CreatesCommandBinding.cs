// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Windows.Input;

namespace ReactiveUI;

/// <summary>
/// AOT-compatible command binding helper that uses generic type parameters instead of reflection.
/// </summary>
internal static class CreatesCommandBinding
{
    /// <summary>
    /// Binds a command to a control using default event discovery. Fully AOT-compatible.
    /// </summary>
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
    /// Binds a command to a control using a specific event. Fully AOT-compatible.
    /// </summary>
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
