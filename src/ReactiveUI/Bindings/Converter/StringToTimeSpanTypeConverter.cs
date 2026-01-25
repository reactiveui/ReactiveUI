// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="string"/> to <see cref="TimeSpan"/> using <see cref="TimeSpan.TryParse(string?, out TimeSpan)"/>.
/// </summary>
public sealed class StringToTimeSpanTypeConverter : BindingTypeConverter<string, TimeSpan>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public override bool TryConvert(string? from, object? conversionHint, [NotNullWhen(true)] out TimeSpan result)
    {
        if (from is null)
        {
            result = default;
            return false;
        }

        return TimeSpan.TryParse(from, out result);
    }
}
