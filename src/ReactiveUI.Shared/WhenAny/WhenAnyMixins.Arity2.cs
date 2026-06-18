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
/// <summary>Provides the arity-2 WhenAny / WhenAnyValue / WhenAnyDynamic extension overloads.</summary>
public static partial class WhenAnyMixins
{
    /// <summary>Provides arity-2 WhenAny extension members for the source object.</summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    extension<TSender>(TSender? sender)
    {
        /// <summary>Observes several properties and projects their values into a tuple.</summary>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <returns>An observable that emits a tuple of the observed property values.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<(T1 Value1, T2 Value2)> WhenAnyValue<T1, T2>(
            Expression<Func<TSender, T1>> property1,
            Expression<Func<TSender, T2>> property2) =>
            sender!.WhenAny(
                property1,
                property2,
                (c1, c2) => (c1.Value, c2.Value));

        /// <summary>AOT-friendly overload that observes properties by name and projects a tuple.</summary>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <param name="property1Name">The name of property 1.</param>
        /// <param name="property2Name">The name of property 2.</param>
        /// <returns>An observable that emits a tuple of the observed property values.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<(T1 Value1, T2 Value2)> WhenAnyValue<T1, T2>(
            string property1Name,
            string property2Name)
        {
            var o1 = sender!.ObservableForProperty<TSender, T1>(property1Name, beforeChange: false, skipInitial: false, isDistinct: true);
            var o2 = sender!.ObservableForProperty<TSender, T2>(property2Name, beforeChange: false, skipInitial: false, isDistinct: true);
            return new WhenAnyValueSink<TSender, T1, T2, (T1 Value1, T2 Value2)>(
                o1,
                o2,
                static (v1, v2) => (v1, v2));
        }

        /// <summary>Observes several properties and projects their values into a tuple.</summary>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits a tuple of the observed property values.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<(T1 Value1, T2 Value2)> WhenAnyValue<T1, T2>(
            Expression<Func<TSender, T1>> property1,
            Expression<Func<TSender, T2>> property2,
            bool isDistinct) =>
            sender!.WhenAny(
                property1,
                property2,
                (c1, c2) => (c1.Value, c2.Value),
                isDistinct);

        /// <summary>AOT-friendly overload that observes properties by name and projects a tuple.</summary>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <param name="property1Name">The name of property 1.</param>
        /// <param name="property2Name">The name of property 2.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits a tuple of the observed property values.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<(T1 Value1, T2 Value2)> WhenAnyValue<T1, T2>(
            string property1Name,
            string property2Name,
            bool isDistinct)
        {
            var o1 = sender!.ObservableForProperty<TSender, T1>(property1Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            var o2 = sender!.ObservableForProperty<TSender, T2>(property2Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            return new WhenAnyValueSink<TSender, T1, T2, (T1 Value1, T2 Value2)>(
                o1,
                o2,
                static (v1, v2) => (v1, v2));
        }

        /// <summary>Observes several properties and combines their values with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="selector">Combines the observed property values into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet, T1, T2>(
            Expression<Func<TSender, T1>> property1,
            Expression<Func<TSender, T2>> property2,
            Func<T1, T2, TRet> selector) =>
            sender!.WhenAny(
                property1,
                property2,
                (c1, c2) => selector(c1.Value, c2.Value));

        /// <summary>AOT-friendly overload that observes properties by name and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <param name="property1Name">The name of property 1.</param>
        /// <param name="property2Name">The name of property 2.</param>
        /// <param name="selector">Combines the observed property values into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet, T1, T2>(
            string property1Name,
            string property2Name,
            Func<T1, T2, TRet> selector)
        {
            var o1 = sender!.ObservableForProperty<TSender, T1>(property1Name, beforeChange: false, skipInitial: false, isDistinct: true);
            var o2 = sender!.ObservableForProperty<TSender, T2>(property2Name, beforeChange: false, skipInitial: false, isDistinct: true);
            return new WhenAnyValueSink<TSender, T1, T2, TRet>(
                o1,
                o2,
                selector);
        }

        /// <summary>Observes several properties and combines their values with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="selector">Combines the observed property values into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet, T1, T2>(
            Expression<Func<TSender, T1>> property1,
            Expression<Func<TSender, T2>> property2,
            Func<T1, T2, TRet> selector,
            bool isDistinct) =>
            sender!.WhenAny(
                property1,
                property2,
                (c1, c2) => selector(c1.Value, c2.Value),
                isDistinct);

        /// <summary>AOT-friendly overload that observes properties by name and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <param name="property1Name">The name of property 1.</param>
        /// <param name="property2Name">The name of property 2.</param>
        /// <param name="selector">Combines the observed property values into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet, T1, T2>(
            string property1Name,
            string property2Name,
            Func<T1, T2, TRet> selector,
            bool isDistinct)
        {
            var o1 = sender!.ObservableForProperty<TSender, T1>(property1Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            var o2 = sender!.ObservableForProperty<TSender, T2>(property2Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            return new WhenAnyValueSink<TSender, T1, T2, TRet>(
                o1,
                o2,
                selector);
        }

        /// <summary>Observes several properties and combines their change notifications with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAny<TRet, T1, T2>(
            Expression<Func<TSender, T1>> property1,
            Expression<Func<TSender, T2>> property2,
            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, TRet> selector) =>
            new WhenAnyChangeSink<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, TRet>(
                sender!.ObservableForProperty(property1, false, false),
                sender!.ObservableForProperty(property2, false, false),
                selector);

        /// <summary>AOT-friendly overload that observes properties by name and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <param name="property1Name">The name of property 1.</param>
        /// <param name="property2Name">The name of property 2.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAny<TRet, T1, T2>(
            string property1Name,
            string property2Name,
            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, TRet> selector) =>
            new WhenAnyChangeSink<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, TRet>(
                sender!.ObservableForProperty<TSender, T1>(property1Name, false, false),
                sender!.ObservableForProperty<TSender, T2>(property2Name, false, false),
                selector);

        /// <summary>Observes several properties and combines their change notifications with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAny<TRet, T1, T2>(
            Expression<Func<TSender, T1>> property1,
            Expression<Func<TSender, T2>> property2,
            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, TRet> selector,
            bool isDistinct) =>
            new WhenAnyChangeSink<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, TRet>(
                sender!.ObservableForProperty(property1, false, false, isDistinct),
                sender!.ObservableForProperty(property2, false, false, isDistinct),
                selector);

        /// <summary>AOT-friendly overload that observes properties by name and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <typeparam name="T2">The type of property 2.</typeparam>
        /// <param name="property1Name">The name of property 1.</param>
        /// <param name="property2Name">The name of property 2.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAny<TRet, T1, T2>(
            string property1Name,
            string property2Name,
            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, TRet> selector,
            bool isDistinct) =>
            new WhenAnyChangeSink<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, TRet>(
                sender!.ObservableForProperty<TSender, T1>(property1Name, false, false, isDistinct),
                sender!.ObservableForProperty<TSender, T2>(property2Name, false, false, isDistinct),
                selector);

        /// <summary>Observes several dynamically-typed property chains and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyDynamic<TRet>(
            Expression? property1,
            Expression? property2,
            Func<IObservedChange<TSender?, object?>, IObservedChange<TSender?, object?>, TRet> selector) =>
            new WhenAnyChangeSink<IObservedChange<TSender, object?>, IObservedChange<TSender, object?>, TRet>(
                sender.SubscribeToExpressionChain<TSender, object?>(property1, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property2, false, false),
                selector);

        /// <summary>Observes several dynamically-typed property chains and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyDynamic<TRet>(
            Expression? property1,
            Expression? property2,
            Func<IObservedChange<TSender?, object?>, IObservedChange<TSender?, object?>, TRet> selector,
            bool isDistinct) =>
            new WhenAnyChangeSink<IObservedChange<TSender, object?>, IObservedChange<TSender, object?>, TRet>(
                sender.SubscribeToExpressionChain<TSender, object?>(property1, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property2, false, false, isDistinct),
                selector);
    }
}
