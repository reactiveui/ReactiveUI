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
    /// Attempts to convert a value to the specified target type using the provided binding type converter.
    /// </summary>
    /// <remarks>The conversion will only be attempted if the converter's ToType matches the specified toType
    /// and the runtime type of from matches the converter's FromType (or is compatible with a nullable value type). No
    /// exceptions are thrown for conversion failures; instead, the method returns false.</remarks>
    /// <param name="converter">The binding type converter to use for the conversion. Cannot be null.</param>
    /// <param name="from">The value to convert. May be null if the target type accepts null values.</param>
    /// <param name="toType">The target type to convert the value to. Must match the converter's ToType. Cannot be null.</param>
    /// <param name="conversionHint">An optional hint object that may influence the conversion process. The meaning of this parameter is determined
    /// by the converter implementation.</param>
    /// <param name="result">When this method returns, contains the converted value if the conversion succeeded; otherwise, null. This
    /// parameter is passed uninitialized.</param>
    /// <returns>true if the value was successfully converted; otherwise, false.</returns>
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
    /// Attempts to convert an object to a specified type using the provided fallback converter.
    /// </summary>
    /// <remarks>This method delegates the conversion to the specified fallback converter. The result is
    /// guaranteed to be non-null only if the conversion succeeds. Callers should check the return value to determine
    /// whether the conversion was successful before using the result.</remarks>
    /// <param name="converter">The fallback converter to use for the conversion operation. Cannot be null.</param>
    /// <param name="fromType">The type of the source object to convert. Used to inform the converter of the input type.</param>
    /// <param name="from">The source object to convert. Cannot be null.</param>
    /// <param name="toType">The target type to convert the object to. Cannot be null.</param>
    /// <param name="conversionHint">An optional hint or context object that may influence the conversion process. May be null.</param>
    /// <param name="result">When this method returns, contains the converted object if the conversion succeeded; otherwise, null. This
    /// parameter is passed uninitialized.</param>
    /// <returns>true if the conversion was successful and the result is non-null; otherwise, false.</returns>
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
    /// Attempts to convert an object to a specified target type using the provided converter.
    /// </summary>
    /// <remarks>This method supports both type-based and fallback converters. If the provided converter does
    /// not implement a supported interface or is null, the method returns false and result is set to null. The method
    /// does not throw exceptions for failed conversions; instead, it returns false to indicate failure.</remarks>
    /// <param name="converter">The converter instance to use for the conversion. Must implement either IBindingTypeConverter or
    /// IBindingFallbackConverter. If null or of an unsupported type, the conversion will not be performed.</param>
    /// <param name="fromType">The type of the source object to convert. Used to determine the appropriate conversion logic.</param>
    /// <param name="from">The source object to convert. May be null if the converter supports null values.</param>
    /// <param name="toType">The target type to convert the source object to. Cannot be null.</param>
    /// <param name="conversionHint">An optional hint or context object that may influence the conversion process. The interpretation of this value
    /// depends on the converter implementation.</param>
    /// <param name="result">When this method returns, contains the converted value if the conversion succeeded; otherwise, null. This
    /// parameter is passed uninitialized.</param>
    /// <returns>true if the conversion was successful and result contains the converted value; otherwise, false.</returns>
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
