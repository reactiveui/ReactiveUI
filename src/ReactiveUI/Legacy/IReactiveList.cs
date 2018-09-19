// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

#pragma warning disable SA1600 // Elements should be documented -- not used for legacy

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
        bool IsEmpty { get; }

        void AddRange(IEnumerable<T> collection);

        void InsertRange(int index, IEnumerable<T> collection);

        void RemoveAll(IEnumerable<T> items);

        void RemoveRange(int index, int count);

        void Sort(IComparer<T> comparer = null);

        void Sort(Comparison<T> comparison);

        void Sort(int index, int count, IComparer<T> comparer);
    }
}
