// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="string"/> to <see cref="long"/> using <see cref="long.TryParse(string?, out long)"/>.
/// </summary>
public sealed class StringToLongTypeConverter : BindingTypeConverter<string, long>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public override bool TryConvert(string? from, object? conversionHint, [NotNullWhen(true)] out long result)
    {
        if (from is null)
        {
            result = default;
            return false;
        }

        return long.TryParse(from, out result);
    }
}
