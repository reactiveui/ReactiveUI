// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides a composite comparer that applies a parent comparer followed by an additional comparison delegate when
/// comparing objects of type T.
/// </summary>
/// <remarks>Use this class to chain multiple comparison strategies, such as for multi-level sorting. The parent
/// comparer is evaluated first; if it determines the objects are equal, the provided comparison delegate is used to
/// break ties.</remarks>
/// <typeparam name="T">The type of objects to compare.</typeparam>
/// <param name="parent">An optional parent comparer to apply first when comparing two objects. If null, only the specified comparison
/// delegate is used.</param>
/// <param name="comparison">A delegate that defines the comparison to apply if the parent comparer considers the objects equal. Cannot be null.</param>
internal sealed class ChainedComparer<T>(IComparer<T>? parent, Comparison<T> comparison) : IComparer<T>
{
    /// <inheritdoc />
    public int Compare(T? x, T? y)
    {
        var parentResult = parent?.Compare(x!, y!) ?? 0;

        if (x is null && y is null)
        {
            return 0;
        }

        return parentResult != 0 ? parentResult : comparison(x!, y!);
    }
}
