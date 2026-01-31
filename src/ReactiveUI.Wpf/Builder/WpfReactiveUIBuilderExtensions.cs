// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

namespace ReactiveUI.Builder;

/// <summary>
/// WPF-specific extensions for the ReactiveUI builder.
/// </summary>
public static class WpfReactiveUIBuilderExtensions
{
    /// <summary>
    /// Gets the WPF main thread scheduler.
    /// </summary>
    /// <value>
    /// The WPF main thread scheduler.
    /// </value>
    public static IScheduler WpfMainThreadScheduler { get; } = new WaitForDispatcherScheduler(static () => DispatcherScheduler.Current);

    /// <summary>
    /// Configures ReactiveUI for WPF platform with appropriate schedulers.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithWpf(this IReactiveUIBuilder builder)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);

        return builder
            .WithPlatformModule<Wpf.Registrations>()
            .WithPlatformServices()
            .WithWpfScheduler()
            .WithTaskPoolScheduler(TaskPoolScheduler.Default);
    }

    /// <summary>
    /// Configures ReactiveUI for WPF platform with appropriate schedulers.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithWpf(this IAppBuilder builder) => ((IReactiveUIBuilder)builder).WithWpf();

    /// <summary>
    /// Withes the WPF scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithWpfScheduler(this IReactiveUIBuilder builder)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);

        return builder.WithMainThreadScheduler(WpfMainThreadScheduler);
    }
}
