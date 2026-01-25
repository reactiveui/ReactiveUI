// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="long"/> to <see cref="Nullable{Int64}"/>.
/// </summary>
public sealed class LongToNullableLongTypeConverter : IBindingTypeConverter<long, long?>
{
    /// <inheritdoc/>
    public Type FromType => typeof(long);

    /// <inheritdoc/>
    public Type ToType => typeof(long?);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public bool TryConvert(long from, object? conversionHint, out long? result)
    {
        result = from;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        if (from is long value)
        {
            result = (long?)value;
            return true;
        }

        result = null;
        return false;
    }
}
