// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

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
