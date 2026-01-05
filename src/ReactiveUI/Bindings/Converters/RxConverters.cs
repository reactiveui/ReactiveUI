// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides static access to the ReactiveUI converter service.
/// </summary>
/// <remarks>
/// <para>
/// This class provides a global access point to the <see cref="ConverterService"/> instance
/// used by ReactiveUI for binding type conversions. It is initialized during application
/// startup via the <see cref="ReactiveUIBuilder"/> pattern.
/// </para>
/// <para>
/// <strong>Recommended Usage:</strong>
/// </para>
/// <para>
/// In most cases, you don't need to access converters directly - the binding system
/// uses them automatically. However, if you need to manually resolve a converter or
/// register custom converters after initialization, use this class.
/// </para>
/// <para>
/// <strong>Initialization:</strong>
/// </para>
/// <para>
/// The converter service is initialized when you call <c>ReactiveUIBuilder.BuildApp()</c>:
/// </para>
/// <code>
/// RxAppBuilder.CreateReactiveUIBuilder()
///     .WithCoreServices()
///     .WithPlatformServices()
///     .BuildApp();
/// </code>
/// <para>
/// <strong>Custom Converter Registration:</strong>
/// </para>
/// <para>
/// Register custom converters during builder configuration (recommended):
/// </para>
/// <code>
/// RxAppBuilder.CreateReactiveUIBuilder()
///     .WithConverter(new MyCustomConverter())
///     .BuildApp();
/// </code>
/// <para>
/// Or register after initialization (not recommended, but supported):
/// </para>
/// <code>
/// RxConverters.Current.TypedConverters.Register(new MyCustomConverter());
/// </code>
/// </remarks>
/// <example>
/// <para>
/// <strong>Example: Manually resolving a converter</strong>
/// </para>
/// <code>
/// var converter = RxConverters.Current.ResolveConverter(typeof(int), typeof(string));
/// if (converter is IBindingTypeConverter&lt;int, string&gt; typedConverter)
/// {
///     if (typedConverter.TryConvert(42, null, out var result))
///     {
///         Console.WriteLine(result); // "42"
///     }
/// }
/// </code>
/// </example>
public static class RxConverters
{
    /// <summary>
    /// Backing field for the converter service.
    /// </summary>
    private static ConverterService _current = new();

    /// <summary>
    /// Gets the current converter service instance.
    /// </summary>
    /// <value>
    /// The <see cref="ConverterService"/> instance used by ReactiveUI.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property provides access to the converter service for manual converter
    /// resolution or registration. In most cases, you don't need to use this directly
    /// as the binding system handles converter selection automatically.
    /// </para>
    /// <para>
    /// The service is initialized during application startup via <see cref="ReactiveUIBuilder"/>.
    /// If you access this property before calling <c>BuildApp()</c>, you'll get an empty
    /// service with no converters registered.
    /// </para>
    /// </remarks>
    public static ConverterService Current => _current;

    /// <summary>
    /// Sets the converter service instance.
    /// </summary>
    /// <param name="service">The converter service to use. Must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// This method is called internally by <see cref="ReactiveUIBuilder"/> during application
    /// initialization. Application code should not call this method directly.
    /// </para>
    /// <para>
    /// <strong>For Testing:</strong> Unit tests can call this method to inject a test service
    /// with mock converters, but should restore the original service after the test completes.
    /// </para>
    /// </remarks>
    internal static void SetService(ConverterService service)
    {
        ArgumentExceptionHelper.ThrowIfNull(service);
        _current = service;
    }
}
