// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="Nullable{Double}"/> to <see cref="double"/>.
/// </summary>
/// <remarks>
/// When the nullable value is null, returns the default value (0.0).
/// </remarks>
public sealed class NullableDoubleToDoubleTypeConverter : IBindingTypeConverter<double?, double>
{
    /// <inheritdoc/>
    public Type FromType => typeof(double?);

    /// <inheritdoc/>
    public Type ToType => typeof(double);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public bool TryConvert(double? from, object? conversionHint, [NotNullWhen(true)] out double result)
    {
        result = from ?? 0.0;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        if (from is null)
        {
            result = 0.0;
            return true;
        }

        if (from is double value)
        {
            result = value;
            return true;
        }

        result = null;
        return false;
    }
}
