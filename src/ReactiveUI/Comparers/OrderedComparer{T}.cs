// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Convenience class providing a starting point for chaining comparers.
/// </summary>
/// <typeparam name="T">The comparison type.</typeparam>
public static class OrderedComparer<T>
{
    /// <summary>
    /// Creates a comparer that will sort elements in ascending order based on the values returned by the provided
    /// selector. The values will be compared using the default comparer for the return type of the selector.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="selector">
    /// A function supplying the values for the comparer.
    /// </param>
    /// <returns>A comparer.</returns>
    public static IComparer<T> OrderBy<TValue>(Func<T, TValue> selector) =>
        ComparerChainingExtensions.ThenBy(null, selector);

    /// <summary>
    /// Creates a comparer that will sort elements in ascending order based on the values returned by the provided
    /// selector. The selector values will be compared using the provided comparer or the default comparer for the
    /// return type of the selector if no comparer is specified.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="selector">
    /// A function supplying the values for the comparer.
    /// </param>
    /// <param name="comparer">
    /// The comparer to use when comparing the values returned by the selector.
    /// The default comparer for that type will be used if this parameter is null.
    /// </param>
    /// <returns>A comparer.</returns>
    public static IComparer<T> OrderBy<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer) =>
        ComparerChainingExtensions.ThenBy(null, selector, comparer);

    /// <summary>
    /// Creates a comparer that will sort elements in descending order based on the values returned by the provided
    /// selector. The values will be compared using the default comparer for the return type of the selector.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="selector">
    /// A function supplying the values for the comparer.
    /// </param>
    /// <returns>A comparer.</returns>
    public static IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector) =>
        ComparerChainingExtensions.ThenByDescending(null, selector);

    /// <summary>
    /// Creates a comparer that will sort elements in descending order based on the values returned by the provided
    /// selector. The selector values will be compared using the provided comparer or the default comparer for the
    /// return type of the selector if no comparer is specified.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="selector">
    /// A function supplying the values for the comparer.
    /// </param>
    /// <param name="comparer">
    /// The comparer to use when comparing the values returned by the selector.
    /// The default comparer for that type will be used if this parameter is null.
    /// </param>
    /// <returns>A comparer.</returns>
    public static IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer) =>
        ComparerChainingExtensions.ThenByDescending(null, selector, comparer);
}
