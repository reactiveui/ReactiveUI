// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Splat;

namespace ReactiveUI;

/// <summary>Default implementation of <see cref="IBindingConverterResolver"/> that resolves binding type converters.</summary>
/// <remarks>
/// This service resolves binding type converters using RxConverters (lock-free) with Splat fallback.
/// It provides type-based converter resolution with affinity scoring to select the best converter
/// when multiple converters are registered for a type pair.
/// </remarks>
[RequiresUnreferencedCode("Uses RxConverters and Splat which may require dynamic type resolution")]
public class BindingConverterResolver : IBindingConverterResolver
{
    /// <summary>Cache of resolved set-method converter delegates, keyed by (fromType, toType) pair.</summary>
    private static readonly
        ConcurrentDictionary<(Type fromType, Type? toType), Func<object?, object?, object?[]?, object?>?>
        _setMethodCache = new();

    /// <inheritdoc/>
    public object? GetBindingConverter(Type fromType, Type toType) =>
        ResolveBestConverter(fromType, toType);

    /// <inheritdoc/>
    public Func<object?, object?, object?[]?, object?>? GetSetMethodConverter(Type? fromType, Type? toType) =>
        fromType is null
            ? null
            : _setMethodCache.GetOrAdd(
                (fromType, toType),
                static key =>
                {
                    var converter = ResolveBestSetMethodConverter(key.fromType, key.toType);
                    return converter is null ? null : converter.PerformSet;
                });

    /// <summary>Resolves the best converter for a given type pair using the ConverterService.</summary>
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
    private static object? ResolveBestConverter(Type fromType, Type toType) =>
        TryResolveFromConverterService(fromType, toType)
        ?? (object?)SelectByHighestAffinity(
            AppLocator.Current.GetServices<IBindingTypeConverter>(),
            candidate => candidate.FromType == fromType && candidate.ToType == toType ? candidate.GetAffinityForObjects() : 0)
        ?? SelectByHighestAffinity(
            AppLocator.Current.GetServices<IBindingFallbackConverter>(),
            candidate => candidate.GetAffinityForObjects(fromType, toType));

    /// <summary>Resolves a converter via <see cref="RxConverters.Current"/>, returning null if it is unavailable or throws.</summary>
    /// <param name="fromType">The source type.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>The resolved converter, or <see langword="null"/>.</returns>
    private static object? TryResolveFromConverterService(Type fromType, Type toType)
    {
        try
        {
            return RxConverters.Current.ResolveConverter(fromType, toType);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Selects the candidate with the highest positive affinity score.</summary>
    /// <typeparam name="T">The candidate type.</typeparam>
    /// <param name="candidates">The candidates to evaluate.</param>
    /// <param name="scoreSelector">Returns the affinity score for a candidate.</param>
    /// <returns>The highest-scoring candidate, or <see langword="null"/> when none has a positive score.</returns>
    private static T? SelectByHighestAffinity<T>(IEnumerable<T> candidates, Func<T, int> scoreSelector)
        where T : class
    {
        var bestScore = -1;
        T? best = null;

        foreach (var candidate in candidates)
        {
            if (candidate is null)
            {
                continue;
            }

            var score = scoreSelector(candidate);
            if (score > bestScore && score > 0)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }

    /// <summary>Resolves the best <see cref="ISetMethodBindingConverter"/> for a given pair.</summary>
    /// <param name="fromType">The inbound runtime type.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>The selected converter, or <see langword="null"/> if none matches.</returns>
    private static ISetMethodBindingConverter? ResolveBestSetMethodConverter(Type fromType, Type? toType) =>
        SelectByHighestAffinity(
            AppLocator.Current.GetServices<ISetMethodBindingConverter>(),
            candidate => candidate.GetAffinityForObjects(fromType, toType));
}
