// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace ReactiveUI.Avalonia;

internal class AvaloniaCreatesCommandBinding : ICreatesCommandBinding
{
    public int GetAffinityForObject(Type type, bool hasEventTarget)
    {
        var isInputElement = typeof(InputElement).IsAssignableFrom(type);
        if (!isInputElement)
        {
            return 0;
        }

        if (hasEventTarget)
        {
            // This method doesn't know which event we are going to bind.
            // Best effort support.
            return 6;
        }

        // Command/CommandParameter bindings is only available on ICommandSource-types (usually Buttons and MenuItem).
        var isCommandSource = typeof(ICommandSource).IsAssignableFrom(type);
        return isCommandSource ? 10 : 0;
    }

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

        if (target is not (InputElement element and ICommandSource))
        {
            throw new InvalidOperationException("Target must be an InputElement and implement ICommandSource.");
        }

        // Button.CommandProperty is reused for all button-like controls and menu item
        element.SetCurrentValue(Button.CommandProperty, command);
        var paramDisposable = element.Bind(Button.CommandParameterProperty, commandParameter);
        return Disposable.Create((avaloniaObject: element, paramDisposable), static (t) =>
        {
            t.paramDisposable.Dispose();
            t.avaloniaObject.ClearValue(Button.CommandProperty);
        });
    }

    public IDisposable? BindCommandToObject<TEventArgs>(ICommand? command, object? target, IObservable<object?> commandParameter, string eventName)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (target is not InputElement element)
        {
            throw new InvalidOperationException("Target must be an InputElement.");
        }

        var routedEvent = FindRoutedEvent(target, eventName);
        if (routedEvent is null)
        {
            throw new InvalidOperationException($"Routed Event {eventName} not found on {target.GetType().Name} element.");
        }

        return new RoutedEventSubscriptionClosure(element, routedEvent, command, commandParameter);
    }

    private static RoutedEvent? FindRoutedEvent(object target, string eventName)
    {
        foreach (var routedEvent in RoutedEventRegistry.Instance.GetRegistered(target.GetType()))
        {
            if (routedEvent.Name == eventName)
            {
                return routedEvent;
            }
        }

        return null;
    }

    private sealed class RoutedEventSubscriptionClosure : IDisposable
    {
        private readonly InputElement _element;
        private readonly RoutedEvent _routedEvent;
        private readonly ICommand _command;
        private readonly IDisposable _commandSubscription;
        private object? _lastCommandParameter;

        public RoutedEventSubscriptionClosure(
            InputElement element,
            RoutedEvent routedEvent,
            ICommand command,
            IObservable<object?> commandParameter)
        {
            _element = element;
            _routedEvent = routedEvent;
            _command = command;
            _commandSubscription = commandParameter.Subscribe(OnCommandParameterChanged);
            element.AddHandler(routedEvent, Handler, RoutingStrategies.Bubble);
        }

        public void Handler(object? sender, RoutedEventArgs args)
        {
            if (_command.CanExecute(_lastCommandParameter))
            {
                _command.Execute(_lastCommandParameter);
            }
        }

        public void Dispose()
        {
            _commandSubscription.Dispose();
            _element.RemoveHandler(_routedEvent, Handler);
            _element.ClearValue(InputElement.IsEnabledProperty);
        }

        private void OnCommandParameterChanged(object? value)
        {
            _lastCommandParameter = value;
            _element.SetCurrentValue(InputElement.IsEnabledProperty, _command.CanExecute(_lastCommandParameter));
        }
    }
}
