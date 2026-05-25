// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="double"/> values to <see cref="string"/>.
/// </summary>
public sealed class DoubleToStringTypeConverter : BindingTypeConverter<double, string>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public override bool TryConvert(double from, object? conversionHint, [NotNullWhen(true)] out string? result)
    {
        switch (conversionHint)
        {
            case int decimalPlaces:
                {
                    result = from.ToString($"F{decimalPlaces}");
                    return true;
                }

            case string format:
                {
                    result = from.ToString(format);
                    return true;
                }

            default:
                {
                    result = from.ToString(System.Globalization.CultureInfo.CurrentCulture);
                    return true;
                }
        }
    }
}
