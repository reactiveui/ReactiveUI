// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if UIKIT || MACOS
using System.Diagnostics.CodeAnalysis;
using Foundation;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="DateTimeOffset"/> to <see cref="NSDate"/>.
/// </summary>
public sealed class DateTimeOffsetToNSDateConverter : BindingTypeConverter<DateTimeOffset, NSDate>
{
    /// <summary>The affinity score returned for supported type pairs.</summary>
    private const int ConverterAffinity = 8;

    /// <inheritdoc/>
    public override int GetAffinityForObjects() => ConverterAffinity;

    /// <inheritdoc/>
    public override bool TryConvert(DateTimeOffset from, object? conversionHint, [NotNullWhen(true)] out NSDate? result)
    {
        result = (NSDate)from.DateTime;
        return true;
    }
}
#endif
