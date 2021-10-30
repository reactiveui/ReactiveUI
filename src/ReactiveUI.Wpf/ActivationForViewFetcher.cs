// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace ReactiveUI
{
    /// <summary>
    /// ActivationForViewFetcher is how ReactiveUI determine when a
    /// View is activated or deactivated. This is usually only used when porting
    /// ReactiveUI to a new UI framework.
    /// </summary>
    public class ActivationForViewFetcher : IActivationForViewFetcher
    {
        /// <inheritdoc/>
        public int GetAffinityForView(Type view) => typeof(FrameworkElement).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ? 10 : 0;

        /// <inheritdoc/>
        public IObservable<bool> GetActivationForView(IActivatableView view)
        {
            if (!(view is FrameworkElement fe))
            {
                return Observable<bool>.Empty;
            }

            var viewLoaded = Observable.FromEvent<RoutedEventHandler, bool>(
                eventHandler =>
                {
                    void Handler(object sender, RoutedEventArgs e) => eventHandler(true);
                    return Handler;
                },
                x => fe.Loaded += x,
                x => fe.Loaded -= x);

            var hitTestVisible = Observable.FromEvent<DependencyPropertyChangedEventHandler, bool>(
                eventHandler =>
                {
                    void Handler(object sender, DependencyPropertyChangedEventArgs e) => eventHandler((bool)e.NewValue);
                    return Handler;
                },
                x => fe.IsHitTestVisibleChanged += x,
                x => fe.IsHitTestVisibleChanged -= x);

            var viewUnloaded = Observable.FromEvent<RoutedEventHandler, bool>(
                eventHandler =>
                {
                    void Handler(object sender, RoutedEventArgs e) => eventHandler(false);
                    return Handler;
                },
                x => fe.Unloaded += x,
                x => fe.Unloaded -= x);

            var windowActivation = GetActivationForWindow(view);

            return viewLoaded
                .Merge(viewUnloaded)
                .Merge(hitTestVisible)
                .Merge(windowActivation)
                .DistinctUntilChanged();
        }

        private static IObservable<bool> GetActivationForWindow(IActivatableView view)
        {
            if (view is not Window window)
            {
                return Observable<bool>.Empty;
            }

            var viewClosed = Observable.FromEvent<EventHandler, bool>(
                eventHandler =>
                {
                    void Handler(object? sender, EventArgs e) => eventHandler(false);
                    return Handler;
                },
                x => window.Closed += x,
                x => window.Closed -= x);

            return viewClosed;
        }
    }
}
