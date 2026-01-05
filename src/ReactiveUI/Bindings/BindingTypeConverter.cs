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

    /// <summary>
    /// Returns the affinity score for this converter.
    /// </summary>
    /// <returns>
    /// A positive integer indicating converter priority. Higher values win when multiple converters match.
    /// Return 0 if the converter cannot handle the type pair.
    /// </returns>
    /// <remarks>
    /// <para><strong>Affinity Guidelines:</strong></para>
    /// <list type="bullet">
    /// <item><description><strong>0</strong> - Cannot convert (no conversion possible)</description></item>
    /// <item><description><strong>1</strong> - Last resort converters (e.g., EqualityTypeConverter)</description></item>
    /// <item><description><strong>2</strong> - Standard ReactiveUI core converters (string, numeric, datetime)</description></item>
    /// <item><description><strong>8</strong> - Platform-specific standard converters (NSDate, WinForms controls)</description></item>
    /// <item><description><strong>100+</strong> - Third-party override range (use to override ReactiveUI defaults)</description></item>
    /// </list>
    /// <para>
    /// When multiple converters match the same type pair, the converter with the highest affinity is selected.
    /// Third-party converters should return 100 or higher to override ReactiveUI defaults.
    /// </para>
    /// </remarks>
    public abstract int GetAffinityForObjects();

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
