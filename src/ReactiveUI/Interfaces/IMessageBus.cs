// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// <para>
    /// IMessageBus represents an object that can act as a "Message Bus", a
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
    public interface IMessageBus : IEnableLogger
    {
        /// <summary>
        /// Registers a scheduler for the type, which may be specified at
        /// runtime, and the contract.
        /// </summary>
        /// <remarks>If a scheduler is already registered for the specified
        /// runtime and contract, this will overwrite the existing
        /// registration.</remarks>
        /// <typeparam name="T">The type of the message to listen to.</typeparam>
        /// <param name="scheduler">The scheduler on which to post the
        /// notifications for the specified type and contract.
        /// CurrentThreadScheduler by default.</param>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        void RegisterScheduler<T>(IScheduler scheduler, string? contract = null);

        /// <summary>
        /// Listen provides an Observable that will fire whenever a Message is
        /// provided for this object via RegisterMessageSource or SendMessage.
        /// </summary>
        /// <typeparam name="T">The type of the message to listen to.</typeparam>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns>An observable sequence.</returns>
        IObservable<T> Listen<T>(string? contract = null);

        /// <summary>
        /// ListenIncludeLatest provides an Observable that will fire whenever a Message is
        /// provided for this object via RegisterMessageSource or SendMessage and fire the
        /// last provided Message immediately if applicable, or null.
        /// </summary>
        /// <typeparam name="T">The type of the message to listen to.</typeparam>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns>An Observable representing the notifications posted to the
        /// message bus.</returns>
        IObservable<T> ListenIncludeLatest<T>(string? contract = null);

        /// <summary>
        /// Determines if a particular message Type is registered.
        /// </summary>
        /// <param name="type">The type of the message.</param>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns>True if messages have been posted for this message Type.</returns>
        bool IsRegistered(Type type, string? contract = null);

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
        /// <returns>A disposable.</returns>
        IDisposable RegisterMessageSource<T>(IObservable<T> source, string? contract = null);

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
        void SendMessage<T>(T message, string? contract = null);
    }
}
