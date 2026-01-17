// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Represents a converter that can handle runtime type pairs not covered by typed converters.
/// Fallback converters are consulted only after all typed converters fail to match.
/// </summary>
/// <remarks>
/// <para>
/// Fallback converters exist for scenarios where conversion logic depends on runtime type
/// characteristics that cannot be expressed as compile-time generic pairs.
/// </para>
/// <para>
/// Common use cases:
/// <list type="bullet">
/// <item><description>Component model type descriptors (reflection-based)</description></item>
/// <item><description>Platform-specific type conversions</description></item>
/// <item><description>IConvertible-based conversions</description></item>
/// </list>
/// </para>
/// <para>
/// Affinity Guidelines:
/// <list type="number">
/// <item><description>0 = Not supported</description></item>
/// <item><description>1 = Last resort (ComponentModel/TypeDescriptor)</description></item>
/// <item><description>3 = Broad runtime conversion (IConvertible/numeric widening)</description></item>
/// <item><description>5 = Strong structural match (enum parsing, nullable unwrapping)</description></item>
/// </list>
/// Do not return affinity ≥10 to avoid competing with typed converters.
/// </para>
/// </remarks>
public interface IBindingFallbackConverter : IEnableLogger
{
    /// <summary>
    /// Calculates affinity for the specified runtime type pair.
    /// </summary>
    /// <param name="fromType">The runtime source type.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>
    /// Affinity score (0-5 range). Higher values indicate stronger match.
    /// Return 0 or negative if this converter cannot handle the pair.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method MUST be:
    /// <list type="bullet">
    /// <item><description>Pure (no side effects)</description></item>
    /// <item><description>Fast (cache any expensive metadata)</description></item>
    /// <item><description>Safe (no exceptions, no user code execution)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// This method is invoked during converter selection and may be called frequently.
    /// Results should be cached internally where appropriate.
    /// </para>
    /// </remarks>
    int GetAffinityForObjects([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType);

    /// <summary>
    /// Attempts to convert the value to the target type.
    /// </summary>
    /// <param name="fromType">The runtime source type (guaranteed non-null).</param>
    /// <param name="from">The value to convert (guaranteed non-null).</param>
    /// <param name="toType">The target type (guaranteed non-null).</param>
    /// <param name="conversionHint">Implementation-defined conversion hint (e.g., format string, culture).</param>
    /// <param name="result">
    /// The converted value. Guaranteed non-null when this method returns <see langword="true"/>.
    /// </param>
    /// <returns><see langword="true"/> if conversion succeeded; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// <para>
    /// When this method returns <see langword="true"/>, the <paramref name="result"/> parameter
    /// is guaranteed to be non-null (modern nullability contract).
    /// </para>
    /// <para>
    /// Null input handling is performed by the dispatch layer. This method will never receive
    /// null as the <paramref name="from"/> parameter.
    /// </para>
    /// </remarks>
    bool TryConvert([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType, object from, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType, object? conversionHint, [NotNullWhen(true)] out object? result);
}
