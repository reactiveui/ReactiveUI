// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace ReactiveUI.Legacy
{
    /// <summary>
    /// IReactiveList of T represents a list that can notify when its
    /// contents are changed (either items are added/removed, or the object
    /// itself changes).
    ///
    /// It is important to implement the Changing/Changed from
    /// IReactiveNotifyPropertyChanged semantically as "Fire when *anything* in
    /// the collection or any of its items have changed, in any way".
    /// </summary>
    /// <typeparam name="T">The list type.</typeparam>
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    public interface IReactiveList<T> : IReactiveCollection<T>, IList<T>
    {
        /// <summary>
        /// Gets a value indicating whether the collection is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Adds a range of elements to the collection.
        /// </summary>
        /// <param name="collection">A collection of values to add.</param>
        void AddRange(IEnumerable<T> collection);

        /// <summary>
        /// Inserts a range of elements to the collection at the specified index.
        /// </summary>
        /// <param name="index">The index to start adding the items to.</param>
        /// <param name="collection">A collection of values to add.</param>
        void InsertRange(int index, IEnumerable<T> collection);

        /// <summary>
        /// Remove all the items contained within the specified collection.
        /// </summary>
        /// <param name="items">The items to remove.</param>
        void RemoveAll(IEnumerable<T> items);

        /// <summary>
        /// Removes items contained at the starting index going to count.
        /// </summary>
        /// <param name="index">The index to start removing items from.</param>
        /// <param name="count">The number of items to remove.</param>
        void RemoveRange(int index, int count);

        /// <summary>
        /// Sort the container using the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer to use to sort the list. If none is specified the default comparer will be used.</param>
        void Sort(IComparer<T> comparer = null);

        /// <summary>
        /// Sort the container using the specified comparison method.
        /// </summary>
        /// <param name="comparison">The comparison type to use to sort the list.</param>
        void Sort(Comparison<T> comparison);

        /// <summary>
        /// Sort the items based at the specified index for the number of items specified by the count.
        /// </summary>
        /// <param name="index">The index to start sorting at.</param>
        /// <param name="count">The count to sort.</param>
        /// <param name="comparer">The comparer to use to sort the list.</param>
        void Sort(int index, int count, IComparer<T> comparer);
    }
}
