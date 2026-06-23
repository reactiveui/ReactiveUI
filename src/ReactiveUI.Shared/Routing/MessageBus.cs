// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using ReactiveUI.Primitives.Disposables;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// <para>
/// MessageBus represents an object that can act as a "Message Bus", a
/// simple way for ViewModels and other objects to communicate with each
/// other in a loosely coupled way.
/// </para>
/// <para>
/// Specifying which messages go where is done via a combination of the Type
/// of the message as well as an additional "Contract" parameter; this is a
/// unique string used to distinguish between messages of the same Type, and
/// is arbitrarily set by the client.
/// </para>
/// </summary>
[System.Diagnostics.DebuggerDisplay("Messages = {_messageBus.Count}, SchedulerMappings = {_schedulerMappings.Count}")]
public class MessageBus : IMessageBus
{
    /// <summary>The internal store mapping type/contract pairs to weak-referenced subjects.</summary>
    private readonly Dictionary<(Type type, string? contract), NotAWeakReference> _messageBus = [];

    /// <summary>Maps type/contract pairs to their associated scheduler overrides.</summary>
    private readonly Dictionary<(Type type, string? contract), ISequencer> _schedulerMappings = [];

    /// <summary>Gets or sets the global message bus instance used for publishing and subscribing to messages across the application.</summary>
    /// <remarks>By default, this property is initialized with a standard message bus implementation.
    /// Assigning a custom implementation allows for advanced scenarios such as testing, customization, or integration
    /// with external messaging systems. This property is static and affects all consumers within the application
    /// domain.</remarks>
    public static IMessageBus Current { get; set; } = new MessageBus();

    /// <summary>Registers a scheduler for the type, which may be specified at runtime, and the contract.</summary>
    /// <remarks>If a scheduler is already registered for the specified runtime and contract, this will overwrite the existing registration.</remarks>
    /// <typeparam name="T">The type of the message to listen to.</typeparam>
    /// <param name="scheduler">The scheduler on which to post the
    /// notifications for the specified type and contract. CurrentThreadScheduler by default.</param>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public void RegisterScheduler<T>(ISequencer scheduler) =>
        RegisterScheduler<T>(scheduler, null);

    /// <summary>Registers a scheduler for the type, which may be specified at runtime, and the contract.</summary>
    /// <remarks>If a scheduler is already registered for the specified runtime and contract, this will overwrite the existing registration.</remarks>
    /// <typeparam name="T">The type of the message to listen to.</typeparam>
    /// <param name="scheduler">The scheduler on which to post the
    /// notifications for the specified type and contract. CurrentThreadScheduler by default.</param>
    /// <param name="contract">A unique string to distinguish messages with
    /// identical types (i.e. "MyCoolViewModel") - if the message type is
    /// only used for one purpose, leave this as null.</param>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public void RegisterScheduler<T>(ISequencer scheduler, string? contract) =>
        _schedulerMappings[(typeof(T), contract)] = scheduler;

    /// <summary>
    /// Listen provides an Observable that will fire whenever a Message is
    /// provided for this object via RegisterMessageSource or SendMessage.
    /// </summary>
    /// <typeparam name="T">The type of the message to listen to.</typeparam>
    /// <returns>An Observable representing the notifications posted to the
    /// message bus.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public IObservable<T> Listen<T>() => Listen<T>(null);

    /// <summary>
    /// Listen provides an Observable that will fire whenever a Message is
    /// provided for this object via RegisterMessageSource or SendMessage.
    /// </summary>
    /// <typeparam name="T">The type of the message to listen to.</typeparam>
    /// <param name="contract">A unique string to distinguish messages with
    /// identical types (i.e. "MyCoolViewModel") - if the message type is
    /// only used for one purpose, leave this as null.</param>
    /// <returns>An Observable representing the notifications posted to the
    /// message bus.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public IObservable<T> Listen<T>(string? contract) => new ListenObservable<T>(this, contract, skipFirst: true);

    /// <summary>
    /// Listen provides an Observable that will fire whenever a Message is
    /// provided for this object via RegisterMessageSource or SendMessage.
    /// </summary>
    /// <typeparam name="T">The type of the message to listen to.</typeparam>
    /// <returns>An Observable representing the notifications posted to the
    /// message bus.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public IObservable<T> ListenIncludeLatest<T>() => ListenIncludeLatest<T>(null);

    /// <summary>
    /// Listen provides an Observable that will fire whenever a Message is
    /// provided for this object via RegisterMessageSource or SendMessage.
    /// </summary>
    /// <typeparam name="T">The type of the message to listen to.</typeparam>
    /// <param name="contract">A unique string to distinguish messages with
    /// identical types (i.e. "MyCoolViewModel") - if the message type is
    /// only used for one purpose, leave this as null.</param>
    /// <returns>An Observable representing the notifications posted to the
    /// message bus.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public IObservable<T> ListenIncludeLatest<T>(string? contract) => new ListenObservable<T>(this, contract, skipFirst: false);

    /// <summary>Determines if a particular message Type is registered.</summary>
    /// <param name="type">The Type of the message to listen to.</param>
    /// <returns>True if messages have been posted for this message Type.</returns>
    public bool IsRegistered(Type type) => IsRegistered(type, null);

    /// <summary>Determines if a particular message Type is registered.</summary>
    /// <param name="type">The Type of the message to listen to.</param>
    /// <param name="contract">A unique string to distinguish messages with
    /// identical types (i.e. "MyCoolViewModel") - if the message type is
    /// only used for one purpose, leave this as null.</param>
    /// <returns>True if messages have been posted for this message Type.</returns>
    public bool IsRegistered(Type type, string? contract)
    {
        var ret = false;
        WithMessageBus(type, contract, (mb, item) => ret = mb.ContainsKey(item) && mb[item].IsAlive);

        return ret;
    }

    /// <summary>
    /// Registers an Observable representing the stream of messages to send.
    /// Another part of the code can then call Listen to retrieve this
    /// Observable.
    /// </summary>
    /// <typeparam name="T">The type of the message to listen to.</typeparam>
    /// <param name="source">An Observable that will be subscribed to, and a
    /// message sent out for each value provided.</param>
    /// <returns>a Disposable.</returns>
    public IDisposable RegisterMessageSource<T>(IObservable<T> source) =>
        RegisterMessageSource(source, null);

    /// <summary>
    /// Registers an Observable representing the stream of messages to send.
    /// Another part of the code can then call Listen to retrieve this
    /// Observable.
    /// </summary>
    /// <typeparam name="T">The type of the message to listen to.</typeparam>
    /// <param name="source">An Observable that will be subscribed to, and a
    /// message sent out for each value provided.</param>
    /// <param name="contract">A unique string to distinguish messages with
    /// identical types (i.e. "MyCoolViewModel") - if the message type is
    /// only used for one purpose, leave this as null.</param>
    /// <returns>a Disposable.</returns>
    public IDisposable RegisterMessageSource<T>(
        IObservable<T> source,
        string? contract)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);

        var subject = SetupSubjectIfNecessary<T>(contract);

        return source.Subscribe(new DelegateObserver<T>(
            subject.OnNext,
            ex => this.Log().Warn(ex, "MessageBus source for {0}:{1} terminated with an error.", typeof(T), contract),
            () => this.Log().Info(
                CultureInfo.InvariantCulture,
                "MessageBus source for {0}:{1} completed.",
                typeof(T),
                contract)));
    }

    /// <summary>
    /// Sends a single message using the specified Type and contract.
    /// Consider using RegisterMessageSource instead if you will be sending
    /// messages in response to other changes such as property changes
    /// or events.
    /// </summary>
    /// <typeparam name="T">The type of the message to send.</typeparam>
    /// <param name="message">The actual message to send.</param>
    public void SendMessage<T>(T message) => SendMessage(message, null);

    /// <summary>
    /// Sends a single message using the specified Type and contract.
    /// Consider using RegisterMessageSource instead if you will be sending
    /// messages in response to other changes such as property changes
    /// or events.
    /// </summary>
    /// <typeparam name="T">The type of the message to send.</typeparam>
    /// <param name="message">The actual message to send.</param>
    /// <param name="contract">A unique string to distinguish messages with
    /// identical types (i.e. "MyCoolViewModel") - if the message type is
    /// only used for one purpose, leave this as null.</param>
    public void SendMessage<T>(T message, string? contract) =>
        SetupSubjectIfNecessary<T>(contract).OnNext(message);

    /// <summary>Ensures that a subject for the specified type and contract exists, creating and registering it if necessary.</summary>
    /// <typeparam name="T">The type of the items published and observed by the subject.</typeparam>
    /// <param name="contract">An optional contract string used to distinguish between different subjects of the same type. Can be null.</param>
    /// <returns>An ISubject{T} instance associated with the specified type and contract. If a subject already exists, it is
    /// returned; otherwise, a new subject is created and registered.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    private ISignal<T> SetupSubjectIfNecessary<T>(string? contract)
    {
        // Inlined rather than routed through WithMessageBus: a captured-result Action allocated a closure plus a
        // delegate on every SendMessage. The get-or-create result is always alive, so the dead-entry sweep that
        // WithMessageBus performs is unnecessary here.
        var item = (typeof(T), contract);
        lock (_messageBus)
        {
            if (_messageBus.TryGetValue(item, out var subjRef) && subjRef.IsAlive)
            {
                return (ISignal<T>)subjRef.Target;
            }

            ISignal<T> ret = new ScheduledSubject<T>(GetScheduler(item), null, new BehaviorSignal<T>(default!));
            _messageBus[item] = new(ret);
            return ret;
        }
    }

    /// <summary>
    /// Executes a specified action while holding a lock on the message bus, providing access to the message bus
    /// dictionary and a key composed of the specified type and contract.
    /// </summary>
    /// <remarks>This method ensures thread-safe access to the message bus by acquiring a lock during the
    /// execution of the specified action. The provided dictionary may be modified within the action. If the referenced
    /// value for the specified key is no longer alive after the action, the key is removed from the
    /// dictionary.</remarks>
    /// <param name="type">The type component of the message bus key to operate on.</param>
    /// <param name="contract">The contract string component of the message bus key to operate on. Can be null to indicate no contract.</param>
    /// <param name="block">The action to execute, which receives the message bus dictionary and the key tuple as parameters.</param>
    private void WithMessageBus(
        Type type,
        string? contract,
        Action<Dictionary<(Type type, string? contract), NotAWeakReference>, (Type type, string? contract)> block)
    {
        lock (_messageBus)
        {
            var item = (type, contract);
            block(_messageBus, item);
            if (_messageBus.TryGetValue(item, out var value) && !value.IsAlive)
            {
                _ = _messageBus.Remove(item);
            }
        }
    }

    /// <summary>
    /// Retrieves the scheduler associated with the specified type and contract, or returns the current thread scheduler
    /// if no mapping exists.
    /// </summary>
    /// <param name="item">A tuple containing the type and an optional contract string used to identify the scheduler mapping.</param>
    /// <returns>The scheduler associated with the specified type and contract, or the current thread scheduler if no mapping is
    /// found.</returns>
    private ISequencer GetScheduler((Type type, string? contract) item)
    {
        _ = _schedulerMappings.TryGetValue(item, out var scheduler);
        return scheduler ?? Sequencer.CurrentThread;
    }

    /// <summary>
    /// Resolves the keyed subject lazily at subscription time and forwards it, optionally skipping the replayed
    /// current value. Specialised to <see cref="Listen{T}(string?)"/> / <see cref="ListenIncludeLatest{T}(string?)"/>.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="bus">The owning message bus.</param>
    /// <param name="contract">The contract identifying the message channel.</param>
    /// <param name="skipFirst">When true, the subject's replayed current value is suppressed.</param>
    private sealed class ListenObservable<T>(MessageBus bus, string? contract, bool skipFirst) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            ISignal<T> source;
            try
            {
                bus.Log().Info(CultureInfo.InvariantCulture, "Listening to {0}:{1}", typeof(T), contract);
                source = bus.SetupSubjectIfNecessary<T>(contract);
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
                return EmptyDisposable.Instance;
            }

            return skipFirst ? source.Subscribe(new SkipFirstObserver(observer)) : source.Subscribe(observer);
        }

        /// <summary>Forwards every value except the first to the downstream observer.</summary>
        /// <param name="downstream">The observer receiving values after the first.</param>
        private sealed class SkipFirstObserver(IObserver<T> downstream) : IObserver<T>
        {
            /// <summary>Whether the first value has been skipped.</summary>
            private bool _skipped;

            /// <inheritdoc/>
            public void OnNext(T value)
            {
                if (!_skipped)
                {
                    _skipped = true;
                    return;
                }

                downstream.OnNext(value);
            }

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }
}
