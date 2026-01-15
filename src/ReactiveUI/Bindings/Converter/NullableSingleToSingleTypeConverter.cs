// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="Nullable{Single}"/> to <see cref="float"/>.
/// </summary>
/// <remarks>
/// When the nullable value is null, returns the default value (0.0f).
/// </remarks>
public sealed class NullableSingleToSingleTypeConverter : IBindingTypeConverter<float?, float>
{
    /// <inheritdoc/>
    public Type FromType => typeof(float?);

    /// <inheritdoc/>
    public Type ToType => typeof(float);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public bool TryConvert(float? from, object? conversionHint, [NotNullWhen(true)] out float result)
    {
        result = from ?? 0.0f;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        if (from is null)
        {
            result = 0.0f;
            return true;
        }

        if (from is float value)
        {
            result = value;
            return true;
        }

        result = null;
        return false;
    }
}
