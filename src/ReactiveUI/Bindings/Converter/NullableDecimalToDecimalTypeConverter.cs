// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="Nullable{Decimal}"/> to <see cref="decimal"/>.
/// </summary>
/// <remarks>
/// When the nullable value is null, returns the default value (0.0M).
/// </remarks>
public sealed class NullableDecimalToDecimalTypeConverter : IBindingTypeConverter<decimal?, decimal>
{
    /// <inheritdoc/>
    public Type FromType => typeof(decimal?);

    /// <inheritdoc/>
    public Type ToType => typeof(decimal);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public bool TryConvert(decimal? from, object? conversionHint, [NotNullWhen(true)] out decimal result)
    {
        result = from ?? 0.0M;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        if (from is null)
        {
            result = 0.0M;
            return true;
        }

        if (from is decimal value)
        {
            result = value;
            return true;
        }

        result = null;
        return false;
    }
}
