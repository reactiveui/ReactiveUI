// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI
{
    /// <summary>
    /// Convenience class providing a starting point for chaining comparers for anonymous types.
    /// </summary>
    /// <remarks>
    /// If the type you're creating a comparer for is known this class is nothing more than an alias for the generic
    /// OrderedComparer. This class can be used to create comparers for anonymous types.
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
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
        [SuppressMessage("Microsoft.Performance", "CA1801", Justification = "Existing API")]
        public static IComparerBuilder<T> For<T>(IEnumerable<T> enumerable) => For<T>();

        /// <summary>
        /// Creates a comparer builder for the specified type. Note that the builder is not a comparer in itself,
        /// you need to use the OrderBy or OrderByDescending methods on the builder to get an actual comparer.
        /// If the type is known at compile time this method is nothing more than an alias for the generic
        /// OrderedComparer class.
        /// </summary>
        /// <typeparam name="T">The comparison type.</typeparam>
        /// <returns>A comparer builder.</returns>
        public static IComparerBuilder<T> For<T>() => OrderedComparerTypeWrapper<T>.Instance;

        private sealed class OrderedComparerTypeWrapper<T> : IComparerBuilder<T>
        {
            public static readonly OrderedComparerTypeWrapper<T> Instance = new();

            public IComparer<T> OrderBy<TValue>(Func<T, TValue> selector) => OrderedComparer<T>.OrderBy(selector);

            public IComparer<T> OrderBy<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer) => OrderedComparer<T>.OrderBy(selector, comparer);

            public IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector) => OrderedComparer<T>.OrderByDescending(selector);

            public IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer) => OrderedComparer<T>.OrderByDescending(selector, comparer);
        }
    }

    /// <summary>
    /// Convenience class providing a starting point for chaining comparers.
    /// </summary>
    /// <typeparam name="T">The comparison type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
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
        public static IComparer<T> OrderBy<TValue>(Func<T, TValue> selector) => ComparerChainingExtensions.ThenBy(null, selector);

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
        public static IComparer<T> OrderBy<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer) => ComparerChainingExtensions.ThenBy(null, selector, comparer);

        /// <summary>
        /// Creates a comparer that will sort elements in descending order based on the values returned by the provided
        /// selector. The values will be compared using the default comparer for the return type of the selector.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="selector">
        /// A function supplying the values for the comparer.
        /// </param>
        /// <returns>A comparer.</returns>
        public static IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector) => ComparerChainingExtensions.ThenByDescending(null, selector);

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
        public static IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer) => ComparerChainingExtensions.ThenByDescending(null, selector, comparer);
    }
}
