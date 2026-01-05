// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts nullable <see cref="short"/> values to <see cref="string"/>.
/// </summary>
public sealed class NullableShortToStringTypeConverter : BindingTypeConverter<short?, string>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 10;

    /// <inheritdoc/>
    public override bool TryConvert(short? from, object? conversionHint, [NotNullWhen(true)] out string? result)
    {
        if (!from.HasValue)
        {
            result = null;
            return false;
        }

        if (conversionHint is int width)
        {
            result = from.Value.ToString($"D{width}");
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
