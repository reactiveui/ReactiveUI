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
    public int GetAffinityForObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] T>(bool hasEventTarget)
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
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    public IDisposable? BindCommandToObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] T>(ICommand? command, T? target, IObservable<object?> commandParameter)
        where T : class
    {
        ArgumentExceptionHelper.ThrowIfNull(target);

        var type = target.GetType();

        var match = _config.Keys
                           .Where(x => x.IsAssignableFrom(type))
                           .OrderByDescending(x => _config[x].Affinity)
                           .FirstOrDefault() ?? throw new NotSupportedException($"CommandBinding for {type.Name} is not supported");
        var typeProperties = _config[match];

        return typeProperties.CreateBinding?.Invoke(command, target, commandParameter) ?? Disposable.Empty;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    public IDisposable? BindCommandToObject<T, TEventArgs>(ICommand? command, T? target, IObservable<object?> commandParameter, string eventName)
        where T : class
        => Disposable.Empty;

    /// <inheritdoc/>
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
        ArgumentNullException.ThrowIfNull(addHandler);
        ArgumentNullException.ThrowIfNull(removeHandler);

        // Match existing binder behavior: if there is no command, create a no-op binding.
        if (command is null)
        {
            return Disposable.Empty;
        }

        // Keep the existing "null means use target" idiom used by ForEvent.
        commandParameter ??= Observable.Return((object?)target);

        // The latest parameter may be updated from a different thread than the event thread.
        object? latestParam = null;

        // Stable handler for deterministic unsubscription.
        void Handler(object? sender, TEventArgs e)
        {
            var param = Volatile.Read(ref latestParam);
            if (command.CanExecute(param))
            {
                command.Execute(param);
            }
        }

        // Subscribe to parameter updates first, then attach the event handler.
        var parameterSub = commandParameter.Subscribe(x => Volatile.Write(ref latestParam, x));
        addHandler(Handler);

        // If we can locate a conventional enabled property on the runtime target, keep it in sync with CanExecute.
        // This is intentionally best-effort and does not throw if the property is absent or cannot be set.
        Action<object?, object?, object?[]?>? enabledSetter = null;
        try
        {
            // Common Android idiom: "Enabled" boolean property.
            // Use runtime type so derived types are supported.
            var enabledProp = typeof(T).GetRuntimeProperty("Enabled");
            if (enabledProp is not null)
            {
                enabledSetter = Reflection.GetValueSetterForProperty(enabledProp);
            }
        }
        catch
        {
            // Best-effort only; ignore reflection failures.
            enabledSetter = null;
        }

        IDisposable? canExecuteSub = null;
        if (enabledSetter is not null)
        {
            // Initial enabled state (default parameter is null until the first commandParameter emission).
            enabledSetter(target, command.CanExecute(Volatile.Read(ref latestParam)), null);

            // Keep Enabled in sync with CanExecuteChanged.
            canExecuteSub = Observable.FromEvent<EventHandler, bool>(
                    eventHandler =>
                    {
                        void CanExecuteHandler(object? s, EventArgs e) =>
                            eventHandler(command.CanExecute(Volatile.Read(ref latestParam)));
                        return CanExecuteHandler;
                    },
                    h => command.CanExecuteChanged += h,
                    h => command.CanExecuteChanged -= h)
                .Subscribe(x => enabledSetter(target, x, null));
        }

        // Dispose ordering: detach event handler and CanExecute subscription after stopping parameter updates.
        // The handler instance is stable, so Remove is correct.
        return canExecuteSub is null
            ? new CompositeDisposable(
                parameterSub,
                Disposable.Create(() => removeHandler(Handler)))
            : new CompositeDisposable(
                parameterSub,
                canExecuteSub,
                Disposable.Create(() => removeHandler(Handler)));
    }

    /// <summary>
    /// Creates a commands binding from event and a property.
    /// </summary>
    /// <returns>The binding from event.</returns>
    /// <param name="command">Command.</param>
    /// <param name="target">Target.</param>
    /// <param name="commandParameter">Command parameter.</param>
    /// <param name="eventName">Event name.</param>
    /// <param name="enabledProperty">Enabled property name.</param>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    protected static IDisposable ForEvent(ICommand? command, object? target, IObservable<object?> commandParameter, string eventName, PropertyInfo enabledProperty)
    {
        ArgumentExceptionHelper.ThrowIfNull(command);

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
    /// Creates a command binding from an event using explicit add/remove handler delegates and optionally
    /// synchronizes an enabled property with <see cref="ICommand.CanExecute(object?)"/>.
    /// </summary>
    /// <typeparam name="TEventArgs">The event arguments type.</typeparam>
    /// <param name="command">The command to bind.</param>
    /// <param name="target">The event source object.</param>
    /// <param name="commandParameter">Observable producing command parameter values. If <see langword="null"/>, <paramref name="target"/> is used.</param>
    /// <param name="addHandler">Adds the handler to the event source.</param>
    /// <param name="removeHandler">Removes the handler from the event source.</param>
    /// <param name="enabledSetter">
    /// Optional setter for an enabled-like property. If <see langword="null"/>, enabled synchronization is skipped.
    /// The setter is expected to accept <paramref name="target"/> as the first argument and a <see cref="bool"/> value as the second.
    /// </param>
    /// <returns>A disposable that unbinds the command and stops enabled synchronization.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="command"/> is <see langword="null"/>.
    /// </exception>
    protected static IDisposable ForEvent<TEventArgs>(
            ICommand? command,
            object? target,
            IObservable<object?>? commandParameter,
            Action<EventHandler<TEventArgs>> addHandler,
            Action<EventHandler<TEventArgs>> removeHandler,
            Action<object?, object?, object?[]?>? enabledSetter)
        where TEventArgs : EventArgs
    {
        ArgumentExceptionHelper.ThrowIfNull(command);
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(addHandler);
        ArgumentNullException.ThrowIfNull(removeHandler);

        // Preserve existing idiom: null commandParameter means use target.
        commandParameter ??= Observable.Return(target);

        object? latestParam = null;

        // Stable handler for deterministic unsubscription.
        void Handler(object? sender, TEventArgs e)
        {
            var param = Volatile.Read(ref latestParam);
            if (command.CanExecute(param))
            {
                command.Execute(param);
            }
        }

        // Subscribe to parameter updates first so the first event sees the latest parameter.
        var parameterSub = commandParameter.Subscribe(x => Volatile.Write(ref latestParam, x));

        // Hook the event without reflection.
        addHandler(Handler);

        // If there is no enabled setter, we're done.
        if (enabledSetter is null)
        {
            return new CompositeDisposable(
                parameterSub,
                Disposable.Create(() => removeHandler(Handler)));
        }

        // Initial enabled state.
        enabledSetter(target, command.CanExecute(Volatile.Read(ref latestParam)), null);

        // Keep enabled state in sync with CanExecuteChanged.
        var canExecuteSub = Observable.FromEvent<EventHandler, bool>(
                eventHandler =>
                {
                    void CanExecuteHandler(object? s, EventArgs e) =>
                        eventHandler(command.CanExecute(Volatile.Read(ref latestParam)));
                    return CanExecuteHandler;
                },
                h => command.CanExecuteChanged += h,
                h => command.CanExecuteChanged -= h)
            .Subscribe(x => enabledSetter(target, x, null));

        return new CompositeDisposable(
            parameterSub,
            canExecuteSub,
            Disposable.Create(() => removeHandler(Handler)));
    }

    /// <summary>
    /// Creates a command binding from an event using explicit add/remove handler delegates and optionally
    /// synchronizes an enabled property with <see cref="ICommand.CanExecute(object?)"/>.
    /// </summary>
    /// <param name="command">The command to bind.</param>
    /// <param name="target">The event source object.</param>
    /// <param name="commandParameter">Observable producing command parameter values. If <see langword="null"/>, <paramref name="target"/> is used.</param>
    /// <param name="addHandler">Adds the handler to the event source.</param>
    /// <param name="removeHandler">Removes the handler from the event source.</param>
    /// <param name="enabledSetter">
    /// Optional setter for an enabled-like property. If <see langword="null"/>, enabled synchronization is skipped.
    /// The setter is expected to accept <paramref name="target"/> as the first argument and a <see cref="bool"/> value as the second.
    /// </param>
    /// <returns>A disposable that unbinds the command and stops enabled synchronization.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="command"/> is <see langword="null"/>.
    /// </exception>
    protected static IDisposable ForEvent(
            ICommand? command,
            object? target,
            IObservable<object?>? commandParameter,
            Action<EventHandler> addHandler,
            Action<EventHandler> removeHandler,
            Action<object?, object?, object?[]?>? enabledSetter)
    {
        ArgumentExceptionHelper.ThrowIfNull(command);
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(addHandler);
        ArgumentNullException.ThrowIfNull(removeHandler);

        // Preserve existing idiom: null commandParameter means use target.
        commandParameter ??= Observable.Return(target);

        object? latestParam = null;

        // Stable handler for deterministic unsubscription.
        void Handler(object? sender, EventArgs e)
        {
            var param = Volatile.Read(ref latestParam);
            if (command.CanExecute(param))
            {
                command.Execute(param);
            }
        }

        // Subscribe to parameter updates first so the first event sees the latest parameter.
        var parameterSub = commandParameter.Subscribe(x => Volatile.Write(ref latestParam, x));

        // Hook the event without reflection.
        addHandler(Handler);

        // If there is no enabled setter, we're done.
        if (enabledSetter is null)
        {
            return new CompositeDisposable(
                parameterSub,
                Disposable.Create(() => removeHandler(Handler)));
        }

        // Initial enabled state.
        enabledSetter(target, command.CanExecute(Volatile.Read(ref latestParam)), null);

        // Keep enabled state in sync with CanExecuteChanged.
        var canExecuteSub = Observable.FromEvent<EventHandler, bool>(
                eventHandler =>
                {
                    void CanExecuteHandler(object? s, EventArgs e) =>
                        eventHandler(command.CanExecute(Volatile.Read(ref latestParam)));
                    return CanExecuteHandler;
                },
                h => command.CanExecuteChanged += h,
                h => command.CanExecuteChanged -= h)
            .Subscribe(x => enabledSetter(target, x, null));

        return new CompositeDisposable(
            parameterSub,
            canExecuteSub,
            Disposable.Create(() => removeHandler(Handler)));
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
