// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="Nullable{Int16}"/> to <see cref="short"/>.
/// </summary>
/// <remarks>
/// When the nullable value is null, returns the default value (0).
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
        result = from ?? 0;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        if (from is null)
        {
            result = (short)0;
            return true;
        }

        if (from is short value)
        {
            result = value;
            return true;
        }

        result = null;
        return false;
    }
}
