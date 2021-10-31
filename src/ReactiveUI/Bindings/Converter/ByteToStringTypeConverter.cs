// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace ReactiveUI;

/// <summary>
/// Short To String Type Converter.
/// </summary>
/// <seealso cref="ReactiveUI.IBindingTypeConverter" />
public class ByteToStringTypeConverter : IBindingTypeConverter
{
    /// <inheritdoc/>
    public int GetAffinityForObjects(Type fromType, Type toType)
    {
        if (fromType == typeof(byte) && toType == typeof(string))
        {
            return 10;
        }

        if (fromType == typeof(string) && toType == typeof(byte))
        {
            return 10;
        }

        return 0;
    }

    /// <inheritdoc/>
    public bool TryConvert(object? from, Type toType, object? conversionHint, out object result)
    {
        if (toType == typeof(string) && from is byte fromByte)
        {
            if (conversionHint is int byteHint)
            {
                result = fromByte.ToString($"D{byteHint}");
                return true;
            }

            result = fromByte.ToString();
            return true;
        }

        if (from is string fromString)
        {
            var success = byte.TryParse(fromString, out var outByte);
            if (success)
            {
                result = outByte;

                return true;
            }
        }

        result = null!;
        return false;
    }
}