// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>Provides extension methods for compatibility with collection operations.</summary>
/// <remarks>This class contains internal extension methods intended to supplement collection handling
/// functionality. These methods are not intended for public use and may be subject to change or removal without
/// notice.</remarks>
internal static class CompatMixins
{
    /// <summary>Provides ForEach extension members for <see cref="IEnumerable{T}"/>.</summary>
    /// <typeparam name="T">The type of the elements in the enumerable collection.</typeparam>
    /// <param name="this">The enumerable collection whose elements the action is performed on. Cannot be null.</param>
    extension<T>(IEnumerable<T> @this)
    {
        /// <summary>Performs the specified action on each element of the enumerable collection.</summary>
        /// <remarks>This method is intended for scenarios where side effects are required for each element in the
        /// collection. It does not modify the collection or return a result.</remarks>
        /// <param name="block">The action to perform on each element of the collection. Cannot be null.</param>
        internal void ForEach(Action<T> block)
        {
            foreach (var v in @this)
            {
                block(v);
            }
        }

        /// <summary>
        /// Returns a new sequence that contains the elements of the input sequence except for the specified number of
        /// elements at the end.
        /// </summary>
        /// <param name="count">The number of elements to omit from the end of the sequence. Must be non-negative.</param>
        /// <returns>An IEnumerable{T} that contains the elements of the input sequence except for the specified number at the end.
        /// If count is greater than or equal to the number of elements in the sequence, an empty sequence is returned.</returns>
        internal IEnumerable<T> SkipLast(int count)
        {
            var inputList = @this.ToList();
            return inputList.Take(inputList.Count - count);
        }
    }
}
