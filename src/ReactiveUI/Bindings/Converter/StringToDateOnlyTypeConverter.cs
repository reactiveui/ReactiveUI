// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NET6_0_OR_GREATER

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="string"/> to <see cref="DateOnly"/> using <see cref="DateOnly.TryParse(string?, out DateOnly)"/>.
/// </summary>
public sealed class StringToDateOnlyTypeConverter : BindingTypeConverter<string, DateOnly>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public override bool TryConvert(string? from, object? conversionHint, out DateOnly result)
    {
        if (from is null)
        {
            result = default;
            return false;
        }

        return DateOnly.TryParse(from, out result);
    }
}
#endif
