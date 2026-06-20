// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows.Input;
using ReactiveUI.Internal;
using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// Provides a base class for creating flexible command binding strategies that associate commands with object events
/// and properties at runtime.
/// </summary>
/// <remarks>FlexibleCommandBinder enables advanced scenarios for binding ICommand instances to various object
/// types, supporting custom event and property conventions. Implementations can register binding strategies for
/// specific types and control how commands are attached to UI elements or other objects. This class is intended for use
/// in frameworks or libraries that require extensible command binding logic, such as MVVM platforms. Thread safety and
/// binding lifetime management are the responsibility of the caller.</remarks>
public abstract class FlexibleCommandBinder : ICreatesCommandBinding
{
    /// <summary>Configuration map.</summary>
    private readonly Dictionary<Type, CommandBindingInfo> _config = [];

    /// <inheritdoc/>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public int GetAffinityForObject<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                    DynamicallyAccessedMemberTypes.PublicProperties)]
    T>(bool hasEventTarget)
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
    public IDisposable? BindCommandToObject<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.PublicEvents |
                                    DynamicallyAccessedMemberTypes.NonPublicEvents)]
    T>(ICommand? command, T? target, IObservable<object?> commandParameter)
        where T : class
    {
        ArgumentExceptionHelper.ThrowIfNull(target);

        var type = target.GetType();

        var match = _config.Keys
            .Where(x => x.IsAssignableFrom(type))
            .OrderByDescending(x => _config[x].Affinity)
            .FirstOrDefault() ?? throw new NotSupportedException($"CommandBinding for {type.Name} is not supported");
        var typeProperties = _config[match];

        return typeProperties.CreateBinding?.Invoke(command, target, commandParameter) ?? EmptyDisposable.Instance;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public IDisposable? BindCommandToObject<T, TEventArgs>(
        ICommand? command,
        T? target,
        IObservable<object?> commandParameter,
        string eventName)
        where T : class
        => EmptyDisposable.Instance;

    /// <inheritdoc/>
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

        if (command is null)
        {
            return EmptyDisposable.Instance;
        }

        commandParameter ??= new SingleValueObservable<object?>(target);

        object? latestParam = null;

        var parameterSub = commandParameter.Subscribe(new DelegateObserver<object?>(x => Volatile.Write(ref latestParam, x)));
        addHandler(Handler);

        Action<object?, object?, object?[]?>? enabledSetter = null;
        try
        {
            var enabledProp = typeof(T).GetRuntimeProperty("Enabled");
            if (enabledProp is not null)
            {
                enabledSetter = Reflection.GetValueSetterForProperty(enabledProp);
            }
        }
        catch
        {
            enabledSetter = null;
        }

        IDisposable? canExecuteSub = null;
        if (enabledSetter is not null)
        {
            enabledSetter(target, command.CanExecute(Volatile.Read(ref latestParam)), null);

            canExecuteSub = new CanExecuteChangedObservable(command, () => command.CanExecute(Volatile.Read(ref latestParam)))
                .Subscribe(new DelegateObserver<bool>(x => enabledSetter(target, x, null)));
        }

        return canExecuteSub is null
            ? new MultipleDisposable(
                parameterSub,
                new ActionDisposable(() => removeHandler(Handler)))
            : new MultipleDisposable(
                parameterSub,
                canExecuteSub,
                new ActionDisposable(() => removeHandler(Handler)));

        void Handler(object? sender, TEventArgs e)
        {
            var param = Volatile.Read(ref latestParam);
            if (!command.CanExecute(param))
            {
                return;
            }

            command.Execute(param);
        }
    }

    /// <summary>Creates a commands binding from event and a property.</summary>
    /// <returns>The binding from event.</returns>
    /// <param name="command">Command.</param>
    /// <param name="target">Target.</param>
    /// <param name="commandParameter">Command parameter.</param>
    /// <param name="eventName">Event name.</param>
    /// <param name="enabledProperty">Enabled property name.</param>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    protected static IDisposable ForEvent(
        ICommand? command,
        object? target,
        IObservable<object?> commandParameter,
        string eventName,
        PropertyInfo enabledProperty)
    {
        ArgumentExceptionHelper.ThrowIfNull(command);

        commandParameter ??= new SingleValueObservable<object?>(target);

        object? latestParam = null;
        var ctl = target!;

        var actionDisp = new EventPatternObservable<EventArgs>(ctl, eventName).Subscribe(new DelegateObserver<EventArgs>(_ =>
        {
            if (!command.CanExecute(latestParam))
            {
                return;
            }

            command.Execute(latestParam);
        }));

        var enabledSetter = Reflection.GetValueSetterForProperty(enabledProperty);
        if (enabledSetter is null)
        {
            return actionDisp;
        }

        enabledSetter(target, command.CanExecute(latestParam), null);

        return new MultipleDisposable(
            actionDisp,
            commandParameter.Subscribe(new DelegateObserver<object?>(x => latestParam = x)),
            new CanExecuteChangedObservable(command, () => command.CanExecute(latestParam))
                .Subscribe(new DelegateObserver<bool>(x => enabledSetter(target, x, null))));
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
        ArgumentExceptionHelper.ThrowIfNull(addHandler);
        ArgumentExceptionHelper.ThrowIfNull(removeHandler);

        commandParameter ??= new SingleValueObservable<object?>(target);

        object? latestParam = null;

        var parameterSub = commandParameter.Subscribe(new DelegateObserver<object?>(x => Volatile.Write(ref latestParam, x)));

        addHandler(Handler);

        if (enabledSetter is null)
        {
            return new MultipleDisposable(
                parameterSub,
                new ActionDisposable(() => removeHandler(Handler)));
        }

        enabledSetter(target, command.CanExecute(Volatile.Read(ref latestParam)), null);

        var canExecuteSub = new CanExecuteChangedObservable(command, () => command.CanExecute(Volatile.Read(ref latestParam)))
            .Subscribe(new DelegateObserver<bool>(x => enabledSetter(target, x, null)));

        return new MultipleDisposable(
            parameterSub,
            canExecuteSub,
            new ActionDisposable(() => removeHandler(Handler)));

        void Handler(object? sender, TEventArgs e)
        {
            var param = Volatile.Read(ref latestParam);
            if (!command.CanExecute(param))
            {
                return;
            }

            command.Execute(param);
        }
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
        ArgumentExceptionHelper.ThrowIfNull(addHandler);
        ArgumentExceptionHelper.ThrowIfNull(removeHandler);

        commandParameter ??= new SingleValueObservable<object?>(target);

        object? latestParam = null;

        var parameterSub = commandParameter.Subscribe(new DelegateObserver<object?>(x => Volatile.Write(ref latestParam, x)));

        addHandler(Handler);

        if (enabledSetter is null)
        {
            return new MultipleDisposable(
                parameterSub,
                new ActionDisposable(() => removeHandler(Handler)));
        }

        enabledSetter(target, command.CanExecute(Volatile.Read(ref latestParam)), null);

        var canExecuteSub = new CanExecuteChangedObservable(command, () => command.CanExecute(Volatile.Read(ref latestParam)))
            .Subscribe(new DelegateObserver<bool>(x => enabledSetter(target, x, null)));

        return new MultipleDisposable(
            parameterSub,
            canExecuteSub,
            new ActionDisposable(() => removeHandler(Handler)));

        void Handler(object? sender, EventArgs e)
        {
            var param = Volatile.Read(ref latestParam);
            if (!command.CanExecute(param))
            {
                return;
            }

            command.Execute(param);
        }
    }

    /// <summary>Registers an observable factory for the specified type and property.</summary>
    /// <param name="type">Type.</param>
    /// <param name="affinity">The affinity for the type.</param>
    /// <param name="createBinding">Creates the binding.</param>
    protected void Register(
        Type type,
        int affinity,
        Func<ICommand?, object?, IObservable<object?>, IDisposable> createBinding) =>
        _config[type] = new() { Affinity = affinity, CreateBinding = createBinding };

    /// <summary>
    /// Emits the command's current can-execute value each time <see cref="ICommand.CanExecuteChanged"/> fires —
    /// replacing <c>Observable.FromEvent</c> over that event. The handler is detached when the subscription is disposed.
    /// </summary>
    /// <param name="command">The command whose <see cref="ICommand.CanExecuteChanged"/> event is observed.</param>
    /// <param name="readCanExecute">Reads the command's current can-execute value when the event fires.</param>
    private sealed class CanExecuteChangedObservable(ICommand command, Func<bool> readCanExecute) : IObservable<bool>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            void Handler(object? sender, EventArgs e) => observer.OnNext(readCanExecute());

            command.CanExecuteChanged += Handler;
            return new ActionDisposable(() => command.CanExecuteChanged -= Handler);
        }
    }

    /// <summary>Provides information about a command binding, including its affinity and a factory for creating the binding.</summary>
    private sealed class CommandBindingInfo
    {
        /// <summary>Gets or sets the affinity that ranks this binding against others for the same type.</summary>
        public int Affinity { get; set; }

        /// <summary>Gets or sets the factory that creates the command binding.</summary>
        public Func<ICommand?, object?, IObservable<object?>, IDisposable>? CreateBinding { get; set; }
    }
}
