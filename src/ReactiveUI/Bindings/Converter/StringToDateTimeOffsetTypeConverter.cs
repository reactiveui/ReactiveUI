// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="string"/> to <see cref="DateTimeOffset"/> using <see cref="DateTimeOffset.TryParse(string?, out DateTimeOffset)"/>.
/// </summary>
public sealed class StringToDateTimeOffsetTypeConverter : BindingTypeConverter<string, DateTimeOffset>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public override bool TryConvert(string? from, object? conversionHint, out DateTimeOffset result)
    {
        if (from is null)
        {
            result = default;
            return false;
        }

        return DateTimeOffset.TryParse(from, out result);
    }
}
