// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities;

/// <summary>Extension helpers for asserting on and transforming sequences in tests.</summary>
public static class EnumerableTestMixins
{
    /// <summary>Provides assertion and transformation extension members for sequences.</summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    extension<T>(IEnumerable<T> source)
    {
        /// <summary>Asserts that two sequences contain the same elements in the same order.</summary>
        /// <param name="rhs">The actual sequence.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous assertion.</returns>
        public async Task AssertAreEqual(IEnumerable<T> rhs)
        {
            var left = source.ToArray();
            var right = rhs.ToArray();

            await Assert.That(left.Length).IsEqualTo(right.Length); // Sequence lengths differ.
            for (var i = 0; i < left.Length; i++)
            {
                await Assert.That(left[i]).IsEqualTo(right[i]); // Sequences differ at index {i}.
            }
        }

        /// <summary>Filters out consecutive duplicate elements, yielding only values that differ from their predecessor.</summary>
        /// <returns>The sequence with consecutive duplicates removed.</returns>
        public IEnumerable<T> DistinctUntilChanged()
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return DistinctUntilChangedIterator(source);
        }
    }

    /// <summary>Iterates the source sequence, yielding only values that differ from their predecessor.</summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="enumerable">The source sequence.</param>
    /// <returns>The sequence with consecutive duplicates removed.</returns>
    private static IEnumerable<T> DistinctUntilChangedIterator<T>(IEnumerable<T> enumerable)
    {
        var isFirst = true;
        var lastValue = default(T);

        foreach (var v in enumerable)
        {
            if (isFirst)
            {
                lastValue = v;
                isFirst = false;
                yield return v;
                continue;
            }

            if (lastValue is null)
            {
                continue;
            }

            if (!EqualityComparer<T>.Default.Equals(v, lastValue))
            {
                yield return v;
            }

            lastValue = v;
        }
    }
}
