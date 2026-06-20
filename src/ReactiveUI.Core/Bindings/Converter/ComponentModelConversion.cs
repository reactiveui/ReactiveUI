// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Splat;

namespace ReactiveUI;

/// <summary>
/// Shared <see cref="System.ComponentModel.TypeDescriptor"/>-based conversion logic backing the per-platform
/// <c>ComponentModelFallbackConverter</c> implementations, so the reflection-based fallback lives in one place.
/// </summary>
/// <remarks>
/// This logic uses reflection and is not AOT-safe; it is consulted only when no typed converter matches. Resolved
/// converters and capability lookups are cached because type metadata is reused and eviction would force repeated
/// component-model lookups.
/// </remarks>
public static class ComponentModelConversion
{
    /// <summary>Cache of resolved component model converters for specific (from, to) pairs.</summary>
    private static readonly ConcurrentDictionary<(Type From, Type To), TypeConverter?> _converterCache = new();

    /// <summary>Cache of component model capability lookups (whether conversion is supported).</summary>
    private static readonly ConcurrentDictionary<(Type From, Type To), bool> _capabilityCache = new();

    /// <summary>Gets the binding affinity for converting between the supplied types using the component model.</summary>
    /// <param name="fromType">The source type.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>1 when the component model can perform the conversion; otherwise 0.</returns>
    [UnconditionalSuppressMessage(
        "ReflectionAnalysis",
        "IL2026:RequiresUnreferencedCode",
        Justification = "The callers of this method ensure getting the converter is trim compatible - i.e. the type is not Nullable<T>.")]
    public static int GetAffinity(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType)
    {
        ArgumentExceptionHelper.ThrowIfNull(fromType);
        ArgumentExceptionHelper.ThrowIfNull(toType);

        var canConvert = _capabilityCache.GetOrAdd((fromType, toType), CapabilityFactory);

        return canConvert ? 1 : 0;

        static bool CapabilityFactory((Type From, Type To) key)
        {
            try
            {
                var fromIsString = key.From == typeof(string);
                var (lookupFrom, lookupTo) = fromIsString ? (key.To, key.From) : (key.From, key.To);
                var converter = TypeDescriptor.GetConverter(lookupFrom);
                if (fromIsString)
                {
                    return converter?.CanConvertFrom(typeof(string)) == true;
                }

                return converter?.CanConvertTo(lookupTo) == true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>Attempts to convert <paramref name="from"/> to <paramref name="toType"/> via the component model.</summary>
    /// <typeparam name="TLogHost">The type used as the logging category (the calling converter).</typeparam>
    /// <param name="logHost">The object used for logging; supplies the log category.</param>
    /// <param name="fromType">The source type.</param>
    /// <param name="from">The value to convert.</param>
    /// <param name="toType">The target type.</param>
    /// <param name="result">The converted value when successful.</param>
    /// <returns><see langword="true"/> when the conversion succeeded; otherwise <see langword="false"/>.</returns>
    [UnconditionalSuppressMessage(
        "ReflectionAnalysis",
        "IL2026:RequiresUnreferencedCode",
        Justification = "The callers of this method ensure getting the converter is trim compatible - i.e. the type is not Nullable<T>.")]
    public static bool TryConvert<TLogHost>(
        TLogHost logHost,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType,
        object from,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType,
        [NotNullWhen(true)] out object? result)
        where TLogHost : class, IEnableLogger
    {
        ArgumentExceptionHelper.ThrowIfNull(fromType);
        ArgumentExceptionHelper.ThrowIfNull(from);
        ArgumentExceptionHelper.ThrowIfNull(toType);

        try
        {
            var converter = GetConverter(fromType, toType);
            if (converter is null)
            {
                logHost.Log().Debug("Component model cannot convert {0} to {1}", fromType, toType);
                result = null;
                return false;
            }

            var converted = fromType == typeof(string)
                ? converter.ConvertFrom(from)
                : converter.ConvertTo(from, toType);

            if (converted is not null)
            {
                result = converted;
                return true;
            }

            result = null;
            return false;
        }
        catch (FormatException ex)
        {
            logHost.Log().Debug(ex, "Component model conversion failed (FormatException) for {0} -> {1}", fromType, toType);
            result = null;
            return false;
        }
        catch (Exception ex) when (ex.InnerException is FormatException or IndexOutOfRangeException)
        {
            logHost.Log().Debug(ex, "Component model conversion failed (wrapped exception) for {0} -> {1}", fromType, toType);
            result = null;
            return false;
        }
        catch (Exception ex)
        {
            logHost.Log().Warn(ex, "Component model conversion threw unexpected exception for {0} -> {1}", fromType, toType);
            result = null;
            return false;
        }
    }

    /// <summary>Resolves a component model <see cref="TypeConverter"/> for the specified pair.</summary>
    /// <param name="fromType">The source type.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>A converter instance if the component model supports the conversion; otherwise <see langword="null"/>.</returns>
    [UnconditionalSuppressMessage(
        "ReflectionAnalysis",
        "IL2026:RequiresUnreferencedCode",
        Justification = "The callers of this method ensure getting the converter is trim compatible - i.e. the type is not Nullable<T>.")]
    private static TypeConverter? GetConverter(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType)
    {
        var fromIsString = fromType == typeof(string);
        var lookupFrom = fromIsString ? toType : fromType;

        return _converterCache.GetOrAdd((fromType, toType), ConverterFactory);

        TypeConverter? ConverterFactory((Type From, Type To) key)
        {
            var converter = TypeDescriptor.GetConverter(lookupFrom);
            var canConvert = fromIsString
                ? converter?.CanConvertFrom(typeof(string)) == true
                : converter?.CanConvertTo(toType) == true;

            return canConvert ? converter : null;
        }
    }
}
