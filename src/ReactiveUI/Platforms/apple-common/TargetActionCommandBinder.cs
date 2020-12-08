// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
        public TargetActionCommandBinder()
        {
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
        }

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
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1011:Closing square brackets should be spaced correctly", Justification = "nullable object array.")]
        public IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            commandParameter = commandParameter ?? Observable.Return(target);

            object? latestParam = null;
            var ctlDelegate = new ControlDelegate(
                x =>
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

            Action<object, object, object[]?>? enabledSetter = Reflection.GetValueSetterForProperty(target.GetType().GetRuntimeProperty("Enabled"));
            if (enabledSetter is null)
            {
                return actionDisp;
            }

            // initial enabled state
            enabledSetter(target, command.CanExecute(latestParam), null);

            var compDisp = new CompositeDisposable(
                actionDisp,
                commandParameter.Subscribe(x => latestParam = x),
                Observable.FromEvent<EventHandler, bool>(
                    eventHandler =>
                    {
                        void Handler(object sender, EventArgs e) => eventHandler(command.CanExecute(latestParam));
                        return Handler;
                    },
                    x => command.CanExecuteChanged += x,
                    x => command.CanExecuteChanged -= x)
                    .Subscribe(x =>
                    {
                        enabledSetter(target, x, null);
                        ctlDelegate.IsEnabled = x;
                    }));

            return compDisp;
        }

        /// <inheritdoc/>
        public IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName)
            where TEventArgs : EventArgs =>
            throw new NotImplementedException();

        private class ControlDelegate : NSObject
        {
            private readonly Action<NSObject> _block;

            public ControlDelegate(Action<NSObject> block) => _block = block;

            public bool IsEnabled { get; set; }

            [Export("theAction:")]
            public void TheAction(NSObject sender) => _block(sender);

#if !UIKIT
            [Export("validateMenuItem:")]
            [SuppressMessage("Redundancy", "CA1801: Redundant parameter", Justification = "Legacy interface")]
            public bool ValidateMenuItem(NSMenuItem menuItem) => IsEnabled;
#endif
        }
    }
}
