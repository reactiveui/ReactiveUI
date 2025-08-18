// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Windows.Input;

#if NETFX_CORE
using Windows.UI.Xaml.Input;
#endif

namespace ReactiveUI;

/// <summary>
/// This binder is the default binder for connecting to arbitrary events.
/// </summary>
public class CreatesCommandBindingViaEvent : ICreatesCommandBinding
{
    // NB: These are in priority order
    private static readonly List<(string name, Type type)> _defaultEventsToBind =
    [
        ("Click", typeof(EventArgs)),
        ("TouchUpInside", typeof(EventArgs)),
        ("MouseUp", typeof(EventArgs)),
#if NETFX_CORE
        ("PointerReleased", typeof(PointerRoutedEventArgs)),
        ("Tapped", typeof(TappedRoutedEventArgs)),
#endif
    ];

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Event binding requires dynamic code generation")]
    [RequiresUnreferencedCode("Event binding may reference members that could be trimmed")]
#endif
    public int GetAffinityForObject(Type type, bool hasEventTarget)
    {
        if (hasEventTarget)
        {
            return 5;
        }

        return _defaultEventsToBind.Any(x =>
        {
            var ei = type.GetRuntimeEvent(x.name);
            return ei is not null;
        }) ? 3 : 0;
    }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Event binding requires dynamic code generation and reflection")]
    [RequiresUnreferencedCode("Event binding may reference members that could be trimmed")]
#endif
    public IDisposable? BindCommandToObject(ICommand? command, object? target, IObservable<object?> commandParameter)
    {
        target.ArgumentNullExceptionThrowIfNull(nameof(target));

        var type = target!.GetType();
        var eventInfo = _defaultEventsToBind
            .Select(x => new { EventInfo = type.GetRuntimeEvent(x.name), Args = x.type })
            .FirstOrDefault(x => x.EventInfo is not null) ?? throw new Exception(
                   $"Couldn't find a default event to bind to on {target.GetType().FullName}, specify an event explicitly");
        var mi = GetType().GetRuntimeMethods().First(x => x.Name == "BindCommandToObject" && x.IsGenericMethod);
        mi = mi.MakeGenericMethod(eventInfo.Args);

        return (IDisposable?)mi.Invoke(this, [command, target, commandParameter, eventInfo.EventInfo?.Name]);
    }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Event binding requires dynamic code generation")]
    [RequiresUnreferencedCode("Event binding may reference members that could be trimmed")]
#endif
    public IDisposable? BindCommandToObject<TEventArgs>(ICommand? command, object? target, IObservable<object?> commandParameter, string eventName)
#if MONO
        where TEventArgs : EventArgs
#endif
    {
        var ret = new CompositeDisposable();

        object? latestParameter = null;
        var evt = Observable.FromEventPattern<TEventArgs>(target!, eventName);

        ret.Add(commandParameter.Subscribe(x => latestParameter = x));

        ret.Add(evt.Subscribe(_ =>
        {
            if (command!.CanExecute(latestParameter))
            {
                command.Execute(latestParameter);
            }
        }));

        return ret;
    }
}
