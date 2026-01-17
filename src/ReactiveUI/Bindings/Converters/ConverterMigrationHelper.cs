// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides helper methods for migrating converters from Splat to the new <see cref="ConverterService"/>.
/// </summary>
/// <remarks>
/// <para>
/// This class assists with migrating from the legacy Splat-based converter registration
/// to the new <see cref="ConverterService"/>-based system introduced in ReactiveUI v20.
/// </para>
/// <para>
/// <strong>When to Use:</strong>
/// </para>
/// <list type="bullet">
/// <item><description>
/// You have existing code that registers converters directly with Splat's <see cref="IMutableDependencyResolver"/>.
/// </description></item>
/// <item><description>
/// You want to preserve existing Splat-registered converters while migrating to the new system.
/// </description></item>
/// <item><description>
/// You need to extract converter instances for inspection or manual registration.
/// </description></item>
/// </list>
/// <para>
/// <strong>Migration Strategies:</strong>
/// </para>
/// <list type="number">
/// <item><description>
/// <strong>Automatic (Recommended):</strong> Use the <c>ReactiveUIBuilder.WithConvertersFrom()</c> method
/// to automatically import all Splat-registered converters during application initialization.
/// </description></item>
/// <item><description>
/// <strong>Manual Extraction:</strong> Use <see cref="ExtractConverters"/> to get all converters
/// from Splat, then inspect or register them manually.
/// </description></item>
/// <item><description>
/// <strong>Direct Import:</strong> Use <see cref="ImportFrom"/> to import converters directly
/// into an existing <see cref="ConverterService"/> instance.
/// </description></item>
/// </list>
/// </remarks>
/// <example>
/// <para>
/// <strong>Example 1: Automatic migration via builder (recommended)</strong>
/// </para>
/// <code>
/// // Import all existing Splat-registered converters automatically
/// RxAppBuilder.CreateReactiveUIBuilder()
///     .WithConvertersFrom(AppLocator.Current)
///     .WithConverter(new AdditionalConverter())  // Add new converters
///     .BuildApp();
/// </code>
/// <para>
/// <strong>Example 2: Manual extraction and inspection</strong>
/// </para>
/// <code>
/// // Extract converters for inspection
/// var (typed, fallback, setMethod) = ConverterMigrationHelper.ExtractConverters(AppLocator.Current);
///
/// Console.WriteLine($"Found {typed.Count} typed converters");
/// Console.WriteLine($"Found {fallback.Count} fallback converters");
/// Console.WriteLine($"Found {setMethod.Count} set-method converters");
///
/// // Register them individually if needed
/// var converterService = new ConverterService();
/// foreach (var converter in typed)
/// {
///     converterService.TypedConverters.Register(converter);
/// }
/// </code>
/// <para>
/// <strong>Example 3: Direct import into existing service</strong>
/// </para>
/// <code>
/// var converterService = RxConverters.Current;
/// converterService.ImportFrom(AppLocator.Current);
/// </code>
/// </example>
public static class ConverterMigrationHelper
{
    /// <summary>
    /// Extracts all converters from a Splat dependency resolver.
    /// </summary>
    /// <param name="resolver">The Splat resolver to extract converters from. Must not be null.</param>
    /// <returns>
    /// A tuple containing lists of typed converters, fallback converters, and set-method converters.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="resolver"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// This method queries the Splat resolver for all registered converters of each type
    /// and returns them as separate lists. Null converter instances are filtered out.
    /// </para>
    /// <para>
    /// The returned lists are new instances - modifying them does not affect the resolver.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var (typed, fallback, setMethod) = ConverterMigrationHelper.ExtractConverters(AppLocator.Current);
    /// Console.WriteLine($"Found {typed.Count} typed converters");
    /// </code>
    /// </example>
    public static (
        IList<IBindingTypeConverter> TypedConverters,
        IList<IBindingFallbackConverter> FallbackConverters,
        IList<ISetMethodBindingConverter> SetMethodConverters)
    ExtractConverters(IReadonlyDependencyResolver resolver)
    {
        ArgumentExceptionHelper.ThrowIfNull(resolver);

        var typed = new List<IBindingTypeConverter>(
            resolver.GetServices<IBindingTypeConverter>().Where(static c => c is not null)!);

        var fallback = new List<IBindingFallbackConverter>(
            resolver.GetServices<IBindingFallbackConverter>().Where(static c => c is not null)!);

        var setMethod = new List<ISetMethodBindingConverter>(
            resolver.GetServices<ISetMethodBindingConverter>().Where(static c => c is not null)!);

        return (typed, fallback, setMethod);
    }

    /// <summary>
    /// Imports converters from a Splat resolver directly into a <see cref="ConverterService"/>.
    /// </summary>
    /// <param name="converterService">The converter service to import into. Must not be null.</param>
    /// <param name="resolver">The Splat resolver to import converters from. Must not be null.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="converterService"/> or <paramref name="resolver"/> is null.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This extension method extracts all converters from the Splat resolver and registers them
    /// with the specified <see cref="ConverterService"/>. This is useful for migrating existing
    /// Splat-based converter registrations to the new system.
    /// </para>
    /// <para>
    /// <strong>Important:</strong> This method imports converters at the time it's called.
    /// Any converters registered with Splat after this call will not be included.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Import all Splat-registered converters into the current service
    /// var converterService = RxConverters.Current;
    /// converterService.ImportFrom(AppLocator.Current);
    ///
    /// // Or create a new service and import into it
    /// var newService = new ConverterService();
    /// newService.ImportFrom(AppLocator.Current);
    /// </code>
    /// </example>
    public static void ImportFrom(
        this ConverterService converterService,
        IReadonlyDependencyResolver resolver)
    {
        ArgumentExceptionHelper.ThrowIfNull(converterService);
        ArgumentExceptionHelper.ThrowIfNull(resolver);

        var (typed, fallback, setMethod) = ExtractConverters(resolver);

        foreach (var converter in typed)
        {
            converterService.TypedConverters.Register(converter);
        }

        foreach (var converter in fallback)
        {
            converterService.FallbackConverters.Register(converter);
        }

        foreach (var converter in setMethod)
        {
            converterService.SetMethodConverters.Register(converter);
        }
    }
}
