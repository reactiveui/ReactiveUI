// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;

namespace ReactiveUI
{
    /// <summary>
    /// Extension methods associated with observables.
    /// </summary>
    public static class ObservableMixins
    {
        /// <summary>
        /// Casts an observable to the specified value.
        /// This version allows for nullability.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="observable">The observable which as nullability.</param>
        /// <returns>The new observable.</returns>
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8605 // Unboxing possible null value
        public static IObservable<T> Cast<T>(this IObservable<object?> observable) => observable.Select(x => (T)x);
#pragma warning restore CS8605 // Unboxing possible null value
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

        /// <summary>
        /// Casts an observable to the specified value.
        /// This version allows for nullability.
        /// </summary>
        /// <typeparam name="TFrom">The type to convert from.</typeparam>
        /// <typeparam name="TTo">The type to convert to.</typeparam>
        /// <param name="observable">The observable which as nullability.</param>
        /// <returns>The new observable.</returns>
        public static IObservable<TTo> Cast<TFrom, TTo>(this IObservable<object> observable)
            where TFrom : notnull =>
            observable.Select(x => (TTo)x);

        /// <summary>
        /// Returns only values that are not null.
        /// Converts the nullability.
        /// </summary>
        /// <typeparam name="T">The type of value emitted by the observable.</typeparam>
        /// <param name="observable">The observable that can contain nulls.</param>
        /// <returns>A non nullable version of the observable that only emits valid values.</returns>
        public static IObservable<T> WhereNotNull<T>(this IObservable<T?> observable) =>
            observable
                .Where(x => x is not null)
                .Select(x => x!);
    }
}
