// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Input;
using UIKit;

namespace ReactiveUI
{
    /// <summary>
    /// Generic command binder platform registrations.
    /// </summary>
    /// <seealso cref="ICreatesCommandBinding" />
    public abstract class FlexibleCommandBinder : ICreatesCommandBinding
    {
        /// <summary>
        /// Configuration map.
        /// </summary>
        private readonly Dictionary<Type, CommandBindingInfo> _config =
            new();

        /// <inheritdoc/>
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
        public IDisposable? BindCommandToObject(ICommand? command, object? target, IObservable<object?> commandParameter)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var type = target.GetType();

            var match = _config.Keys
                .Where(x => x.IsAssignableFrom(type))
                .OrderByDescending(x => _config[x].Affinity)
                .FirstOrDefault();

            if (match is null)
            {
                throw new NotSupportedException($"CommandBinding for {type.Name} is not supported");
            }

            var typeProperties = _config[match];

            return typeProperties?.CreateBinding?.Invoke(command, target, commandParameter) ?? Disposable.Empty;
        }

        /// <inheritdoc/>
        public IDisposable? BindCommandToObject<TEventArgs>(ICommand? command, object? target, IObservable<object?> commandParameter, string eventName)
            where TEventArgs : EventArgs =>
#pragma warning disable RCS1079 // Throwing of new NotImplementedException.
            throw new NotImplementedException();
#pragma warning restore RCS1079 // Throwing of new NotImplementedException.

        /// <summary>
        /// Creates a commands binding from event and a property.
        /// </summary>
        /// <returns>The binding from event.</returns>
        /// <param name="command">Command.</param>
        /// <param name="target">Target.</param>
        /// <param name="commandParameter">Command parameter.</param>
        /// <param name="eventName">Event name.</param>
        /// <param name="enabledProperty">Enabled Property.</param>
        protected static IDisposable ForEvent(ICommand? command, object? target, IObservable<object?> commandParameter, string eventName, PropertyInfo enabledProperty)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            commandParameter ??= Observable.Return(target);

            object? latestParam = null;
            var ctl = target!;

            var actionDisp = Observable.FromEventPattern(ctl, eventName).Subscribe((_) =>
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
                        void Handler(object sender, EventArgs e) => eventHandler(command.CanExecute(latestParam));
                        return Handler;
                    },
                    x => command.CanExecuteChanged += x,
                    x => command.CanExecuteChanged -= x)
                    .Subscribe(x => enabledSetter(target, x, null)));
        }

        /// <summary>
        /// Creates a commands binding from event and a property.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="target">The target object.</param>
        /// <param name="commandParameter">The command parameter.</param>
        /// <param name="enabledProperty">The enabled property.</param>
        /// <returns>Returns a disposable.</returns>
        protected static IDisposable ForTargetAction(ICommand? command, object? target, IObservable<object?> commandParameter, PropertyInfo enabledProperty)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            commandParameter ??= Observable.Return(target);

            object? latestParam = null;

            var actionDisposable = Disposable.Empty;

            if (target is UIControl ctl)
            {
                var eh = new EventHandler((o, e) =>
                {
                    if (command.CanExecute(latestParam))
                    {
                        command.Execute(latestParam);
                    }
                });

                ctl.AddTarget(eh, UIControlEvent.TouchUpInside);
                actionDisposable = Disposable.Create(() => ctl.RemoveTarget(eh, UIControlEvent.TouchUpInside));
            }

            var enabledSetter = Reflection.GetValueSetterForProperty(enabledProperty);
            if (enabledSetter is null)
            {
                return actionDisposable;
            }

            // Initial enabled state
            enabledSetter(target, command.CanExecute(latestParam), null);

            return new CompositeDisposable(
                actionDisposable,
                commandParameter.Subscribe(x => latestParam = x),
                Observable.FromEvent<EventHandler, bool>(
                    eventHandler =>
                    {
                        void Handler(object sender, EventArgs e) => eventHandler(command.CanExecute(latestParam));
                        return Handler;
                    },
                    x => command.CanExecuteChanged += x,
                    x => command.CanExecuteChanged -= x)
                    .Subscribe(x => enabledSetter(target, x, null)));
        }

        /// <summary>
        /// Registers an observable factory for the specified type and property.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="affinity">The affinity.</param>
        /// <param name="createBinding">The create binding.</param>
        protected void Register(Type type, int affinity, Func<ICommand?, object?, IObservable<object?>, IDisposable> createBinding) => _config[type] = new CommandBindingInfo { Affinity = affinity, CreateBinding = createBinding };

        private class CommandBindingInfo
        {
            public int Affinity { get; set; }

            public Func<ICommand?, object?, IObservable<object?>, IDisposable>? CreateBinding { get; set; }
        }
    }
}
