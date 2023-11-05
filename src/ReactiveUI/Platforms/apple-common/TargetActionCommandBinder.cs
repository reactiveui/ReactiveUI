// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
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

namespace ReactiveUI
{
    /// <summary>
    /// TargetActionCommandBinder is an implementation of command binding that
    /// understands Cocoa's Target / Action Framework. Many controls in Cocoa
    /// that are effectively command sources (i.e. Buttons, Menus, etc),
    /// participate in this framework.
    /// </summary>
    public class TargetActionCommandBinder : ICreatesCommandBinding
    {
        private readonly Type[] _validTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetActionCommandBinder"/> class.
        /// </summary>
        public TargetActionCommandBinder() =>
#if UIKIT
            _validTypes = new[]
            {
                typeof(UIControl),
            };
#else
            _validTypes = new[]
            {
                typeof(NSControl),
                typeof(NSCell),
                typeof(NSMenu),
                typeof(NSMenuItem),
                typeof(NSToolbarItem),
            };
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
        public IDisposable? BindCommandToObject(ICommand? command, object? target, IObservable<object?> commandParameter)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            commandParameter ??= Observable.Return(target);

            object? latestParam = null;
            var ctlDelegate = new ControlDelegate(
                _ =>
                {
                    if (command.CanExecute(latestParam))
                    {
                        command.Execute(latestParam);
                    }
                }) { IsEnabled = command.CanExecute(latestParam) };

            var sel = new Selector("theAction:");

            // TODO how does this work? Is there an Action property?
            Reflection.GetValueSetterOrThrow(target.GetType().GetRuntimeProperty("Action"))?.Invoke(target, sel, null);

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
        public IDisposable? BindCommandToObject<TEventArgs>(ICommand? command, object? target, IObservable<object?> commandParameter, string eventName)
            where TEventArgs : EventArgs =>
#pragma warning disable RCS1079 // Throwing of new NotImplementedException.
            throw new NotImplementedException();
#pragma warning restore RCS1079 // Throwing of new NotImplementedException.

        private class ControlDelegate(Action<NSObject> block) : NSObject
        {
            private readonly Action<NSObject> _block = block;

            public bool IsEnabled { get; set; }

            [Export("theAction:")]
            public void TheAction(NSObject sender) => _block(sender);

#if !UIKIT
            [Export("validateMenuItem:")]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable IDE0060 // Remove unused parameter
            public bool ValidateMenuItem(NSMenuItem menuItem) => IsEnabled;
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RCS1163 // Unused parameter.
#endif
        }
    }
}
