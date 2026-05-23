// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>Provides the arity-4 WhenAny / WhenAnyValue / WhenAnyDynamic extension overloads.</summary>
public static partial class WhenAnyMixin
{
    /// <summary>
    /// Observes several properties and projects their values into a tuple.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="T1">The type of property 1.</typeparam>
    /// <typeparam name="T2">The type of property 2.</typeparam>
    /// <typeparam name="T3">The type of property 3.</typeparam>
    /// <typeparam name="T4">The type of property 4.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    /// <param name="property1">An expression pointing to property 1.</param>
    /// <param name="property2">An expression pointing to property 2.</param>
    /// <param name="property3">An expression pointing to property 3.</param>
    /// <param name="property4">An expression pointing to property 4.</param>
    /// <returns>An observable that emits a tuple of the observed property values.</returns>
    public static IObservable<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)> WhenAnyValue<TSender, T1, T2, T3, T4>(
        this TSender? sender,
        Expression<Func<TSender, T1>> property1,
        Expression<Func<TSender, T2>> property2,
        Expression<Func<TSender, T3>> property3,
        Expression<Func<TSender, T4>> property4) =>
        sender!.WhenAny(
            property1,
            property2,
            property3,
            property4,
            (c1, c2, c3, c4) => (c1.Value, c2.Value, c3.Value, c4.Value));

    /// <summary>
    /// AOT-friendly overload that observes properties by name and projects a tuple.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="T1">The type of property 1.</typeparam>
    /// <typeparam name="T2">The type of property 2.</typeparam>
    /// <typeparam name="T3">The type of property 3.</typeparam>
    /// <typeparam name="T4">The type of property 4.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    /// <param name="property1Name">The name of property 1.</param>
    /// <param name="property2Name">The name of property 2.</param>
    /// <param name="property3Name">The name of property 3.</param>
    /// <param name="property4Name">The name of property 4.</param>
    /// <returns>An observable that emits a tuple of the observed property values.</returns>
    public static IObservable<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)> WhenAnyValue<TSender, T1, T2, T3, T4>(
        this TSender? sender,
        string property1Name,
        string property2Name,
        string property3Name,
        string property4Name)
    {
        var o1 = sender!.ObservableForProperty<TSender, T1>(property1Name, beforeChange: false, skipInitial: false, isDistinct: true);
        var o2 = sender!.ObservableForProperty<TSender, T2>(property2Name, beforeChange: false, skipInitial: false, isDistinct: true);
        var o3 = sender!.ObservableForProperty<TSender, T3>(property3Name, beforeChange: false, skipInitial: false, isDistinct: true);
        var o4 = sender!.ObservableForProperty<TSender, T4>(property4Name, beforeChange: false, skipInitial: false, isDistinct: true);
        return new WhenAnyValueSink<TSender, T1, T2, T3, T4, (T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(
            o1,
            o2,
            o3,
            o4,
            static (v1, v2, v3, v4) => (v1, v2, v3, v4));
    }

    /// <summary>
    /// Observes several properties and projects their values into a tuple.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="T1">The type of property 1.</typeparam>
    /// <typeparam name="T2">The type of property 2.</typeparam>
    /// <typeparam name="T3">The type of property 3.</typeparam>
    /// <typeparam name="T4">The type of property 4.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    /// <param name="property1">An expression pointing to property 1.</param>
    /// <param name="property2">An expression pointing to property 2.</param>
    /// <param name="property3">An expression pointing to property 3.</param>
    /// <param name="property4">An expression pointing to property 4.</param>
    /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
    /// <returns>An observable that emits a tuple of the observed property values.</returns>
    public static IObservable<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)> WhenAnyValue<TSender, T1, T2, T3, T4>(
        this TSender? sender,
        Expression<Func<TSender, T1>> property1,
        Expression<Func<TSender, T2>> property2,
        Expression<Func<TSender, T3>> property3,
        Expression<Func<TSender, T4>> property4,
        bool isDistinct) =>
        sender!.WhenAny(
            property1,
            property2,
            property3,
            property4,
            (c1, c2, c3, c4) => (c1.Value, c2.Value, c3.Value, c4.Value),
            isDistinct);

    /// <summary>
    /// AOT-friendly overload that observes properties by name and projects a tuple.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="T1">The type of property 1.</typeparam>
    /// <typeparam name="T2">The type of property 2.</typeparam>
    /// <typeparam name="T3">The type of property 3.</typeparam>
    /// <typeparam name="T4">The type of property 4.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    /// <param name="property1Name">The name of property 1.</param>
    /// <param name="property2Name">The name of property 2.</param>
    /// <param name="property3Name">The name of property 3.</param>
    /// <param name="property4Name">The name of property 4.</param>
    /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
    /// <returns>An observable that emits a tuple of the observed property values.</returns>
    public static IObservable<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)> WhenAnyValue<TSender, T1, T2, T3, T4>(
        this TSender? sender,
        string property1Name,
        string property2Name,
        string property3Name,
        string property4Name,
        bool isDistinct)
    {
        var o1 = sender!.ObservableForProperty<TSender, T1>(property1Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
        var o2 = sender!.ObservableForProperty<TSender, T2>(property2Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
        var o3 = sender!.ObservableForProperty<TSender, T3>(property3Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
        var o4 = sender!.ObservableForProperty<TSender, T4>(property4Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
        return new WhenAnyValueSink<TSender, T1, T2, T3, T4, (T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(
            o1,
            o2,
            o3,
            o4,
            static (v1, v2, v3, v4) => (v1, v2, v3, v4));
    }

    /// <summary>
    /// Observes several properties and combines their values with a selector.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="TRet">The type of the resulting value.</typeparam>
    /// <typeparam name="T1">The type of property 1.</typeparam>
    /// <typeparam name="T2">The type of property 2.</typeparam>
    /// <typeparam name="T3">The type of property 3.</typeparam>
    /// <typeparam name="T4">The type of property 4.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    /// <param name="property1">An expression pointing to property 1.</param>
    /// <param name="property2">An expression pointing to property 2.</param>
    /// <param name="property3">An expression pointing to property 3.</param>
    /// <param name="property4">An expression pointing to property 4.</param>
    /// <param name="selector">Combines the observed property values into a result.</param>
    /// <returns>An observable that emits the projected result on each change.</returns>
    public static IObservable<TRet> WhenAnyValue<TSender, TRet, T1, T2, T3, T4>(
        this TSender? sender,
        Expression<Func<TSender, T1>> property1,
        Expression<Func<TSender, T2>> property2,
        Expression<Func<TSender, T3>> property3,
        Expression<Func<TSender, T4>> property4,
        Func<T1, T2, T3, T4, TRet> selector) =>
        sender!.WhenAny(
            property1,
            property2,
            property3,
            property4,
            (c1, c2, c3, c4) => selector(c1.Value, c2.Value, c3.Value, c4.Value));

    /// <summary>
    /// AOT-friendly overload that observes properties by name and combines them with a selector.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="TRet">The type of the resulting value.</typeparam>
    /// <typeparam name="T1">The type of property 1.</typeparam>
    /// <typeparam name="T2">The type of property 2.</typeparam>
    /// <typeparam name="T3">The type of property 3.</typeparam>
    /// <typeparam name="T4">The type of property 4.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    /// <param name="property1Name">The name of property 1.</param>
    /// <param name="property2Name">The name of property 2.</param>
    /// <param name="property3Name">The name of property 3.</param>
    /// <param name="property4Name">The name of property 4.</param>
    /// <param name="selector">Combines the observed property values into a result.</param>
    /// <returns>An observable that emits the projected result on each change.</returns>
    public static IObservable<TRet> WhenAnyValue<TSender, TRet, T1, T2, T3, T4>(
        this TSender? sender,
        string property1Name,
        string property2Name,
        string property3Name,
        string property4Name,
        Func<T1, T2, T3, T4, TRet> selector)
    {
        var o1 = sender!.ObservableForProperty<TSender, T1>(property1Name, beforeChange: false, skipInitial: false, isDistinct: true);
        var o2 = sender!.ObservableForProperty<TSender, T2>(property2Name, beforeChange: false, skipInitial: false, isDistinct: true);
        var o3 = sender!.ObservableForProperty<TSender, T3>(property3Name, beforeChange: false, skipInitial: false, isDistinct: true);
        var o4 = sender!.ObservableForProperty<TSender, T4>(property4Name, beforeChange: false, skipInitial: false, isDistinct: true);
        return new WhenAnyValueSink<TSender, T1, T2, T3, T4, TRet>(
            o1,
            o2,
            o3,
            o4,
            selector);
    }

    /// <summary>
    /// Observes several properties and combines their values with a selector.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="TRet">The type of the resulting value.</typeparam>
    /// <typeparam name="T1">The type of property 1.</typeparam>
    /// <typeparam name="T2">The type of property 2.</typeparam>
    /// <typeparam name="T3">The type of property 3.</typeparam>
    /// <typeparam name="T4">The type of property 4.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    /// <param name="property1">An expression pointing to property 1.</param>
    /// <param name="property2">An expression pointing to property 2.</param>
    /// <param name="property3">An expression pointing to property 3.</param>
    /// <param name="property4">An expression pointing to property 4.</param>
    /// <param name="selector">Combines the observed property values into a result.</param>
    /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
    /// <returns>An observable that emits the projected result on each change.</returns>
    public static IObservable<TRet> WhenAnyValue<TSender, TRet, T1, T2, T3, T4>(
        this TSender? sender,
        Expression<Func<TSender, T1>> property1,
        Expression<Func<TSender, T2>> property2,
        Expression<Func<TSender, T3>> property3,
        Expression<Func<TSender, T4>> property4,
        Func<T1, T2, T3, T4, TRet> selector,
        bool isDistinct) =>
        sender!.WhenAny(
            property1,
            property2,
            property3,
            property4,
            (c1, c2, c3, c4) => selector(c1.Value, c2.Value, c3.Value, c4.Value),
            isDistinct);

    /// <summary>
    /// AOT-friendly overload that observes properties by name and combines them with a selector.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="TRet">The type of the resulting value.</typeparam>
    /// <typeparam name="T1">The type of property 1.</typeparam>
    /// <typeparam name="T2">The type of property 2.</typeparam>
    /// <typeparam name="T3">The type of property 3.</typeparam>
    /// <typeparam name="T4">The type of property 4.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    /// <param name="property1Name">The name of property 1.</param>
    /// <param name="property2Name">The name of property 2.</param>
    /// <param name="property3Name">The name of property 3.</param>
    /// <param name="property4Name">The name of property 4.</param>
    /// <param name="selector">Combines the observed property values into a result.</param>
    /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
    /// <returns>An observable that emits the projected result on each change.</returns>
    public static IObservable<TRet> WhenAnyValue<TSender, TRet, T1, T2, T3, T4>(
        this TSender? sender,
        string property1Name,
        string property2Name,
        string property3Name,
        string property4Name,
        Func<T1, T2, T3, T4, TRet> selector,
        bool isDistinct)
    {
        var o1 = sender!.ObservableForProperty<TSender, T1>(property1Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
        var o2 = sender!.ObservableForProperty<TSender, T2>(property2Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
        var o3 = sender!.ObservableForProperty<TSender, T3>(property3Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
        var o4 = sender!.ObservableForProperty<TSender, T4>(property4Name, beforeChange: false, skipInitial: false, isDistinct: isDistinct);
        return new WhenAnyValueSink<TSender, T1, T2, T3, T4, TRet>(
            o1,
            o2,
            o3,
            o4,
            selector);
    }

    /// <summary>
    /// Observes several properties and combines their change notifications with a selector.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="TRet">The type of the resulting value.</typeparam>
    /// <typeparam name="T1">The type of property 1.</typeparam>
    /// <typeparam name="T2">The type of property 2.</typeparam>
    /// <typeparam name="T3">The type of property 3.</typeparam>
    /// <typeparam name="T4">The type of property 4.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    /// <param name="property1">An expression pointing to property 1.</param>
    /// <param name="property2">An expression pointing to property 2.</param>
    /// <param name="property3">An expression pointing to property 3.</param>
    /// <param name="property4">An expression pointing to property 4.</param>
    /// <param name="selector">Combines the observed change notifications into a result.</param>
    /// <returns>An observable that emits the projected result on each change.</returns>
    public static IObservable<TRet> WhenAny<TSender, TRet, T1, T2, T3, T4>(
        this TSender? sender,
        Expression<Func<TSender, T1>> property1,
        Expression<Func<TSender, T2>> property2,
        Expression<Func<TSender, T3>> property3,
        Expression<Func<TSender, T4>> property4,
        Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, TRet> selector) =>
        new WhenAnyChangeSink<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, TRet>(
            sender!.ObservableForProperty(property1, false, false),
            sender!.ObservableForProperty(property2, false, false),
            sender!.ObservableForProperty(property3, false, false),
            sender!.ObservableForProperty(property4, false, false),
            selector);

    /// <summary>
    /// AOT-friendly overload that observes properties by name and combines them with a selector.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="TRet">The type of the resulting value.</typeparam>
    /// <typeparam name="T1">The type of property 1.</typeparam>
    /// <typeparam name="T2">The type of property 2.</typeparam>
    /// <typeparam name="T3">The type of property 3.</typeparam>
    /// <typeparam name="T4">The type of property 4.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    /// <param name="property1Name">The name of property 1.</param>
    /// <param name="property2Name">The name of property 2.</param>
    /// <param name="property3Name">The name of property 3.</param>
    /// <param name="property4Name">The name of property 4.</param>
    /// <param name="selector">Combines the observed change notifications into a result.</param>
    /// <returns>An observable that emits the projected result on each change.</returns>
    public static IObservable<TRet> WhenAny<TSender, TRet, T1, T2, T3, T4>(
        this TSender? sender,
        string property1Name,
        string property2Name,
        string property3Name,
        string property4Name,
        Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, TRet> selector) =>
        new WhenAnyChangeSink<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, TRet>(
            sender!.ObservableForProperty<TSender, T1>(property1Name, false, false),
            sender!.ObservableForProperty<TSender, T2>(property2Name, false, false),
            sender!.ObservableForProperty<TSender, T3>(property3Name, false, false),
            sender!.ObservableForProperty<TSender, T4>(property4Name, false, false),
            selector);

    /// <summary>
    /// Observes several properties and combines their change notifications with a selector.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="TRet">The type of the resulting value.</typeparam>
    /// <typeparam name="T1">The type of property 1.</typeparam>
    /// <typeparam name="T2">The type of property 2.</typeparam>
    /// <typeparam name="T3">The type of property 3.</typeparam>
    /// <typeparam name="T4">The type of property 4.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    /// <param name="property1">An expression pointing to property 1.</param>
    /// <param name="property2">An expression pointing to property 2.</param>
    /// <param name="property3">An expression pointing to property 3.</param>
    /// <param name="property4">An expression pointing to property 4.</param>
    /// <param name="selector">Combines the observed change notifications into a result.</param>
    /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
    /// <returns>An observable that emits the projected result on each change.</returns>
    public static IObservable<TRet> WhenAny<TSender, TRet, T1, T2, T3, T4>(
        this TSender? sender,
        Expression<Func<TSender, T1>> property1,
        Expression<Func<TSender, T2>> property2,
        Expression<Func<TSender, T3>> property3,
        Expression<Func<TSender, T4>> property4,
        Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, TRet> selector,
        bool isDistinct) =>
        new WhenAnyChangeSink<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, TRet>(
            sender!.ObservableForProperty(property1, false, false, isDistinct),
            sender!.ObservableForProperty(property2, false, false, isDistinct),
            sender!.ObservableForProperty(property3, false, false, isDistinct),
            sender!.ObservableForProperty(property4, false, false, isDistinct),
            selector);

    /// <summary>
    /// AOT-friendly overload that observes properties by name and combines them with a selector.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="TRet">The type of the resulting value.</typeparam>
    /// <typeparam name="T1">The type of property 1.</typeparam>
    /// <typeparam name="T2">The type of property 2.</typeparam>
    /// <typeparam name="T3">The type of property 3.</typeparam>
    /// <typeparam name="T4">The type of property 4.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    /// <param name="property1Name">The name of property 1.</param>
    /// <param name="property2Name">The name of property 2.</param>
    /// <param name="property3Name">The name of property 3.</param>
    /// <param name="property4Name">The name of property 4.</param>
    /// <param name="selector">Combines the observed change notifications into a result.</param>
    /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
    /// <returns>An observable that emits the projected result on each change.</returns>
    public static IObservable<TRet> WhenAny<TSender, TRet, T1, T2, T3, T4>(
        this TSender? sender,
        string property1Name,
        string property2Name,
        string property3Name,
        string property4Name,
        Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, TRet> selector,
        bool isDistinct) =>
        new WhenAnyChangeSink<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, TRet>(
            sender!.ObservableForProperty<TSender, T1>(property1Name, false, false, isDistinct),
            sender!.ObservableForProperty<TSender, T2>(property2Name, false, false, isDistinct),
            sender!.ObservableForProperty<TSender, T3>(property3Name, false, false, isDistinct),
            sender!.ObservableForProperty<TSender, T4>(property4Name, false, false, isDistinct),
            selector);

    /// <summary>
    /// Observes several dynamically-typed property chains and combines them with a selector.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="TRet">The type of the resulting value.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    /// <param name="property1">An expression pointing to property 1.</param>
    /// <param name="property2">An expression pointing to property 2.</param>
    /// <param name="property3">An expression pointing to property 3.</param>
    /// <param name="property4">An expression pointing to property 4.</param>
    /// <param name="selector">Combines the observed change notifications into a result.</param>
    /// <returns>An observable that emits the projected result on each change.</returns>
    public static IObservable<TRet> WhenAnyDynamic<TSender, TRet>(
        this TSender? sender,
        Expression? property1,
        Expression? property2,
        Expression? property3,
        Expression? property4,
        Func<IObservedChange<TSender?, object?>, IObservedChange<TSender?, object?>, IObservedChange<TSender?, object?>, IObservedChange<TSender?, object?>, TRet> selector) =>
        new WhenAnyChangeSink<IObservedChange<TSender, object?>, IObservedChange<TSender, object?>, IObservedChange<TSender, object?>, IObservedChange<TSender, object?>, TRet>(
            sender.SubscribeToExpressionChain<TSender, object?>(property1, false, false),
            sender.SubscribeToExpressionChain<TSender, object?>(property2, false, false),
            sender.SubscribeToExpressionChain<TSender, object?>(property3, false, false),
            sender.SubscribeToExpressionChain<TSender, object?>(property4, false, false),
            selector);

    /// <summary>
    /// Observes several dynamically-typed property chains and combines them with a selector.
    /// </summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <typeparam name="TRet">The type of the resulting value.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    /// <param name="property1">An expression pointing to property 1.</param>
    /// <param name="property2">An expression pointing to property 2.</param>
    /// <param name="property3">An expression pointing to property 3.</param>
    /// <param name="property4">An expression pointing to property 4.</param>
    /// <param name="selector">Combines the observed change notifications into a result.</param>
    /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
    /// <returns>An observable that emits the projected result on each change.</returns>
    public static IObservable<TRet> WhenAnyDynamic<TSender, TRet>(
        this TSender? sender,
        Expression? property1,
        Expression? property2,
        Expression? property3,
        Expression? property4,
        Func<IObservedChange<TSender?, object?>, IObservedChange<TSender?, object?>, IObservedChange<TSender?, object?>, IObservedChange<TSender?, object?>, TRet> selector,
        bool isDistinct) =>
        new WhenAnyChangeSink<IObservedChange<TSender, object?>, IObservedChange<TSender, object?>, IObservedChange<TSender, object?>, IObservedChange<TSender, object?>, TRet>(
            sender.SubscribeToExpressionChain<TSender, object?>(property1, false, false, isDistinct),
            sender.SubscribeToExpressionChain<TSender, object?>(property2, false, false, isDistinct),
            sender.SubscribeToExpressionChain<TSender, object?>(property3, false, false, isDistinct),
            sender.SubscribeToExpressionChain<TSender, object?>(property4, false, false, isDistinct),
            selector);
}
