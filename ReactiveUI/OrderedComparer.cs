using System;
using System.Collections.Generic;

namespace ReactiveUI
{
    /// <summary>
    /// Convienience class providing a starting point for chaining comparers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class OrderedComparer<T>
    {
        /// <summary>
        /// Creates a comparer that will sort elements in ascending order based on the values returned by the provided
        /// selector. The values will be compared using the default comparer for the return type of the selector.
        /// </summary>
        /// <param name="selector">A function supplying the values for the comparator.</param>
        public static IComparer<T> OrderBy<TValue>(Func<T,TValue> selector)
        {
            return ComparerChainingExtensions.ThenBy<T, TValue>(null, selector);
        }

        /// <summary>
        /// Creates a comparer that will sort elements in ascending order based on the values returned by the provided
        /// selector. The selector values will be compared using the provided comparer or the default comparer for the 
        /// return type of the selector if no comparer is specified.
        /// </summary>
        /// <param name="selector">A function supplying the values for the comparator.</param>
        /// <param name="comparer">
        /// The comparer to use when comparing the values returned by the selector. 
        /// The default comparer for that type will be used if this parameter is null.
        /// </param>
        public static IComparer<T> OrderBy<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer)
        {
            return ComparerChainingExtensions.ThenBy<T, TValue>(null, selector, comparer);
        }

        /// <summary>
        /// Creates a comparer that will sort elements in descending order based on the values returned by the provided
        /// selector. The values will be compared using the default comparer for the return type of the selector.
        /// </summary>
        /// <param name="selector">A function supplying the values for the comparator.</param>
        public static IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector)
        {
            return ComparerChainingExtensions.ThenByDescending<T, TValue>(null, selector);
        }

        /// <summary>
        /// Creates a comparer that will sort elements in descending order based on the values returned by the provided
        /// selector. The selector values will be compared using the provided comparer or the default comparer for the 
        /// return type of the selector if no comparer is specified.
        /// </summary>
        /// <param name="selector">A function supplying the values for the comparator.</param>
        /// <param name="comparer">
        /// The comparer to use when comparing the values returned by the selector. 
        /// The default comparer for that type will be used if this parameter is null.
        /// </param>
        public static IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer)
        {
            return ComparerChainingExtensions.ThenByDescending<T, TValue>(null, selector, comparer);
        }
    }

    public static class ComparerChainingExtensions
    {
        /// <summary>
        /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements 
        /// using the parent comparer first. If the parent considers the values equal elements will be sorted 
        /// in ascending order based on the values returned by the provided selector. The selector values will be 
        /// compared using the default comparer for the return type of the selector.
        /// </summary>
        /// <param name="selector">A function supplying the values for the comparator.</param>
        public static IComparer<T> ThenBy<T, TValue>(this IComparer<T> parent, Func<T, TValue> selector)
        {
            return ThenBy(parent, selector, Comparer<TValue>.Default);
        }

        /// <summary>
        /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements 
        /// using the parent comparer first. If the parent considers the values equal elements will be sorted 
        /// in ascending order based on the values returned by the provided selector. The selector values will be 
        /// compared using the provided comparer or the default comparer for the return type of the selector if no 
        /// comparer is specified.
        /// </summary>
        /// <param name="selector">A function supplying the values for the comparator.</param>
        public static IComparer<T> ThenBy<T, TValue>(this IComparer<T> parent, Func<T, TValue> selector, IComparer<TValue> comparer)
        {
            return new ChainedComparer<T>(parent, (x, y) => comparer.Compare(selector(x), selector(y)));
        }

        /// <summary>
        /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements 
        /// using the parent comparer first. If the parent considers the values equal elements will be sorted 
        /// in descending order based on the values returned by the provided selector. The selector values will be 
        /// compared using the default comparer for the return type of the selector.
        /// </summary>
        /// <param name="selector">A function supplying the values for the comparator.</param>
        public static IComparer<T> ThenByDescending<T, TValue>(this IComparer<T> parent, Func<T, TValue> selector)
        {
            return ThenByDescending(parent, selector, Comparer<TValue>.Default);
        }

        /// <summary>
        /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements 
        /// using the parent comparer first. If the parent considers the values equal elements will be sorted 
        /// in descending order based on the values returned by the provided selector. The selector values will be 
        /// compared using the provided comparer or the default comparer for the return type of the selector if no 
        /// comparer is specified.
        /// </summary>
        /// <param name="selector">A function supplying the values for the comparator.</param>
        public static IComparer<T> ThenByDescending<T, TValue>(this IComparer<T> parent, Func<T, TValue> selector, IComparer<TValue> comparer)
        {
            return new ChainedComparer<T>(parent, (x, y) => -comparer.Compare(selector(x), selector(y)));
        }
    }

    internal sealed class ChainedComparer<T> : IComparer<T>
    {
        private IComparer<T> parent;
        private Comparison<T> inner;

        public ChainedComparer(IComparer<T> parent, Comparison<T> comparison)
        {
            if (comparison == null)
                throw new ArgumentNullException("comparison");

            this.parent = parent;
            this.inner = comparison;
        }

        public int Compare(T x, T y)
        {
            int parentResult = parent == null ? 0 : parent.Compare(x, y);

            return parentResult != 0 ? parentResult : inner(x, y);
        }
    }
}
