// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="Nullable{Int64}"/> to <see cref="long"/>.
/// </summary>
/// <remarks>
/// When the nullable value is null, returns the default value (0).
/// </remarks>
public sealed class NullableLongToLongTypeConverter : IBindingTypeConverter<long?, long>
{
    /// <inheritdoc/>
    public Type FromType => typeof(long?);

    /// <inheritdoc/>
    public Type ToType => typeof(long);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public bool TryConvert(long? from, object? conversionHint, [NotNullWhen(true)] out long result)
    {
        result = from ?? 0;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        if (from is null)
        {
            result = 0L;
            return true;
        }

        if (from is long value)
        {
            result = value;
            return true;
        }

        result = null;
        return false;
    }
}
