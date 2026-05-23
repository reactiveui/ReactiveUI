// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>
/// Provides extension methods for IRoutableViewModel to observe and manage navigation-related focus and lifecycle
/// events within a navigation stack.
/// </summary>
/// <remarks>These methods enable ViewModels to react to navigation changes, such as gaining or losing focus, by
/// subscribing to observables or setting up disposable resources. They are intended to be used in scenarios where
/// ViewModels participate in navigation stacks and need to manage resources or state based on their navigation status.
/// All methods require a non-null IRoutableViewModel instance and are typically used within applications utilizing
/// reactive navigation patterns.</remarks>
public static class RoutableViewModelMixin
{
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
        ArgumentExceptionHelper.ThrowIfNull(item);

        IDisposable? inner = null;

        var router = item.HostScreen.Router;
        var navigationStackChanged = router.NavigationChanged.WhenCountChanged();
        return navigationStackChanged.Subscribe(new DelegateObserver<IReactiveChangeSet<IRoutableViewModel>>(_ =>
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
        }));
    }

    /// <summary>
    /// <para>
    /// This method will return an observable that fires events every time
    /// the topmost ViewModel in the navigation stack is this ViewModel.
    /// This allows you to set up connections that only operate while the
    /// ViewModel has focus.
    /// </para>
    /// <para>
    /// The observable will complete when the ViewModel is removed completely
    /// from the navigation stack. If your ViewModel can be _removed_ from
    /// the navigation stack and then reused later, you must call this method
    /// and resubscribe each time it is reused.
    /// </para>
    /// </summary>
    /// <param name="item">The ViewModel to watch for navigation changes.</param>
    /// <returns>An IObservable{Unit} that signals when the ViewModel has
    /// been added or brought to the top of the navigation stack. The
    /// observable completes when the ViewModel is no longer a part of the
    /// navigation stack.</returns>
    public static IObservable<Unit> WhenNavigatedToObservable(this IRoutableViewModel item)
    {
        ArgumentExceptionHelper.ThrowIfNull(item);

        var router = item.HostScreen.Router;
        var navigationStackChanged = router.NavigationChanged.WhenCountChanged();

        return new NavigatedToObservable(navigationStackChanged, router, item);
    }

    /// <summary>
    /// <para>
    /// This method will return an observable that fires events _just before_
    /// the ViewModel is no longer the topmost ViewModel in the navigation
    /// stack. This allows you to clean up anything before losing focus.
    /// </para>
    /// <para>
    /// The observable will complete when the ViewModel is removed completely
    /// from the navigation stack. If your ViewModel can be _removed_ from
    /// the navigation stack and then reused later, you must call this method
    /// and resubscribe each time it is reused.
    /// </para>
    /// </summary>
    /// /// <param name="item">The ViewModel to watch for navigation changes.</param>
    /// <returns>An IObservable{Unit} that signals when the ViewModel is no
    /// longer the topmost ViewModel in the navigation stack. The observable
    /// completes when the ViewModel is no longer a part of the navigation
    /// stack.</returns>
    public static IObservable<Unit> WhenNavigatingFromObservable(this IRoutableViewModel item)
    {
        ArgumentExceptionHelper.ThrowIfNull(item);

        var router = item.HostScreen.Router;
        var navigationStackChanged = router.NavigationChanged.WhenCountChanged();

        return new NavigatingFromObservable(navigationStackChanged, router, item);
    }

    /// <summary>
    /// Determines whether the specified item was removed from the change set.
    /// </summary>
    /// <param name="changeSet">The set of changes to evaluate for item removal.</param>
    /// <param name="item">The item to check for removal within the change set.</param>
    /// <returns>true if the item was removed according to the change set; otherwise, false.</returns>
    private static bool WasItemRemoved(IReactiveChangeSet<IRoutableViewModel> changeSet, IRoutableViewModel item)
    {
        // A reset/clear is flattened to one Remove per prior item, so a removal of this item (directly or via a
        // clear) always appears as a Remove change carrying the item.
        for (var i = 0; i < changeSet.Count; i++)
        {
            var change = changeSet[i];
            if (change.Reason == ReactiveChangeReason.Remove && ReferenceEquals(change.Current, item))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Emits when this view model is (or becomes) the topmost view model, and completes when it is removed from the
    /// stack. Fuses the prior <c>Where</c> + <c>Select</c> + <c>TakeUntil</c> pipeline into one sink.
    /// </summary>
    /// <param name="source">The navigation-stack change stream.</param>
    /// <param name="router">The router whose current view model is inspected.</param>
    /// <param name="item">The view model being watched.</param>
    private sealed class NavigatedToObservable(
        IObservable<IReactiveChangeSet<IRoutableViewModel>> source,
        RoutingState router,
        IRoutableViewModel item) : IObservable<Unit>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Sink(observer, router, item).Run(source);
        }

        /// <summary>Emits a unit when the watched view model is current, completing once it is removed.</summary>
        /// <param name="downstream">The observer receiving the focus signal.</param>
        /// <param name="router">The router whose current view model is inspected.</param>
        /// <param name="item">The view model being watched.</param>
        private sealed class Sink(IObserver<Unit> downstream, RoutingState router, IRoutableViewModel item)
            : IObserver<IReactiveChangeSet<IRoutableViewModel>>, IDisposable
        {
            /// <summary>The subscription to the navigation-stack change stream.</summary>
            private IDisposable? _subscription;

            /// <summary>Whether the downstream has terminated.</summary>
            private bool _stopped;

            /// <summary>Subscribes to the source.</summary>
            /// <param name="changes">The navigation-stack change stream.</param>
            /// <returns>The sink, which stops the run when disposed.</returns>
            public Sink Run(IObservable<IReactiveChangeSet<IRoutableViewModel>> changes)
            {
                _subscription = changes.Subscribe(this);
                return this;
            }

            /// <inheritdoc/>
            public void OnNext(IReactiveChangeSet<IRoutableViewModel> value)
            {
                if (_stopped)
                {
                    return;
                }

                if (WasItemRemoved(value, item))
                {
                    Complete();
                    return;
                }

                if (router.GetCurrentViewModel() != item)
                {
                    return;
                }

                downstream.OnNext(Unit.Default);
            }

            /// <inheritdoc/>
            public void OnError(Exception error)
            {
                if (_stopped)
                {
                    return;
                }

                _stopped = true;
                downstream.OnError(error);
            }

            /// <inheritdoc/>
            public void OnCompleted() => Complete();

            /// <inheritdoc/>
            public void Dispose() => _subscription?.Dispose();

            /// <summary>Completes the downstream and disposes the subscription exactly once.</summary>
            private void Complete()
            {
                if (_stopped)
                {
                    return;
                }

                _stopped = true;
                downstream.OnCompleted();
                _subscription?.Dispose();
            }
        }
    }

    /// <summary>
    /// Emits when this view model stops being the topmost view model, and completes when it is removed from the stack.
    /// Fuses the prior <c>Scan</c> + <c>Where</c> + <c>Select</c> + <c>TakeUntil</c> pipeline into one sink.
    /// </summary>
    /// <param name="source">The navigation-stack change stream.</param>
    /// <param name="router">The router whose current view model is inspected.</param>
    /// <param name="item">The view model being watched.</param>
    private sealed class NavigatingFromObservable(
        IObservable<IReactiveChangeSet<IRoutableViewModel>> source,
        RoutingState router,
        IRoutableViewModel item) : IObservable<Unit>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Sink(observer, router, item).Run(source);
        }

        /// <summary>Emits a unit when the watched view model was the previous current view model, completing on removal.</summary>
        /// <param name="downstream">The observer receiving the lost-focus signal.</param>
        /// <param name="router">The router whose current view model is inspected.</param>
        /// <param name="item">The view model being watched.</param>
        private sealed class Sink(IObserver<Unit> downstream, RoutingState router, IRoutableViewModel item)
            : IObserver<IReactiveChangeSet<IRoutableViewModel>>, IDisposable
        {
            /// <summary>The subscription to the navigation-stack change stream.</summary>
            private IDisposable? _subscription;

            /// <summary>The current view model recorded at the previous change.</summary>
            private IRoutableViewModel? _previousCurrent;

            /// <summary>Whether the downstream has terminated.</summary>
            private bool _stopped;

            /// <summary>Subscribes to the source.</summary>
            /// <param name="changes">The navigation-stack change stream.</param>
            /// <returns>The sink, which stops the run when disposed.</returns>
            public Sink Run(IObservable<IReactiveChangeSet<IRoutableViewModel>> changes)
            {
                _subscription = changes.Subscribe(this);
                return this;
            }

            /// <inheritdoc/>
            public void OnNext(IReactiveChangeSet<IRoutableViewModel> value)
            {
                if (_stopped)
                {
                    return;
                }

                if (WasItemRemoved(value, item))
                {
                    Complete();
                    return;
                }

                if (_previousCurrent == item)
                {
                    downstream.OnNext(Unit.Default);
                }

                _previousCurrent = router.GetCurrentViewModel();
            }

            /// <inheritdoc/>
            public void OnError(Exception error)
            {
                if (_stopped)
                {
                    return;
                }

                _stopped = true;
                downstream.OnError(error);
            }

            /// <inheritdoc/>
            public void OnCompleted() => Complete();

            /// <inheritdoc/>
            public void Dispose() => _subscription?.Dispose();

            /// <summary>Completes the downstream and disposes the subscription exactly once.</summary>
            private void Complete()
            {
                if (_stopped)
                {
                    return;
                }

                _stopped = true;
                downstream.OnCompleted();
                _subscription?.Dispose();
            }
        }
    }
}
