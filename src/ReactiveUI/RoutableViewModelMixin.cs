﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveUI
{
    public static class RoutableViewModelMixin
    {
        /// <summary>
        /// This method allows you to set up connections that only operate
        /// while the ViewModel has focus, and cleans up when the ViewModel
        /// loses focus.
        /// </summary>
        /// <param name="this">The ViewModel to watch for focus changes.</param>
        /// <param name="onNavigatedTo">Called when the ViewModel is navigated
        /// to - return an IDisposable that cleans up all of the things that are
        /// configured in the method.</param>
        /// <returns>An IDisposable that lets you disconnect the entire process
        /// earlier than normal.</returns>
        public static IDisposable WhenNavigatedTo(this IRoutableViewModel @this, Func<IDisposable> onNavigatedTo)
        {
            IDisposable inner = null;

            var router = @this.HostScreen.Router;
            return router.NavigationStack.CountChanged.Subscribe(_ => {
                if (router.GetCurrentViewModel() == @this) {
                    if (inner != null)  inner.Dispose();
                    inner = onNavigatedTo();
                } else {
                    if (inner != null) {
                        inner.Dispose();
                        inner = null;
                    }
                }
            });
        }

        /// <summary>
        /// This method will return an observable that fires events every time
        /// the topmost ViewModel in the navigation stack is this ViewModel.
        /// This allows you to set up connections that only operate while the
        /// ViewModel has focus.
        /// 
        /// The observable will complete when the ViewModel is removed completely
        /// from the navigation stack. If your ViewModel can be _removed_ from
        /// the navigation stack and then reused later, you must call this method
        /// and resubscribe each time it is reused.
        /// </summary>
        /// <param name="this">The viewmodel to watch for navigation changes</param>
        /// <returns>An IObservable{Unit} that signals when the ViewModel has
        /// been added or brought to the top of the navigation stack. The
        /// observable completes when the ViewModel is no longer a part of the
        /// navigation stack.</returns>
        public static IObservable<Unit> WhenNavigatedToObservable(this IRoutableViewModel @this)
        {
            var router = @this.HostScreen.Router;
            return router.NavigationStack.CountChanged
                .Where(_ => router.GetCurrentViewModel() == @this)
                .Select(_ => Unit.Default)
                .TakeUntil(router.NavigationStack.BeforeItemsRemoved
                    .Where(itemRemoved => itemRemoved == @this));
        }

        /// <summary>
        /// This method will return an observable that fires events _just before_
        /// the ViewModel is no longer the topmost ViewModel in the navigation
        /// stack. This allows you to clean up anything before losing focus.
        /// 
        /// The observable will complete when the ViewModel is removed completely
        /// from the navigation stack. If your ViewModel can be _removed_ from
        /// the navigation stack and then reused later, you must call this method
        /// and resubscribe each time it is reused.
        /// </summary>
        /// /// <param name="this">The viewmodel to watch for navigation changes</param>
        /// <returns>An IObservable{Unit} that signals when the ViewModel is no
        /// longer the topmost ViewModel in the navigation stack. The observable
        /// completes when the ViewModel is no longer a part of the navigation
        /// stack.</returns>
        public static IObservable<Unit> WhenNavigatingFromObservable(this IRoutableViewModel @this)
        {
            var router = @this.HostScreen.Router;
            return router.NavigationStack.CountChanging
                .Where(_ => router.GetCurrentViewModel() == @this)
                .Select(_ => Unit.Default)
                .TakeUntil(router.NavigationStack.ItemsRemoved
                    .Where(itemRemoved => itemRemoved == @this));
        }
    }
}
