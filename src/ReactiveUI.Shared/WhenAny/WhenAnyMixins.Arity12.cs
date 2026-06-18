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
/// <summary>Provides the arity-12 WhenAny / WhenAnyValue / WhenAnyDynamic extension overloads.</summary>
[SuppressMessage(
    "Major Code Smell",
    "S107:Methods should not have too many parameters",
    Justification = "Arity-N variadic overloads intentionally expose more than seven parameters.")]
public static partial class WhenAnyMixins
{
    /// <summary>Provides the arity-12 WhenAny / WhenAnyValue / WhenAnyDynamic extension members for an observed source object.</summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    extension<TSender>(TSender? sender)
    {
        /// <summary>Observes several properties and combines their values with a selector.</summary>
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
        /// <typeparam name="T12">The type of property 12.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="property3">An expression pointing to property 3.</param>
        /// <param name="property4">An expression pointing to property 4.</param>
        /// <param name="property5">An expression pointing to property 5.</param>
        /// <param name="property6">An expression pointing to property 6.</param>
        /// <param name="property7">An expression pointing to property 7.</param>
        /// <param name="property8">An expression pointing to property 8.</param>
        /// <param name="property9">An expression pointing to property 9.</param>
        /// <param name="property10">An expression pointing to property 10.</param>
        /// <param name="property11">An expression pointing to property 11.</param>
        /// <param name="property12">An expression pointing to property 12.</param>
        /// <param name="selector">Combines the observed property values into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            Expression<Func<TSender, T1>> property1,
            Expression<Func<TSender, T2>> property2,
            Expression<Func<TSender, T3>> property3,
            Expression<Func<TSender, T4>> property4,
            Expression<Func<TSender, T5>> property5,
            Expression<Func<TSender, T6>> property6,
            Expression<Func<TSender, T7>> property7,
            Expression<Func<TSender, T8>> property8,
            Expression<Func<TSender, T9>> property9,
            Expression<Func<TSender, T10>> property10,
            Expression<Func<TSender, T11>> property11,
            Expression<Func<TSender, T12>> property12,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet> selector) =>
            sender!.WhenAny(
                property1,
                property2,
                property3,
                property4,
                property5,
                property6,
                property7,
                property8,
                property9,
                property10,
                property11,
                property12,
                (c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12) => selector(
                    c1.Value,
                    c2.Value,
                    c3.Value,
                    c4.Value,
                    c5.Value,
                    c6.Value,
                    c7.Value,
                    c8.Value,
                    c9.Value,
                    c10.Value,
                    c11.Value,
                    c12.Value));

        /// <summary>AOT-friendly overload that observes properties by name and combines them with a selector.</summary>
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
        /// <typeparam name="T12">The type of property 12.</typeparam>
        /// <param name="property1Name">The name of property 1.</param>
        /// <param name="property2Name">The name of property 2.</param>
        /// <param name="property3Name">The name of property 3.</param>
        /// <param name="property4Name">The name of property 4.</param>
        /// <param name="property5Name">The name of property 5.</param>
        /// <param name="property6Name">The name of property 6.</param>
        /// <param name="property7Name">The name of property 7.</param>
        /// <param name="property8Name">The name of property 8.</param>
        /// <param name="property9Name">The name of property 9.</param>
        /// <param name="property10Name">The name of property 10.</param>
        /// <param name="property11Name">The name of property 11.</param>
        /// <param name="property12Name">The name of property 12.</param>
        /// <param name="selector">Combines the observed property values into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            string property1Name,
            string property2Name,
            string property3Name,
            string property4Name,
            string property5Name,
            string property6Name,
            string property7Name,
            string property8Name,
            string property9Name,
            string property10Name,
            string property11Name,
            string property12Name,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet> selector)
        {
            var o1 = sender!.ObservableForProperty<TSender, T1>(property1Name, beforeChange: false, skipInitial: false, isDistinct: true);
            var o2 = sender!.ObservableForProperty<TSender, T2>(property2Name, beforeChange: false, skipInitial: false, isDistinct: true);
            var o3 = sender!.ObservableForProperty<TSender, T3>(property3Name, beforeChange: false, skipInitial: false, isDistinct: true);
            var o4 = sender!.ObservableForProperty<TSender, T4>(property4Name, beforeChange: false, skipInitial: false, isDistinct: true);
            var o5 = sender!.ObservableForProperty<TSender, T5>(property5Name, beforeChange: false, skipInitial: false, isDistinct: true);
            var o6 = sender!.ObservableForProperty<TSender, T6>(property6Name, beforeChange: false, skipInitial: false, isDistinct: true);
            var o7 = sender!.ObservableForProperty<TSender, T7>(property7Name, beforeChange: false, skipInitial: false, isDistinct: true);
            var o8 = sender!.ObservableForProperty<TSender, T8>(property8Name, beforeChange: false, skipInitial: false, isDistinct: true);
            var o9 = sender!.ObservableForProperty<TSender, T9>(property9Name, beforeChange: false, skipInitial: false, isDistinct: true);
            var o10 = sender!.ObservableForProperty<TSender, T10>(property10Name, beforeChange: false, skipInitial: false, isDistinct: true);
            var o11 = sender!.ObservableForProperty<TSender, T11>(property11Name, beforeChange: false, skipInitial: false, isDistinct: true);
            var o12 = sender!.ObservableForProperty<TSender, T12>(property12Name, beforeChange: false, skipInitial: false, isDistinct: true);
            return new WhenAnyValueSink<TSender, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet>(
                o1,
                o2,
                o3,
                o4,
                o5,
                o6,
                o7,
                o8,
                o9,
                o10,
                o11,
                o12,
                selector);
        }

        /// <summary>Observes several properties and combines their values with a selector.</summary>
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
        /// <typeparam name="T12">The type of property 12.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="property3">An expression pointing to property 3.</param>
        /// <param name="property4">An expression pointing to property 4.</param>
        /// <param name="property5">An expression pointing to property 5.</param>
        /// <param name="property6">An expression pointing to property 6.</param>
        /// <param name="property7">An expression pointing to property 7.</param>
        /// <param name="property8">An expression pointing to property 8.</param>
        /// <param name="property9">An expression pointing to property 9.</param>
        /// <param name="property10">An expression pointing to property 10.</param>
        /// <param name="property11">An expression pointing to property 11.</param>
        /// <param name="property12">An expression pointing to property 12.</param>
        /// <param name="selector">Combines the observed property values into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            Expression<Func<TSender, T1>> property1,
            Expression<Func<TSender, T2>> property2,
            Expression<Func<TSender, T3>> property3,
            Expression<Func<TSender, T4>> property4,
            Expression<Func<TSender, T5>> property5,
            Expression<Func<TSender, T6>> property6,
            Expression<Func<TSender, T7>> property7,
            Expression<Func<TSender, T8>> property8,
            Expression<Func<TSender, T9>> property9,
            Expression<Func<TSender, T10>> property10,
            Expression<Func<TSender, T11>> property11,
            Expression<Func<TSender, T12>> property12,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet> selector,
            bool isDistinct) =>
            sender!.WhenAny(
                property1,
                property2,
                property3,
                property4,
                property5,
                property6,
                property7,
                property8,
                property9,
                property10,
                property11,
                property12,
                (c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12) => selector(
                    c1.Value,
                    c2.Value,
                    c3.Value,
                    c4.Value,
                    c5.Value,
                    c6.Value,
                    c7.Value,
                    c8.Value,
                    c9.Value,
                    c10.Value,
                    c11.Value,
                    c12.Value),
                isDistinct);

        /// <summary>AOT-friendly overload that observes properties by name and combines them with a selector.</summary>
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
        /// <typeparam name="T12">The type of property 12.</typeparam>
        /// <param name="property1Name">The name of property 1.</param>
        /// <param name="property2Name">The name of property 2.</param>
        /// <param name="property3Name">The name of property 3.</param>
        /// <param name="property4Name">The name of property 4.</param>
        /// <param name="property5Name">The name of property 5.</param>
        /// <param name="property6Name">The name of property 6.</param>
        /// <param name="property7Name">The name of property 7.</param>
        /// <param name="property8Name">The name of property 8.</param>
        /// <param name="property9Name">The name of property 9.</param>
        /// <param name="property10Name">The name of property 10.</param>
        /// <param name="property11Name">The name of property 11.</param>
        /// <param name="property12Name">The name of property 12.</param>
        /// <param name="selector">Combines the observed property values into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyValue<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            string property1Name,
            string property2Name,
            string property3Name,
            string property4Name,
            string property5Name,
            string property6Name,
            string property7Name,
            string property8Name,
            string property9Name,
            string property10Name,
            string property11Name,
            string property12Name,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet> selector,
            bool isDistinct)
        {
            var o1 = sender!.ObservableForProperty<TSender, T1>(property1Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            var o2 = sender!.ObservableForProperty<TSender, T2>(property2Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            var o3 = sender!.ObservableForProperty<TSender, T3>(property3Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            var o4 = sender!.ObservableForProperty<TSender, T4>(property4Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            var o5 = sender!.ObservableForProperty<TSender, T5>(property5Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            var o6 = sender!.ObservableForProperty<TSender, T6>(property6Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            var o7 = sender!.ObservableForProperty<TSender, T7>(property7Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            var o8 = sender!.ObservableForProperty<TSender, T8>(property8Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            var o9 = sender!.ObservableForProperty<TSender, T9>(property9Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            var o10 = sender!.ObservableForProperty<TSender, T10>(property10Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            var o11 = sender!.ObservableForProperty<TSender, T11>(property11Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            var o12 = sender!.ObservableForProperty<TSender, T12>(property12Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
            return new WhenAnyValueSink<TSender, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet>(
                o1,
                o2,
                o3,
                o4,
                o5,
                o6,
                o7,
                o8,
                o9,
                o10,
                o11,
                o12,
                selector);
        }

        /// <summary>Observes several properties and combines their change notifications with a selector.</summary>
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
        /// <typeparam name="T12">The type of property 12.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="property3">An expression pointing to property 3.</param>
        /// <param name="property4">An expression pointing to property 4.</param>
        /// <param name="property5">An expression pointing to property 5.</param>
        /// <param name="property6">An expression pointing to property 6.</param>
        /// <param name="property7">An expression pointing to property 7.</param>
        /// <param name="property8">An expression pointing to property 8.</param>
        /// <param name="property9">An expression pointing to property 9.</param>
        /// <param name="property10">An expression pointing to property 10.</param>
        /// <param name="property11">An expression pointing to property 11.</param>
        /// <param name="property12">An expression pointing to property 12.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAny<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            Expression<Func<TSender, T1>> property1,
            Expression<Func<TSender, T2>> property2,
            Expression<Func<TSender, T3>> property3,
            Expression<Func<TSender, T4>> property4,
            Expression<Func<TSender, T5>> property5,
            Expression<Func<TSender, T6>> property6,
            Expression<Func<TSender, T7>> property7,
            Expression<Func<TSender, T8>> property8,
            Expression<Func<TSender, T9>> property9,
            Expression<Func<TSender, T10>> property10,
            Expression<Func<TSender, T11>> property11,
            Expression<Func<TSender, T12>> property12,
            Func<
                IObservedChange<TSender, T1>,
                IObservedChange<TSender, T2>,
                IObservedChange<TSender, T3>,
                IObservedChange<TSender, T4>,
                IObservedChange<TSender, T5>,
                IObservedChange<TSender, T6>,
                IObservedChange<TSender, T7>,
                IObservedChange<TSender, T8>,
                IObservedChange<TSender, T9>,
                IObservedChange<TSender, T10>,
                IObservedChange<TSender, T11>,
                IObservedChange<TSender, T12>,
                TRet> selector) =>
            new WhenAnyChangeSink<
                IObservedChange<TSender, T1>,
                IObservedChange<TSender, T2>,
                IObservedChange<TSender, T3>,
                IObservedChange<TSender, T4>,
                IObservedChange<TSender, T5>,
                IObservedChange<TSender, T6>,
                IObservedChange<TSender, T7>,
                IObservedChange<TSender, T8>,
                IObservedChange<TSender, T9>,
                IObservedChange<TSender, T10>,
                IObservedChange<TSender, T11>,
                IObservedChange<TSender, T12>,
                TRet>(
                sender!.ObservableForProperty(property1, false, false),
                sender!.ObservableForProperty(property2, false, false),
                sender!.ObservableForProperty(property3, false, false),
                sender!.ObservableForProperty(property4, false, false),
                sender!.ObservableForProperty(property5, false, false),
                sender!.ObservableForProperty(property6, false, false),
                sender!.ObservableForProperty(property7, false, false),
                sender!.ObservableForProperty(property8, false, false),
                sender!.ObservableForProperty(property9, false, false),
                sender!.ObservableForProperty(property10, false, false),
                sender!.ObservableForProperty(property11, false, false),
                sender!.ObservableForProperty(property12, false, false),
                selector);

        /// <summary>AOT-friendly overload that observes properties by name and combines them with a selector.</summary>
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
        /// <typeparam name="T12">The type of property 12.</typeparam>
        /// <param name="property1Name">The name of property 1.</param>
        /// <param name="property2Name">The name of property 2.</param>
        /// <param name="property3Name">The name of property 3.</param>
        /// <param name="property4Name">The name of property 4.</param>
        /// <param name="property5Name">The name of property 5.</param>
        /// <param name="property6Name">The name of property 6.</param>
        /// <param name="property7Name">The name of property 7.</param>
        /// <param name="property8Name">The name of property 8.</param>
        /// <param name="property9Name">The name of property 9.</param>
        /// <param name="property10Name">The name of property 10.</param>
        /// <param name="property11Name">The name of property 11.</param>
        /// <param name="property12Name">The name of property 12.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAny<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            string property1Name,
            string property2Name,
            string property3Name,
            string property4Name,
            string property5Name,
            string property6Name,
            string property7Name,
            string property8Name,
            string property9Name,
            string property10Name,
            string property11Name,
            string property12Name,
            Func<
                IObservedChange<TSender, T1>,
                IObservedChange<TSender, T2>,
                IObservedChange<TSender, T3>,
                IObservedChange<TSender, T4>,
                IObservedChange<TSender, T5>,
                IObservedChange<TSender, T6>,
                IObservedChange<TSender, T7>,
                IObservedChange<TSender, T8>,
                IObservedChange<TSender, T9>,
                IObservedChange<TSender, T10>,
                IObservedChange<TSender, T11>,
                IObservedChange<TSender, T12>,
                TRet> selector) =>
            new WhenAnyChangeSink<
                IObservedChange<TSender, T1>,
                IObservedChange<TSender, T2>,
                IObservedChange<TSender, T3>,
                IObservedChange<TSender, T4>,
                IObservedChange<TSender, T5>,
                IObservedChange<TSender, T6>,
                IObservedChange<TSender, T7>,
                IObservedChange<TSender, T8>,
                IObservedChange<TSender, T9>,
                IObservedChange<TSender, T10>,
                IObservedChange<TSender, T11>,
                IObservedChange<TSender, T12>,
                TRet>(
                sender!.ObservableForProperty<TSender, T1>(property1Name, false, false),
                sender!.ObservableForProperty<TSender, T2>(property2Name, false, false),
                sender!.ObservableForProperty<TSender, T3>(property3Name, false, false),
                sender!.ObservableForProperty<TSender, T4>(property4Name, false, false),
                sender!.ObservableForProperty<TSender, T5>(property5Name, false, false),
                sender!.ObservableForProperty<TSender, T6>(property6Name, false, false),
                sender!.ObservableForProperty<TSender, T7>(property7Name, false, false),
                sender!.ObservableForProperty<TSender, T8>(property8Name, false, false),
                sender!.ObservableForProperty<TSender, T9>(property9Name, false, false),
                sender!.ObservableForProperty<TSender, T10>(property10Name, false, false),
                sender!.ObservableForProperty<TSender, T11>(property11Name, false, false),
                sender!.ObservableForProperty<TSender, T12>(property12Name, false, false),
                selector);

        /// <summary>Observes several properties and combines their change notifications with a selector.</summary>
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
        /// <typeparam name="T12">The type of property 12.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="property3">An expression pointing to property 3.</param>
        /// <param name="property4">An expression pointing to property 4.</param>
        /// <param name="property5">An expression pointing to property 5.</param>
        /// <param name="property6">An expression pointing to property 6.</param>
        /// <param name="property7">An expression pointing to property 7.</param>
        /// <param name="property8">An expression pointing to property 8.</param>
        /// <param name="property9">An expression pointing to property 9.</param>
        /// <param name="property10">An expression pointing to property 10.</param>
        /// <param name="property11">An expression pointing to property 11.</param>
        /// <param name="property12">An expression pointing to property 12.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAny<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            Expression<Func<TSender, T1>> property1,
            Expression<Func<TSender, T2>> property2,
            Expression<Func<TSender, T3>> property3,
            Expression<Func<TSender, T4>> property4,
            Expression<Func<TSender, T5>> property5,
            Expression<Func<TSender, T6>> property6,
            Expression<Func<TSender, T7>> property7,
            Expression<Func<TSender, T8>> property8,
            Expression<Func<TSender, T9>> property9,
            Expression<Func<TSender, T10>> property10,
            Expression<Func<TSender, T11>> property11,
            Expression<Func<TSender, T12>> property12,
            Func<
                IObservedChange<TSender, T1>,
                IObservedChange<TSender, T2>,
                IObservedChange<TSender, T3>,
                IObservedChange<TSender, T4>,
                IObservedChange<TSender, T5>,
                IObservedChange<TSender, T6>,
                IObservedChange<TSender, T7>,
                IObservedChange<TSender, T8>,
                IObservedChange<TSender, T9>,
                IObservedChange<TSender, T10>,
                IObservedChange<TSender, T11>,
                IObservedChange<TSender, T12>,
                TRet> selector,
            bool isDistinct) =>
            new WhenAnyChangeSink<
                IObservedChange<TSender, T1>,
                IObservedChange<TSender, T2>,
                IObservedChange<TSender, T3>,
                IObservedChange<TSender, T4>,
                IObservedChange<TSender, T5>,
                IObservedChange<TSender, T6>,
                IObservedChange<TSender, T7>,
                IObservedChange<TSender, T8>,
                IObservedChange<TSender, T9>,
                IObservedChange<TSender, T10>,
                IObservedChange<TSender, T11>,
                IObservedChange<TSender, T12>,
                TRet>(
                sender!.ObservableForProperty(property1, false, false, isDistinct),
                sender!.ObservableForProperty(property2, false, false, isDistinct),
                sender!.ObservableForProperty(property3, false, false, isDistinct),
                sender!.ObservableForProperty(property4, false, false, isDistinct),
                sender!.ObservableForProperty(property5, false, false, isDistinct),
                sender!.ObservableForProperty(property6, false, false, isDistinct),
                sender!.ObservableForProperty(property7, false, false, isDistinct),
                sender!.ObservableForProperty(property8, false, false, isDistinct),
                sender!.ObservableForProperty(property9, false, false, isDistinct),
                sender!.ObservableForProperty(property10, false, false, isDistinct),
                sender!.ObservableForProperty(property11, false, false, isDistinct),
                sender!.ObservableForProperty(property12, false, false, isDistinct),
                selector);

        /// <summary>AOT-friendly overload that observes properties by name and combines them with a selector.</summary>
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
        /// <typeparam name="T12">The type of property 12.</typeparam>
        /// <param name="property1Name">The name of property 1.</param>
        /// <param name="property2Name">The name of property 2.</param>
        /// <param name="property3Name">The name of property 3.</param>
        /// <param name="property4Name">The name of property 4.</param>
        /// <param name="property5Name">The name of property 5.</param>
        /// <param name="property6Name">The name of property 6.</param>
        /// <param name="property7Name">The name of property 7.</param>
        /// <param name="property8Name">The name of property 8.</param>
        /// <param name="property9Name">The name of property 9.</param>
        /// <param name="property10Name">The name of property 10.</param>
        /// <param name="property11Name">The name of property 11.</param>
        /// <param name="property12Name">The name of property 12.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAny<TRet, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            string property1Name,
            string property2Name,
            string property3Name,
            string property4Name,
            string property5Name,
            string property6Name,
            string property7Name,
            string property8Name,
            string property9Name,
            string property10Name,
            string property11Name,
            string property12Name,
            Func<
                IObservedChange<TSender, T1>,
                IObservedChange<TSender, T2>,
                IObservedChange<TSender, T3>,
                IObservedChange<TSender, T4>,
                IObservedChange<TSender, T5>,
                IObservedChange<TSender, T6>,
                IObservedChange<TSender, T7>,
                IObservedChange<TSender, T8>,
                IObservedChange<TSender, T9>,
                IObservedChange<TSender, T10>,
                IObservedChange<TSender, T11>,
                IObservedChange<TSender, T12>,
                TRet> selector,
            bool isDistinct) =>
            new WhenAnyChangeSink<
                IObservedChange<TSender, T1>,
                IObservedChange<TSender, T2>,
                IObservedChange<TSender, T3>,
                IObservedChange<TSender, T4>,
                IObservedChange<TSender, T5>,
                IObservedChange<TSender, T6>,
                IObservedChange<TSender, T7>,
                IObservedChange<TSender, T8>,
                IObservedChange<TSender, T9>,
                IObservedChange<TSender, T10>,
                IObservedChange<TSender, T11>,
                IObservedChange<TSender, T12>,
                TRet>(
                sender!.ObservableForProperty<TSender, T1>(property1Name, false, false, isDistinct),
                sender!.ObservableForProperty<TSender, T2>(property2Name, false, false, isDistinct),
                sender!.ObservableForProperty<TSender, T3>(property3Name, false, false, isDistinct),
                sender!.ObservableForProperty<TSender, T4>(property4Name, false, false, isDistinct),
                sender!.ObservableForProperty<TSender, T5>(property5Name, false, false, isDistinct),
                sender!.ObservableForProperty<TSender, T6>(property6Name, false, false, isDistinct),
                sender!.ObservableForProperty<TSender, T7>(property7Name, false, false, isDistinct),
                sender!.ObservableForProperty<TSender, T8>(property8Name, false, false, isDistinct),
                sender!.ObservableForProperty<TSender, T9>(property9Name, false, false, isDistinct),
                sender!.ObservableForProperty<TSender, T10>(property10Name, false, false, isDistinct),
                sender!.ObservableForProperty<TSender, T11>(property11Name, false, false, isDistinct),
                sender!.ObservableForProperty<TSender, T12>(property12Name, false, false, isDistinct),
                selector);

        /// <summary>Observes several dynamically-typed property chains and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="property3">An expression pointing to property 3.</param>
        /// <param name="property4">An expression pointing to property 4.</param>
        /// <param name="property5">An expression pointing to property 5.</param>
        /// <param name="property6">An expression pointing to property 6.</param>
        /// <param name="property7">An expression pointing to property 7.</param>
        /// <param name="property8">An expression pointing to property 8.</param>
        /// <param name="property9">An expression pointing to property 9.</param>
        /// <param name="property10">An expression pointing to property 10.</param>
        /// <param name="property11">An expression pointing to property 11.</param>
        /// <param name="property12">An expression pointing to property 12.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyDynamic<TRet>(
            Expression? property1,
            Expression? property2,
            Expression? property3,
            Expression? property4,
            Expression? property5,
            Expression? property6,
            Expression? property7,
            Expression? property8,
            Expression? property9,
            Expression? property10,
            Expression? property11,
            Expression? property12,
            Func<
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                TRet> selector) =>
            new WhenAnyChangeSink<
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                TRet>(
                sender.SubscribeToExpressionChain<TSender, object?>(property1, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property2, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property3, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property4, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property5, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property6, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property7, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property8, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property9, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property10, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property11, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property12, false, false),
                selector);

        /// <summary>Observes several dynamically-typed property chains and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="property3">An expression pointing to property 3.</param>
        /// <param name="property4">An expression pointing to property 4.</param>
        /// <param name="property5">An expression pointing to property 5.</param>
        /// <param name="property6">An expression pointing to property 6.</param>
        /// <param name="property7">An expression pointing to property 7.</param>
        /// <param name="property8">An expression pointing to property 8.</param>
        /// <param name="property9">An expression pointing to property 9.</param>
        /// <param name="property10">An expression pointing to property 10.</param>
        /// <param name="property11">An expression pointing to property 11.</param>
        /// <param name="property12">An expression pointing to property 12.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyDynamic<TRet>(
            Expression? property1,
            Expression? property2,
            Expression? property3,
            Expression? property4,
            Expression? property5,
            Expression? property6,
            Expression? property7,
            Expression? property8,
            Expression? property9,
            Expression? property10,
            Expression? property11,
            Expression? property12,
            Func<
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                TRet> selector,
            bool isDistinct) =>
            new WhenAnyChangeSink<
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                TRet>(
                sender.SubscribeToExpressionChain<TSender, object?>(property1, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property2, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property3, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property4, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property5, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property6, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property7, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property8, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property9, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property10, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property11, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property12, false, false, isDistinct),
                selector);
    }
}
