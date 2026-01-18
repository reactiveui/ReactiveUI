// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Internal interface for resolving binding type converters.
/// </summary>
/// <remarks>
/// This service resolves binding type converters using RxConverters (lock-free) with Splat fallback.
/// It provides type-based converter resolution with affinity scoring to select the best converter
/// when multiple converters are registered for a type pair.
/// </remarks>
internal interface IBindingConverterResolver
{
    /// <summary>
    /// Gets a binding type converter for the specified type pair.
    /// </summary>
    /// <param name="fromType">The source type.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>
    /// A converter instance (IBindingTypeConverter or IBindingFallbackConverter),
    /// or null if no converter is registered.
    /// </returns>
    /// <remarks>
    /// This method first checks RxConverters for registered converters, then falls back to
    /// Splat for legacy converter resolution. When multiple converters are available,
    /// the one with the highest affinity score is selected.
    /// </remarks>
    object? GetBindingConverter(Type fromType, Type toType);

    /// <summary>
    /// Gets a set-method binding converter for the specified type pair.
    /// </summary>
    /// <param name="fromType">The source type.</param>
    /// <param name="toType">The target type (may be null).</param>
    /// <returns>A conversion function, or null if no converter is applicable.</returns>
    /// <remarks>
    /// Set-method converters are cached to avoid repeated resolution for the same type pairs.
    /// This method is optimized for scenarios where converters are used in hot paths during binding.
    /// </remarks>
    Func<object?, object?, object?[]?, object?>? GetSetMethodConverter(Type? fromType, Type? toType);
}
