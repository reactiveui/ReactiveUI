// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Generic type-safe interface for converting between specific types.
/// Implement this alongside <see cref="IBindingTypeConverter"/> for AOT-safe conversions.
/// </summary>
/// <typeparam name="TFrom">The source type to convert from.</typeparam>
/// <typeparam name="TTo">The target type to convert to.</typeparam>
/// <remarks>
/// <para>
/// This interface provides compile-time type safety for type conversions,
/// enabling AOT-compatible code paths that avoid boxing and reflection.
/// </para>
/// <para>
/// The generic <see cref="TryConvert(TFrom?, object?, out TTo?)"/> method
/// is preferred over the base interface's object-based method when types
/// are known at compile time.
/// </para>
/// </remarks>
public interface IBindingTypeConverter<TFrom, TTo> : IBindingTypeConverter
{
    /// <summary>
    /// Convert a value to the target type in a type-safe manner.
    /// </summary>
    /// <param name="from">The value to convert.</param>
    /// <param name="conversionHint">Implementation-defined hint for conversion (e.g., format string, locale).</param>
    /// <param name="result">The converted value. May be <see langword="null"/> when conversion succeeds for nullable targets.</param>
    /// <returns><see langword="true"/> if conversion succeeded; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// <para>
    /// This method is AOT-safe as all types are known at compile time.
    /// No reflection or boxing is required for value types.
    /// </para>
    /// <para>
    /// When this method returns <see langword="true"/>, the <paramref name="result"/> parameter
    /// may still be <see langword="null"/> for converters that map null inputs or empty values
    /// to nullable targets.
    /// </para>
    /// </remarks>
    bool TryConvert(TFrom? from, object? conversionHint, [MaybeNullWhen(true)] out TTo? result);
}
