// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Binding;

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
            var navigationStackChanged = router.NavigationChanged.CountChanged();
            return navigationStackChanged.Subscribe(_ => {
                if (router.GetCurrentViewModel() == @this) {
                    inner?.Dispose();

                    inner = onNavigatedTo();
                } else {
                    inner?.Dispose();
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
            var navigationStackChanged = router.NavigationChanged.CountChanged();

            var itemRemoved = navigationStackChanged
                .Where(x => x.Any(change => change.Reason == ListChangeReason.Remove && change.Item.Current == @this));

            return navigationStackChanged
                .Where(_ => router.GetCurrentViewModel() == @this)
                .Select(_ => Unit.Default)
                .TakeUntil(itemRemoved);
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
            var navigationStackChanged = router.NavigationChanged.CountChanged();

            var itemRemoved = navigationStackChanged
                .Where(x => x.Any(change => change.Reason == ListChangeReason.Remove && change.Item.Current == @this));

            var viewModelsChanged = navigationStackChanged.Scan(new IRoutableViewModel[2], (previous, current) => new[] {previous[1], router.GetCurrentViewModel()});
            return viewModelsChanged
                .Where(x => x[0] == @this)
                .Select(_ => Unit.Default)
                .TakeUntil(itemRemoved);
        }
    }
}
