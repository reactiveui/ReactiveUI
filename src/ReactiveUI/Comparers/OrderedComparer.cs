// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Convenience class providing a starting point for chaining comparers for anonymous types.
/// </summary>
/// <remarks>
/// If the type you're creating a comparer for is known this class is nothing more than an alias for the generic
/// OrderedComparer. This class can be used to create comparers for anonymous types.
/// </remarks>
public static class OrderedComparer
{
    /// <summary>
    /// Creates a type inferred comparer builder for the element type of the enumerable. Useful for creating
    /// comparers for anonymous types. Note that the builder is not a comparer in itself, you need to use the
    /// OrderBy or OrderByDescending methods on the builder to get an actual comparer.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>A comparer builder.</returns>
    public static IComparerBuilder<T> For<T>(IEnumerable<T> enumerable) => For<T>();

    /// <summary>
    /// Creates a comparer builder for the specified type. Note that the builder is not a comparer in itself,
    /// you need to use the OrderBy or OrderByDescending methods on the builder to get an actual comparer.
    /// If the type is known at compile time this method is nothing more than an alias for the generic
    /// OrderedComparer class.
    /// </summary>
    /// <typeparam name="T">The comparison type.</typeparam>
    /// <returns>A comparer builder.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public static IComparerBuilder<T> For<T>() => OrderedComparerTypeWrapper<T>.Instance;

    /// <summary>
    /// Provides a type-safe wrapper for building ordered comparers for elements of type <typeparamref name="T"/>.
    /// Implements the <see cref="IComparerBuilder{T}"/> interface to enable fluent construction of comparison logic.
    /// </summary>
    /// <remarks>This class is intended for internal use to facilitate the creation of ordered comparers using
    /// the <see cref="IComparerBuilder{T}"/> interface. It exposes static methods for ordering elements by specified
    /// keys, supporting both ascending and descending order, with optional custom comparers.</remarks>
    /// <typeparam name="T">The type of elements to compare.</typeparam>
    private sealed class OrderedComparerTypeWrapper<T> : IComparerBuilder<T>
    {
        /// <summary>
        /// The singleton instance of this wrapper used to provide the IComparerBuilder for type T.
        /// </summary>
        public static readonly OrderedComparerTypeWrapper<T> Instance = new();

        /// <summary>
        /// Creates a comparer that orders elements based on a key extracted from each element using the specified
        /// selector function.
        /// </summary>
        /// <remarks>Use this method to define custom ordering for elements by specifying a key selector.
        /// The resulting comparer can be used with sorting methods or data structures that accept an
        /// IComparer{T}.</remarks>
        /// <typeparam name="TValue">The type of the key returned by the selector function.</typeparam>
        /// <param name="selector">A function that extracts the key from an element to use for ordering. Cannot be null.</param>
        /// <returns>An IComparer{T} that compares elements according to the values returned by the selector function.</returns>
        public IComparer<T> OrderBy<TValue>(Func<T, TValue> selector) => OrderedComparer<T>.OrderBy(selector);

        /// <summary>
        /// Creates a comparer that orders elements by a specified key using the provided key selector and comparer.
        /// </summary>
        /// <remarks>Use this method to perform custom ordering of elements based on a key. This is
        /// typically used in conjunction with sorting operations or when building composite comparers.</remarks>
        /// <typeparam name="TValue">The type of the key returned by the selector function.</typeparam>
        /// <param name="selector">A function that extracts the key from an element to use for ordering. Cannot be null.</param>
        /// <param name="comparer">An object that compares keys for ordering. If null, the default comparer for the key type is used.</param>
        /// <returns>An IComparer{T} that compares elements based on the specified key and comparer.</returns>
        public IComparer<T> OrderBy<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer) =>
            OrderedComparer<T>.OrderBy(selector, comparer);

        /// <summary>
        /// Creates a comparer that orders elements in descending order according to a specified key selector.
        /// </summary>
        /// <remarks>Use this method to sort elements in descending order by a specific property or value.
        /// The comparer can be used with sorting methods that accept an IComparer{T}.</remarks>
        /// <typeparam name="TValue">The type of the key returned by the selector function.</typeparam>
        /// <param name="selector">A function to extract the key from an element for comparison. Cannot be null.</param>
        /// <returns>An IComparer{T} that compares elements based on the descending order of the selected key.</returns>
        public IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector) =>
            OrderedComparer<T>.OrderByDescending(selector);

        /// <summary>
        /// Creates a comparer that orders elements in descending order according to a specified key selector and
        /// comparer.
        /// </summary>
        /// <typeparam name="TValue">The type of the key returned by the selector function.</typeparam>
        /// <param name="selector">A function that extracts the key from an element to determine its order. Cannot be null.</param>
        /// <param name="comparer">An optional comparer to use for comparing keys. If null, the default comparer for the key type is used.</param>
        /// <returns>An IComparer{T} that compares elements in descending order based on the specified key and comparer.</returns>
        public IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer) =>
            OrderedComparer<T>.OrderByDescending(selector, comparer);
    }
}
