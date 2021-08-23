// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace ReactiveUI.WinUI
{
    /// <summary>
    /// ActiveationForViewFetcher is how ReactiveUI determine when a
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
            if (view is not FrameworkElement fe)
            {
                return Observable<bool>.Empty;
            }

            var viewLoaded = Observable.FromEvent<TypedEventHandler<FrameworkElement, object>, bool>(
                eventHandler => (FrameworkElement sender1, object e1) => Handler(sender1, e1, eventHandler),
                x => fe.Loading += x,
                x => fe.Loading -= x);

            var viewUnloaded = Observable.FromEvent<RoutedEventHandler, bool>(
                eventHandler => (object sender, RoutedEventArgs e) => Handler1(sender, e, eventHandler),
                x => fe.Unloaded += x,
                x => fe.Unloaded -= x);

            return viewLoaded
                .Merge(viewUnloaded)
                .Select(b => b ? fe.WhenAnyValue(x => x.IsHitTestVisible).SkipWhile(x => !x) : Observables.False)
                .Switch()
                .DistinctUntilChanged();
        }

        private static void Handler1(object sender, RoutedEventArgs e, Action<bool> eventHandler) => eventHandler(false);

        private static void Handler(FrameworkElement sender, object e, Action<bool> eventHandler) => eventHandler(true);
    }
}
