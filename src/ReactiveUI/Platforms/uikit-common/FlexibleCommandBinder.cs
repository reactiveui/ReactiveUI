// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Windows.Input;

using UIKit;

namespace ReactiveUI;

/// <summary>
/// Base class for platform command binders that register per-type binding factories with an affinity score.
/// </summary>
/// <remarks>
/// <para>
/// This type is intended for platform implementations (Android, iOS, etc.) that need to bind an
/// <see cref="ICommand"/> to UI controls with platform-specific semantics.
/// </para>
/// <para>
/// Threading: registrations are mutable; lookups are served from a versioned snapshot to avoid locking on
/// the common path. Binding factories are invoked outside locks.
/// </para>
/// <para>
/// Trimming/AOT: the default binding selection method accepts an unknown runtime target type and may call
/// reflection-based helpers (e.g., <see cref="ForEvent"/>). Reflection-based methods are annotated with
/// <see cref="RequiresUnreferencedCodeAttribute"/> and <see cref="RequiresDynamicCodeAttribute"/> where applicable.
/// Prefer the add/remove handler overload for AOT-safe event binding.
/// </para>
/// </remarks>
public abstract class FlexibleCommandBinder : ICreatesCommandBinding
{
    /// <summary>
    /// A single synchronization gate for all mutable state in this instance.
    /// </summary>
    private readonly object _gate = new();

    /// <summary>
    /// Mutable registration map; only accessed under <see cref="_gate"/>.
    /// </summary>
    private readonly Dictionary<Type, CommandBindingInfo> _config = [];

    /// <summary>
    /// A version counter incremented on each registration mutation.
    /// </summary>
    private int _version;

    /// <summary>
    /// A snapshot of registrations used for lock-free reads.
    /// </summary>
    private Entry[]? _snapshot;

    /// <summary>
    /// A snapshot version that corresponds to <see cref="_snapshot"/>.
    /// </summary>
    private int _snapshotVersion;

    /// <inheritdoc/>
    public int GetAffinityForObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] T>(bool hasEventTarget)
    {
        if (hasEventTarget)
        {
            return 0;
        }

        var entries = GetSnapshot();
        var targetType = typeof(T);

        var bestAffinity = 0;

        // Scan all assignable registrations; choose highest affinity.
        for (var i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            if (!entry.Type.IsAssignableFrom(targetType))
            {
                continue;
            }

            var affinity = entry.Affinity;
            if (affinity > bestAffinity)
            {
                bestAffinity = affinity;
            }
        }

        return bestAffinity;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    public IDisposable? BindCommandToObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] T>(
        ICommand? command,
        T? target,
        IObservable<object?> commandParameter)
        where T : class
    {
        ArgumentExceptionHelper.ThrowIfNull(target);

        var entries = GetSnapshot();
        var runtimeType = target.GetType();

        Entry? best = null;
        var bestAffinity = int.MinValue;

        for (var i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            if (!entry.Type.IsAssignableFrom(runtimeType))
            {
                continue;
            }

            if (entry.Affinity > bestAffinity)
            {
                bestAffinity = entry.Affinity;
                best = entry;
            }
        }

        if (best is null || best.Value.Factory is null)
        {
            throw new NotSupportedException($"CommandBinding for {runtimeType.Name} is not supported");
        }

        // Never invoke user code under locks; snapshot factories are safe to call directly here.
        return best.Value.Factory(command, target, commandParameter) ?? Disposable.Empty;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    public virtual IDisposable? BindCommandToObject<T, TEventArgs>(
        ICommand? command,
        T? target,
        IObservable<object?> commandParameter,
        string eventName)
        where T : class =>
        Disposable.Empty;

    /// <inheritdoc/>
    public virtual IDisposable? BindCommandToObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] T, TEventArgs>(
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

        // Match existing binder behavior: null command means "no binding".
        if (command is null)
        {
            return Disposable.Empty;
        }

        commandParameter ??= Observable.Return((object?)target);

        object? latestParam = null;

        void Handler(object? sender, TEventArgs e)
        {
            var param = Volatile.Read(ref latestParam);
            if (command.CanExecute(param))
            {
                command.Execute(param);
            }
        }

        var paramSub = commandParameter.Subscribe(x => Volatile.Write(ref latestParam, x));
        addHandler(Handler);

        return new CompositeDisposable(
            paramSub,
            Disposable.Create(() => removeHandler(Handler)));
    }

    /// <summary>
    /// Creates a command binding from an explicit event subscription API and an enabled property.
    /// </summary>
    /// <typeparam name="TTarget">The target type that exposes the event.</typeparam>
    /// <typeparam name="TEventArgs">The event args type.</typeparam>
    /// <param name="command">The command to execute when the event fires.</param>
    /// <param name="target">The target object that exposes the event.</param>
    /// <param name="commandParameter">An observable providing the latest command parameter.</param>
    /// <param name="addHandler">Adds the event handler to the target.</param>
    /// <param name="removeHandler">Removes the event handler from the target.</param>
    /// <param name="enabledProperty">A property used to set enabled state (best-effort).</param>
    /// <returns>A disposable that unsubscribes the event and stops updating enabled state.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="command"/>, <paramref name="target"/>, <paramref name="addHandler"/>,
    /// <paramref name="removeHandler"/>, or <paramref name="enabledProperty"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// This overload is AOT-compatible: it does not use reflection-based event subscription.
    /// Enabled state synchronization still depends on the provided <paramref name="enabledProperty"/>.
    /// </remarks>
    protected static IDisposable ForEvent<TTarget, TEventArgs>(
        ICommand? command,
        TTarget target,
        IObservable<object?> commandParameter,
        Action<EventHandler<TEventArgs>> addHandler,
        Action<EventHandler<TEventArgs>> removeHandler,
        PropertyInfo enabledProperty)
        where TTarget : class
        where TEventArgs : EventArgs
    {
        ArgumentExceptionHelper.ThrowIfNull(command);
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(addHandler);
        ArgumentNullException.ThrowIfNull(removeHandler);
        ArgumentExceptionHelper.ThrowIfNull(enabledProperty);

        commandParameter ??= Observable.Return((object?)target);

        object? latestParam = null;

        void Handler(object? sender, TEventArgs e)
        {
            var param = Volatile.Read(ref latestParam);
            if (command.CanExecute(param))
            {
                command.Execute(param);
            }
        }

        // Subscribe parameter first so we have best effort latest value before the first event.
        var paramSub = commandParameter.Subscribe(x => Volatile.Write(ref latestParam, x));

        addHandler(Handler);
        var eventDisp = Disposable.Create(() => removeHandler(Handler));

        var enabledSetter = Reflection.GetValueSetterForProperty(enabledProperty);
        if (enabledSetter is null)
        {
            return new CompositeDisposable(paramSub, eventDisp);
        }

        // Initial enabled state.
        enabledSetter(target, command.CanExecute(Volatile.Read(ref latestParam)), null);

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

        return new CompositeDisposable(paramSub, eventDisp, canExecuteSub);
    }

    /// <summary>
    /// Creates a command binding from an explicit event subscription API and an enabled property.
    /// </summary>
    /// <typeparam name="TTarget">The target type that exposes the event.</typeparam>
    /// <param name="command">The command to execute when the event fires.</param>
    /// <param name="target">The target object that exposes the event.</param>
    /// <param name="commandParameter">An observable providing the latest command parameter.</param>
    /// <param name="addHandler">Adds the event handler to the target.</param>
    /// <param name="removeHandler">Removes the event handler from the target.</param>
    /// <param name="enabledProperty">A property used to set enabled state (best-effort).</param>
    /// <returns>A disposable that unsubscribes the event and stops updating enabled state.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="command"/>, <paramref name="target"/>, <paramref name="addHandler"/>,
    /// <paramref name="removeHandler"/>, or <paramref name="enabledProperty"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// This overload is AOT-compatible: it does not use reflection-based event subscription.
    /// Enabled state synchronization still depends on the provided <paramref name="enabledProperty"/>.
    /// </remarks>
    protected static IDisposable ForEvent<TTarget>(
        ICommand? command,
        TTarget target,
        IObservable<object?> commandParameter,
        Action<EventHandler> addHandler,
        Action<EventHandler> removeHandler,
        PropertyInfo enabledProperty)
        where TTarget : class
    {
        ArgumentExceptionHelper.ThrowIfNull(command);
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(addHandler);
        ArgumentNullException.ThrowIfNull(removeHandler);
        ArgumentExceptionHelper.ThrowIfNull(enabledProperty);

        commandParameter ??= Observable.Return((object?)target);

        object? latestParam = null;

        void Handler(object? sender, EventArgs e)
        {
            var param = Volatile.Read(ref latestParam);
            if (command.CanExecute(param))
            {
                command.Execute(param);
            }
        }

        // Subscribe parameter first so we have best effort latest value before the first event.
        var paramSub = commandParameter.Subscribe(x => Volatile.Write(ref latestParam, x));

        addHandler(Handler);
        var eventDisp = Disposable.Create(() => removeHandler(Handler));

        var enabledSetter = Reflection.GetValueSetterForProperty(enabledProperty);
        if (enabledSetter is null)
        {
            return new CompositeDisposable(paramSub, eventDisp);
        }

        // Initial enabled state.
        enabledSetter(target, command.CanExecute(Volatile.Read(ref latestParam)), null);

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

        return new CompositeDisposable(paramSub, eventDisp, canExecuteSub);
    }

    /// <summary>
    /// Creates a command binding from a named event and an enabled property.
    /// </summary>
    /// <param name="command">The command to execute when the event fires.</param>
    /// <param name="target">The UI target object that exposes the event.</param>
    /// <param name="commandParameter">An observable providing the latest command parameter.</param>
    /// <param name="eventName">The event name to subscribe to.</param>
    /// <param name="enabledProperty">A property to set enabled state (best-effort).</param>
    /// <returns>A disposable that unsubscribes the event and stops updating enabled state.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    /// <remarks>
    /// This helper uses reflection-based event subscription and is not trimming-safe.
    /// </remarks>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    [RequiresDynamicCode("String/reflection-based event binding uses reflection and may require dynamic code generation.")]
    protected static IDisposable ForEvent(
        ICommand? command,
        object? target,
        IObservable<object?> commandParameter,
        string eventName,
        PropertyInfo enabledProperty)
    {
        ArgumentExceptionHelper.ThrowIfNull(command);
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(eventName);
        ArgumentExceptionHelper.ThrowIfNull(enabledProperty);

        commandParameter ??= Observable.Return(target);

        object? latestParam = null;

        var paramSub = commandParameter.Subscribe(x => Volatile.Write(ref latestParam, x));

        var actionSub = Observable.FromEventPattern(target, eventName).Subscribe(_ =>
        {
            var param = Volatile.Read(ref latestParam);
            if (command.CanExecute(param))
            {
                command.Execute(param);
            }
        });

        var enabledSetter = Reflection.GetValueSetterForProperty(enabledProperty);
        if (enabledSetter is null)
        {
            return new CompositeDisposable(paramSub, actionSub);
        }

        enabledSetter(target, command.CanExecute(Volatile.Read(ref latestParam)), null);

        var canExecuteSub = Observable.FromEvent<EventHandler, bool>(
                eventHandler =>
                {
                    void Handler(object? sender, EventArgs e) =>
                        eventHandler(command.CanExecute(Volatile.Read(ref latestParam)));
                    return Handler;
                },
                h => command.CanExecuteChanged += h,
                h => command.CanExecuteChanged -= h)
            .Subscribe(x => enabledSetter(target, x, null));

        return new CompositeDisposable(paramSub, actionSub, canExecuteSub);
    }

    /// <summary>
    /// Creates a command binding for UIKit controls using <see cref="UIControlEvent.TouchUpInside"/>
    /// and an enabled property.
    /// </summary>
    /// <param name="command">The command to execute when the control is activated.</param>
    /// <param name="target">The target object, expected to be a <see cref="UIControl"/>.</param>
    /// <param name="commandParameter">An observable providing the latest command parameter.</param>
    /// <param name="enabledProperty">The property used to set enabled state.</param>
    /// <returns>A disposable that unbinds the handler and stops updating enabled state.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    protected static IDisposable ForTargetAction(
        ICommand? command,
        object? target,
        IObservable<object?> commandParameter,
        PropertyInfo enabledProperty)
    {
        ArgumentExceptionHelper.ThrowIfNull(command);
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(enabledProperty);

        commandParameter ??= Observable.Return(target);

        if (target is not UIControl ctl)
        {
            return Disposable.Empty;
        }

        object? latestParam = null;

        // Stable handler instance for deterministic unsubscribe.
        void Handler(object? sender, EventArgs e)
        {
            var param = Volatile.Read(ref latestParam);
            if (command.CanExecute(param))
            {
                command.Execute(param);
            }
        }

        var paramSub = commandParameter.Subscribe(x => Volatile.Write(ref latestParam, x));

        // UIKit target-action via EventHandler is supported through UIControl's AddTarget overload.
        ctl.AddTarget(Handler, UIControlEvent.TouchUpInside);
        var actionDisp = Disposable.Create(() => ctl.RemoveTarget(Handler, UIControlEvent.TouchUpInside));

        var enabledSetter = Reflection.GetValueSetterForProperty(enabledProperty);
        if (enabledSetter is null)
        {
            return new CompositeDisposable(paramSub, actionDisp);
        }

        enabledSetter(target, command.CanExecute(Volatile.Read(ref latestParam)), null);

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

        return new CompositeDisposable(paramSub, actionDisp, canExecuteSub);
    }

    /// <summary>
    /// Registers a binding factory for a type with an affinity score.
    /// </summary>
    /// <param name="type">The registered type.</param>
    /// <param name="affinity">The affinity score used to select among candidates.</param>
    /// <param name="createBinding">The factory that creates the binding.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> or <paramref name="createBinding"/> is null.</exception>
    protected void Register(Type type, int affinity, Func<ICommand?, object?, IObservable<object?>, IDisposable> createBinding)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(createBinding);

        lock (_gate)
        {
            _config[type] = new CommandBindingInfo(affinity, createBinding);
            _version++;
            _snapshot = null;
        }
    }

    /// <summary>
    /// Produces or returns a cached snapshot of registrations for lock-free reads.
    /// </summary>
    /// <returns>The current snapshot.</returns>
    private Entry[] GetSnapshot()
    {
        var snapshot = Volatile.Read(ref _snapshot);
        var snapshotVersion = Volatile.Read(ref _snapshotVersion);

        if (snapshot is not null && snapshotVersion == Volatile.Read(ref _version))
        {
            return snapshot;
        }

        lock (_gate)
        {
            // Recheck under lock.
            var v = _version;
            snapshot = _snapshot;
            if (snapshot is not null && _snapshotVersion == v)
            {
                return snapshot;
            }

            var entries = new Entry[_config.Count];
            var i = 0;

            foreach (var kvp in _config)
            {
                var info = kvp.Value;
                entries[i++] = new Entry(kvp.Key, info.Affinity, info.CreateBinding);
            }

            Volatile.Write(ref _snapshotVersion, v);
            Volatile.Write(ref _snapshot, entries);

            return entries;
        }
    }

    /// <summary>
    /// Immutable snapshot entry for a registered binding factory.
    /// </summary>
    private readonly record struct Entry(
        Type Type,
        int Affinity,
        Func<ICommand?, object?, IObservable<object?>, IDisposable>? Factory);

    /// <summary>
    /// Stores binding configuration for a registered type.
    /// </summary>
    private sealed class CommandBindingInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBindingInfo"/> class.
        /// </summary>
        /// <param name="affinity">The affinity score.</param>
        /// <param name="createBinding">The binding factory.</param>
        public CommandBindingInfo(int affinity, Func<ICommand?, object?, IObservable<object?>, IDisposable> createBinding)
        {
            Affinity = affinity;
            CreateBinding = createBinding;
        }

        /// <summary>
        /// Gets the affinity score for this binding.
        /// </summary>
        public int Affinity { get; }

        /// <summary>
        /// Gets the binding factory.
        /// </summary>
        public Func<ICommand?, object?, IObservable<object?>, IDisposable> CreateBinding { get; }
    }
}
