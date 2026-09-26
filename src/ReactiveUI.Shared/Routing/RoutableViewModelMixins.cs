// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// Provides extension methods for IRoutableViewModel to observe and manage navigation-related focus and lifecycle
/// events within a navigation stack.
/// </summary>
/// <remarks>These methods enable ViewModels to react to navigation changes, such as gaining or losing focus, by
/// subscribing to observables or setting up disposable resources. They are intended to be used in scenarios where
/// ViewModels participate in navigation stacks and need to manage resources or state based on their navigation status.
/// All methods require a non-null IRoutableViewModel instance and are typically used within applications utilizing
/// reactive navigation patterns.</remarks>
public static class RoutableViewModelMixins
{
    /// <summary>Provides navigation focus and lifecycle extension members for <see cref="IRoutableViewModel"/>.</summary>
    /// <param name="item">The ViewModel to watch for navigation changes.</param>
    extension(IRoutableViewModel item)
    {
        /// <summary>
        /// This method allows you to set up connections that only operate
        /// while the ViewModel has focus, and cleans up when the ViewModel
        /// loses focus.
        /// </summary>
        /// <param name="onNavigatedTo">Called when the ViewModel is navigated
        /// to - return an IDisposable that cleans up all of the things that are
        /// configured in the method.</param>
        /// <returns>An IDisposable that lets you disconnect the entire process
        /// earlier than normal. Disposing it also disposes whatever
        /// <paramref name="onNavigatedTo"/> most recently returned.</returns>
        public IDisposable WhenNavigatedTo(Func<IDisposable> onNavigatedTo)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);
            ArgumentExceptionHelper.ThrowIfNull(onNavigatedTo);

            return new NavigationFocusScope(item, onNavigatedTo).Run(item.HostScreen.Router);
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
        /// <returns>An IObservable{Unit} that signals when the ViewModel has
        /// been added or brought to the top of the navigation stack. The
        /// observable completes when the ViewModel is no longer a part of the
        /// navigation stack.</returns>
        public IObservable<RxVoid> WhenNavigatedToObservable()
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            return new NavigationFocusObservable(item.HostScreen.Router, item, NavigationFocusTransition.Arrival);
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
        /// <returns>An IObservable{Unit} that signals when the ViewModel is no
        /// longer the topmost ViewModel in the navigation stack. The observable
        /// completes when the ViewModel is no longer a part of the navigation
        /// stack.</returns>
        public IObservable<RxVoid> WhenNavigatingFromObservable()
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            return new NavigationFocusObservable(item.HostScreen.Router, item, NavigationFocusTransition.Departure);
        }
    }

    /// <summary>Which side of a navigation-focus change an observable reports.</summary>
    private enum NavigationFocusTransition
    {
        /// <summary>The watched view model has become the topmost view model.</summary>
        Arrival = 0,

        /// <summary>The watched view model has stopped being the topmost view model.</summary>
        Departure = 1,
    }

    /// <summary>
    /// Emits on one side of a navigation-focus change for the watched view model, and completes when that view
    /// model is removed from the stack. The two focus directions differ only in the transition test.
    /// </summary>
    /// <param name="router">The router whose navigation stack is watched.</param>
    /// <param name="item">The view model being watched.</param>
    /// <param name="transition">Which side of the focus change to report.</param>
    private sealed class NavigationFocusObservable(
        RoutingState router,
        IRoutableViewModel item,
        NavigationFocusTransition transition) : IObservable<RxVoid>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<RxVoid> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Sink(observer, item, transition).Run(router);
        }

        /// <summary>Emits a unit on each matching focus change, completing once the watched view model is removed.</summary>
        /// <param name="downstream">The observer receiving the focus signal.</param>
        /// <param name="item">The view model being watched.</param>
        /// <param name="transition">Which side of the focus change to report.</param>
        private sealed class Sink(
            IObserver<RxVoid> downstream,
            IRoutableViewModel item,
            NavigationFocusTransition transition) : IObserver<IReadOnlyList<IRoutableViewModel>>, IDisposable
        {
            /// <summary>The subscription to the navigation-stack stream.</summary>
            private readonly OnceDisposable _subscription = new();

            /// <summary>The current view model recorded at the previous change; only used for departures.</summary>
            private IRoutableViewModel? _previousCurrent;

            /// <summary>The stack count recorded at the previous change.</summary>
            private int _previousCount;

            /// <summary>Whether the watched view model was in the stack at the previous change.</summary>
            private bool _wasInStack;

            /// <summary>Whether the downstream has terminated; latched to 1 by the first thread to terminate it.</summary>
            private int _stopped;

            /// <inheritdoc/>
            public void OnNext(IReadOnlyList<IRoutableViewModel> value)
            {
                if (Volatile.Read(ref _stopped) != 0)
                {
                    return;
                }

                // Every piece of bookkeeping advances before anything is handed downstream, so a downstream handler
                // that synchronously drives more navigation sees state that is already current.
                var isInStack = Contains(value, item);
                var removed = _wasInStack && !isInStack;
                _wasInStack = isInStack;

                var countChanged = value.Count != _previousCount;
                _previousCount = value.Count;

                if (removed)
                {
                    Complete();
                    return;
                }

                if (!countChanged || !HasTransitioned(RoutingState.Top(value)))
                {
                    return;
                }

                downstream.OnNext(RxVoid.Default);
            }

            /// <inheritdoc/>
            public void OnError(Exception error)
            {
                if (Interlocked.Exchange(ref _stopped, 1) != 0)
                {
                    return;
                }

                downstream.OnError(error);
                _subscription.Dispose();
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted() => Complete();

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() => _subscription.Dispose();

            /// <summary>Judges the stack as it is now, then subscribes to its later changes.</summary>
            /// <param name="router">The router whose navigation stack is watched.</param>
            /// <returns>The sink, which stops the run when disposed.</returns>
            internal Sink Run(RoutingState router)
            {
                OnNext(router.NavigationStack);
                _subscription.Disposable = router.NavigationStackChanged.Subscribe(this);
                return this;
            }

            /// <summary>Determines whether a stack snapshot holds the watched view model.</summary>
            /// <param name="stack">The stack snapshot.</param>
            /// <param name="watched">The view model to look for.</param>
            /// <returns><see langword="true"/> if the snapshot holds the view model; otherwise <see langword="false"/>.</returns>
            private static bool Contains(IReadOnlyList<IRoutableViewModel> stack, IRoutableViewModel watched)
            {
                for (var i = 0; i < stack.Count; i++)
                {
                    if (ReferenceEquals(stack[i], watched))
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>Determines whether this change is the focus transition being watched for.</summary>
            /// <param name="current">The view model now on top of the stack.</param>
            /// <returns><see langword="true"/> when the watched transition happened; otherwise <see langword="false"/>.</returns>
            private bool HasTransitioned(IRoutableViewModel? current)
            {
                if (transition == NavigationFocusTransition.Arrival)
                {
                    return ReferenceEquals(current, item);
                }

                var departed = ReferenceEquals(_previousCurrent, item);
                _previousCurrent = current;
                return departed;
            }

            /// <summary>Completes the downstream and disposes the subscription exactly once.</summary>
            private void Complete()
            {
                if (Interlocked.Exchange(ref _stopped, 1) != 0)
                {
                    return;
                }

                downstream.OnCompleted();
                _subscription.Dispose();
            }
        }
    }

    /// <summary>
    /// Runs a caller-supplied scope for exactly as long as the watched view model is the topmost one, replacing it
    /// on every change to the stack's size and disposing it along with the handle returned to the caller.
    /// </summary>
    /// <param name="item">The view model being watched.</param>
    /// <param name="onNavigatedTo">Builds the scope that lives while the view model has focus.</param>
    private sealed class NavigationFocusScope(
        IRoutableViewModel item,
        Func<IDisposable> onNavigatedTo) : IObserver<IReadOnlyList<IRoutableViewModel>>, IDisposable
    {
        /// <summary>The scope currently in force; replacing or clearing it disposes the previous one.</summary>
        private readonly SwapDisposable _scope = new();

        /// <summary>The subscription to the navigation-stack stream.</summary>
        private readonly OnceDisposable _subscription = new();

        /// <summary>The stack count recorded at the previous change.</summary>
        private int _previousCount;

        /// <inheritdoc/>
        public void OnNext(IReadOnlyList<IRoutableViewModel> value)
        {
            if (value.Count == _previousCount)
            {
                return;
            }

            _previousCount = value.Count;

            // Tear the old scope down before building the new one, so the two never overlap.
            _scope.Disposable = null;

            if (!ReferenceEquals(RoutingState.Top(value), item))
            {
                return;
            }

            _scope.Disposable = onNavigatedTo();
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
            // The navigation-stack stream is a collection-change projection and does not fault in practice. A
            // fault leaves the scope in force: the view model has not lost focus, and the caller's handle still
            // owns the teardown.
        }

        /// <inheritdoc/>
        public void OnCompleted()
        {
            // Likewise: the stream ending does not mean focus was lost, so the scope stays in force until the
            // caller disposes the handle.
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _subscription.Dispose();
            _scope.Dispose();
        }

        /// <summary>Judges the stack as it is now, then subscribes to its later changes.</summary>
        /// <param name="router">The router whose navigation stack is watched.</param>
        /// <returns>The scope, which stops the run and disposes the active scope when disposed.</returns>
        internal NavigationFocusScope Run(RoutingState router)
        {
            OnNext(router.NavigationStack);
            _subscription.Disposable = router.NavigationStackChanged.Subscribe(this);
            return this;
        }
    }
}
