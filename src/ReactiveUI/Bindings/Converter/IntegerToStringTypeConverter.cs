// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Integer To String Type Converter.
/// </summary>
/// <seealso cref="ReactiveUI.IBindingTypeConverter" />
public class IntegerToStringTypeConverter : IBindingTypeConverter
{
    /// <inheritdoc/>
    public int GetAffinityForObjects(Type fromType, Type toType)
    {
        if (fromType == typeof(int) && toType == typeof(string))
        {
            return 10;
        }

        if (fromType == typeof(string) && toType == typeof(int))
        {
            return 10;
        }

        return 0;
    }

    /// <inheritdoc/>
    public bool TryConvert(object? from, Type toType, object? conversionHint, out object result)
    {
        if (toType == typeof(string) && from is int fromInt)
        {
            if (conversionHint is int intHint)
            {
                result = fromInt.ToString($"D{intHint}");
                return true;
            }

            result = fromInt.ToString();
            return true;
        }

        if (from is string fromString)
        {
            var success = int.TryParse(fromString, out var outInt);
            if (success)
            {
                result = outInt;

                return true;
            }
        }

        result = null!;
        return false;
    }
}
