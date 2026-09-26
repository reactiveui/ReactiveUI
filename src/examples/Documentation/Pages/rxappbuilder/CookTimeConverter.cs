// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>Converts a recipe's cook time to a short display string, such as "1h 30m".</summary>
[System.Diagnostics.DebuggerDisplay("CookTimeConverter")]
public sealed class CookTimeConverter : BindingTypeConverter<TimeSpan, string>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 10;

    /// <inheritdoc/>
    public override bool TryConvert(TimeSpan from, object? conversionHint, out string? result)
    {
        result = from.Hours > 0 ? $"{from.Hours}h {from.Minutes}m" : $"{from.Minutes}m";
        return true;
    }
}
