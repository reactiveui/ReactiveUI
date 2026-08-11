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
/// The arity-N (no-selector) WhenAnyObservable sink: each time the observed properties produce a new set of inner
/// observables, it unsubscribes from the previous set and merges the new set, forwarding every value from every
/// current inner.
/// </summary>
/// <typeparam name="TResult">The element type produced by the inner observables.</typeparam>
/// <param name="sources">The outer observable; each emission is the current set of inner observables.</param>
/// <remarks>
/// A generation supersedes the previous one and the inners within a generation are merged, which is
/// <see cref="SwitchSignal{T}"/> over a <see cref="MergeSignal{T}"/> per emission. Both come from
/// ReactiveUI.Primitives; this type exists only to name the shape at the WhenAnyObservable call sites.
/// </remarks>
internal sealed class WhenAnyObservableMergeSink<TResult>(IObservable<IObservable<TResult>[]> sources) : IObservable<TResult>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Subscribe(IObserver<TResult> observer) =>
        new SwitchSignal<TResult>(
            new MapSignal<IObservable<TResult>[], IObservable<TResult>>(sources, static inners => new MergeSignal<TResult>(inners)))
            .Subscribe(observer);
}
