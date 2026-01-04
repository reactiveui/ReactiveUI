// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if UIKIT || MACOS
using System.Diagnostics.CodeAnalysis;
using Foundation;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="NSDate"/> to nullable <see cref="DateTimeOffset"/>.
/// </summary>
public sealed class NSDateToNullableDateTimeOffsetConverter : BindingTypeConverter<NSDate, DateTimeOffset?>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 100;

    /// <inheritdoc/>
    public override bool TryConvert(NSDate? from, object? conversionHint, [NotNullWhen(true)] out DateTimeOffset? result)
    {
        if (from is null)
        {
            result = null;
            return false;
        }

        result = new DateTimeOffset((DateTime)from);
        return true;
    }
}
#endif
