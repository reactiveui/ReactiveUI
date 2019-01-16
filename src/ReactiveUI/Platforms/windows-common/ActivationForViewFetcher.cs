// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;

#if NETFX_CORE
using Windows.UI.Xaml;
#endif

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
        public IObservable<bool> GetActivationForView(IActivatable view)
        {
            var fe = view as FrameworkElement;

            if (fe == null)
            {
                return Observable<bool>.Empty;
            }
#if WINDOWS_UWP
            var viewLoaded = WindowsObservable.FromEventPattern<FrameworkElement, object>(
                x => fe.Loading += x,
                x => fe.Loading -= x)
                .Select(_ => true);

            var hitTestVisible = fe.WhenAnyValue(x => x.IsHitTestVisible);
#else
            var viewLoaded = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                x => fe.Loaded += x,
                x => fe.Loaded -= x)
                .Select(_ => true);

            var hitTestVisible = Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(
                x => fe.IsHitTestVisibleChanged += x,
                x => fe.IsHitTestVisibleChanged -= x)
                .Select(x => (bool)x.EventArgs.NewValue);
#endif

            var viewUnloaded = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                x => fe.Unloaded += x,
                x => fe.Unloaded -= x)
                .Select(_ => false);

            return viewLoaded
                .Merge(viewUnloaded)
                .Merge(hitTestVisible)
                .DistinctUntilChanged();
        }
    }
}
