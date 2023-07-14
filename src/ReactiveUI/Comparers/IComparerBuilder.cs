// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Convenience interface for providing a starting point for chaining comparers.
/// </summary>
/// <typeparam name="T">The comparison type.</typeparam>
public interface IComparerBuilder<T>
{
    /// <summary>
    /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements
    /// using the parent comparer first. If the parent considers the values equal elements will be sorted
    /// in ascending order based on the values returned by the provided selector. The selector values will be
    /// compared using the default comparer for the return type of the selector.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="selector">
    /// A function supplying the values for the comparer.
    /// </param>
    /// <returns>A comparer.</returns>
    IComparer<T> OrderBy<TValue>(Func<T, TValue> selector);

    /// <summary>
    /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements
    /// using the parent comparer first. If the parent considers the values equal elements will be sorted
    /// in ascending order based on the values returned by the provided selector. The selector values will be
    /// compared using the provided comparer or the default comparer for the return type of the selector if no
    /// comparer is specified.
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
    IComparer<T> OrderBy<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer);

    /// <summary>
    /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements
    /// using the parent comparer first. If the parent considers the values equal elements will be sorted
    /// in descending order based on the values returned by the provided selector. The selector values will be
    /// compared using the default comparer for the return type of the selector.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="selector">
    /// A function supplying the values for the comparer.
    /// </param>
    /// <returns>A comparer.</returns>
    IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector);

    /// <summary>
    /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements
    /// using the parent comparer first. If the parent considers the values equal elements will be sorted
    /// in descending order based on the values returned by the provided selector. The selector values will be
    /// compared using the provided comparer or the default comparer for the return type of the selector if no
    /// comparer is specified.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="selector">
    /// A function supplying the values for the comparer.
    /// </param>
    /// /// <param name="comparer">
    /// The comparer to use when comparing the values returned by the selector.
    /// The default comparer for that type will be used if this parameter is null.
    /// </param>
    /// <returns>A comparer.</returns>
    IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer);
}