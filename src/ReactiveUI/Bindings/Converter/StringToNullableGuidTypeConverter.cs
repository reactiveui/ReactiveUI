// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="string"/> to nullable <see cref="Guid"/> using <see cref="Guid.TryParse(string?, out Guid)"/>.
/// </summary>
public sealed class StringToNullableGuidTypeConverter : BindingTypeConverter<string, Guid?>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public override bool TryConvert(string? from, object? conversionHint, [MaybeNullWhen(true)] out Guid? result)
    {
        if (string.IsNullOrEmpty(from))
        {
            result = null;
            return true;
        }

        if (Guid.TryParse(from, out var value))
        {
            result = value;
            return true;
        }

        result = null;
        return false;
    }
}
