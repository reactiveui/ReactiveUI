// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows.Input;
using ReactiveUI.Internal;

// System.Reactive pulls in WindowsBase (System.Windows.Input), so MouseEventArgs is ambiguous with WPF's; the
// Windows Forms command binder always means the Forms type.
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Winforms;
#else
namespace ReactiveUI.Winforms;
#endif

/// <summary>Default command binder for Windows Forms controls that connects an <see cref="ICommand"/> to an event on a target object.</summary>
/// <remarks>
/// <para>
/// This binder supports a small set of conventional "default" events (for example, <c>Click</c>, <c>MouseUp</c>),
/// and can also bind to an explicitly named event.
/// </para>
/// <para>
/// Reflection-based event lookup and string-based event subscription are not trimming/AOT-safe in general.
/// Use the generic overloads with explicit add/remove handler delegates to avoid the reflection cost.
/// </para>
/// </remarks>
public sealed class CreatesWinformsCommandBinding : ICreatesCommandBinding
{
    /// <summary>The affinity returned when the target is a known Windows Forms control with an event target.</summary>
    private const int EventTargetAffinity = 6;

    /// <summary>The affinity returned when the target exposes one of the conventional default events.</summary>
    private const int DefaultEventAffinity = 4;

    /// <summary>The conventional default events to bind to, listed in priority order.</summary>
    private static readonly List<(string name, Type type)> _defaultEventsToBind =
    [
        ("Click", typeof(EventArgs)),
        ("MouseUp", typeof(MouseEventArgs))
    ];

    /// <inheritdoc/>
    public int GetAffinityForObject<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                    DynamicallyAccessedMemberTypes.PublicProperties)]
    T>(bool hasEventTarget)
    {
        var isWinformControl = typeof(Control).IsAssignableFrom(typeof(T));

        if (isWinformControl)
        {
            return BindingAffinity.ExactType;
        }

        if (hasEventTarget)
        {
            return EventTargetAffinity;
        }

        return _defaultEventsToBind.Exists(static x =>
        {
            var ei = typeof(T).GetEvent(
                x.name,
                BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
            return ei is not null;
        })
            ? DefaultEventAffinity
            : 0;
    }

    /// <summary>
    /// Binds a command to the default event on a Windows Forms control.
    /// This method uses direct type checking and the AOT-safe add/remove handler overload instead of reflection.
    /// </summary>
    /// <typeparam name="T">The type of the target object.</typeparam>
    /// <param name="command">The command to bind. If <see langword="null"/>, no binding is created.</param>
    /// <param name="target">The target object.</param>
    /// <param name="commandParameter">An observable that supplies command parameter values.</param>
    /// <returns>A disposable that unbinds the command, or null if no default event was found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is <see langword="null"/>.</exception>
    public IDisposable? BindCommandToObject<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.PublicEvents |
                                    DynamicallyAccessedMemberTypes.NonPublicEvents)]
    T>(ICommand? command, T? target, IObservable<object?> commandParameter)
        where T : class
    {
        ArgumentExceptionHelper.ThrowIfNull(target);

        // Preserve typical binding semantics: null command => no-op binding.
        if (command is null)
        {
            return EmptyDisposable.Instance;
        }

        // Use direct type checking for known WinForms types first (AOT-friendly)
        if (target is Control control)
        {
            // Most controls have a Click event (uses non-generic EventHandler)
            return BindCommandToObject(
                command,
                control,
                commandParameter,
                h => control.Click += h,
                h => control.Click -= h);
        }

        // Fall back to reflection-based event discovery for other types
        const BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        var type = typeof(T);
        EventInfo? matchedEvent = null;
        Type? matchedArgs = null;
        foreach (var (name, argType) in _defaultEventsToBind)
        {
            var candidate = type.GetEvent(name, BindingFlags);
            if (candidate is not null)
            {
                matchedEvent = candidate;
                matchedArgs = argType;
                break;
            }
        }

        if (matchedEvent is null)
        {
            return null;
        }

        // Dynamically call the correct generic method based on event args type
        if (matchedArgs == typeof(EventArgs))
        {
            return BindCommandToObject<T, EventArgs>(command, target, commandParameter, matchedEvent.Name);
        }

        return matchedArgs == typeof(MouseEventArgs)
            ? BindCommandToObject<T, MouseEventArgs>(command, target, commandParameter, matchedEvent.Name)
            : null;
    }

    /// <inheritdoc/>
    public IDisposable? BindCommandToObject<T, TEventArgs>(
        ICommand? command,
        T? target,
        IObservable<object?> commandParameter,
        string eventName)
        where T : class
    {
        ArgumentExceptionHelper.ThrowIfNull(command);
        ArgumentExceptionHelper.ThrowIfNull(target);

        var ret = new MultipleDisposable();

        object? latestParameter = null;
        var targetType = target.GetType();

        ret.Add(commandParameter.Subscribe(new DelegateObserver<object?>(x => latestParameter = x)));

        var evt = new EventPatternObservable<TEventArgs>(target, eventName);
        ret.Add(evt.Subscribe(new DelegateObserver<TEventArgs>(_ =>
        {
            if (!command.CanExecute(latestParameter))
            {
                return;
            }

            command.Execute(latestParameter);
        })));

        // We initially only accepted Controls here, but this is too restrictive:
        // there are a number of Components that can trigger Commands and also
        // have an Enabled property, just like Controls.
        // For example: System.Windows.Forms.ToolStripButton.
        if (typeof(Component).IsAssignableFrom(targetType))
        {
            var enabledProperty = targetType.GetRuntimeProperty("Enabled");

            if (enabledProperty is null)
            {
                return ret;
            }

            // Replaces FromEvent(CanExecuteChanged).StartWith(initial).Subscribe(...).
            var canExecuteChanged = new FromEventObservable<bool>(onNext =>
            {
                EventHandler handler = (_, _) => onNext(command.CanExecute(latestParameter));
                command.CanExecuteChanged += handler;
                return new ActionDisposable(() => command.CanExecuteChanged -= handler);
            });

            ret.Add(new StartWithObservable<bool>(canExecuteChanged, command.CanExecute(latestParameter))
                .Subscribe(new DelegateObserver<bool>(x => enabledProperty.SetValue(target, x, null))));
        }

        return ret;
    }

    /// <summary>
    /// Binds a command to an event on a target object using explicit add/remove handler delegates.
    /// This overload is AOT-safe and doesn't require reflection.
    /// </summary>
    /// <typeparam name="T">The type of the target object.</typeparam>
    /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
    /// <param name="command">The command to bind. If <see langword="null"/>, no binding is created.</param>
    /// <param name="target">The target object.</param>
    /// <param name="commandParameter">An observable that supplies command parameter values.</param>
    /// <param name="addHandler">Action that subscribes an event handler to the target event.</param>
    /// <param name="removeHandler">Action that unsubscribes an event handler from the target event.</param>
    /// <returns>A disposable that unbinds the command.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/>, <paramref name="addHandler"/>, or <paramref name="removeHandler"/> is <see langword="null"/>.</exception>
    public IDisposable? BindCommandToObject<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.PublicEvents |
                                    DynamicallyAccessedMemberTypes.NonPublicEvents)]
    T, TEventArgs>(
        ICommand? command,
        T? target,
        IObservable<object?> commandParameter,
        Action<EventHandler<TEventArgs>> addHandler,
        Action<EventHandler<TEventArgs>> removeHandler)
        where T : class
        where TEventArgs : EventArgs
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(addHandler);
        ArgumentExceptionHelper.ThrowIfNull(removeHandler);

        return command is null
            ? EmptyDisposable.Instance
            : BindToHandler(command, target, typeof(T), commandParameter, execute =>
            {
                EventHandler<TEventArgs> handler = (_, _) => execute();
                addHandler(handler);
                return new ActionDisposable(() => removeHandler(handler));
            });
    }

    /// <summary>
    /// Binds a command to an event on a target object using explicit add/remove handler delegates for non-generic EventHandler.
    /// This overload is AOT-safe and supports WinForms controls that use EventHandler instead of EventHandler&lt;TEventArgs&gt;.
    /// </summary>
    /// <typeparam name="T">The type of the target object.</typeparam>
    /// <param name="command">The command to bind. If <see langword="null"/>, no binding is created.</param>
    /// <param name="target">The target object.</param>
    /// <param name="commandParameter">An observable that supplies command parameter values.</param>
    /// <param name="addHandler">Action that subscribes an event handler to the target event.</param>
    /// <param name="removeHandler">Action that unsubscribes an event handler from the target event.</param>
    /// <returns>A disposable that unbinds the command.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/>, <paramref name="addHandler"/>, or <paramref name="removeHandler"/> is <see langword="null"/>.</exception>
    public IDisposable? BindCommandToObject<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.PublicEvents |
                                    DynamicallyAccessedMemberTypes.NonPublicEvents)]
    T>(
        ICommand? command,
        T? target,
        IObservable<object?> commandParameter,
        Action<EventHandler> addHandler,
        Action<EventHandler> removeHandler)
        where T : class
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(addHandler);
        ArgumentExceptionHelper.ThrowIfNull(removeHandler);

        return command is null
            ? EmptyDisposable.Instance
            : BindToHandler(command, target, typeof(T), commandParameter, execute =>
            {
                EventHandler handler = (_, _) => execute();
                addHandler(handler);
                return new ActionDisposable(() => removeHandler(handler));
            });
    }

    /// <summary>
    /// Shared command-binding core: subscribes the command parameter, attaches the event handler that executes the
    /// command when it can, and — for <see cref="Component"/> targets that expose an <c>Enabled</c> property — keeps
    /// that property in sync with <see cref="ICommand.CanExecute(object?)"/>. Each public overload supplies
    /// <paramref name="subscribeHandler"/> to wire its own strongly typed event delegate; everything else is identical.
    /// </summary>
    /// <param name="command">The command to bind.</param>
    /// <param name="target">The target object whose <c>Enabled</c> property is synchronized.</param>
    /// <param name="targetType">The declared target type used to discover the <c>Enabled</c> property.</param>
    /// <param name="commandParameter">An observable that supplies command parameter values.</param>
    /// <param name="subscribeHandler">Attaches a handler that invokes the supplied callback when the event fires and returns a disposable that detaches it.</param>
    /// <returns>A disposable that unbinds the command.</returns>
    private static MultipleDisposable BindToHandler(
        ICommand command,
        object target,
        Type targetType,
        IObservable<object?> commandParameter,
        Func<Action, IDisposable> subscribeHandler)
    {
        object? latestParameter = null;

        var ret = new MultipleDisposable { commandParameter.Subscribe(new DelegateObserver<object?>(x => Volatile.Write(ref latestParameter, x))) };

        ret.Add(subscribeHandler(() =>
        {
            var param = Volatile.Read(ref latestParameter);
            if (!command.CanExecute(param))
            {
                return;
            }

            command.Execute(param);
        }));

        // We initially only accepted Controls here, but this is too restrictive: there are a number of
        // Components that can trigger Commands and also have an Enabled property, just like Controls
        // (for example System.Windows.Forms.ToolStripButton).
        if (typeof(Component).IsAssignableFrom(targetType))
        {
            var enabledProperty = targetType.GetRuntimeProperty("Enabled");

            if (enabledProperty is not null)
            {
                // Replaces FromEvent(CanExecuteChanged).StartWith(initial).Subscribe(...).
                var canExecuteChanged = new FromEventObservable<bool>(onNext =>
                {
                    EventHandler canExecuteHandler = (_, _) => onNext(command.CanExecute(Volatile.Read(ref latestParameter)));
                    command.CanExecuteChanged += canExecuteHandler;
                    return new ActionDisposable(() => command.CanExecuteChanged -= canExecuteHandler);
                });

                ret.Add(new StartWithObservable<bool>(canExecuteChanged, command.CanExecute(Volatile.Read(ref latestParameter)))
                    .Subscribe(new DelegateObserver<bool>(x => enabledProperty.SetValue(target, x, null))));
            }
        }

        return ret;
    }
}
