// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ReactiveUI.Winforms;

/// <summary>
/// Default command binder for Windows Forms controls that connects an <see cref="ICommand"/> to an event on a target object.
/// </summary>
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
    // NB: These are in priority order
    private static readonly List<(string name, Type type)> _defaultEventsToBind =
    [
        ("Click", typeof(EventArgs)),
        ("MouseUp", typeof(System.Windows.Forms.MouseEventArgs))];

    /// <inheritdoc/>
    public int GetAffinityForObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] T>(bool hasEventTarget)
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
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    public IDisposable? BindCommandToObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] T>(ICommand? command, T? target, IObservable<object?> commandParameter)
        where T : class
    {
        ArgumentExceptionHelper.ThrowIfNull(target);

        // Preserve typical binding semantics: null command => no-op binding.
        if (command is null)
        {
            return Disposable.Empty;
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
        const BindingFlags bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        var type = typeof(T);
        var eventInfo = _defaultEventsToBind
                        .Select(x => new { EventInfo = type.GetEvent(x.name, bf), Args = x.type })
                        .FirstOrDefault(x => x.EventInfo is not null);

        if (eventInfo is null)
        {
            return null;
        }

        // Dynamically call the correct generic method based on event args type
        if (eventInfo.Args == typeof(EventArgs))
        {
            return BindCommandToObject<T, EventArgs>(command, target, commandParameter, eventInfo.EventInfo?.Name!);
        }
        else if (eventInfo.Args == typeof(System.Windows.Forms.MouseEventArgs))
        {
            return BindCommandToObject<T, System.Windows.Forms.MouseEventArgs>(command, target, commandParameter, eventInfo.EventInfo?.Name!);
        }

        return null;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    public IDisposable? BindCommandToObject<T, TEventArgs>(ICommand? command, T? target, IObservable<object?> commandParameter, string eventName)
        where T : class
    {
        ArgumentExceptionHelper.ThrowIfNull(command);
        ArgumentExceptionHelper.ThrowIfNull(target);

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
    public IDisposable? BindCommandToObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] T, TEventArgs>(
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

        if (command is null)
        {
            return Disposable.Empty;
        }

        object? latestParameter = null;

        void Handler(object? s, TEventArgs e)
        {
            var param = Volatile.Read(ref latestParameter);
            if (command.CanExecute(param))
            {
                command.Execute(param);
            }
        }

        var ret = new CompositeDisposable();
        ret.Add(commandParameter.Subscribe(x => Volatile.Write(ref latestParameter, x)));

        addHandler(Handler);
        ret.Add(Disposable.Create(() => removeHandler(Handler)));

        // Handle Enabled property binding for Components
        var targetType = typeof(T);
        if (typeof(Component).IsAssignableFrom(targetType))
        {
            var enabledProperty = targetType.GetRuntimeProperty("Enabled");

            if (enabledProperty is not null)
            {
                object? latestParam = null;
                ret.Add(commandParameter.Subscribe(x => Volatile.Write(ref latestParam, x)));

                ret.Add(Observable.FromEvent<EventHandler, bool>(
                                                                 eventHandler => (_, _) => eventHandler(command.CanExecute(Volatile.Read(ref latestParam))),
                                                                 x => command.CanExecuteChanged += x,
                                                                 x => command.CanExecuteChanged -= x)
                                  .StartWith(command.CanExecute(latestParam))
                                  .Subscribe(x => enabledProperty.SetValue(target, x, null)));
            }
        }

        return ret;
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
    public IDisposable? BindCommandToObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] T>(
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

        if (command is null)
        {
            return Disposable.Empty;
        }

        object? latestParameter = null;

        void Handler(object? s, EventArgs e)
        {
            var param = Volatile.Read(ref latestParameter);
            if (command.CanExecute(param))
            {
                command.Execute(param);
            }
        }

        var ret = new CompositeDisposable();
        ret.Add(commandParameter.Subscribe(x => Volatile.Write(ref latestParameter, x)));

        addHandler(Handler);
        ret.Add(Disposable.Create(() => removeHandler(Handler)));

        // Handle Enabled property binding for Components
        var targetType = typeof(T);
        if (typeof(Component).IsAssignableFrom(targetType))
        {
            var enabledProperty = targetType.GetRuntimeProperty("Enabled");

            if (enabledProperty is not null)
            {
                object? latestParam = null;
                ret.Add(commandParameter.Subscribe(x => Volatile.Write(ref latestParam, x)));

                ret.Add(Observable.FromEvent<EventHandler, bool>(
                                                                 eventHandler => (_, _) => eventHandler(command.CanExecute(Volatile.Read(ref latestParam))),
                                                                 x => command.CanExecuteChanged += x,
                                                                 x => command.CanExecuteChanged -= x)
                                  .StartWith(command.CanExecute(latestParam))
                                  .Subscribe(x => enabledProperty.SetValue(target, x, null)));
            }
        }

        return ret;
    }
}
