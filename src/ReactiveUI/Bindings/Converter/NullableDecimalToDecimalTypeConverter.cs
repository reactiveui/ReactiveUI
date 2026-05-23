// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="Nullable{Decimal}"/> to <see cref="decimal"/>.
/// </summary>
/// <remarks>
/// When the nullable value is null, the conversion fails and returns false.
/// </remarks>
public sealed class NullableDecimalToDecimalTypeConverter : IBindingTypeConverter<decimal?, decimal>
{
    /// <inheritdoc/>
    public Type FromType => typeof(decimal?);

    /// <inheritdoc/>
    public Type ToType => typeof(decimal);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public bool TryConvert(decimal? from, object? conversionHint, out decimal result)
    {
        if (from is null)
        {
            result = 0;
            return false;
        }

        result = from.Value;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        switch (from)
        {
            case null:
                {
                    result = null;
                    return TryConvert(null, conversionHint, out _);
                }

            case decimal value when TryConvert(value, conversionHint, out var typedResult):
                {
                    result = typedResult;
                    return true;
                }

            default:
                {
                    result = null;
                    return false;
                }
        }
    }
}
