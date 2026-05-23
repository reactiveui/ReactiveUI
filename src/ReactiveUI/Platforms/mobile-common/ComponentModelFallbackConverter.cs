// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.ComponentModel;

namespace ReactiveUI;

/// <summary>
/// Fallback converter using System.ComponentModel.TypeDescriptor for reflection-based type conversion.
/// This converter is consulted only when no typed converter matches.
/// </summary>
/// <remarks>
/// <para>
/// This converter uses reflection and is not AOT-safe. It should be used as a last resort
/// when no typed converter can handle the conversion.
/// </para>
/// <para>
/// The converter caches component model capability lookups to avoid repeated expensive
/// reflection operations.
/// </para>
/// </remarks>
[Preserve(AllMembers = true)]
public sealed class ComponentModelFallbackConverter : IBindingFallbackConverter
{
    /// <summary>
    /// Cache of resolved component model converters for specific (from,to) pairs.
    /// </summary>
    /// <remarks>
    /// This is a stable cache because type metadata tends to be reused and eviction causes repeated component model lookup.
    /// </remarks>
    private static readonly ConcurrentDictionary<(Type From, Type To), TypeConverter?> _converterCache = new();

    /// <summary>
    /// Cache of component model capability lookups (whether conversion is supported).
    /// </summary>
    private static readonly ConcurrentDictionary<(Type From, Type To), bool> _capabilityCache = new();

    /// <inheritdoc/>
    [UnconditionalSuppressMessage(
        "ReflectionAnalysis",
        "IL2026:RequiresUnreferencedCode",
        Justification = "The callers of this method ensure getting the converter is trim compatible - i.e. the type is not Nullable<T>.")]
    public int GetAffinityForObjects(
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

    /// <inheritdoc/>
    [UnconditionalSuppressMessage(
        "ReflectionAnalysis",
        "IL2026:RequiresUnreferencedCode",
        Justification = "The callers of this method ensure getting the converter is trim compatible - i.e. the type is not Nullable<T>.")]
    public bool TryConvert(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType,
        object from,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType,
        object? conversionHint,
        [NotNullWhen(true)] out object? result)
    {
        ArgumentExceptionHelper.ThrowIfNull(from);
        ArgumentExceptionHelper.ThrowIfNull(toType);

        try
        {
            var converter = GetConverter(fromType, toType);
            if (converter is null)
            {
                this.Log().Debug("Component model cannot convert {0} to {1}", fromType, toType);
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
            this.Log().Debug(
                ex,
                "Component model conversion failed (FormatException) for {0} -> {1}",
                fromType,
                toType);
            result = null;
            return false;
        }
        catch (Exception ex) when (ex.InnerException is FormatException or IndexOutOfRangeException)
        {
            this.Log().Debug(
                ex,
                "Component model conversion failed (wrapped exception) for {0} -> {1}",
                fromType,
                toType);
            result = null;
            return false;
        }
        catch (Exception ex)
        {
            this.Log().Warn(
                ex,
                "Component model conversion threw unexpected exception for {0} -> {1}",
                fromType,
                toType);
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Resolves a component model <see cref="TypeConverter"/> for the specified pair.
    /// </summary>
    /// <param name="fromType">The source type.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>
    /// A converter instance if component model supports the conversion; otherwise <see langword="null"/>.
    /// </returns>
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
