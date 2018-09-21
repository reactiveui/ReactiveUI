// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Splat;

namespace ReactiveUI.Legacy
{
    /// <summary>
    /// Extension methods to create collections from observables.
    /// </summary>
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    public static class ReactiveCollectionMixins
    {
        /// <summary>
        /// Creates a collection based on an an Observable by adding items
        /// provided until the Observable completes. This method guarantees that
        /// items are always added in the context of the provided scheduler.
        /// </summary>
        /// <typeparam name="T">The list type.</typeparam>
        /// <param name="fromObservable">
        /// The Observable whose items will be put into the new collection.
        /// </param>
        /// <param name="scheduler">
        /// Optionally specifies the scheduler on which
        /// the collection will be populated. Defaults to the main scheduler.
        /// </param>
        /// <returns>
        /// A new collection which will be populated with the Observable.
        /// </returns>
        public static IReactiveDerivedList<T> CreateCollection<T>(
            this IObservable<T> fromObservable,
            IScheduler scheduler)
        {
            return new ReactiveDerivedCollectionFromObservable<T>(fromObservable, scheduler: scheduler);
        }

        /// <summary>
        /// Creates a collection based on an an Observable by adding items
        /// provided until the Observable completes, optionally ensuring a
        /// delay. Note that if the Observable never completes and withDelay is
        /// set, this method will leak a Timer. This method also guarantees that
        /// items are always added in the context of the provided scheduler.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="fromObservable">
        /// The Observable whose items will be put into the new collection.
        /// </param>
        /// <param name="withDelay">
        /// If set, items will be populated in the collection no faster than the delay provided.
        /// </param>
        /// <param name="onError">
        /// The handler for errors from the Observable. If not specified,
        /// an error will go to DefaultExceptionHandler.
        /// </param>
        /// <param name="scheduler">
        /// Optionally specifies the scheduler on which the collection will be populated.
        /// Defaults to the main scheduler.
        /// </param>
        /// <returns>
        /// A new collection which will be populated with the Observable.
        /// </returns>
        public static IReactiveDerivedList<T> CreateCollection<T>(
            this IObservable<T> fromObservable,
            TimeSpan? withDelay = null,
            Action<Exception> onError = null,
            IScheduler scheduler = null)
        {
            return new ReactiveDerivedCollectionFromObservable<T>(fromObservable, withDelay, onError, scheduler);
        }
    }
}
