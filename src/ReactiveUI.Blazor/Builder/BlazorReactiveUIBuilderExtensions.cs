// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder;

/// <summary>
/// Blazor-specific extensions for the ReactiveUI builder.
/// </summary>
public static class BlazorReactiveUIBuilderExtensions
{
    /// <summary>
    /// Gets the blazor main thread scheduler.
    /// </summary>
    /// <value>
    /// The blazor main thread scheduler.
    /// </value>
    public static IScheduler BlazorMainThreadScheduler { get; } = CurrentThreadScheduler.Instance;

    /// <summary>
    /// Gets the blazor wasm scheduler.
    /// </summary>
    /// <value>
    /// The blazor wasm scheduler.
    /// </value>
    public static IScheduler BlazorWasmScheduler { get; } = WasmScheduler.Default;

    /// <summary>
    /// Configures ReactiveUI for Blazor platform with appropriate schedulers.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithBlazor(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder
            .WithBlazorScheduler()
            .WithTaskPoolScheduler(TaskPoolScheduler.Default)
            .WithPlatformModule<Blazor.Registrations>();
    }

    /// <summary>
    /// Configures ReactiveUI for Blazor platform with appropriate schedulers.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithBlazorWasm(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder
            .WithBlazorWasmScheduler()
            .WithTaskPoolScheduler(TaskPoolScheduler.Default)
            .WithPlatformModule<Blazor.Registrations>();
    }

    /// <summary>
    /// Withes the blazor scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithBlazorScheduler(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithMainThreadScheduler(BlazorMainThreadScheduler);
    }

    /// <summary>
    /// Withes the blazor scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithBlazorWasmScheduler(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithMainThreadScheduler(BlazorWasmScheduler);
    }
}
