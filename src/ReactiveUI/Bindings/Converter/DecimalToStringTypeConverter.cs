// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace ReactiveUI;

/// <summary>
/// Decimal To String Type Converter.
/// </summary>
/// <seealso cref="ReactiveUI.IBindingTypeConverter" />
public class DecimalToStringTypeConverter : IBindingTypeConverter
{
    /// <inheritdoc/>
    public int GetAffinityForObjects(Type fromType, Type toType)
    {
        if (fromType == typeof(decimal) && toType == typeof(string))
        {
            return 10;
        }

        if (fromType == typeof(string) && toType == typeof(decimal))
        {
            return 10;
        }

        return 0;
    }

    /// <inheritdoc/>
    public bool TryConvert(object? from, Type toType, object? conversionHint, out object result)
    {
        if (toType == typeof(string) && from is decimal fromDecimal)
        {
            if (conversionHint is int decimalHint)
            {
                result = fromDecimal.ToString($"F{decimalHint}");
                return true;
            }

            result = fromDecimal.ToString();
            return true;
        }

        if (from is string fromString)
        {
            var success = decimal.TryParse(fromString, out var outDecimal);
            if (success)
            {
                if (conversionHint is int decimalHint)
                {
                    result = Math.Round(outDecimal, decimalHint);
                    return true;
                }

                result = outDecimal;

                return true;
            }
        }

        result = null!;
        return false;
    }
}