// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;

namespace ReactiveUI;

/// <summary>
/// Extension methods associated with observables.
/// </summary>
public static class ObservableMixins
{
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