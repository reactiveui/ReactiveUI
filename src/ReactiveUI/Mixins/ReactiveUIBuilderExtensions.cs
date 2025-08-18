// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using Splat.Builder;

namespace ReactiveUI;

/// <summary>
/// Extension methods for ReactiveUI Builder functionality.
/// </summary>
public static class ReactiveUIBuilderExtensions
{
    /// <summary>
    /// Creates a builder for configuring ReactiveUI without using reflection.
    /// This provides an AOT-compatible alternative to the reflection-based InitializeReactiveUI method.
    /// </summary>
    /// <param name="resolver">The dependency resolver to configure.</param>
    /// <returns>A ReactiveUIBuilder instance for fluent configuration.</returns>
 #if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Does not use reflection")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Members annotated with 'RequiresDynamicCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Does not use reflection")]
#endif
    public static AppBuilder CreateBuilder(this IMutableDependencyResolver resolver)
    {
        resolver.ArgumentNullExceptionThrowIfNull(nameof(resolver));
        var builder = new Builder.ReactiveUIBuilder(resolver);

        // Queue core registrations by default so Build() always provides the basics
        builder.WithCoreServices();
        return builder;
    }

    /// <summary>
    /// Automatically registers all views that implement IViewFor from the specified assembly.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly to scan for views.</param>
    /// <returns>
    /// The builder instance for method chaining.
    /// </returns>
    /// <exception cref="System.ArgumentException">The builder must be of type ReactiveUIBuilder. - builder.</exception>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("WithViewsFromAssembly uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("WithViewsFromAssembly uses methods that may require unreferenced code")]
#endif
    public static AppBuilder WithViewsFromAssembly(this AppBuilder builder, Assembly assembly)
        {
        builder.ArgumentNullExceptionThrowIfNull(nameof(builder));
        assembly.ArgumentNullExceptionThrowIfNull(nameof(assembly));

        if (builder is not Builder.ReactiveUIBuilder reactiveUIBuilder)
        {
            throw new ArgumentException("The builder must be of type ReactiveUIBuilder.", nameof(builder));
        }

        return reactiveUIBuilder.WithViewsFromAssembly(assembly);
    }

    /// <summary>
    /// Registers the platform-specific ReactiveUI services.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>
    /// The builder instance for method chaining.
    /// </returns>
    /// <exception cref="System.ArgumentException">The builder must be of type ReactiveUIBuilder. - builder.</exception>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("WithPlatformServices may use reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("WithPlatformServices may use reflection and will not work in AOT environments.")]
#endif
    public static AppBuilder WithPlatformServices(this AppBuilder builder)
    {
        builder.ArgumentNullExceptionThrowIfNull(nameof(builder));

        if (builder is not Builder.ReactiveUIBuilder reactiveUIBuilder)
        {
            throw new ArgumentException("The builder must be of type ReactiveUIBuilder.", nameof(builder));
        }

        return reactiveUIBuilder.WithPlatformServices();
    }

    /// <summary>
    /// Registers a view for a view model via generics without reflection.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="builder">The builder.</param>
    /// <param name="contract">An optional contract.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static AppBuilder RegisterViewForViewModel<TView, TViewModel>(this AppBuilder builder, string? contract = null)
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class
    {
        builder.ArgumentNullExceptionThrowIfNull(nameof(builder));
        if (builder is not Builder.ReactiveUIBuilder reactiveUIBuilder)
        {
            throw new ArgumentException("The builder must be of type ReactiveUIBuilder.", nameof(builder));
        }

        return reactiveUIBuilder.RegisterViewForViewModel<TView, TViewModel>(contract);
    }

    /// <summary>
    /// Registers a singleton view for a view model via generics without reflection.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="builder">The builder.</param>
    /// <param name="contract">An optional contract.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static AppBuilder RegisterSingletonViewForViewModel<TView, TViewModel>(this AppBuilder builder, string? contract = null)
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class
    {
        builder.ArgumentNullExceptionThrowIfNull(nameof(builder));
        if (builder is not Builder.ReactiveUIBuilder reactiveUIBuilder)
        {
            throw new ArgumentException("The builder must be of type ReactiveUIBuilder.", nameof(builder));
        }

        return reactiveUIBuilder.RegisterSingletonViewForViewModel<TView, TViewModel>(contract);
    }
}
