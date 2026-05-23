// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="string"/> to nullable <see cref="int"/> using <see cref="int.TryParse(string?, out int)"/>.
/// </summary>
public sealed class StringToNullableIntegerTypeConverter : BindingTypeConverter<string, int?>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public override bool TryConvert(string? from, object? conversionHint, out int? result)
    {
        if (string.IsNullOrEmpty(from))
        {
            result = null;
            return true;
        }

        if (int.TryParse(from, out var value))
        {
            result = value;
            return true;
        }

        result = null;
        return false;
    }
}
