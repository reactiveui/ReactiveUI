// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities;

/// <summary>Compatibility helper extension methods used by the tests.</summary>
public static class CompatMixins
{
    /// <summary>Provides compatibility extension members for sequences.</summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    extension<T>(IEnumerable<T> source)
    {
        /// <summary>Invokes the supplied action for each element in the sequence.</summary>
        /// <param name="block">The action to invoke for each element.</param>
        public void Run(Action<T> block)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(block);

            foreach (var v in source)
            {
                block(v);
            }
        }

        /// <summary>Returns the sequence with the last <paramref name="count"/> elements removed.</summary>
        /// <param name="count">The number of trailing elements to skip.</param>
        /// <returns>The sequence without its last <paramref name="count"/> elements.</returns>
        public IEnumerable<T> SkipLast(int count) => source.Take(source.Count() - count);
    }
}
