// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Windows.Input;

using Foundation;

using ObjCRuntime;

#if UIKIT
using UIKit;
#else
using AppKit;
#endif

namespace ReactiveUI;

/// <summary>
/// TargetActionCommandBinder is an implementation of command binding that
/// understands Cocoa's Target / Action Framework. Many controls in Cocoa
/// that are effectively command sources (i.e. Buttons, Menus, etc),
/// participate in this framework.
/// </summary>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("TargetActionCommandBinder uses reflection for property access and Objective-C runtime features which require dynamic code generation")]
[RequiresUnreferencedCode("TargetActionCommandBinder uses reflection for property access and Objective-C runtime features which may require unreferenced code")]
#endif
public class TargetActionCommandBinder : ICreatesCommandBinding
{
    private readonly Type[] _validTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetActionCommandBinder"/> class.
    /// </summary>
    public TargetActionCommandBinder() =>
#if UIKIT
        _validTypes =
        [
            typeof(UIControl),
        ];
#else
        _validTypes =
        [
            typeof(NSControl),
            typeof(NSCell),
            typeof(NSMenu),
            typeof(NSMenuItem),
            typeof(NSToolbarItem),
        ];
#endif

    /// <inheritdoc/>
    public int GetAffinityForObject(Type type, bool hasEventTarget)
    {
        if (!_validTypes.Any(x => x.IsAssignableFrom(type)))
        {
            return 0;
        }

        return !hasEventTarget ? 4 : 0;
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
        if (!_validTypes.Any(static x => x.IsAssignableFrom(typeof(T))))
        {
            return 0;
        }

        return !hasEventTarget ? 4 : 0;
    }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("BindCommandToObject uses Reflection.GetValueSetterOrThrow and GetValueSetterForProperty which require dynamic code generation")]
    [RequiresUnreferencedCode("BindCommandToObject uses Reflection.GetValueSetterOrThrow and GetValueSetterForProperty which may require unreferenced code")]
#endif
    public IDisposable? BindCommandToObject(ICommand? command, object? target, IObservable<object?> commandParameter)
    {
        ArgumentExceptionHelper.ThrowIfNull(command);
        ArgumentExceptionHelper.ThrowIfNull(target);

        commandParameter ??= Observable.Return(target);

        object? latestParam = null;
        var ctlDelegate = new ControlDelegate(
            _ =>
            {
                if (command!.CanExecute(latestParam))
                {
                    command.Execute(latestParam);
                }
            })
        { IsEnabled = command!.CanExecute(latestParam) };

        var sel = new Selector("theAction:");

        // TODO how does this work? Is there an Action property?
        Reflection.GetValueSetterOrThrow(target!.GetType().GetRuntimeProperty("Action"))?.Invoke(target, sel, null);

        var targetSetter = Reflection.GetValueSetterOrThrow(target.GetType().GetRuntimeProperty("Target"));
        targetSetter?.Invoke(target, ctlDelegate, null);
        var actionDisp = Disposable.Create(() => targetSetter?.Invoke(target, null, null));

        var enabledSetter = Reflection.GetValueSetterForProperty(target.GetType().GetRuntimeProperty("Enabled"));
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
                .Subscribe(x =>
                {
                    enabledSetter(target, x, null);
                    ctlDelegate.IsEnabled = x;
                }));
    }

    /// <inheritdoc/>
    public IDisposable BindCommandToObject<TEventArgs>(ICommand? command, object? target, IObservable<object?> commandParameter, string eventName)
        where TEventArgs : EventArgs => throw new NotImplementedException();

    private class ControlDelegate(Action<NSObject> block) : NSObject
    {
        private readonly Action<NSObject> _block = block;

        public bool IsEnabled { get; set; }

        [Export("theAction:")]
        public void TheAction(NSObject sender) => _block(sender);

#if !UIKIT
        [Export("validateMenuItem:")]
        public bool ValidateMenuItem(NSMenuItem menuItem) => IsEnabled;
#endif
    }
}
