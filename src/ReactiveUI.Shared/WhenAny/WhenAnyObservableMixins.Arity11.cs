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
/// <summary>Provides the arity-11 WhenAnyObservable extension overloads.</summary>
[SuppressMessage(
    "Design",
    "SST1472:Method declares too many parameters",
    Justification = "Parameter count is intrinsic to the fixed WhenAny arity API.")]
public static partial class WhenAnyObservableMixins
{
    /// <summary>Provides arity-11 WhenAnyObservable extension members for a source object.</summary>
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
        /// <param name="obs5">An expression pointing to observable property 5.</param>
        /// <param name="obs6">An expression pointing to observable property 6.</param>
        /// <param name="obs7">An expression pointing to observable property 7.</param>
        /// <param name="obs8">An expression pointing to observable property 8.</param>
        /// <param name="obs9">An expression pointing to observable property 9.</param>
        /// <param name="obs10">An expression pointing to observable property 10.</param>
        /// <param name="obs11">An expression pointing to observable property 11.</param>
        /// <returns>An observable that produces the merged latest values of the observed observables.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyObservable<TRet>(
            Expression<Func<TSender, IObservable<TRet>?>> obs1,
            Expression<Func<TSender, IObservable<TRet>?>> obs2,
            Expression<Func<TSender, IObservable<TRet>?>> obs3,
            Expression<Func<TSender, IObservable<TRet>?>> obs4,
            Expression<Func<TSender, IObservable<TRet>?>> obs5,
            Expression<Func<TSender, IObservable<TRet>?>> obs6,
            Expression<Func<TSender, IObservable<TRet>?>> obs7,
            Expression<Func<TSender, IObservable<TRet>?>> obs8,
            Expression<Func<TSender, IObservable<TRet>?>> obs9,
            Expression<Func<TSender, IObservable<TRet>?>> obs10,
            Expression<Func<TSender, IObservable<TRet>?>> obs11) =>
            new WhenAnyObservableMergeSink<TRet>(
                sender.WhenAny(
                    obs1,
                    obs2,
                    obs3,
                    obs4,
                    obs5,
                    obs6,
                    obs7,
                    obs8,
                    obs9,
                    obs10,
                    obs11,
                    static (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11) => new[]
                    {
                        o1.Value!.EmptyIfNull(),
                        o2.Value!.EmptyIfNull(),
                        o3.Value!.EmptyIfNull(),
                        o4.Value!.EmptyIfNull(),
                        o5.Value!.EmptyIfNull(),
                        o6.Value!.EmptyIfNull(),
                        o7.Value!.EmptyIfNull(),
                        o8.Value!.EmptyIfNull(),
                        o9.Value!.EmptyIfNull(),
                        o10.Value!.EmptyIfNull(),
                        o11.Value!.EmptyIfNull(),
                    }));

        /// <summary>Observes several observable-valued properties and combines their latest values with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <typeparam name="T3">The type of property 3.</typeparam>
        /// <typeparam name="T4">The type of property 4.</typeparam>
        /// <typeparam name="T5">The type of property 5.</typeparam>
        /// <typeparam name="T6">The type of property 6.</typeparam>
        /// <typeparam name="T7">The type of property 7.</typeparam>
        /// <typeparam name="T8">The type of property 8.</typeparam>
        /// <typeparam name="T9">The type of property 9.</typeparam>
        /// <typeparam name="T10">The type of property 10.</typeparam>
        /// <typeparam name="T11">The type of property 11.</typeparam>
        /// <param name="obs1">An expression pointing to observable property 1.</param>
        /// <param name="obs2">An expression pointing to observable property 2.</param>
        /// <param name="obs3">An expression pointing to observable property 3.</param>
        /// <param name="obs4">An expression pointing to observable property 4.</param>
        /// <param name="obs5">An expression pointing to observable property 5.</param>
        /// <param name="obs6">An expression pointing to observable property 6.</param>
        /// <param name="obs7">An expression pointing to observable property 7.</param>
        /// <param name="obs8">An expression pointing to observable property 8.</param>
        /// <param name="obs9">An expression pointing to observable property 9.</param>
        /// <param name="obs10">An expression pointing to observable property 10.</param>
        /// <param name="obs11">An expression pointing to observable property 11.</param>
        /// <param name="selector">Combines the latest values of the observed observables into a result.</param>
        /// <returns>An observable that produces the projected result of the combined observables.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyObservable<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            Expression<Func<TSender, IObservable<T1>?>> obs1,
            Expression<Func<TSender, IObservable<T2>?>> obs2,
            Expression<Func<TSender, IObservable<T3>?>> obs3,
            Expression<Func<TSender, IObservable<T4>?>> obs4,
            Expression<Func<TSender, IObservable<T5>?>> obs5,
            Expression<Func<TSender, IObservable<T6>?>> obs6,
            Expression<Func<TSender, IObservable<T7>?>> obs7,
            Expression<Func<TSender, IObservable<T8>?>> obs8,
            Expression<Func<TSender, IObservable<T9>?>> obs9,
            Expression<Func<TSender, IObservable<T10>?>> obs10,
            Expression<Func<TSender, IObservable<T11>?>> obs11,
            Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, TRet> selector) =>
            new WhenAnyObservableSwitchSink<TRet>(
                sender.WhenAny(
                    obs1,
                    obs2,
                    obs3,
                    obs4,
                    obs5,
                    obs6,
                    obs7,
                    obs8,
                    obs9,
                    obs10,
                    obs11,
                    (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11) => (IObservable<TRet>)new WhenAnyChangeSink<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRet>(
                        o1.Value!.EmptyIfNull(),
                        o2.Value!.EmptyIfNull(),
                        o3.Value!.EmptyIfNull(),
                        o4.Value!.EmptyIfNull(),
                        o5.Value!.EmptyIfNull(),
                        o6.Value!.EmptyIfNull(),
                        o7.Value!.EmptyIfNull(),
                        o8.Value!.EmptyIfNull(),
                        o9.Value!.EmptyIfNull(),
                        o10.Value!.EmptyIfNull(),
                        o11.Value!.EmptyIfNull(),
                        selector)));
    }
}
