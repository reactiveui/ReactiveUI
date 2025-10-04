// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Windows.Input;

namespace ReactiveUI;

/// <summary>
/// Command binder for android controls.
/// </summary>
public abstract class FlexibleCommandBinder : ICreatesCommandBinding
{
    /// <summary>
    /// Configuration map.
    /// </summary>
    private readonly Dictionary<Type, CommandBindingInfo> _config = [];

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("GetAffinityForObject uses Reflection.GetValueSetterForProperty which requires dynamic code generation")]
    [RequiresUnreferencedCode("GetAffinityForObject uses Reflection.GetValueSetterForProperty which may require unreferenced code")]
#endif
    public int GetAffinityForObject(Type type, bool hasEventTarget)
    {
        if (hasEventTarget)
        {
            return 0;
        }

        var match = _config.Keys
                           .Where(x => x.IsAssignableFrom(type))
                           .OrderByDescending(x => _config[x].Affinity)
                           .FirstOrDefault();

        if (match is null)
        {
            return 0;
        }

        var typeProperties = _config[match];
        return typeProperties.Affinity;
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
        if (hasEventTarget)
        {
            return 0;
        }

        var match = _config.Keys
                           .Where(x => x.IsAssignableFrom(typeof(T)))
                           .OrderByDescending(x => _config[x].Affinity)
                           .FirstOrDefault();

        if (match is null)
        {
            return 0;
        }

        var typeProperties = _config[match];
        return typeProperties.Affinity;
    }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("BindCommandToObject uses Reflection.GetValueSetterForProperty which requires dynamic code generation")]
    [RequiresUnreferencedCode("BindCommandToObject uses Reflection.GetValueSetterForProperty which may require unreferenced code")]
#endif
    public IDisposable BindCommandToObject(ICommand? command, object? target, IObservable<object?> commandParameter)
    {
        ArgumentNullException.ThrowIfNull(target);

        var type = target.GetType();

        var match = _config.Keys
                           .Where(x => x.IsAssignableFrom(type))
                           .OrderByDescending(x => _config[x].Affinity)
                           .FirstOrDefault() ?? throw new NotSupportedException($"CommandBinding for {type.Name} is not supported");
        var typeProperties = _config[match];

        return typeProperties.CreateBinding?.Invoke(command, target, commandParameter) ?? Disposable.Empty;
    }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("BindCommandToObject uses Reflection.GetValueSetterForProperty which requires dynamic code generation")]
    [RequiresUnreferencedCode("BindCommandToObject uses Reflection.GetValueSetterForProperty which may require unreferenced code")]
#endif
    public IDisposable BindCommandToObject<TEventArgs>(ICommand? command, object? target, IObservable<object?> commandParameter, string eventName)
        where TEventArgs : EventArgs
        => throw new NotImplementedException();

    /// <summary>
    /// Creates a commands binding from event and a property.
    /// </summary>
    /// <returns>The binding from event.</returns>
    /// <param name="command">Command.</param>
    /// <param name="target">Target.</param>
    /// <param name="commandParameter">Command parameter.</param>
    /// <param name="eventName">Event name.</param>
    /// <param name="enabledProperty">Enabled property name.</param>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("This member uses reflection to discover event members and associated delegate types.")]
#endif
    protected static IDisposable ForEvent(ICommand? command, object? target, IObservable<object?> commandParameter, string eventName, PropertyInfo enabledProperty)
    {
        ArgumentNullException.ThrowIfNull(command);

        commandParameter ??= Observable.Return(target);

        object? latestParam = null;
        var ctl = target!;

        var actionDisp = Observable.FromEventPattern(ctl, eventName).Subscribe(_ =>
        {
            if (command.CanExecute(latestParam))
            {
                command.Execute(latestParam);
            }
        });

        var enabledSetter = Reflection.GetValueSetterForProperty(enabledProperty);
        if (enabledSetter is null)
        {
            return actionDisp;
        }

        // initial enabled state
        enabledSetter(target, command.CanExecute(latestParam), null);

        return new CompositeDisposable(
                                       actionDisp,
                                       commandParameter.Subscribe(x => latestParam = x),
                                       Observable.FromEvent<EventHandler, bool>(
                                                                                eventHandler =>
                                                                                {
                                                                                    void Handler(object? sender, EventArgs e) => eventHandler(command.CanExecute(latestParam));
                                                                                    return Handler;
                                                                                },
                                                                                x => command.CanExecuteChanged += x,
                                                                                x => command.CanExecuteChanged -= x)
                                                 .Subscribe(x => enabledSetter(target, x, null)));
    }

    /// <summary>
    /// Registers an observable factory for the specified type and property.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="affinity">The affinity for the type.</param>
    /// <param name="createBinding">Creates the binding.</param>
    protected void Register(Type type, int affinity, Func<ICommand?, object?, IObservable<object?>, IDisposable> createBinding) => _config[type] = new CommandBindingInfo { Affinity = affinity, CreateBinding = createBinding };

    private class CommandBindingInfo
    {
        public int Affinity { get; set; }

        public Func<ICommand?, object?, IObservable<object?>, IDisposable>? CreateBinding { get; set; }
    }
}
