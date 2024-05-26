// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Linq;
using Mopups.Events;
using Mopups.Pages;
using Mopups.Services;

namespace ReactiveUI.Maui.Plugins.Popup;

/// <summary>
/// INavigation Mixins.
/// </summary>
public static class INavigationMixins
{
    /// <summary>
    /// Pops all popup.
    /// </summary>
    /// <param name="navigation">The navigation.</param>
    /// <param name="animate">if set to <c>true</c> [animate].</param>
    /// <returns>An Observable of Unit.</returns>
    public static IObservable<Unit> PopAllPopup(this INavigation navigation, bool animate = true) =>
               Observable.FromAsync(async _ => await MopupService.Instance.PopAllAsync(animate).ConfigureAwait(false));

    /// <summary>
    /// Pops the popup.
    /// </summary>
    /// <param name="navigation">The navigation.</param>
    /// <param name="animate">if set to <c>true</c> [animate].</param>
    /// <returns>An Observable of Unit.</returns>
    public static IObservable<Unit> PopPopup(this INavigation navigation, bool animate = true) =>
        Observable.FromAsync(async _ => await MopupService.Instance.PopAsync(animate).ConfigureAwait(false));

    /// <summary>
    /// Pushes the popup.
    /// </summary>
    /// <typeparam name="T">The Type of Popup Page.</typeparam>
    /// <param name="navigation">The navigation.</param>
    /// <param name="page">The popup page.</param>
    /// <param name="animate">if set to <c>true</c> [animate].</param>
    /// <returns>An Observable of Unit.</returns>
    public static IObservable<Unit> PushPopup<T>(this INavigation navigation, T page, bool animate = true)
        where T : PopupPage => Observable.FromAsync(async _ => await MopupService.Instance.PushAsync(page, animate).ConfigureAwait(false));

    /// <summary>
    /// Removes the popup page.
    /// </summary>
    /// <typeparam name="T">The Type of Popup Page.</typeparam>
    /// <param name="navigation">The navigation.</param>
    /// <param name="page">The popup page.</param>
    /// <param name="animate">if set to <c>true</c> [animate].</param>
    /// <returns>An Observable of Unit.</returns>
    public static IObservable<Unit> RemovePopupPage<T>(this INavigation navigation, T page, bool animate = true)
        where T : PopupPage => Observable.FromAsync(async _ => await MopupService.Instance.RemovePageAsync(page, animate).ConfigureAwait(false));

    /// <summary>
    /// Poppings the specified service.
    /// </summary>
    /// <param name="navigation">The service.</param>
    /// <returns>A PopupNavigationEventArgs.</returns>
    public static IObservable<PopupNavigationEventArgs> PoppingObservable(this INavigation navigation) =>
        Observable.FromEvent<EventHandler<PopupNavigationEventArgs>, PopupNavigationEventArgs>(
                    handler =>
                    {
                        void EventHandler(object? sender, PopupNavigationEventArgs args) => handler(args);
                        return EventHandler;
                    },
                    x => MopupService.Instance.Popping += x,
                    x => MopupService.Instance.Popping -= x);

    /// <summary>
    /// Poppeds the observable.
    /// </summary>
    /// <param name="navigation">The service.</param>
    /// <returns>A PopupNavigationEventArgs.</returns>
    public static IObservable<PopupNavigationEventArgs> PoppedObservable(this INavigation navigation) =>
        Observable.FromEvent<EventHandler<PopupNavigationEventArgs>, PopupNavigationEventArgs>(
                    handler =>
                    {
                        void EventHandler(object? sender, PopupNavigationEventArgs args) => handler(args);
                        return EventHandler;
                    },
                    x => MopupService.Instance.Popped += x,
                    x => MopupService.Instance.Popped -= x);

    /// <summary>
    /// Pushings the observable.
    /// </summary>
    /// <param name="navigation">The service.</param>
    /// <returns>A PopupNavigationEventArgs.</returns>
    public static IObservable<PopupNavigationEventArgs> PushingObservable(this INavigation navigation) =>
        Observable.FromEvent<EventHandler<PopupNavigationEventArgs>, PopupNavigationEventArgs>(
                    handler =>
                    {
                        void EventHandler(object? sender, PopupNavigationEventArgs args) => handler(args);
                        return EventHandler;
                    },
                    x => MopupService.Instance.Pushing += x,
                    x => MopupService.Instance.Pushing -= x);

    /// <summary>
    /// Pusheds the observable.
    /// </summary>
    /// <param name="navigation">The service.</param>
    /// <returns>A PopupNavigationEventArgs.</returns>
    public static IObservable<PopupNavigationEventArgs> PushedObservable(this INavigation navigation) =>
        Observable.FromEvent<EventHandler<PopupNavigationEventArgs>, PopupNavigationEventArgs>(
                    handler =>
                    {
                        void EventHandler(object? sender, PopupNavigationEventArgs args) => handler(args);
                        return EventHandler;
                    },
                    x => MopupService.Instance.Pushed += x,
                    x => MopupService.Instance.Pushed -= x);
}
