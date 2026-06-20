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
/// <summary>Provides the arity-4 WhenAnyObservable extension overloads.</summary>
public static partial class WhenAnyObservableMixins
{
    /// <summary>Provides arity-4 WhenAnyObservable extension members for a source object.</summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <param name="sender">The object whose observable properties are observed.</param>
    extension<TSender>(TSender? sender)
        where TSender : class
    {
        /// <summary>Observes several observable-valued properties and merges their latest values.</summary>
        /// <typeparam name="TRet">The type of the observables' value.</typeparam>
        /// <param name="obs1">An expression pointing to observable property 1.</param>
        /// <param name="obs2">An expression pointing to observable property 2.</param>
        /// <param name="obs3">An expression pointing to observable property 3.</param>
        /// <param name="obs4">An expression pointing to observable property 4.</param>
        /// <returns>An observable that produces the merged latest values of the observed observables.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyObservable<TRet>(
            Expression<Func<TSender, IObservable<TRet>?>> obs1,
            Expression<Func<TSender, IObservable<TRet>?>> obs2,
            Expression<Func<TSender, IObservable<TRet>?>> obs3,
            Expression<Func<TSender, IObservable<TRet>?>> obs4)
        {
            return new WhenAnyObservableMergeSink<TRet>(
                sender.WhenAny(
                    obs1,
                    obs2,
                    obs3,
                    obs4,
                    (o1, o2, o3, o4) => new[]
                    {
                        o1.Value!.EmptyIfNull(),
                        o2.Value!.EmptyIfNull(),
                        o3.Value!.EmptyIfNull(),
                        o4.Value!.EmptyIfNull(),
                    }));
        }

        /// <summary>Observes several observable-valued properties and combines their latest values with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <typeparam name="T3">The type of property 3.</typeparam>
        /// <typeparam name="T4">The type of property 4.</typeparam>
        /// <param name="obs1">An expression pointing to observable property 1.</param>
        /// <param name="obs2">An expression pointing to observable property 2.</param>
        /// <param name="obs3">An expression pointing to observable property 3.</param>
        /// <param name="obs4">An expression pointing to observable property 4.</param>
        /// <param name="selector">Combines the latest values of the observed observables into a result.</param>
        /// <returns>An observable that produces the projected result of the combined observables.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyObservable<TRet, T1, T2, T3, T4>(
            Expression<Func<TSender, IObservable<T1>?>> obs1,
            Expression<Func<TSender, IObservable<T2>?>> obs2,
            Expression<Func<TSender, IObservable<T3>?>> obs3,
            Expression<Func<TSender, IObservable<T4>?>> obs4,
            Func<T1?, T2?, T3?, T4?, TRet> selector) =>
            new WhenAnyObservableSwitchSink<TRet>(
                sender.WhenAny(
                    obs1,
                    obs2,
                    obs3,
                    obs4,
                    (o1, o2, o3, o4) => (IObservable<TRet>)new WhenAnyChangeSink<T1, T2, T3, T4, TRet>(
                        o1.Value!.EmptyIfNull(),
                        o2.Value!.EmptyIfNull(),
                        o3.Value!.EmptyIfNull(),
                        o4.Value!.EmptyIfNull(),
                        selector)));
    }
}
