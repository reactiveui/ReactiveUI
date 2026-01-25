// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="double"/> to <see cref="Nullable{Double}"/>.
/// </summary>
public sealed class DoubleToNullableDoubleTypeConverter : IBindingTypeConverter<double, double?>
{
    /// <inheritdoc/>
    public Type FromType => typeof(double);

    /// <inheritdoc/>
    public Type ToType => typeof(double?);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public bool TryConvert(double from, object? conversionHint, out double? result)
    {
        result = from;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        if (from is double value)
        {
            result = (double?)value;
            return true;
        }

        result = null;
        return false;
    }
}
