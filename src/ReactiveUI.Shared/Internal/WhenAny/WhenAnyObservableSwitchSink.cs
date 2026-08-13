// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
#if REACTIVE_SHIM
using ReactiveUI.Primitives.Reactive.Advanced;
#else
using ReactiveUI.Primitives.Advanced;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Internal;
#else
namespace ReactiveUI.Internal;
#endif

/// <summary>
/// The arity-1 WhenAnyObservable sink: observes an outer stream of inner observables and forwards values from the
/// most recent inner, unsubscribing from the previous inner each time a new one arrives.
/// </summary>
/// <typeparam name="TResult">The element type produced by the inner observables.</typeparam>
/// <param name="sources">The outer observable whose latest inner observable is subscribed.</param>
/// <remarks>
/// The switching itself is <see cref="SwitchSignal{T}"/> from ReactiveUI.Primitives; this type exists only to name
/// the shape at the WhenAnyObservable call sites.
/// </remarks>
internal sealed class WhenAnyObservableSwitchSink<TResult>(IObservable<IObservable<TResult>> sources) : IObservable<TResult>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Subscribe(IObserver<TResult> observer) => new SwitchSignal<TResult>(sources).Subscribe(observer);
}
