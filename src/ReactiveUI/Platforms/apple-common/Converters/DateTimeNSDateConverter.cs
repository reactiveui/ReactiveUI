// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;

namespace ReactiveUI;

/// <summary>
/// Binding Type Converter for DateTime to NSDateTime.
/// </summary>
public class DateTimeNSDateConverter : IBindingTypeConverter
{
    internal static Lazy<DateTimeNSDateConverter> Instance { get; } = new();

    /// <inheritdoc/>
    public int GetAffinityForObjects(Type fromType, Type toType) =>
        (fromType == typeof(DateTime) && toType == typeof(NSDate)) ||
        (toType == typeof(DateTime) && fromType == typeof(NSDate)) ? 4 : -1;

    /// <inheritdoc/>
    public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
    {
        result = null;

        if (from?.GetType() == typeof(DateTime) && toType == typeof(NSDate))
        {
            var dt = (DateTime)from;
            result = (NSDate)dt;
            return true;
        }
        else if (from?.GetType() == typeof(NSDate) && toType == typeof(DateTime))
        {
            var dt = (NSDate)from;
            result = (DateTime)dt;
            return true;
        }

        return false;
    }
}
