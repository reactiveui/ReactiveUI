// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Extension methods associated with the ISuspensionHost interface.
    /// </summary>
    public static class SuspensionHostExtensions
    {
        /// <summary>
        /// Observe changes to the AppState of a class derived from ISuspensionHost.
        /// </summary>
        /// <typeparam name="T">The observable type.</typeparam>
        /// <param name="this">The suspension host.</param>
        /// <returns>An observable of the app state.</returns>
        public static IObservable<T> ObserveAppState<T>(this ISuspensionHost @this)
            where T : class
        {
            return @this.WhenAny(x => x.AppState, x => (T)x.Value)
                        .Where(x => x != null);
        }

        /// <summary>
        /// Get the current App State of a class derived from ISuspensionHost.
        /// </summary>
        /// <typeparam name="T">The app state type.</typeparam>
        /// <param name="this">The suspenstion host.</param>
        /// <returns>The app state.</returns>
        public static T GetAppState<T>(this ISuspensionHost @this)
        {
            return (T)@this.AppState;
        }

        /// <summary>
        /// Setup our suspension driver for a class derived off ISuspensionHost interface.
        /// This will make your suspension host respond to suspend and resume requests.
        /// </summary>
        /// <param name="this">The suspension host.</param>
        /// <param name="driver">The suspension driver.</param>
        /// <returns>A disposable which will stop responding to Suspend and Resume requests.</returns>
        public static IDisposable SetupDefaultSuspendResume(this ISuspensionHost @this, ISuspensionDriver driver = null)
        {
            var ret = new CompositeDisposable();
            driver = driver ?? Locator.Current.GetService<ISuspensionDriver>();

            ret.Add(@this.ShouldInvalidateState
                         .SelectMany(_ => driver.InvalidateState())
                         .LoggedCatch(@this, Observables.Unit, "Tried to invalidate app state")
                         .Subscribe(_ => @this.Log().Info("Invalidated app state")));

            ret.Add(@this.ShouldPersistState
                         .SelectMany(x => driver.SaveState(@this.AppState).Finally(x.Dispose))
                         .LoggedCatch(@this, Observables.Unit, "Tried to persist app state")
                         .Subscribe(_ => @this.Log().Info("Persisted application state")));

            ret.Add(Observable.Merge(@this.IsResuming, @this.IsLaunchingNew)
                              .SelectMany(x => driver.LoadState())
                              .LoggedCatch(
                                  @this,
                                  Observable.Defer(() => Observable.Return(@this.CreateNewAppState())),
                                  "Failed to restore app state from storage, creating from scratch")
                                  .Subscribe(x => @this.AppState = x ?? @this.CreateNewAppState()));

            return ret;
        }
    }
}
