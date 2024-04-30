// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Short To String Type Converter.
/// </summary>
/// <seealso cref="ReactiveUI.IBindingTypeConverter" />
public class NullableShortToStringTypeConverter : IBindingTypeConverter
{
    /// <inheritdoc/>
    public int GetAffinityForObjects(Type fromType, Type toType)
    {
        if (fromType == typeof(short?) && toType == typeof(string))
        {
            return 10;
        }

        if (fromType == typeof(string) && toType == typeof(short?))
        {
            return 10;
        }

        return 0;
    }

    /// <inheritdoc/>
    public bool TryConvert(object? from, Type toType, object? conversionHint, out object result)
    {
        if (toType == typeof(string) && from is short fromShort)
        {
            if (conversionHint is int shortHint)
            {
                result = fromShort.ToString($"D{shortHint}");
                return true;
            }

            result = fromShort.ToString();
            return true;
        }

        if (from is null)
        {
            result = null!;
            return true;
        }

        if (from is string fromString)
        {
            if (string.IsNullOrEmpty(fromString))
            {
                result = null!;
                return true;
            }

            var success = short.TryParse(fromString, out var outShort);
            if (success)
            {
                result = outShort;

                return true;
            }
        }

        result = null!;
        return false;
    }
}
