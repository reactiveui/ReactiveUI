// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts nullable <see cref="decimal"/> values to <see cref="string"/>.
/// </summary>
public sealed class NullableDecimalToStringTypeConverter : BindingTypeConverter<decimal?, string>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public override bool TryConvert(decimal? from, object? conversionHint, [MaybeNullWhen(true)] out string? result)
    {
        if (!from.HasValue)
        {
            result = null;
            return true;
        }

        if (conversionHint is int decimalPlaces)
        {
            result = from.Value.ToString($"F{decimalPlaces}");
            return true;
        }

        if (conversionHint is string format)
        {
            result = from.Value.ToString(format);
            return true;
        }

        result = from.Value.ToString();
        return true;
    }
}
