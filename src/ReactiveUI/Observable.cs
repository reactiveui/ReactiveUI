// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Internal;

namespace System.Reactive.Linq;

/// <summary>
/// Provides commonly required, statically-allocated, pre-canned observables, backed by tailored sinks.
/// </summary>
/// <typeparam name="T">
/// The observable type.
/// </typeparam>
internal static class Observable<T>
{
    /// <summary>
    /// An empty observable of type <typeparamref name="T"/>.
    /// </summary>
    public static readonly IObservable<T> Empty = EmptyObservable<T>.Instance;

    /// <summary>
    /// An observable of type <typeparamref name="T"/> that never ticks a value.
    /// </summary>
    public static readonly IObservable<T> Never = NeverObservable<T>.Instance;

    /// <summary>
    /// An observable of type <typeparamref name="T"/> that ticks a single, default value.
    /// </summary>
    public static readonly IObservable<T> Default = new SingleValueObservable<T>(default!);
}
