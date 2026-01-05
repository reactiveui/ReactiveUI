// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides unified access to all converter registries in ReactiveUI.
/// </summary>
/// <remarks>
/// <para>
/// This service manages three types of converters:
/// </para>
/// <list type="bullet">
/// <item><description>
/// <strong>Typed Converters:</strong> Exact type-pair converters (e.g., int → string).
/// Registered via <see cref="TypedConverters"/> and selected based on affinity for exact matches.
/// </description></item>
/// <item><description>
/// <strong>Fallback Converters:</strong> Runtime type converters using reflection or type descriptors.
/// Registered via <see cref="FallbackConverters"/> and consulted when no typed converter matches.
/// </description></item>
/// <item><description>
/// <strong>Set-Method Converters:</strong> Specialized converters for binding set operations.
/// Registered via <see cref="SetMethodConverters"/> for platform-specific control binding.
/// </description></item>
/// </list>
/// <para>
/// <strong>Converter Selection Algorithm:</strong>
/// </para>
/// <list type="number">
/// <item><description>
/// <strong>Phase 1:</strong> Search for exact type-pair match in <see cref="TypedConverters"/>.
/// If found, return the typed converter with the highest affinity.
/// </description></item>
/// <item><description>
/// <strong>Phase 2:</strong> If no typed converter found, search <see cref="FallbackConverters"/>.
/// Return the fallback converter with the highest affinity for the runtime types.
/// </description></item>
/// <item><description>
/// <strong>Result:</strong> If neither phase finds a converter, return <see langword="null"/>.
/// </description></item>
/// </list>
/// <para>
/// <strong>Affinity Guidelines:</strong>
/// </para>
/// <list type="bullet">
/// <item><description><strong>0:</strong> Converter cannot handle the type pair</description></item>
/// <item><description><strong>1:</strong> Last resort (e.g., EqualityTypeConverter)</description></item>
/// <item><description><strong>2:</strong> Standard ReactiveUI converters (string, numeric, datetime)</description></item>
/// <item><description><strong>8:</strong> Platform-specific standard converters (NSDate, WinForms controls)</description></item>
/// <item><description><strong>100+:</strong> Third-party override range (use to override ReactiveUI defaults)</description></item>
/// </list>
/// </remarks>
public sealed class ConverterService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConverterService"/> class.
    /// </summary>
    public ConverterService()
    {
        TypedConverters = new BindingTypeConverterRegistry();
        FallbackConverters = new BindingFallbackConverterRegistry();
        SetMethodConverters = new SetMethodBindingConverterRegistry();
    }

    /// <summary>
    /// Gets the registry for typed binding converters.
    /// </summary>
    /// <value>
    /// The typed converter registry for exact type-pair conversions.
    /// </value>
    /// <remarks>
    /// Use this registry to register and retrieve converters for specific (FromType, ToType) pairs.
    /// Typed converters are consulted first during binding operations and provide the best performance.
    /// </remarks>
    public BindingTypeConverterRegistry TypedConverters { get; }

    /// <summary>
    /// Gets the registry for fallback binding converters.
    /// </summary>
    /// <value>
    /// The fallback converter registry for runtime type conversions.
    /// </value>
    /// <remarks>
    /// Use this registry to register and retrieve converters that use runtime type checking.
    /// Fallback converters are consulted only when no exact type-pair converter is found.
    /// These typically use reflection or System.ComponentModel.TypeDescriptor for conversion.
    /// </remarks>
    public BindingFallbackConverterRegistry FallbackConverters { get; }

    /// <summary>
    /// Gets the registry for set-method binding converters.
    /// </summary>
    /// <value>
    /// The set-method converter registry for specialized set operations.
    /// </value>
    /// <remarks>
    /// Use this registry to register and retrieve converters for platform-specific binding set operations,
    /// such as populating collection controls or handling specialized platform widgets.
    /// </remarks>
    public SetMethodBindingConverterRegistry SetMethodConverters { get; }

    /// <summary>
    /// Resolves the best converter for the specified type pair.
    /// </summary>
    /// <param name="fromType">The source type to convert from.</param>
    /// <param name="toType">The target type to convert to.</param>
    /// <returns>
    /// The best converter for the type pair (either typed or fallback), or <see langword="null"/> if no converter is available.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="fromType"/> or <paramref name="toType"/> is null.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method implements the two-phase converter selection algorithm:
    /// </para>
    /// <list type="number">
    /// <item><description>
    /// <strong>Phase 1:</strong> Try to find an exact type-pair match in <see cref="TypedConverters"/>.
    /// If found, return the typed converter with the highest affinity.
    /// </description></item>
    /// <item><description>
    /// <strong>Phase 2:</strong> If no typed converter found, search <see cref="FallbackConverters"/>.
    /// Return the fallback converter with the highest affinity for the runtime types.
    /// </description></item>
    /// </list>
    /// <para>
    /// This method is thread-safe and lock-free, making it safe to call from multiple threads concurrently.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var converter = converterService.ResolveConverter(typeof(int), typeof(string));
    /// if (converter is IBindingTypeConverter typedConverter)
    /// {
    ///     // Use typed converter
    /// }
    /// else if (converter is IBindingFallbackConverter fallbackConverter)
    /// {
    ///     // Use fallback converter
    /// }
    /// </code>
    /// </example>
    public object? ResolveConverter(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType)
    {
        ArgumentExceptionHelper.ThrowIfNull(fromType);
        ArgumentExceptionHelper.ThrowIfNull(toType);

        // Phase 1: Try exact type-pair match (typed converters)
        var typed = TypedConverters.TryGetConverter(fromType, toType);
        if (typed is not null)
        {
            return typed;
        }

        // Phase 2: Try fallback converters (runtime type checking)
        var fallback = FallbackConverters.TryGetConverter(fromType, toType);
        return fallback;
    }

    /// <summary>
    /// Resolves the best set-method converter for the specified type pair.
    /// </summary>
    /// <param name="fromType">The source type to convert from. May be null.</param>
    /// <param name="toType">The target type to convert to. May be null.</param>
    /// <returns>
    /// The best set-method converter for the type pair, or <see langword="null"/> if no converter is available.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method queries all registered set-method converters and returns the one with the
    /// highest affinity (&gt; 0) for the specified type pair.
    /// </para>
    /// <para>
    /// This method is thread-safe and lock-free, making it safe to call from multiple threads concurrently.
    /// </para>
    /// </remarks>
    public ISetMethodBindingConverter? ResolveSetMethodConverter(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? fromType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? toType)
    {
        return SetMethodConverters.TryGetConverter(fromType, toType);
    }
}
