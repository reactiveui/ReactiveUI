// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Manages notification state shared by reactive object implementations.</summary>
internal static class ReactiveNotificationHelpers
{
    /// <summary>Adds a property-changing event handler.</summary>
    /// <param name="source">The reactive object owning the event.</param>
    /// <param name="subscribed">Tracks whether the observable has been initialized.</param>
    /// <param name="handlers">The event handler store.</param>
    /// <param name="handler">The handler to add.</param>
    internal static void AddPropertyChanging(
        IReactiveObject source,
        ref bool subscribed,
        ref PropertyChangingEventHandler? handlers,
        PropertyChangingEventHandler? handler)
    {
        if (!subscribed)
        {
            source.SubscribePropertyChangingEvents();
            subscribed = true;
        }

        handlers += handler;
    }

    /// <summary>Adds a property-changed event handler.</summary>
    /// <param name="source">The reactive object owning the event.</param>
    /// <param name="subscribed">Tracks whether the observable has been initialized.</param>
    /// <param name="handlers">The event handler store.</param>
    /// <param name="handler">The handler to add.</param>
    internal static void AddPropertyChanged(
        IReactiveObject source,
        ref bool subscribed,
        ref PropertyChangedEventHandler? handlers,
        PropertyChangedEventHandler? handler)
    {
        if (!subscribed)
        {
            source.SubscribePropertyChangedEvents();
            subscribed = true;
        }

        handlers += handler;
    }

    /// <summary>Gets the lazily initialized property-changing observable.</summary>
    /// <param name="source">The reactive object owning the observable.</param>
    /// <param name="observable">The observable store.</param>
    /// <returns>The property-changing observable.</returns>
    internal static IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> GetChanging(
        IReactiveObject source,
        ref IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>? observable)
    {
        var current = Volatile.Read(ref observable);
        if (current is not null)
        {
            return current;
        }

        _ = Interlocked.CompareExchange(ref observable, source.GetChangingObservable(), null);
        return Volatile.Read(ref observable)!;
    }

    /// <summary>Gets the lazily initialized property-changed observable.</summary>
    /// <param name="source">The reactive object owning the observable.</param>
    /// <param name="observable">The observable store.</param>
    /// <returns>The property-changed observable.</returns>
    internal static IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> GetChanged(
        IReactiveObject source,
        ref IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>? observable)
    {
        var current = Volatile.Read(ref observable);
        if (current is not null)
        {
            return current;
        }

        _ = Interlocked.CompareExchange(ref observable, source.GetChangedObservable(), null);
        return Volatile.Read(ref observable)!;
    }

    /// <summary>Gets the lazily initialized exception observable.</summary>
    /// <param name="source">The reactive object owning the observable.</param>
    /// <param name="observable">The observable store.</param>
    /// <returns>The exception observable.</returns>
    internal static IObservable<Exception> GetThrownExceptions(
        IReactiveObject source,
        ref IObservable<Exception>? observable)
    {
        var current = Volatile.Read(ref observable);
        if (current is not null)
        {
            return current;
        }

        _ = Interlocked.CompareExchange(ref observable, source.GetThrownExceptionsObservable(), null);
        return Volatile.Read(ref observable)!;
    }
}
