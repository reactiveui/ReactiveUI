﻿// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Single To String Type Converter.
/// </summary>
/// <seealso cref="ReactiveUI.IBindingTypeConverter" />
public class SingleToStringTypeConverter : IBindingTypeConverter
{
    /// <inheritdoc/>
    public int GetAffinityForObjects(Type fromType, Type toType)
    {
        if (fromType == typeof(float) && toType == typeof(string))
        {
            return 10;
        }

        if (fromType == typeof(string) && toType == typeof(float))
        {
            return 10;
        }

        return 0;
    }

    /// <inheritdoc/>
    public bool TryConvert(object? from, Type toType, object? conversionHint, out object result)
    {
        if (toType == typeof(string) && from is float fromSingle)
        {
            if (conversionHint is int singleHint)
            {
                result = fromSingle.ToString($"F{singleHint}");
                return true;
            }

            result = fromSingle.ToString();
            return true;
        }

        if (from is string fromString)
        {
            var success = float.TryParse(fromString, out var outSingle);
            if (success)
            {
                if (conversionHint is int singleHint)
                {
                    result = Convert.ToSingle(Math.Round(outSingle, singleHint));
                    return true;
                }

                result = outSingle;

                return true;
            }
        }

        result = null!;
        return false;
    }
}
