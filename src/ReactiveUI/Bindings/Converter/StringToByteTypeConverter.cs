// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="string"/> to <see cref="byte"/> using <see cref="byte.TryParse(string?, out byte)"/>.
/// </summary>
public sealed class StringToByteTypeConverter : BindingTypeConverter<string, byte>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public override bool TryConvert(string? from, object? conversionHint, out byte result)
    {
        if (from is null)
        {
            result = 0;
            return false;
        }

        return byte.TryParse(from, out result);
    }
}
