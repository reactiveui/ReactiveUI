// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Linq;
using Mopups.Events;
using Mopups.Interfaces;
using Mopups.Pages;

namespace ReactiveUI.Maui.Plugins.Popup;

/// <summary>
/// IPopupNavigation Mixins.
/// </summary>
public static class IPopupNavigationMixins
{
    /// <summary>
    /// Pops all popup.
    /// </summary>
    /// <param name="service">The navigation.</param>
    /// <param name="animate">if set to <c>true</c> [animate].</param>
    /// <returns>An Observable of Unit.</returns>
    public static IObservable<Unit> PopAllPopup(this IPopupNavigation service, bool animate = true) =>
               Observable.FromAsync(async _ => await service.PopAllAsync(animate).ConfigureAwait(false));

    /// <summary>
    /// Pops the popup.
    /// </summary>
    /// <param name="service">The navigation.</param>
    /// <param name="animate">if set to <c>true</c> [animate].</param>
    /// <returns>An Observable of Unit.</returns>
    public static IObservable<Unit> PopPopup(this IPopupNavigation service, bool animate = true) =>
        Observable.FromAsync(async _ => await service.PopAsync(animate).ConfigureAwait(false));

    /// <summary>
    /// Pushes the popup.
    /// </summary>
    /// <typeparam name="T">The Type of Popup Page.</typeparam>
    /// <param name="service">The navigation.</param>
    /// <param name="page">The popup page.</param>
    /// <param name="animate">if set to <c>true</c> [animate].</param>
    /// <returns>An Observable of Unit.</returns>
    public static IObservable<Unit> PushPopup<T>(this IPopupNavigation service, T page, bool animate = true)
        where T : PopupPage => Observable.FromAsync(async _ => await service.PushAsync(page, animate).ConfigureAwait(false));

    /// <summary>
    /// Removes the popup page.
    /// </summary>
    /// <typeparam name="T">The Type of Popup Page.</typeparam>
    /// <param name="service">The navigation.</param>
    /// <param name="page">The popup page.</param>
    /// <param name="animate">if set to <c>true</c> [animate].</param>
    /// <returns>An Observable of Unit.</returns>
    public static IObservable<Unit> RemovePopupPage<T>(this IPopupNavigation service, T page, bool animate = true)
        where T : PopupPage => Observable.FromAsync(async _ => await service.RemovePageAsync(page, animate).ConfigureAwait(false));

    /// <summary>
    /// Poppings the specified service.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <returns>A PopupNavigationEventArgs.</returns>
    public static IObservable<PopupNavigationEventArgs> PoppingObservable(this IPopupNavigation service) =>
        Observable.FromEvent<EventHandler<PopupNavigationEventArgs>, PopupNavigationEventArgs>(
                    handler =>
                    {
                        void EventHandler(object? sender, PopupNavigationEventArgs args) => handler(args);
                        return EventHandler;
                    },
                    x => service.Popping += x,
                    x => service.Popping -= x);

    /// <summary>
    /// Poppeds the observable.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <returns>A PopupNavigationEventArgs.</returns>
    public static IObservable<PopupNavigationEventArgs> PoppedObservable(this IPopupNavigation service) =>
        Observable.FromEvent<EventHandler<PopupNavigationEventArgs>, PopupNavigationEventArgs>(
                    handler =>
                    {
                        void EventHandler(object? sender, PopupNavigationEventArgs args) => handler(args);
                        return EventHandler;
                    },
                    x => service.Popped += x,
                    x => service.Popped -= x);

    /// <summary>
    /// Pushings the observable.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <returns>A PopupNavigationEventArgs.</returns>
    public static IObservable<PopupNavigationEventArgs> PushingObservable(this IPopupNavigation service) =>
        Observable.FromEvent<EventHandler<PopupNavigationEventArgs>, PopupNavigationEventArgs>(
                    handler =>
                    {
                        void EventHandler(object? sender, PopupNavigationEventArgs args) => handler(args);
                        return EventHandler;
                    },
                    x => service.Pushing += x,
                    x => service.Pushing -= x);

    /// <summary>
    /// Pusheds the observable.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <returns>A PopupNavigationEventArgs.</returns>
    public static IObservable<PopupNavigationEventArgs> PushedObservable(this IPopupNavigation service) =>
        Observable.FromEvent<EventHandler<PopupNavigationEventArgs>, PopupNavigationEventArgs>(
                    handler =>
                    {
                        void EventHandler(object? sender, PopupNavigationEventArgs args) => handler(args);
                        return EventHandler;
                    },
                    x => service.Pushed += x,
                    x => service.Pushed -= x);
}
