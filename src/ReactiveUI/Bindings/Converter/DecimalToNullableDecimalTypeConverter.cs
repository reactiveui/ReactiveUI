// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="decimal"/> to <see cref="Nullable{Decimal}"/>.
/// </summary>
public sealed class DecimalToNullableDecimalTypeConverter : IBindingTypeConverter<decimal, decimal?>
{
    /// <inheritdoc/>
    public Type FromType => typeof(decimal);

    /// <inheritdoc/>
    public Type ToType => typeof(decimal?);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public bool TryConvert(decimal from, object? conversionHint, out decimal? result)
    {
        result = from;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        if (from is decimal value)
        {
            result = (decimal?)value;
            return true;
        }

        result = null;
        return false;
    }
}
