// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="Nullable{Int16}"/> to <see cref="short"/>.
/// </summary>
/// <remarks>
/// When the nullable value is null, the conversion fails and returns false.
/// </remarks>
public sealed class NullableShortToShortTypeConverter : IBindingTypeConverter<short?, short>
{
    /// <inheritdoc/>
    public Type FromType => typeof(short?);

    /// <inheritdoc/>
    public Type ToType => typeof(short);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public bool TryConvert(short? from, object? conversionHint, [NotNullWhen(true)] out short result)
    {
        if (from is null)
        {
            result = default;
            return false;
        }

        result = from.Value;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        if (from is null)
        {
            result = null;
            return TryConvert(null, conversionHint, out _);
        }

        if (from is short value)
        {
            return TryConvert(value, conversionHint, out var typedResult)
                ? (result = typedResult) is not null
                : (result = default) is null && false;
        }

        result = null;
        return false;
    }
}
