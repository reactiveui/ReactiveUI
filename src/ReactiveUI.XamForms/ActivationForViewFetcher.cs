﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace ReactiveUI.XamForms
{
    /// <summary>
    /// This class is the default implementation that determines when views are Activated and Deactivated.
    /// </summary>
    /// <seealso cref="ReactiveUI.IActivationForViewFetcher" />
    public class ActivationForViewFetcher : IActivationForViewFetcher
    {
        /// <inheritdoc/>
        public int GetAffinityForView(Type view)
        {
            return
                typeof(Page).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ||
                typeof(View).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ||
                typeof(Cell).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())
                ? 10 : 0;
        }

        /// <inheritdoc/>
        public IObservable<bool> GetActivationForView(IActivatableView view)
        {
            var activation =
                GetActivationFor(view as ICanActivate) ??
                GetActivationFor(view as Page) ??
                GetActivationFor(view as View) ??
                GetActivationFor(view as Cell) ??
                Observable<bool>.Never;

            return activation.DistinctUntilChanged();
        }

        private static IObservable<bool>? GetActivationFor(ICanActivate? canActivate)
        {
            if (canActivate == null)
            {
                return null;
            }

            return Observable.Merge(
                canActivate.Activated.Select(_ => true),
                canActivate.Deactivated.Select(_ => false));
        }

        private static IObservable<bool>? GetActivationFor(Page? page)
        {
            if (page == null)
            {
                return null;
            }

            var appearing = Observable.FromEvent<EventHandler, bool>(
                eventHandler =>
                {
                    void Handler(object sender, EventArgs e) => eventHandler(true);
                    return Handler;
                },
                x => page.Appearing += x,
                x => page.Appearing -= x);

            var disappearing = Observable.FromEvent<EventHandler, bool>(
                eventHandler =>
                {
                    void Handler(object sender, EventArgs e) => eventHandler(false);
                    return Handler;
                },
                x => page.Disappearing += x,
                x => page.Disappearing -= x);

            return Observable.Merge(appearing, disappearing);
        }

        private static IObservable<bool>? GetActivationFor(View? view)
        {
            if (view == null)
            {
                return null;
            }

            var propertyChanged = Observable.FromEvent<PropertyChangedEventHandler, string>(
                eventHandler =>
                {
                    void Handler(object sender, PropertyChangedEventArgs e) => eventHandler(e.PropertyName);
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
            if (cell == null)
            {
                return null;
            }

            var appearing = Observable.FromEvent<EventHandler, bool>(
                    eventHandler =>
                    {
                        void Handler(object sender, EventArgs e) => eventHandler(true);
                        return Handler;
                    },
                    x => cell.Appearing += x,
                    x => cell.Appearing -= x);

            var disappearing = Observable.FromEvent<EventHandler, bool>(
                    eventHandler =>
                    {
                        void Handler(object sender, EventArgs e) => eventHandler(false);
                        return Handler;
                    },
                    x => cell.Disappearing += x,
                    x => cell.Disappearing -= x);

            return Observable.Merge(appearing, disappearing);
        }
    }
}
