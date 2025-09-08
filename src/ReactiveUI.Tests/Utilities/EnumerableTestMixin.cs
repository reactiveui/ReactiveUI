// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using NUnit.Framework.Legacy;

namespace ReactiveUI.Tests;

public static class EnumerableTestMixin
{
    public static void AssertAreEqual<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs)
    {
        var left = lhs.ToArray();
        var right = rhs.ToArray();

        try
        {
            ClassicAssert.Equals(left.Length, right.Length);
            for (var i = 0; i < left.Length; i++)
            {
                ClassicAssert.Equals(left[i], right[i]);
            }
        }
        catch
        {
            Debug.WriteLine("lhs: [{0}]", string.Join(",", lhs.ToArray()));
            Debug.WriteLine("rhs: [{0}]", string.Join(",", rhs.ToArray()));
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
