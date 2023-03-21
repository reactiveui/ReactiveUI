// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace System.Reactive.Linq;

/// <summary>
/// Provides commonly required, statically-allocated, pre-canned observables.
/// </summary>
/// <typeparam name="T">
/// The observable type.
/// </typeparam>
internal static class Observable<T>
{
    /// <summary>
    /// An empty observable of type <typeparamref name="T"/>.
    /// </summary>
    public static readonly IObservable<T> Empty = Observable.Empty<T>();

    /// <summary>
    /// An observable of type <typeparamref name="T"/> that never ticks a value.
    /// </summary>
    public static readonly IObservable<T> Never = Observable.Never<T>();

    /// <summary>
    /// An observable of type <typeparamref name="T"/> that ticks a single, default value.
    /// </summary>
    public static readonly IObservable<T> Default = Observable.Return(default(T)!);
}