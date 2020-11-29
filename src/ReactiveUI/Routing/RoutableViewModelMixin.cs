// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using DynamicData;

namespace ReactiveUI
{
    /// <summary>
    /// Extension methods associated with the IRoutableViewModel interface.
    /// </summary>
    public static class RoutableViewModelMixin
    {
        private static readonly ListChangeReason[] NavigationStackRemovalOperations = { ListChangeReason.Remove, ListChangeReason.RemoveRange };

        /// <summary>
        /// This method allows you to set up connections that only operate
        /// while the ViewModel has focus, and cleans up when the ViewModel
        /// loses focus.
        /// </summary>
        /// <param name="item">The ViewModel to watch for focus changes.</param>
        /// <param name="onNavigatedTo">Called when the ViewModel is navigated
        /// to - return an IDisposable that cleans up all of the things that are
        /// configured in the method.</param>
        /// <returns>An IDisposable that lets you disconnect the entire process
        /// earlier than normal.</returns>
        public static IDisposable WhenNavigatedTo(this IRoutableViewModel item, Func<IDisposable> onNavigatedTo)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            IDisposable? inner = null;

            var router = item.HostScreen.Router;
            var navigationStackChanged = router.NavigationChanged.CountChanged();
            return navigationStackChanged.Subscribe(_ =>
            {
                if (router.GetCurrentViewModel() == item)
                {
                    inner?.Dispose();

                    inner = onNavigatedTo();
                }
                else
                {
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
        /// <param name="item">The viewmodel to watch for navigation changes.</param>
        /// <returns>An IObservable{Unit} that signals when the ViewModel has
        /// been added or brought to the top of the navigation stack. The
        /// observable completes when the ViewModel is no longer a part of the
        /// navigation stack.</returns>
        public static IObservable<Unit> WhenNavigatedToObservable(this IRoutableViewModel item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var router = item.HostScreen.Router;
            var navigationStackChanged = router.NavigationChanged.CountChanged();
            var itemRemoved = navigationStackChanged.Where(x => WasItemRemoved(x, item));

            return navigationStackChanged
                .Where(_ => router?.GetCurrentViewModel() == item)
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
        /// /// <param name="item">The viewmodel to watch for navigation changes.</param>
        /// <returns>An IObservable{Unit} that signals when the ViewModel is no
        /// longer the topmost ViewModel in the navigation stack. The observable
        /// completes when the ViewModel is no longer a part of the navigation
        /// stack.</returns>
        public static IObservable<Unit> WhenNavigatingFromObservable(this IRoutableViewModel item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var router = item.HostScreen.Router;
            var navigationStackChanged = router.NavigationChanged.CountChanged();
            var itemRemoved = navigationStackChanged.Where(x => WasItemRemoved(x, item));
            var viewModelsChanged = navigationStackChanged.Scan(new IRoutableViewModel?[2], (previous, current) => new[] { previous[1], router.GetCurrentViewModel() });

            return viewModelsChanged
                .Where(x => x[0] == item)
                .Select(_ => Unit.Default)
                .TakeUntil(itemRemoved);
        }

        private static bool WasItemRemoved(IChangeSet<IRoutableViewModel> changeSet, IRoutableViewModel item) =>
            changeSet
                .Any(
                    change => change.Reason == ListChangeReason.Clear ||
                              (NavigationStackRemovalOperations.Contains(change.Reason) && change.Item.Current == item));
    }
}
