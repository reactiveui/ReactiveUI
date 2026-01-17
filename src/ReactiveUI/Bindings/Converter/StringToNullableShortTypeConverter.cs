// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="string"/> to nullable <see cref="short"/> using <see cref="short.TryParse(string?, out short)"/>.
/// </summary>
public sealed class StringToNullableShortTypeConverter : BindingTypeConverter<string, short?>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public override bool TryConvert(string? from, object? conversionHint, [MaybeNullWhen(true)] out short? result)
    {
        if (string.IsNullOrEmpty(from))
        {
            result = null;
            return true;
        }

        if (short.TryParse(from, out var value))
        {
            result = value;
            return true;
        }

        result = null;
        return false;
    }
}
