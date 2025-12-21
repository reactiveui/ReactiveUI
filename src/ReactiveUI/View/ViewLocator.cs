// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides access to the <see cref="IViewLocator"/> registered in the global dependency resolver.
/// </summary>
/// <remarks>
/// <para>
/// The locator is resolved from <see cref="AppLocator.Current"/>. Applications typically configure the container via
/// <c>UseReactiveUI</c> or <c>services.AddReactiveUI()</c>, which registers the default locator and any platform views.
/// </para>
/// <para>
/// Accessing <see cref="Current"/> throws <see cref="ViewLocatorNotFoundException"/> when no locator has been registered,
/// which usually indicates the app skipped ReactiveUI initialization or trimmed required assemblies.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// var locator = ViewLocator.Current;
/// var view = locator.ResolveView(shell.Router.GetCurrentViewModel());
/// ]]>
/// </code>
/// </example>
public static class ViewLocator
{
    /// <summary>
    /// Gets the currently registered <see cref="IViewLocator"/> from <see cref="AppLocator.Current"/>.
    /// </summary>
    /// <exception cref="ViewLocatorNotFoundException">
    /// Thrown when no locator has been registered with the dependency resolver. Ensure ReactiveUI initialization
    /// has run and required assemblies are referenced.
    /// </exception>
    [SuppressMessage("Microsoft.Reliability", "CA1065", Justification = "Exception required to keep interface same.")]
    public static IViewLocator Current =>
        AppLocator.Current.GetService<IViewLocator>() ?? throw new ViewLocatorNotFoundException("Could not find a default ViewLocator. This should never happen, your dependency resolver is broken");
}
