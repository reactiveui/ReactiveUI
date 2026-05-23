// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities;

/// <summary>
/// Compatibility helper extension methods used by the tests.
/// </summary>
public static class CompatMixins
{
    /// <summary>
    /// Invokes the supplied action for each element in the sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="this">The sequence to iterate.</param>
    /// <param name="block">The action to invoke for each element.</param>
    public static void Run<T>(this IEnumerable<T> @this, Action<T> block)
    {
        ArgumentNullException.ThrowIfNull(@this);
        ArgumentNullException.ThrowIfNull(block);

        foreach (var v in @this)
        {
            block(v);
        }
    }

    /// <summary>
    /// Returns the sequence with the last <paramref name="count"/> elements removed.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="this">The source sequence.</param>
    /// <param name="count">The number of trailing elements to skip.</param>
    /// <returns>The sequence without its last <paramref name="count"/> elements.</returns>
    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> @this, int count) => @this.Take(@this.Count() - count);
}
