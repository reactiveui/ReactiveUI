// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Windows.Input;

namespace ReactiveUI.Winforms;

/// <summary>
/// This binder is the default binder for connecting to arbitrary events.
/// </summary>
public class CreatesWinformsCommandBinding : ICreatesCommandBinding
{
    // NB: These are in priority order
    private static readonly List<(string name, Type type)> _defaultEventsToBind =
    [
        ("Click", typeof(EventArgs)),
        ("MouseUp", typeof(System.Windows.Forms.MouseEventArgs)),
    ];

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("GetAffinityForObject uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("GetAffinityForObject uses methods that may require unreferenced code")]
#endif
    public int GetAffinityForObject(Type type, bool hasEventTarget)
    {
        var isWinformControl = typeof(Control).IsAssignableFrom(type);

        if (isWinformControl)
        {
            return 10;
        }

        if (hasEventTarget)
        {
            return 6;
        }

        return _defaultEventsToBind.Any(x =>
        {
            var ei = type.GetEvent(x.name, BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
            return ei is not null;
        }) ? 4 : 0;
    }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    public int GetAffinityForObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        bool hasEventTarget)
#else
    public int GetAffinityForObject<T>(
        bool hasEventTarget)
#endif
    {
        var isWinformControl = typeof(Control).IsAssignableFrom(typeof(T));

        if (isWinformControl)
        {
            return 10;
        }

        if (hasEventTarget)
        {
            return 6;
        }

        return _defaultEventsToBind.Any(static x =>
        {
            var ei = typeof(T).GetEvent(x.name, BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
            return ei is not null;
        }) ? 4 : 0;
    }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("GetAffinityForObject uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("GetAffinityForObject uses methods that may require unreferenced code")]
#endif
    public IDisposable? BindCommandToObject(ICommand? command, object? target, IObservable<object?> commandParameter)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(target);
#else
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }
#endif

        const BindingFlags bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        var type = target.GetType();
        var eventInfo = _defaultEventsToBind
                        .Select(x => new { EventInfo = type.GetEvent(x.name, bf), Args = x.type })
                        .FirstOrDefault(x => x.EventInfo is not null);

        if (eventInfo is null)
        {
            return null;
        }

        var mi = GetType().GetMethods().First(x => x.Name == "BindCommandToObject" && x.IsGenericMethod);
        mi = mi.MakeGenericMethod(eventInfo.Args);

        return (IDisposable?)mi.Invoke(this, [command, target, commandParameter, eventInfo.EventInfo?.Name]);
    }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("GetAffinityForObject uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("GetAffinityForObject uses methods that may require unreferenced code")]
#endif
    public IDisposable BindCommandToObject<TEventArgs>(ICommand? command, object? target, IObservable<object?> commandParameter, string eventName)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(target);
#else
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }
#endif

        var ret = new CompositeDisposable();

        object? latestParameter = null;
        var targetType = target.GetType();

        ret.Add(commandParameter.Subscribe(x => latestParameter = x));

        var evt = Observable.FromEventPattern<TEventArgs>(target, eventName);
        ret.Add(evt.Subscribe(_ =>
        {
            if (command.CanExecute(latestParameter))
            {
                command.Execute(latestParameter);
            }
        }));

        // We initially only accepted Controls here, but this is too restrictive:
        // there are a number of Components that can trigger Commands and also
        // have an Enabled property, just like Controls.
        // For example: System.Windows.Forms.ToolStripButton.
        if (typeof(Component).IsAssignableFrom(targetType))
        {
            var enabledProperty = targetType.GetRuntimeProperty("Enabled");

            if (enabledProperty is not null)
            {
                object? latestParam = null;
                ret.Add(commandParameter.Subscribe(x => latestParam = x));

                ret.Add(Observable.FromEvent<EventHandler, bool>(
                                                                 eventHandler => (_, _) => eventHandler(command.CanExecute(latestParam)),
                                                                 x => command.CanExecuteChanged += x,
                                                                 x => command.CanExecuteChanged -= x)
                                  .StartWith(command.CanExecute(latestParam))
                                  .Subscribe(x => enabledProperty.SetValue(target, x, null)));
            }
        }

        return ret;
    }
}
