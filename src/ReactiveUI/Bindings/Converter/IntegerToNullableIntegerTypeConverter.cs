// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="int"/> to <see cref="Nullable{Int32}"/>.
/// </summary>
public sealed class IntegerToNullableIntegerTypeConverter : IBindingTypeConverter<int, int?>
{
    /// <inheritdoc/>
    public Type FromType => typeof(int);

    /// <inheritdoc/>
    public Type ToType => typeof(int?);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public bool TryConvert(int from, object? conversionHint, out int? result)
    {
        result = from;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        if (from is int value)
        {
            result = (int?)value;
            return true;
        }

        result = null;
        return false;
    }
}
