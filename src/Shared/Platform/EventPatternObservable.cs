// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Bridges a reflection-resolved CLR event into an observable, emitting the event arguments on each raise. A tailored
/// replacement for <c>Observable.FromEventPattern</c>.
/// </summary>
/// <typeparam name="TEventArgs">The event arguments type.</typeparam>
/// <param name="target">The object exposing the event.</param>
/// <param name="eventName">The name of the event to subscribe to.</param>
[RequiresUnreferencedCode("Resolves the event and its handler delegate type by reflection, which may be trimmed.")]
internal sealed class EventPatternObservable<TEventArgs>(object target, string eventName) : IObservable<TEventArgs>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<TEventArgs> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        var eventInfo = target.GetType().GetRuntimeEvent(eventName)
            ?? throw new InvalidOperationException($"Could not find event '{eventName}' on '{target.GetType()}'.");
        var handlerType = eventInfo.EventHandlerType
            ?? throw new InvalidOperationException($"Event '{eventName}' on '{target.GetType()}' has no handler type.");

        var forwarder = new Forwarder(observer);
        var handler = forwarder.CreateHandler(handlerType);
        eventInfo.AddEventHandler(target, handler);

        return new ActionDisposable(() => eventInfo.RemoveEventHandler(target, handler));
    }

    /// <summary>Forwards raised events to the observer, adapting to the event's handler delegate type.</summary>
    /// <param name="observer">The observer receiving the event arguments.</param>
    private sealed class Forwarder(IObserver<TEventArgs> observer)
    {
        /// <summary>Creates a delegate of the event's handler type that forwards to <see cref="OnEvent"/>.</summary>
        /// <param name="handlerType">The event's handler delegate type.</param>
        /// <returns>A delegate compatible with the event.</returns>
        public Delegate CreateHandler(Type handlerType) =>
            typeof(Forwarder).GetMethod(nameof(OnEvent), BindingFlags.Instance | BindingFlags.Public)!.CreateDelegate(handlerType, this);

        /// <summary>Forwards a raised event's arguments to the observer.</summary>
        /// <param name="sender">The event source.</param>
        /// <param name="args">The event arguments.</param>
        public void OnEvent(object? sender, TEventArgs args) => observer.OnNext(args);
    }
}
