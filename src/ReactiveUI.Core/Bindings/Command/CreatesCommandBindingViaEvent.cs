// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ReactiveUI.Internal;
using ReactiveUI.Primitives.Disposables;

namespace ReactiveUI;

/// <summary>Default command binder that connects an <see cref="ICommand"/> to an event on a target object.</summary>
/// <remarks>
/// <para>
/// This binder supports a small set of conventional "default" events (for example, <c>Click</c>),
/// and can also bind to an explicitly named event.
/// </para>
/// <para>
/// Reflection-based event lookup and string-based event subscription are not trimming/AOT-safe in general.
/// Use the generic overloads with explicit with the add/remove handler delegates to avoid the reflection cost.
/// </para>
/// </remarks>
public sealed class CreatesCommandBindingViaEvent : ICreatesCommandBinding
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            return BindingAffinity.Explicit;
        }

        return DefaultEventCache<T>.HasDefaultEvent ? BindingAffinity.DefaultEvent : 0;
    }

    /// <summary>Binds a command to the default event on a target object using a generic type parameter.</summary>
    /// <typeparam name="T">The type of the target object.</typeparam>
    /// <param name="command">The command to bind. If <see langword="null"/>, no binding is created.</param>
    /// <param name="target">The target object.</param>
    /// <param name="commandParameter">An observable that supplies command parameter values.</param>
    /// <returns>A disposable that unbinds the command.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is <see langword="null"/>.</exception>
    /// <exception cref="Exception">
    /// Thrown when no default event exists on <typeparamref name="T"/> and the caller did not specify an event explicitly.
    /// </exception>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    public IDisposable? BindCommandToObject<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.PublicEvents |
                                    DynamicallyAccessedMemberTypes.NonPublicEvents)]
    T>(
        ICommand? command,
        T? target,
        IObservable<object?> commandParameter)
        where T : class
    {
        ArgumentExceptionHelper.ThrowIfNull(target);

        if (command is null)
        {
            return EmptyDisposable.Instance;
        }

        var eventName = DefaultEventCache<T>.DefaultEventName ?? throw new InvalidOperationException(
                $"Couldn't find a default event to bind to on {typeof(T).FullName}, specify an event explicitly");
        return BindCommandToObject<T, EventArgs>(command, target, commandParameter, eventName);
    }

    /// <summary>Binds a command to a specific event on a target object using generic type parameters.</summary>
    /// <typeparam name="T">The type of the target object.</typeparam>
    /// <typeparam name="TEventArgs">The event arguments type.</typeparam>
    /// <param name="command">The command to bind. If <see langword="null"/>, no binding is created.</param>
    /// <param name="target">The target object.</param>
    /// <param name="commandParameter">An observable that supplies command parameter values.</param>
    /// <param name="eventName">The name of the event to bind to.</param>
    /// <returns>A disposable that unbinds the command.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="target"/> or <paramref name="eventName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="eventName"/> is empty.
    /// </exception>
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
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(eventName);

        if (eventName.Length == 0)
        {
            throw new ArgumentException("Event name must not be empty.", nameof(eventName));
        }

        if (command is null)
        {
            return EmptyDisposable.Instance;
        }

        object? latestParameter = null;

        MultipleDisposable ret = [];

        ret.Add(commandParameter.Subscribe(new DelegateObserver<object?>(
            static x =>
                Volatile.Write(ref Unsafe.As<object?, object?>(ref x), x))));

        ret.Clear();

        ret.Add(commandParameter.Subscribe(new DelegateObserver<object?>(x => Volatile.Write(ref latestParameter, x))));

        var evt = new EventPatternObservable<TEventArgs>(target, eventName);

        ret.Add(evt.Subscribe(new DelegateObserver<TEventArgs>(_ =>
        {
            var param = Volatile.Read(ref latestParameter);
            if (!command.CanExecute(param))
            {
                return;
            }

            command.Execute(param);
        })));

        return ret;
    }

    /// <summary>Binds a command to a specific event on a target object using explicit add/remove handler delegates.</summary>
    /// <typeparam name="T">The type of the target object.</typeparam>
    /// <typeparam name="TEventArgs">The event arguments type.</typeparam>
    /// <param name="command">The command to bind. If <see langword="null"/>, no binding is created.</param>
    /// <param name="target">The target object.</param>
    /// <param name="commandParameter">An observable that supplies command parameter values.</param>
    /// <param name="addHandler">Adds the handler to the target event.</param>
    /// <param name="removeHandler">Removes the handler from the target event.</param>
    /// <returns>A disposable that unbinds the command.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="target"/>, <paramref name="addHandler"/>, or <paramref name="removeHandler"/> is <see langword="null"/>.
    /// </exception>
    public IDisposable BindCommandToObject<
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

        object? latestParameter = null;

        var parameterSub = commandParameter.Subscribe(new DelegateObserver<object?>(x => Volatile.Write(ref latestParameter, x)));

        addHandler(Handler);

        return new MultipleDisposable(
            parameterSub,
            new ActionDisposable(() => removeHandler(Handler)));

        void Handler(object? sender, TEventArgs eventArgs)
        {
            _ = sender;
            _ = eventArgs;
            var param = Volatile.Read(ref latestParameter);
            if (!command.CanExecute(param))
            {
                return;
            }

            command.Execute(param);
        }
    }

    /// <summary>Binds a command to an event using explicit add/remove handler actions (non-reflection).</summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="command">The command to bind. If <see langword="null"/>, no binding is created.</param>
    /// <param name="target">The target object.</param>
    /// <param name="commandParameter">An observable that supplies command parameter values.</param>
    /// <param name="addHandler">Adds the handler to the target event.</param>
    /// <param name="removeHandler">Removes the handler from the target event.</param>
    /// <returns>A disposable that unbinds the command.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="target"/>, <paramref name="addHandler"/>, or <paramref name="removeHandler"/> is <see langword="null"/>.
    /// </exception>
    public IDisposable BindCommandToObject<T>(
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
            return EmptyDisposable.Instance;
        }

        object? latestParameter = null;

        MultipleDisposable ret =
        [
            commandParameter.Subscribe(new DelegateObserver<object?>(x => Volatile.Write(ref latestParameter, x))),
            new ActionDisposable(() => removeHandler(Handler))
        ];

        addHandler(Handler);
        return ret;

        void Handler(object? sender, EventArgs eventArgs)
        {
            _ = sender;
            _ = eventArgs;
            var param = Volatile.Read(ref latestParameter);
            if (!command.CanExecute(param))
            {
                return;
            }

            command.Execute(param);
        }
    }

    /// <summary>Per-closed-generic cache for default event resolution.</summary>
    /// <typeparam name="T">The target type.</typeparam>
    [SuppressMessage("Design", "SST1431:Reference a type parameter, or move the member off the generic type", Justification = "Deliberate per-closed-generic reflection cache.")]
    private static class DefaultEventCache<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents)]
    T>
    {
        /// <summary>Gets the selected default event name for <typeparamref name="T"/>, or <see langword="null"/> if none exists.</summary>
        public static readonly string? DefaultEventName = FindDefaultEventName();

        /// <summary>Gets a value indicating whether <typeparamref name="T"/> has any default event supported by this binder.</summary>
        public static readonly bool HasDefaultEvent = DefaultEventName is not null;

        /// <summary>Default events to attempt, in priority order.</summary>
        /// <remarks>
        /// The first event found on the target type is used.
        /// </remarks>
        private static readonly (string Name, Type ArgsType)[] DefaultEventsToBind =
        [
            ("Click", typeof(EventArgs)),
            ("TouchUpInside", typeof(EventArgs)),
            ("MouseUp", typeof(EventArgs))
        ];

        /// <summary>Scans the default events list and returns the name of the first event found on the type, or null if none exist.</summary>
        /// <returns>The name of the first supported default event, or null if none exist.</returns>
        private static string? FindDefaultEventName()
        {
            var type = typeof(T);

            for (var i = 0; i < DefaultEventsToBind.Length; i++)
            {
                var name = DefaultEventsToBind[i].Name;
                if (type.GetRuntimeEvent(name) is not null)
                {
                    return name;
                }
            }

            return null;
        }
    }
}
