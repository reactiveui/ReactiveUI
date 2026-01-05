// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="float"/> values to <see cref="string"/>.
/// </summary>
public sealed class SingleToStringTypeConverter : BindingTypeConverter<float, string>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public override bool TryConvert(float from, object? conversionHint, [NotNullWhen(true)] out string? result)
    {
        if (conversionHint is int decimalPlaces)
        {
            result = from.ToString($"F{decimalPlaces}");
            return true;
        }

        if (conversionHint is string format)
        {
            result = from.ToString(format);
            return true;
        }

        result = from.ToString();
        return true;
    }
}
