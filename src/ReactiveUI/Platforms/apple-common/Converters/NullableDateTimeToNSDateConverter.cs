// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if UIKIT || MACOS
using System.Diagnostics.CodeAnalysis;
using Foundation;

namespace ReactiveUI;

/// <summary>
/// Converts nullable <see cref="DateTime"/> to <see cref="NSDate"/>.
/// </summary>
public sealed class NullableDateTimeToNSDateConverter : BindingTypeConverter<DateTime?, NSDate>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 100;

    /// <inheritdoc/>
    public override bool TryConvert(DateTime? from, object? conversionHint, [NotNullWhen(true)] out NSDate? result)
    {
        if (!from.HasValue)
        {
            result = null;
            return false;
        }

        result = (NSDate)from.Value;
        return true;
    }
}
#endif
