using System;
using System.Collections.Generic;

namespace ReactiveUI
{
    /// <summary>
    /// Convienience interface for providing a starting point for chaining comparers.
    /// </summary>
    public interface IComparerBuilder<T>
    {
        /// <summary>
        /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements
        /// using the parent comparer first. If the parent considers the values equal elements will be sorted
        /// in ascending order based on the values returned by the provided selector. The selector values will be
        /// compared using the default comparer for the return type of the selector.
        /// </summary>
        /// <param name="selector">
        /// A function supplying the values for the comparer.
        /// </param>
        IComparer<T> OrderBy<TValue>(Func<T, TValue> selector);

        /// <summary>
        /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements
        /// using the parent comparer first. If the parent considers the values equal elements will be sorted
        /// in ascending order based on the values returned by the provided selector. The selector values will be
        /// compared using the provided comparer or the default comparer for the return type of the selector if no
        /// comparer is specified.
        /// </summary>
        /// <param name="selector">
        /// A function supplying the values for the comparer.
        /// </param>
        /// <param name="comparer">
        /// The comparer to use when comparing the values returned by the selector. 
        /// The default comparer for that type will be used if this parameter is null.
        /// </param>
        IComparer<T> OrderBy<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer);

        /// <summary>
        /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements
        /// using the parent comparer first. If the parent considers the values equal elements will be sorted
        /// in descending order based on the values returned by the provided selector. The selector values will be
        /// compared using the default comparer for the return type of the selector.
        /// </summary>
        /// <param name="selector">
        /// A function supplying the values for the comparer.
        /// </param>
        IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector);

        /// <summary>
        /// Creates a derived comparer based on the given parent comparer. The returned comparer will sort elements
        /// using the parent comparer first. If the parent considers the values equal elements will be sorted
        /// in descending order based on the values returned by the provided selector. The selector values will be
        /// compared using the provided comparer or the default comparer for the return type of the selector if no
        /// comparer is specified.
        /// </summary>
        /// <param name="selector">
        /// A function supplying the values for the comparer.
        /// </param>
        /// /// <param name="comparer">
        /// The comparer to use when comparing the values returned by the selector. 
        /// The default comparer for that type will be used if this parameter is null.
        /// </param>
        IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer);
    }

    /// <summary>
    /// Convienience class providing a starting point for chaining comparers for anonymous types.
    /// </summary>
    /// <remarks>
    /// If the type you're creating a comparer for is known this class is nothing more than an alias for the generic
    /// OrderedComparer. This class can be used to create comparers for anonymous types
    /// </remarks>
    public static class OrderedComparer
    {
        private sealed class OrderedComparerTypeWrapper<T> : IComparerBuilder<T>
        {
            public static readonly OrderedComparerTypeWrapper<T> Instance = new OrderedComparerTypeWrapper<T>();

            public IComparer<T> OrderBy<TValue>(Func<T, TValue> selector)
            {
                return OrderedComparer<T>.OrderBy(selector);
            }

            public IComparer<T> OrderBy<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer)
            {
                return OrderedComparer<T>.OrderBy(selector, comparer);
            }

            public IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector)
            {
                return OrderedComparer<T>.OrderByDescending(selector);
            }

            public IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer)
            {
                return OrderedComparer<T>.OrderByDescending(selector, comparer);
            }
        }

        /// <summary>
        /// Creates a type inferred comparer builder for the element type of the enumerable. Useful for creating
        /// comparers for anonymous types. Note that the builder is not a comparer in itself, you need to use the
        /// OrderBy or OrderByDescending methods on the builder to get an actual comparer.
        /// </summary>
        public static IComparerBuilder<T> For<T>(IEnumerable<T> enumerable)
        {
            return For<T>();
        }

        /// <summary>
        /// Creates a comparer builder for the specified type. Note that the builder is not a comparer in itself,
        /// you need to use the OrderBy or OrderByDescending methods on the builder to get an actual comparer.
        /// If the type is known at compile time this method is nothing more than an alias for the generic
        /// OrdedComparer class.
        /// </summary>
        public static IComparerBuilder<T> For<T>()
        {
            return OrderedComparerTypeWrapper<T>.Instance;
        }
    }

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
        /// <param name="selector">
        /// A function supplying the values for the comparer.
        /// </param>
        public static IComparer<T> OrderBy<TValue>(Func<T, TValue> selector)
        {
            return ComparerChainingExtensions.ThenBy<T, TValue>(null, selector);
        }

        /// <summary>
        /// Creates a comparer that will sort elements in ascending order based on the values returned by the provided
        /// selector. The selector values will be compared using the provided comparer or the default comparer for the 
        /// return type of the selector if no comparer is specified.
        /// </summary>
        /// <param name="selector">
        /// A function supplying the values for the comparer.
        /// </param>
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
        /// <param name="selector">
        /// A function supplying the values for the comparer.
        /// </param>
        public static IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector)
        {
            return ComparerChainingExtensions.ThenByDescending<T, TValue>(null, selector);
        }

        /// <summary>
        /// Creates a comparer that will sort elements in descending order based on the values returned by the provided
        /// selector. The selector values will be compared using the provided comparer or the default comparer for the 
        /// return type of the selector if no comparer is specified.
        /// </summary>
        /// <param name="selector">
        /// A function supplying the values for the comparer.
        /// </param>
        /// <param name="comparer">
        /// The comparer to use when comparing the values returned by the selector. 
        /// The default comparer for that type will be used if this parameter is null.
        /// </param>
        public static IComparer<T> OrderByDescending<TValue>(Func<T, TValue> selector, IComparer<TValue> comparer)
        {
            return ComparerChainingExtensions.ThenByDescending<T, TValue>(null, selector, comparer);
        }
    }

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
        /// <param name="parent">
        /// The parent comparer to use first.
        /// </param>
        /// <param name="selector">
        /// A function supplying the values for the comparer.
        /// </param>
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
        /// <param name="parent">
        /// The parent comparer to use first.
        /// </param>
        /// <param name="selector">
        /// A function supplying the values for the comparer.
        /// </param>
        /// <param name="comparer">
        /// The comparer to use when comparing the values returned by the selector. 
        /// </param>
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
        /// <param name="parent">
        /// The parent comparer to use first.
        /// </param>
        /// <param name="selector">
        /// A function supplying the values for the comparer.
        /// </param>
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
        /// <param name="parent">
        /// The parent comparer to use first.
        /// </param>
        /// <param name="selector">
        /// A function supplying the values for the comparer.
        /// </param>
        /// <param name="comparer">
        /// The comparer to use when comparing the values returned by the selector. 
        /// </param>
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
