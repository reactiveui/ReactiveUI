// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts nullable <see cref="TimeOnly"/> to <see cref="string"/>.
/// </summary>
public sealed class NullableTimeOnlyToStringTypeConverter : BindingTypeConverter<TimeOnly?, string>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public override bool TryConvert(TimeOnly? from, object? conversionHint, [NotNullWhen(true)] out string? result)
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
