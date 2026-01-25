// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace ReactiveUI;

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
public class MessageBus : IMessageBus
{
    private readonly Dictionary<(Type type, string? contract), NotAWeakReference> _messageBus = [];

    private readonly Dictionary<(Type type, string? contract), IScheduler> _schedulerMappings = [];

    /// <summary>
    /// Gets or sets the global message bus instance used for publishing and subscribing to messages across the
    /// application.
    /// </summary>
    /// <remarks>By default, this property is initialized with a standard message bus implementation.
    /// Assigning a custom implementation allows for advanced scenarios such as testing, customization, or integration
    /// with external messaging systems. This property is static and affects all consumers within the application
    /// domain.</remarks>
    public static IMessageBus Current { get; set; } = new MessageBus();

    /// <summary>
    /// Registers a scheduler for the type, which may be specified at runtime, and the contract.
    /// </summary>
    /// <remarks>If a scheduler is already registered for the specified runtime and contract, this will overwrite the existing registration.</remarks>
    /// <typeparam name="T">The type of the message to listen to.</typeparam>
    /// <param name="scheduler">The scheduler on which to post the
    /// notifications for the specified type and contract. CurrentThreadScheduler by default.</param>
    /// <param name="contract">A unique string to distinguish messages with
    /// identical types (i.e. "MyCoolViewModel") - if the message type is
    /// only used for one purpose, leave this as null.</param>
    public void RegisterScheduler<T>(IScheduler scheduler, string? contract = null) =>
        _schedulerMappings[(typeof(T), contract)] = scheduler;

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
    public IObservable<T> Listen<T>(string? contract = null)
    {
        return Observable.Defer(() =>
        {
            this.Log().Info(CultureInfo.InvariantCulture, "Listening to {0}:{1}", typeof(T), contract);
            return SetupSubjectIfNecessary<T>(contract).Skip(1);
        });
    }

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
    public IObservable<T> ListenIncludeLatest<T>(string? contract = null)
    {
        return Observable.Defer(() =>
        {
            this.Log().Info(CultureInfo.InvariantCulture, "Listening to {0}:{1}", typeof(T), contract);
            return SetupSubjectIfNecessary<T>(contract);
        });
    }

    /// <summary>
    /// Determines if a particular message Type is registered.
    /// </summary>
    /// <param name="type">The Type of the message to listen to.</param>
    /// <param name="contract">A unique string to distinguish messages with
    /// identical types (i.e. "MyCoolViewModel") - if the message type is
    /// only used for one purpose, leave this as null.</param>
    /// <returns>True if messages have been posted for this message Type.</returns>
    public bool IsRegistered(Type type, string? contract = null)
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
    /// <param name="contract">A unique string to distinguish messages with
    /// identical types (i.e. "MyCoolViewModel") - if the message type is
    /// only used for one purpose, leave this as null.</param>
    /// <returns>a Disposable.</returns>
    public IDisposable RegisterMessageSource<T>(
        IObservable<T> source,
        string? contract = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);

        var subject = SetupSubjectIfNecessary<T>(contract);

        return source.Subscribe(
            subject.OnNext,
            ex => this.Log().Warn(ex, "MessageBus source for {0}:{1} terminated with an error.", typeof(T), contract),
            () => this.Log().Info(CultureInfo.InvariantCulture, "MessageBus source for {0}:{1} completed.", typeof(T), contract));
    }

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
    public void SendMessage<T>(T message, string? contract = null) => SetupSubjectIfNecessary<T>(contract).OnNext(message);

    /// <summary>
    /// Ensures that a subject for the specified type and contract exists, creating and registering it if necessary.
    /// </summary>
    /// <typeparam name="T">The type of the items published and observed by the subject.</typeparam>
    /// <param name="contract">An optional contract string used to distinguish between different subjects of the same type. Can be null.</param>
    /// <returns>An ISubject{T} instance associated with the specified type and contract. If a subject already exists, it is
    /// returned; otherwise, a new subject is created and registered.</returns>
    private ISubject<T> SetupSubjectIfNecessary<T>(string? contract)
    {
        ISubject<T>? ret = null;

        WithMessageBus(typeof(T), contract, (mb, item) =>
        {
            if (mb.TryGetValue(item, out var subjRef) && subjRef.IsAlive)
            {
                ret = (ISubject<T>)subjRef.Target;
                return;
            }

            ret = new ScheduledSubject<T>(GetScheduler(item), null, new BehaviorSubject<T>(default!));
            mb[item] = new NotAWeakReference(ret);
        });

        return ret!;
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
                _messageBus.Remove(item);
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
    private IScheduler GetScheduler((Type type, string? contract) item)
    {
        _schedulerMappings.TryGetValue(item, out var scheduler);
        return scheduler ?? CurrentThreadScheduler.Instance;
    }
}
