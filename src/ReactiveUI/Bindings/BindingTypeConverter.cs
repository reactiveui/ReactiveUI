// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Base class for type-pair binding converters.
/// </summary>
/// <typeparam name="TFrom">The source type to convert from.</typeparam>
/// <typeparam name="TTo">The target type to convert to.</typeparam>
/// <remarks>
/// This base class supplies the "type-only" metadata (<see cref="FromType"/>/<see cref="ToType"/>) and the
/// object-based shim (<see cref="TryConvertTyped(object?, object?, out object?)"/>), allowing the dispatch
/// layer to avoid reflection.
/// </remarks>
public abstract class BindingTypeConverter<TFrom, TTo> : IBindingTypeConverter<TFrom, TTo>
{
    /// <inheritdoc/>
    public Type FromType => typeof(TFrom);

    /// <inheritdoc/>
    public Type ToType => typeof(TTo);

    /// <inheritdoc/>
    /// <remarks>
    /// The default implementation returns a constant affinity for an exact type-pair converter.
    /// Override if you need different selection semantics within a pair-based registry.
    /// </remarks>
    public virtual int GetAffinityForObjects() => 10;

    /// <inheritdoc/>
    public abstract bool TryConvert(TFrom? from, object? conversionHint, [NotNullWhen(true)] out TTo? result);

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
    {
        // Enforce the modern nullability contract:
        // - Successful conversion must yield a non-null result.
        // - Null input (common in UI clearing scenarios) is treated as "not converted" here,
        //   so the binding pipeline can decide whether to flow null directly.
        if (from is null)
        {
            result = null;
            return false;
        }

        if (from is not TFrom castFrom)
        {
            result = null;
            return false;
        }

        if (!TryConvert(castFrom, conversionHint, out var typedResult))
        {
            result = null;
            return false;
        }

        // Defensive: even though the typed method is annotated, ensure we never report success with null.
        if (typedResult is null)
        {
            result = null;
            return false;
        }

        result = typedResult;
        return true;
    }
}
