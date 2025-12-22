// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

using static TUnit.Assertions.Assert;

namespace ReactiveUI.Tests;

public static class EnumerableTestMixin
{
    public static void AssertAreEqual<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs)
    {
        var left = lhs.ToArray();
        var right = rhs.ToArray();

        try
        {
            Assert.That(left, Has.Length.EqualTo(right.Length), "Sequence lengths differ.");
            for (var i = 0; i < left.Length; i++)
            {
                Assert.That(left[i], Is.EqualTo(right[i]), $"Sequences differ at index {i}.");
            }
        }
        catch
        {
            Debug.WriteLine("lhs: [{0}]", string.Join(",", left));
            Debug.WriteLine("rhs: [{0}]", string.Join(",", right));
            throw;
        }
    }

    public static IEnumerable<T> DistinctUntilChanged<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable is null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }

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