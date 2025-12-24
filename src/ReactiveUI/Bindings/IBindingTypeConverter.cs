// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// This interface is the extensible implementation of IValueConverters for
/// Bind and OneWayBind. Implement this to teach Bind and OneWayBind how to
/// convert between types.
/// </summary>
public interface IBindingTypeConverter : IEnableLogger
{
    /// <summary>
    /// Returns a positive integer when this class supports
    /// TryConvert for this particular Type. If the method isn't supported at
    /// all, return a non-positive integer. When multiple implementations
    /// return a positive value, the host will use the one which returns
    /// the highest value. When in doubt, return '2' or '0'.
    /// </summary>
    /// <param name="fromType">The source type to convert from.</param>
    /// <param name="toType">The target type to convert to.</param>
    /// <returns>A positive integer if TryConvert is supported,
    /// zero or a negative value otherwise.</returns>
    int GetAffinityForObjects(Type fromType, Type toType);

    /// <summary>
    /// Convert a given object to the specified type.
    /// </summary>
    /// <param name="from">The object to convert.</param>
    /// <param name="toType">The type to coerce the object to.</param>
    /// <param name="conversionHint">An implementation-defined value,
    ///     usually to specify things like locale awareness.</param>
    /// <param name="result">An object that is of the type <paramref name="toType"/>.</param>
    /// <returns>True if conversion was successful.</returns>
    bool TryConvert(object? from, Type toType, object? conversionHint, out object? result);
}
