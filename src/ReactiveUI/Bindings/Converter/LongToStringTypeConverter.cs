// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Integer To String Type Converter.
/// </summary>
/// <seealso cref="ReactiveUI.IBindingTypeConverter" />
public class LongToStringTypeConverter : IBindingTypeConverter
{
    /// <inheritdoc/>
    public int GetAffinityForObjects(Type fromType, Type toType)
    {
        if (fromType == typeof(long) && toType == typeof(string))
        {
            return 10;
        }

        if (fromType == typeof(string) && toType == typeof(long))
        {
            return 10;
        }

        return 0;
    }

    /// <inheritdoc/>
    public bool TryConvert(object? from, Type toType, object? conversionHint, out object result)
    {
        if (toType == typeof(string) && from is long fromLong)
        {
            if (conversionHint is int longHint)
            {
                result = fromLong.ToString($"D{longHint}");
                return true;
            }

            result = fromLong.ToString();
            return true;
        }

        if (from is string fromString)
        {
            var success = long.TryParse(fromString, out var outLong);
            if (success)
            {
                result = outLong;

                return true;
            }
        }

        result = null!;
        return false;
    }
}
