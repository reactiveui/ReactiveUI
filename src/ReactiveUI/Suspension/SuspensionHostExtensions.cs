// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
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
        /// <param name="item">The suspension host.</param>
        /// <returns>An observable of the app state.</returns>
        public static IObservable<T> ObserveAppState<T>(this ISuspensionHost item)
            where T : class
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return item.WhenAny(suspensionHost => suspensionHost.AppState, observedChange => observedChange.Value)
                       .WhereNotNull()
                       .Cast<T>();
        }

        /// <summary>
        /// Get the current App State of a class derived from ISuspensionHost.
        /// </summary>
        /// <typeparam name="T">The app state type.</typeparam>
        /// <param name="item">The suspension host.</param>
        /// <returns>The app state.</returns>
        public static T GetAppState<T>(this ISuspensionHost item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return (T)item.AppState!;
        }

        /// <summary>
        /// Setup our suspension driver for a class derived off ISuspensionHost interface.
        /// This will make your suspension host respond to suspend and resume requests.
        /// </summary>
        /// <param name="item">The suspension host.</param>
        /// <param name="driver">The suspension driver.</param>
        /// <returns>A disposable which will stop responding to Suspend and Resume requests.</returns>
        public static IDisposable SetupDefaultSuspendResume(this ISuspensionHost item, ISuspensionDriver? driver = null)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var ret = new CompositeDisposable();
            driver ??= Locator.Current.GetService<ISuspensionDriver>();

            ret.Add(item.ShouldInvalidateState
                        .SelectMany(_ => driver.InvalidateState())
                        .LoggedCatch(item, Observables.Unit, "Tried to invalidate app state")
                        .Subscribe(_ => item.Log().Info("Invalidated app state")));

            ret.Add(item.ShouldPersistState
                        .SelectMany(x => driver.SaveState(item.AppState!).Finally(x.Dispose))
                        .LoggedCatch(item, Observables.Unit, "Tried to persist app state")
                        .Subscribe(_ => item.Log().Info("Persisted application state")));

            ret.Add(item.IsResuming.Merge(item.IsLaunchingNew)
                        .SelectMany(x => driver.LoadState())
                        .LoggedCatch(
                            item,
                            Observable.Defer(() => Observable.Return(item.CreateNewAppState?.Invoke())),
                            "Failed to restore app state from storage, creating from scratch")
                        .Subscribe(x => item.AppState = x ?? item.CreateNewAppState?.Invoke()));

            return ret;
        }
    }
}
