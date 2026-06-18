// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>Converts <see cref="string"/> to <see cref="Guid"/> using <see cref="Guid.TryParse(string?, out Guid)"/>.</summary>
public sealed class StringToGuidTypeConverter : BindingTypeConverter<string, Guid>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public override bool TryConvert(string? from, object? conversionHint, out Guid result)
    {
        if (from is not null)
        {
            return Guid.TryParse(from, out result);
        }

        result = Guid.Empty;
        return false;
    }
}
