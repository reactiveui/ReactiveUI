// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Provides the arity-2 WhenAnyObservable extension overloads.</summary>
public static partial class WhenAnyObservableMixins
{
    /// <summary>Provides arity-2 WhenAnyObservable extension members for a source object.</summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <param name="sender">The object whose observable properties are observed.</param>
    extension<TSender>(TSender? sender)
        where TSender : class
    {
        /// <summary>Observes several observable-valued properties and merges their latest values.</summary>
        /// <typeparam name="TRet">The type of the observables' value.</typeparam>
        /// <param name="obs1">An expression pointing to observable property 1.</param>
        /// <param name="obs2">An expression pointing to observable property 2.</param>
        /// <returns>An observable that produces the merged latest values of the observed observables.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyObservable<TRet>(
            Expression<Func<TSender, IObservable<TRet>?>> obs1,
            Expression<Func<TSender, IObservable<TRet>?>> obs2)
        {
            return new WhenAnyObservableMergeSink<TRet>(
                sender.WhenAny(
                    obs1,
                    obs2,
                    (o1, o2) => new[]
                    {
                        o1.Value!.EmptyIfNull(),
                        o2.Value!.EmptyIfNull(),
                    }));
        }

        /// <summary>Observes several observable-valued properties and combines their latest values with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <param name="obs1">An expression pointing to observable property 1.</param>
        /// <param name="obs2">An expression pointing to observable property 2.</param>
        /// <param name="selector">Combines the latest values of the observed observables into a result.</param>
        /// <returns>An observable that produces the projected result of the combined observables.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyObservable<TRet, T1, T2>(
            Expression<Func<TSender, IObservable<T1>?>> obs1,
            Expression<Func<TSender, IObservable<T2>?>> obs2,
            Func<T1?, T2?, TRet> selector) =>
            new WhenAnyObservableSwitchSink<TRet>(
                sender.WhenAny(
                    obs1,
                    obs2,
                    (o1, o2) => (IObservable<TRet>)new WhenAnyChangeSink<T1, T2, TRet>(
                        o1.Value!.EmptyIfNull(),
                        o2.Value!.EmptyIfNull(),
                        selector)));
    }
}
