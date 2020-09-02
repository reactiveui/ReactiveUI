﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;

using Windows.Foundation;
using Windows.UI.Xaml;

namespace ReactiveUI.Uno
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

#pragma warning disable SA1114 // Parameter list after.
#if NETSTANDARD
            var viewLoaded = Observable.FromEvent<RoutedEventHandler, bool>(
#else
            var viewLoaded = Observable.FromEvent<TypedEventHandler<DependencyObject, object>, bool>(
#endif
                eventHandler => (_, __) => eventHandler(true),
                x => fe.Loading += x,
                x => fe.Loading -= x);

            var viewUnloaded = Observable.FromEvent<RoutedEventHandler, bool>(
                handler =>
                {
                    void EventHandler(object sender, RoutedEventArgs e) => handler(false);
                    return EventHandler;
                },
                x => fe.Unloaded += x,
                x => fe.Unloaded -= x);

            return viewLoaded
                .Merge(viewUnloaded)
                .Select(b => b ? fe.WhenAnyValue(x => x.IsHitTestVisible).SkipWhile(x => !x) : Observables.False)
                .Switch()
                .DistinctUntilChanged();
        }
    }
}
