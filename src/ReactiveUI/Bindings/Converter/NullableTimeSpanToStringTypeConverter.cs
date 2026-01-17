// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts nullable <see cref="TimeSpan"/> to <see cref="string"/>.
/// </summary>
public sealed class NullableTimeSpanToStringTypeConverter : BindingTypeConverter<TimeSpan?, string>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public override bool TryConvert(TimeSpan? from, object? conversionHint, [MaybeNullWhen(true)] out string? result)
    {
        if (!from.HasValue)
        {
            result = null;
            return true;
        }

        result = from.Value.ToString();
        return true;
    }
}
