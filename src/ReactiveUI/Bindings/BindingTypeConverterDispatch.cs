// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Dispatches conversions using a type-only fast-path, avoiding reflection.
/// </summary>
internal static class BindingTypeConverterDispatch
{
    /// <summary>
    /// Attempts conversion via the converter's type-only metadata (<see cref="IBindingTypeConverter.FromType"/> and
    /// <see cref="IBindingTypeConverter.ToType"/>) and object shim (<see cref="IBindingTypeConverter.TryConvertTyped"/>).
    /// </summary>
    /// <param name="converter">The converter.</param>
    /// <param name="from">The source value.</param>
    /// <param name="toType">The target type requested by the caller.</param>
    /// <param name="conversionHint">Implementation-defined hint.</param>
    /// <param name="result">The converted result.</param>
    /// <returns><see langword="true"/> if conversion succeeded; otherwise <see langword="false"/>.</returns>
    internal static bool TryConvert(
        IBindingTypeConverter converter,
        object? from,
        Type toType,
        object? conversionHint,
        out object? result)
    {
        ArgumentExceptionHelper.ThrowIfNull(converter);
        ArgumentExceptionHelper.ThrowIfNull(toType);

        if (converter.ToType != toType)
        {
            result = null;
            return false;
        }

        if (from is null)
        {
            var fromType = converter.FromType;
            if (fromType.IsValueType && Nullable.GetUnderlyingType(fromType) is null)
            {
                result = null;
                return false;
            }

            return converter.TryConvertTyped(null, conversionHint, out result);
        }

        var runtimeType = from.GetType();
        var converterFromType = converter.FromType;

        // Exact pair match keeps dispatch predictable and avoids assignability ambiguity,
        // but allow nullable<T> converters to accept boxed T values.
        if (converterFromType != runtimeType &&
            Nullable.GetUnderlyingType(converterFromType) != runtimeType)
        {
            result = null;
            return false;
        }

        return converter.TryConvertTyped(from, conversionHint, out result);
    }

    /// <summary>
    /// Attempts conversion using a fallback converter.
    /// </summary>
    /// <param name="converter">The fallback converter.</param>
    /// <param name="fromType">The source runtime type.</param>
    /// <param name="from">The source value (guaranteed non-null by caller).</param>
    /// <param name="toType">The target type.</param>
    /// <param name="conversionHint">Implementation-defined hint.</param>
    /// <param name="result">The converted result.</param>
    /// <returns><see langword="true"/> if conversion succeeded; otherwise, <see langword="false"/>.</returns>
    internal static bool TryConvertFallback(
        IBindingFallbackConverter converter,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType,
        object from,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType,
        object? conversionHint,
        out object? result)
    {
        ArgumentExceptionHelper.ThrowIfNull(converter);
        ArgumentExceptionHelper.ThrowIfNull(from);
        ArgumentExceptionHelper.ThrowIfNull(toType);

        // Delegate to fallback converter (from is guaranteed non-null)
        if (!converter.TryConvert(fromType, from, toType, conversionHint, out result))
        {
            result = null;
            return false;
        }

        // Fallback converters must still guarantee a non-null result on success.
        if (result is null)
        {
            result = null;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Unified dispatch method that handles both typed and fallback converters.
    /// </summary>
    /// <param name="converter">The converter (either <see cref="IBindingTypeConverter"/> or <see cref="IBindingFallbackConverter"/>).</param>
    /// <param name="fromType">The source runtime type.</param>
    /// <param name="from">The source value.</param>
    /// <param name="toType">The target type.</param>
    /// <param name="conversionHint">Implementation-defined hint.</param>
    /// <param name="result">The converted result.</param>
    /// <returns><see langword="true"/> if conversion succeeded; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// This method automatically dispatches to the appropriate converter type:
    /// <list type="bullet">
    /// <item><description><see cref="IBindingTypeConverter"/> - uses exact pair matching</description></item>
    /// <item><description><see cref="IBindingFallbackConverter"/> - requires non-null input</description></item>
    /// </list>
    /// </remarks>
    internal static bool TryConvertAny(
        object? converter,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType,
        object? from,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType,
        object? conversionHint,
        out object? result)
    {
        ArgumentExceptionHelper.ThrowIfNull(toType);

        if (converter is null)
        {
            result = null;
            return false;
        }

        // Dispatch to typed converter
        if (converter is IBindingTypeConverter typedConverter)
        {
            return TryConvert(typedConverter, from, toType, conversionHint, out result);
        }

        // Dispatch to fallback converter (requires non-null input)
        if (converter is IBindingFallbackConverter fallbackConverter)
        {
            if (from is null)
            {
                result = null;
                return false;
            }

            return TryConvertFallback(fallbackConverter, fromType, from, toType, conversionHint, out result);
        }

        // Unknown converter type
        result = null;
        return false;
    }
}
