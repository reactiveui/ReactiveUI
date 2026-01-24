// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="float"/> to <see cref="Nullable{Single}"/>.
/// </summary>
public sealed class SingleToNullableSingleTypeConverter : IBindingTypeConverter<float, float?>
{
    /// <inheritdoc/>
    public Type FromType => typeof(float);

    /// <inheritdoc/>
    public Type ToType => typeof(float?);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public bool TryConvert(float from, object? conversionHint, out float? result)
    {
        result = from;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        if (from is float value)
        {
            result = (float?)value;
            return true;
        }

        result = null;
        return false;
    }
}
