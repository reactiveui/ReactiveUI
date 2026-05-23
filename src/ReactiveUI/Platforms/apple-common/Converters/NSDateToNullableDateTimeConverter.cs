// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if UIKIT || MACOS
using System.Diagnostics.CodeAnalysis;
using Foundation;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="NSDate"/> to nullable <see cref="DateTime"/>.
/// </summary>
public sealed class NSDateToNullableDateTimeConverter : BindingTypeConverter<NSDate, DateTime?>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 8;

    /// <inheritdoc/>
    public override bool TryConvert(NSDate? from, object? conversionHint, [NotNullWhen(true)] out DateTime? result)
    {
        if (from is null)
        {
            result = null;
            return false;
        }

        result = (DateTime)from;
        return true;
    }
}
#endif
