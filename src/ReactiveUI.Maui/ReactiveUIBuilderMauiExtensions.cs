// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

namespace ReactiveUI.Maui;

/// <summary>
/// MAUI-specific extensions for ReactiveUIBuilder.
/// </summary>
public static class ReactiveUIBuilderMauiExtensions
{
    /// <summary>
    /// Registers MAUI-specific services.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for method chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("WithMaui uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("WithMaui uses methods that may require unreferenced code")]
#endif
    public static AppBuilder WithMaui(this Builder.ReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithPlatformModule<Registrations>();
    }

    /// <summary>
    /// Registers MAUI-specific services (AOT-friendly shortcut for non-builder code).
    /// </summary>
    /// <param name="resolver">Resolver to register into.</param>
    /// <returns>The resolver for chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("WithMaui uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("WithMaui uses methods that may require unreferenced code")]
#endif
    public static IMutableDependencyResolver WithMaui(this IMutableDependencyResolver resolver)
    {
        resolver.ArgumentNullExceptionThrowIfNull(nameof(resolver));

        // Use the same module the builder uses to avoid duplication.
        var reg = new Registrations();
        reg.Register((f, t) => resolver.RegisterConstant(f(), t));
        return resolver;
    }

    /// <summary>
    /// Registers MAUI-specific services.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for method chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("WithMaui uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("WithMaui uses methods that may require unreferenced code")]
#endif
    public static AppBuilder WithMaui(this AppBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (builder is not Builder.ReactiveUIBuilder reactiveUIBuilder)
        {
            throw new ArgumentException("The builder must be of type ReactiveUIBuilder.", nameof(builder));
        }

        return reactiveUIBuilder.WithPlatformModule<Registrations>();
    }
}
