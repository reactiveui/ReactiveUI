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
    /// Gets the source type supported by this converter.
    /// </summary>
    Type FromType { get; }

    /// <summary>
    /// Gets the target type supported by this converter.
    /// </summary>
    Type ToType { get; }

    /// <summary>
    /// Returns a positive integer when this class supports
    /// TryConvert for this particular Type. If the method isn't supported at
    /// all, return a non-positive integer. When multiple implementations
    /// return a positive value, the host will use the one which returns
    /// the highest value. When in doubt, return '2' or '0'.
    /// </summary>
    /// <returns>A positive integer if TryConvert is supported,
    /// zero or a negative value otherwise.</returns>
    int GetAffinityForObjects();

    /// <summary>
    /// Attempts to convert using the typed implementation, exposed via an object-based shim.
    /// </summary>
    /// <param name="from">The source value.</param>
    /// <param name="conversionHint">Implementation-defined hint.</param>
    /// <param name="result">The converted value.</param>
    /// <returns><see langword="true"/> if conversion succeeded; otherwise <see langword="false"/>.</returns>
    bool TryConvertTyped(object? from, object? conversionHint, out object? result);
}
