// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Convenience class to help chain selectors onto existing parent comparers.
/// </summary>
public static class ComparerChainingExtensions
{
    /// <summary>
    /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements
    /// using the parent comparer first. If the parent considers the values equal elements will be sorted
    /// in ascending order based on the values returned by the provided selector. The selector values will be
    /// compared using the default comparer for the return type of the selector.
    /// </summary>
    /// <typeparam name="T">The comparison type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="parent">
    /// The parent comparer to use first.
    /// </param>
    /// <param name="selector">
    /// A function supplying the values for the comparer.
    /// </param>
    /// <returns>A comparer.</returns>
    public static IComparer<T> ThenBy<T, TValue>(this IComparer<T>? parent, Func<T, TValue> selector) => ThenBy(parent, selector, Comparer<TValue>.Default);

    /// <summary>
    /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements
    /// using the parent comparer first. If the parent considers the values equal elements will be sorted
    /// in ascending order based on the values returned by the provided selector. The selector values will be
    /// compared using the provided comparer or the default comparer for the return type of the selector if no
    /// comparer is specified.
    /// </summary>
    /// <typeparam name="T">The comparison type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="parent">
    /// The parent comparer to use first.
    /// </param>
    /// <param name="selector">
    /// A function supplying the values for the comparer.
    /// </param>
    /// <param name="comparer">
    /// The comparer to use when comparing the values returned by the selector.
    /// </param>
    /// <returns>A comparer.</returns>
    public static IComparer<T> ThenBy<T, TValue>(this IComparer<T>? parent, Func<T, TValue> selector, IComparer<TValue> comparer) => new ChainedComparer<T>(parent, (x, y) => comparer.Compare(selector(x), selector(y)));

    /// <summary>
    /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements
    /// using the parent comparer first. If the parent considers the values equal elements will be sorted
    /// in descending order based on the values returned by the provided selector. The selector values will be
    /// compared using the default comparer for the return type of the selector.
    /// </summary>
    /// <typeparam name="T">The comparison type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="parent">
    /// The parent comparer to use first.
    /// </param>
    /// <param name="selector">
    /// A function supplying the values for the comparer.
    /// </param>
    /// <returns>A comparer.</returns>
    public static IComparer<T> ThenByDescending<T, TValue>(this IComparer<T>? parent, Func<T, TValue> selector) => ThenByDescending(parent, selector, Comparer<TValue>.Default);

    /// <summary>
    /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements
    /// using the parent comparer first. If the parent considers the values equal elements will be sorted
    /// in descending order based on the values returned by the provided selector. The selector values will be
    /// compared using the provided comparer or the default comparer for the return type of the selector if no
    /// comparer is specified.
    /// </summary>
    /// <typeparam name="T">The comparison type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="parent">
    /// The parent comparer to use first.
    /// </param>
    /// <param name="selector">
    /// A function supplying the values for the comparer.
    /// </param>
    /// <param name="comparer">
    /// The comparer to use when comparing the values returned by the selector.
    /// </param>
    /// <returns>A comparer.</returns>
    public static IComparer<T> ThenByDescending<T, TValue>(this IComparer<T>? parent, Func<T, TValue> selector, IComparer<TValue> comparer) => new ChainedComparer<T>(parent, (x, y) => -comparer.Compare(selector(x), selector(y)));
}
