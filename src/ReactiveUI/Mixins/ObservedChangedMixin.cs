// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace ReactiveUI;

/// <summary>
/// A collection of helpers for <see cref="IObservedChange{TSender, TValue}"/>.
/// </summary>
public static class ObservedChangedMixin
{
    /// <summary>
    /// Returns the name of a property which has been changed.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="item">The observed change.</param>
    /// <returns>
    /// The name of the property which has changed.
    /// </returns>
    public static string GetPropertyName<TSender, TValue>(this IObservedChange<TSender, TValue> item) =>
        item is null
            ? throw new ArgumentNullException(nameof(item))
            : Reflection.ExpressionToPropertyNames(item.Expression);

    /// <summary>
    /// Returns the current value of a property given a notification that
    /// it has changed.
    /// </summary>
    /// <typeparam name="TSender">The sender.</typeparam>
    /// <typeparam name="TValue">The changed value.</typeparam>
    /// <param name="item">
    /// The <see cref="IObservedChange{TSender, TValue}"/> instance to get the value of.
    /// </param>
    /// <returns>
    /// The current value of the property.
    /// </returns>
    public static TValue GetValue<TSender, TValue>(this IObservedChange<TSender, TValue> item) =>
        item is null
            ? throw new ArgumentNullException(nameof(item))
            : !item.TryGetValue(out var returnValue)
                ? throw new Exception($"One of the properties in the expression '{item.GetPropertyName()}' was null")
                : returnValue;

    /// <summary>
    /// Returns the current value of a property given a notification that
    /// it has changed.
    /// </summary>
    /// <typeparam name="TSender">The sender.</typeparam>
    /// <typeparam name="TValue">The changed value.</typeparam>
    /// <param name="item">
    /// The <see cref="IObservedChange{TSender, TValue}"/> instance to get the value of.
    /// </param>
    /// <returns>
    /// The current value of the property.
    /// </returns>
    public static TValue? GetValueOrDefault<TSender, TValue>(this IObservedChange<TSender, TValue> item) => // TODO: Create Test
        item is null ? throw new ArgumentNullException(nameof(item)) : !item.TryGetValue(out var returnValue) ? default : returnValue;

    /// <summary>
    /// Given a stream of notification changes, this method will convert
    /// the property changes to the current value of the property.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="item">
    /// The change notification stream to get the values of.
    /// </param>
    /// <returns>
    /// An Observable representing the stream of current values of
    /// the given change notification stream.
    /// </returns>
    public static IObservable<TValue> Value<TSender, TValue>(this IObservable<IObservedChange<TSender, TValue>> item) => // TODO: Create Test
        item.Select(GetValue);

    /// <summary>
    /// Attempts to return the current value of a property given a
    /// notification that it has changed. If any property in the
    /// property expression is null, false is returned.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="item">
    /// The <see cref="IObservedChange{TSender, TValue}"/> instance to get the value of.
    /// </param>
    /// <param name="changeValue">
    /// The value of the property expression.
    /// </param>
    /// <returns>
    /// True if the entire expression was able to be followed, false otherwise.
    /// </returns>
    internal static bool TryGetValue<TSender, TValue>(this IObservedChange<TSender, TValue> item, out TValue changeValue)
    {
        if (!Equals(item.Value, default(TValue)))
        {
            changeValue = item.Value;
            return true;
        }

        return Reflection.TryGetValueForPropertyChain(out changeValue, item.Sender, item.Expression!.GetExpressionChain());
    }

    /// <summary>
    /// Given a fully filled-out IObservedChange object, SetValueToProperty
    /// will apply it to the specified object (i.e. it will ensure that
    /// target.property == This.GetValue() and "replay" the observed change
    /// onto another object).
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TTarget">The target type.</typeparam>
    /// <param name="item">
    /// The <see cref="IObservedChange{TSender, TValue}"/> instance to use as a
    /// value to apply.
    /// </param>
    /// <param name="target">
    /// The target object to apply the change to.
    /// </param>
    /// <param name="property">
    /// The target property to apply the change to.
    /// </param>
    internal static void SetValueToProperty<TSender, TValue, TTarget>(
        this IObservedChange<TSender, TValue> item,
        TTarget target,
        Expression<Func<TTarget, TValue>> property)
    {
        if (target is not null)
        {
            Reflection.TrySetValueToPropertyChain(target, Reflection.Rewrite(property.Body).GetExpressionChain(), item.GetValue());
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
