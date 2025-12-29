// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Double To String Type Converter.
/// </summary>
/// <seealso cref="IBindingTypeConverter" />
public class DoubleToStringTypeConverter : IBindingTypeConverter
{
    /// <inheritdoc/>
    public int GetAffinityForObjects(Type fromType, Type toType)
    {
        if (fromType == typeof(double) && toType == typeof(string))
        {
            return 10;
        }

        if (fromType == typeof(string) && toType == typeof(double))
        {
            return 10;
        }

        return 0;
    }

    /// <inheritdoc/>
    public bool TryConvert(object? from, Type toType, object? conversionHint, out object result)
    {
        if (toType == typeof(string) && from is double fromDouble)
        {
            if (conversionHint is int doubleHint)
            {
                result = fromDouble.ToString($"F{doubleHint}");
                return true;
            }

            result = fromDouble.ToString();
            return true;
        }

        if (from is string fromString)
        {
            var success = double.TryParse(fromString, out var outDouble);
            if (success)
            {
                if (conversionHint is int doubleHint)
                {
                    result = Math.Round(outDouble, doubleHint);
                    return true;
                }

                result = outDouble;

                return true;
            }
        }

        result = null!;
        return false;
    }
}
