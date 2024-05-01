// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
#if WINUI_TARGET
using Microsoft.UI.Xaml;
using Windows.Foundation;
#endif

#if IS_WINUI
namespace ReactiveUI.WinUI;
#endif
#if IS_MAUI
using Microsoft.Maui.Controls;

namespace ReactiveUI.Maui;
#endif

/// <summary>
/// This class is the default implementation that determines when views are Activated and Deactivated.
/// </summary>
/// <seealso cref="IActivationForViewFetcher" />
public class ActivationForViewFetcher : IActivationForViewFetcher
{
    /// <inheritdoc/>
    public int GetAffinityForView(Type view) =>
#if IS_WINUI
       typeof(FrameworkElement).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())
#endif
#if IS_MAUI
       typeof(Page).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ||
       typeof(View).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ||
       typeof(Cell).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())
#endif
            ? 10 : 0;

    /// <inheritdoc/>
    public IObservable<bool> GetActivationForView(IActivatableView view)
    {
        var activation =
            GetActivationFor(view as ICanActivate) ??
#if IS_WINUI
            GetActivationFor(view as FrameworkElement) ??
#endif
#if IS_MAUI
            GetActivationFor(view as Page) ??
            GetActivationFor(view as View) ??
            GetActivationFor(view as Cell) ??
#endif
            Observable<bool>.Never;

        return activation.DistinctUntilChanged();
    }

    private static IObservable<bool>? GetActivationFor(ICanActivate? canActivate) =>
        canActivate?.Activated.Select(_ => true).Merge(canActivate.Deactivated.Select(_ => false));

#if IS_MAUI
    private static IObservable<bool>? GetActivationFor(Page? page)
    {
        if (page is null)
        {
            return null;
        }

        var appearing = Observable.FromEvent<EventHandler, bool>(
                                                                 eventHandler =>
                                                                 {
                                                                     void Handler(object? sender, EventArgs e) => eventHandler(true);
                                                                     return Handler;
                                                                 },
                                                                 x => page.Appearing += x,
                                                                 x => page.Appearing -= x);

        var disappearing = Observable.FromEvent<EventHandler, bool>(
                                                                    eventHandler =>
                                                                    {
                                                                        void Handler(object? sender, EventArgs e) => eventHandler(false);
                                                                        return Handler;
                                                                    },
                                                                    x => page.Disappearing += x,
                                                                    x => page.Disappearing -= x);

        return appearing.Merge(disappearing);
    }
#endif

#if IS_MAUI
    private static IObservable<bool>? GetActivationFor(View? view)
    {
        if (view is null)
        {
            return null;
        }

        var propertyChanged = Observable.FromEvent<PropertyChangedEventHandler, string?>(
         eventHandler =>
         {
             void Handler(object? sender, PropertyChangedEventArgs e) => eventHandler(e.PropertyName);
             return Handler;
         },
         x => view.PropertyChanged += x,
         x => view.PropertyChanged -= x);

        return propertyChanged
               .Where(x => x == "IsVisible")
               .Select(_ => view.IsVisible)
               .StartWith(view.IsVisible);
    }

    private static IObservable<bool>? GetActivationFor(Cell? cell)
    {
        if (cell is null)
        {
            return null;
        }

        var appearing = Observable.FromEvent<EventHandler, bool>(
                                                                 eventHandler =>
                                                                 {
                                                                     void Handler(object? sender, EventArgs e) => eventHandler(true);
                                                                     return Handler;
                                                                 },
                                                                 x => cell.Appearing += x,
                                                                 x => cell.Appearing -= x);

        var disappearing = Observable.FromEvent<EventHandler, bool>(
                                                                    eventHandler =>
                                                                    {
                                                                        void Handler(object? sender, EventArgs e) => eventHandler(false);
                                                                        return Handler;
                                                                    },
                                                                    x => cell.Disappearing += x,
                                                                    x => cell.Disappearing -= x);

        return appearing.Merge(disappearing);
    }
#else
    private static IObservable<bool>? GetActivationFor(FrameworkElement? view)
    {
        if (view is null)
        {
            return null;
        }

        var viewLoaded = Observable.FromEvent<TypedEventHandler<FrameworkElement, object>, bool>(
         eventHandler =>
         {
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
             void Handler(FrameworkElement _, object __) => eventHandler(true);
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
             return Handler;
         },
         x => view.Loading += x,
         x => view.Loading -= x);

        var viewUnloaded = Observable.FromEvent<RoutedEventHandler, bool>(
                                                                          eventHandler =>
                                                                          {
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
                                                                              void Handler(object _, RoutedEventArgs __) => eventHandler(false);
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
                                                                              return Handler;
                                                                          },
                                                                          x => view.Unloaded += x,
                                                                          x => view.Unloaded -= x);

        return viewLoaded
               .Merge(viewUnloaded)
               .Select(b => b ? view.WhenAnyValue(x => x.IsHitTestVisible).SkipWhile(x => !x) : Observables.False)
               .Switch()
               .DistinctUntilChanged();
    }
#endif
}
