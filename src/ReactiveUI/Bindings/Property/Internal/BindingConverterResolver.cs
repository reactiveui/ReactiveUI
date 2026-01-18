// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Default implementation of <see cref="IBindingConverterResolver"/> that resolves binding type converters.
/// </summary>
/// <remarks>
/// This service resolves binding type converters using RxConverters (lock-free) with Splat fallback.
/// It provides type-based converter resolution with affinity scoring to select the best converter
/// when multiple converters are registered for a type pair.
/// </remarks>
[RequiresUnreferencedCode("Uses RxConverters and Splat which may require dynamic type resolution")]
internal class BindingConverterResolver : IBindingConverterResolver
{
    private static readonly ConcurrentDictionary<(Type fromType, Type? toType), Func<object?, object?, object?[]?, object?>?> _setMethodCache = new();

    /// <inheritdoc/>
    public object? GetBindingConverter(Type fromType, Type toType) =>
        ResolveBestConverter(fromType, toType);

    /// <inheritdoc/>
    public Func<object?, object?, object?[]?, object?>? GetSetMethodConverter(Type? fromType, Type? toType)
    {
        if (fromType is null)
        {
            return null;
        }

        return _setMethodCache.GetOrAdd(
            (fromType, toType),
            static key =>
            {
                var converter = ResolveBestSetMethodConverter(key.fromType, key.toType);
                if (converter is null)
                {
                    return null;
                }

                // Adapt the converter's contract to the local call shape expected by SetThenGet.
                // Cache the delegate to ensure reference equality for repeated calls.
                return (currentValue, newValue, indexParameters) => converter.PerformSet(currentValue, newValue, indexParameters);
            });
    }

    /// <summary>
    /// Resolves the best converter for a given type pair using the ConverterService.
    /// </summary>
    /// <param name="fromType">The source type.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>
    /// The selected converter (typed preferred), or <see langword="null"/> if none matches.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method first attempts to use <see cref="RxConverters.Current"/> for lock-free converter resolution.
    /// If no ConverterService is available (legacy initialization), it falls back to Splat-based resolution.
    /// </para>
    /// <para>
    /// The ConverterService provides:
    /// <list type="bullet">
    /// <item><description>Lock-free reads via snapshot pattern</description></item>
    /// <item><description>Built-in affinity-based selection (highest wins)</description></item>
    /// <item><description>Two-phase resolution: typed converters first, then fallback converters</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    private static object? ResolveBestConverter(Type fromType, Type toType)
    {
        // Try to use the new ConverterService first (lock-free, optimized)
        try
        {
            var converter = RxConverters.Current.ResolveConverter(fromType, toType);
            if (converter is not null)
            {
                return converter;
            }
        }
        catch
        {
            // ConverterService not available, fall back to Splat
        }

        // Fallback to Splat-based resolution for backward compatibility
        // Phase 1: exact-pair typed converters by affinity.
        var typed = AppLocator.Current.GetServices<IBindingTypeConverter>();
        var bestTypedScore = -1;
        IBindingTypeConverter? bestTyped = null;

        foreach (var c in typed)
        {
            if (c is null || c.FromType != fromType || c.ToType != toType)
            {
                continue;
            }

            var score = c.GetAffinityForObjects();
            if (score > bestTypedScore && score > 0)
            {
                bestTypedScore = score;
                bestTyped = c;
            }
        }

        if (bestTyped is not null)
        {
            return bestTyped;
        }

        // Phase 2: fallback converters by affinity.
        var fallbacks = AppLocator.Current.GetServices<IBindingFallbackConverter>();
        var bestFallbackScore = -1;
        IBindingFallbackConverter? bestFallback = null;

        foreach (var c in fallbacks)
        {
            if (c is null)
            {
                continue;
            }

            var score = c.GetAffinityForObjects(fromType, toType);
            if (score > bestFallbackScore && score > 0)
            {
                bestFallbackScore = score;
                bestFallback = c;
            }
        }

        return bestFallback;
    }

    /// <summary>
    /// Resolves the best <see cref="ISetMethodBindingConverter"/> for a given pair.
    /// </summary>
    /// <param name="fromType">The inbound runtime type.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>The selected converter, or <see langword="null"/> if none matches.</returns>
    private static ISetMethodBindingConverter? ResolveBestSetMethodConverter(Type fromType, Type? toType)
    {
        var converters = AppLocator.Current.GetServices<ISetMethodBindingConverter>();

        var bestScore = -1;
        ISetMethodBindingConverter? best = null;

        foreach (var c in converters)
        {
            if (c is null)
            {
                continue;
            }

            var score = c.GetAffinityForObjects(fromType, toType);
            if (score > bestScore && score > 0)
            {
                bestScore = score;
                best = c;
            }
        }

        return best;
    }
}
