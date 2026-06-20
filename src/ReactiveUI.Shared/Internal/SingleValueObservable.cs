// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Internal;
#else
namespace ReactiveUI.Internal;
#endif
/// <summary>
/// Provides pre-allocated <see cref="SingleValueObservable{T}"/> singletons for values emitted often enough to be
/// worth sharing rather than re-allocating on each use.
/// </summary>
internal static class SingleValueObservable
{
    /// <summary>
    /// A shared cold observable that emits <see cref="RxVoid.Default"/> once and then completes. Reused to avoid the
    /// allocation of a fresh <see cref="SingleValueObservable{T}"/> for the very common "completed unit" case.
    /// </summary>
    public static readonly IObservable<RxVoid> Void = new SingleValueObservable<RxVoid>(RxVoid.Default);

    /// <summary>A shared cold observable that emits a single <c>true</c> and then completes.</summary>
    public static readonly IObservable<bool> True = new SingleValueObservable<bool>(true);

    /// <summary>A shared cold observable that emits a single <c>false</c> and then completes.</summary>
    public static readonly IObservable<bool> False = new SingleValueObservable<bool>(false);
}
