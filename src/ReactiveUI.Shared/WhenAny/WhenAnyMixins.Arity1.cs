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
/// <summary>Provides the arity-1 WhenAny / WhenAnyValue / WhenAnyDynamic extension overloads.</summary>
[RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
[SuppressMessage(
    "Major Code Smell",
    "S4018:Generic methods should provide type parameter",
    Justification = "Type parameters are supplied explicitly by the caller by design; they identify the observed types and cannot be inferred from the arguments.")]
public static partial class WhenAnyMixins
{
    /// <summary>Provides arity-1 WhenAny extension members for the source object.</summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <param name="sender">The object whose property is observed.</param>
    extension<TSender>(TSender? sender)
    {
        /// <summary>Observes the value of a property, providing an initial value when set up.</summary>
        /// <typeparam name="TRet">The type of the property value.</typeparam>
        /// <param name="property1">An expression pointing to the property to observe.</param>
        /// <returns>An observable that emits the property value and subsequent changes.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet>(
            Expression<Func<TSender, TRet>> property1) =>
            sender!.WhenAny(property1, c1 => c1.Value);

        /// <summary>AOT-friendly overload that observes a property by name instead of an expression.</summary>
        /// <typeparam name="TRet">The type of the property value.</typeparam>
        /// <param name="propertyName">The name of the property to observe.</param>
        /// <returns>An observable that emits the property value and subsequent changes.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet>(
            string propertyName) =>
            new WhenAnyValueSink<TSender, TRet, TRet>(
                sender!.ObservableForProperty<TSender, TRet>(propertyName, beforeChange: false, skipInitial: false, isDistinct: true),
                static x => x);

        /// <summary>Observes the value of a property, optionally emitting only distinct values.</summary>
        /// <typeparam name="TRet">The type of the property value.</typeparam>
        /// <param name="property1">An expression pointing to the property to observe.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the property value and subsequent changes.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet>(
            Expression<Func<TSender, TRet>> property1,
            bool isDistinct) =>
            sender!.WhenAny(property1, c1 => c1.Value, isDistinct);

        /// <summary>AOT-friendly overload that observes a property by name, optionally distinct.</summary>
        /// <typeparam name="TRet">The type of the property value.</typeparam>
        /// <param name="propertyName">The name of the property to observe.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the property value and subsequent changes.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet>(
            string propertyName,
            bool isDistinct) =>
            new WhenAnyValueSink<TSender, TRet, TRet>(
                sender!.ObservableForProperty<TSender, TRet>(propertyName, beforeChange: false, skipInitial: false, isDistinct: isDistinct),
                static x => x);

        /// <summary>Observes several properties and combines their values with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="selector">Combines the observed property values into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet, T1>(
            Expression<Func<TSender, T1>> property1,
            Func<T1, TRet> selector) =>
            sender!.WhenAny(
                property1,
                c1 => selector(c1.Value));

        /// <summary>AOT-friendly overload that observes properties by name and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <param name="property1Name">The name of property 1.</param>
        /// <param name="selector">Combines the observed property values into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet, T1>(
            string property1Name,
            Func<T1, TRet> selector)
        {
            var o1 = sender!.ObservableForProperty<TSender, T1>(property1Name, beforeChange: false, skipInitial: false, isDistinct: true);
            return new WhenAnyValueSink<TSender, T1, TRet>(
                o1,
                selector);
        }

        /// <summary>Observes several properties and combines their values with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="selector">Combines the observed property values into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet, T1>(
            Expression<Func<TSender, T1>> property1,
            Func<T1, TRet> selector,
            bool isDistinct) =>
            sender!.WhenAny(
                property1,
                c1 => selector(c1.Value),
                isDistinct);

        /// <summary>AOT-friendly overload that observes properties by name and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <param name="property1Name">The name of property 1.</param>
        /// <param name="selector">Combines the observed property values into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet, T1>(
            string property1Name,
            Func<T1, TRet> selector,
            bool isDistinct)
        {
            var o1 = sender!.ObservableForProperty<TSender, T1>(property1Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            return new WhenAnyValueSink<TSender, T1, TRet>(
                o1,
                selector);
        }

        /// <summary>Observes several properties and combines their change notifications with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAny<TRet, T1>(
            Expression<Func<TSender, T1>> property1,
            Func<IObservedChange<TSender, T1>, TRet> selector) =>
            new WhenAnyChangeSink<IObservedChange<TSender, T1>, TRet>(
                sender!.ObservableForProperty(property1, false, false),
                selector);

        /// <summary>AOT-friendly overload that observes properties by name and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <param name="property1Name">The name of property 1.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAny<TRet, T1>(
            string property1Name,
            Func<IObservedChange<TSender, T1>, TRet> selector) =>
            new WhenAnyChangeSink<IObservedChange<TSender, T1>, TRet>(
                sender!.ObservableForProperty<TSender, T1>(property1Name, false, false),
                selector);

        /// <summary>Observes several properties and combines their change notifications with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAny<TRet, T1>(
            Expression<Func<TSender, T1>> property1,
            Func<IObservedChange<TSender, T1>, TRet> selector,
            bool isDistinct) =>
            new WhenAnyChangeSink<IObservedChange<TSender, T1>, TRet>(
                sender!.ObservableForProperty(property1, false, false, isDistinct),
                selector);

        /// <summary>AOT-friendly overload that observes properties by name and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <typeparam name="T1">The type of property 1.</typeparam>
        /// <param name="property1Name">The name of property 1.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAny<TRet, T1>(
            string property1Name,
            Func<IObservedChange<TSender, T1>, TRet> selector,
            bool isDistinct) =>
            new WhenAnyChangeSink<IObservedChange<TSender, T1>, TRet>(
                sender!.ObservableForProperty<TSender, T1>(property1Name, false, false, isDistinct),
                selector);

        /// <summary>Observes several dynamically-typed property chains and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyDynamic<TRet>(
            Expression? property1,
            Func<IObservedChange<TSender?, object?>, TRet> selector) =>
            new WhenAnyChangeSink<IObservedChange<TSender, object?>, TRet>(
                sender.SubscribeToExpressionChain<TSender, object?>(property1, false, false),
                selector);

        /// <summary>Observes several dynamically-typed property chains and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyDynamic<TRet>(
            Expression? property1,
            Func<IObservedChange<TSender?, object?>, TRet> selector,
            bool isDistinct) =>
            new WhenAnyChangeSink<IObservedChange<TSender, object?>, TRet>(
                sender.SubscribeToExpressionChain<TSender, object?>(property1, false, false, isDistinct),
                selector);
    }
}
