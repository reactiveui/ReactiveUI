// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="string"/> to nullable <see cref="DateTimeOffset"/> using <see cref="DateTimeOffset.TryParse(string?, out DateTimeOffset)"/>.
/// </summary>
public sealed class StringToNullableDateTimeOffsetTypeConverter : BindingTypeConverter<string, DateTimeOffset?>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public override bool TryConvert(
        string? from,
        object? conversionHint,
        out DateTimeOffset? result)
    {
        if (string.IsNullOrEmpty(from))
        {
            result = null;
            return true;
        }

        if (DateTimeOffset.TryParse(from, out var value))
        {
            result = value;
            return true;
        }

        result = null;
        return false;
    }
}
