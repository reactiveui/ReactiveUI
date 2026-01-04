// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts nullable <see cref="DateOnly"/> to <see cref="string"/>.
/// </summary>
public sealed class NullableDateOnlyToStringTypeConverter : BindingTypeConverter<DateOnly?, string>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 10;

    /// <inheritdoc/>
    public override bool TryConvert(DateOnly? from, object? conversionHint, [NotNullWhen(true)] out string? result)
    {
        if (!from.HasValue)
        {
            result = null;
            return false;
        }

        result = from.Value.ToString();
        return true;
    }
}
#endif
