// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>Provides the arity-1 WhenAnyObservable extension overloads.</summary>
[RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
public static partial class WhenAnyObservableMixin
{
    /// <summary>
    /// Observes a property that is itself an observable and subscribes to its latest value.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="TRet">The type of the observable's value.</typeparam>
    /// <param name="sender">The object whose observable property is observed.</param>
    /// <param name="obs1">An expression pointing to the observable property.</param>
    /// <returns>An observable that produces the latest value of the observed observable.</returns>
    public static IObservable<TRet> WhenAnyObservable<TSender, TRet>(
        this TSender? sender,
        Expression<Func<TSender, IObservable<TRet>?>> obs1)
        where TSender : class =>
        new WhenAnyObservableSwitchSink<TRet>(
            sender.WhenAny(obs1, static x => x.Value!.EmptyIfNull()));
}
