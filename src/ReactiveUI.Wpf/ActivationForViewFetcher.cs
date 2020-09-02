﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;

namespace ReactiveUI
{
    /// <summary>
    /// ActiveationForViewFetcher is how ReactiveUI determine when a
    /// View is activated or deactivated. This is usually only used when porting
    /// ReactiveUI to a new UI framework.
    /// </summary>
    public class ActivationForViewFetcher : IActivationForViewFetcher
    {
        /// <inheritdoc/>
        public int GetAffinityForView(Type view)
        {
            return typeof(FrameworkElement).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ? 10 : 0;
        }

        /// <inheritdoc/>
        public IObservable<bool> GetActivationForView(IActivatableView view)
        {
            var fe = view as FrameworkElement;

            if (fe == null)
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

            return viewLoaded
                .Merge(viewUnloaded)
                .Merge(hitTestVisible)
                .DistinctUntilChanged();
        }
    }
}
