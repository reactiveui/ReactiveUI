// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder;

/// <summary>
/// WinUI-specific extensions for the ReactiveUI builder.
/// </summary>
public static class WinUIReactiveUIBuilderExtensions
{
    /// <summary>
    /// Gets the win UI main thread scheduler.
    /// </summary>
    /// <value>
    /// The win UI main thread scheduler.
    /// </value>
    public static IScheduler WinUIMainThreadScheduler { get; } = new WaitForDispatcherScheduler(static () => DispatcherQueueScheduler.Current);

    /// <summary>
    /// Configures ReactiveUI for WinUI platform with appropriate schedulers.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithWinUI(this IReactiveUIBuilder builder)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);

        return ((IReactiveUIBuilder)builder.WithCoreServices())
            .WithWinUIScheduler()
            .WithTaskPoolScheduler(TaskPoolScheduler.Default)
            .WithWinUIConverters()
            .WithPlatformModule<WinUI.Registrations>();
    }

    /// <summary>
    /// Configures the builder to use the WinUI main thread scheduler for reactive operations.
    /// </summary>
    /// <remarks>Use this method when building reactive applications targeting WinUI to ensure that main
    /// thread operations are scheduled appropriately for the WinUI environment.</remarks>
    /// <param name="builder">The builder to configure with the WinUI main thread scheduler. Cannot be null.</param>
    /// <returns>The same builder instance configured to use the WinUI main thread scheduler.</returns>
    public static IReactiveUIBuilder WithWinUIScheduler(this IReactiveUIBuilder builder)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);

        return builder.WithMainThreadScheduler(WinUIMainThreadScheduler);
    }

    /// <summary>
    /// Registers WinUI-specific converters to the ConverterService.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <remarks>
    /// This method registers WinUI-specific converters (<see cref="BooleanToVisibilityTypeConverter"/>,
    /// <see cref="VisibilityToBooleanTypeConverter"/>) and the <see cref="ComponentModelFallbackConverter"/>
    /// to the <c>ConverterService</c> so they are available when using the builder pattern.
    /// </remarks>
    public static IReactiveUIBuilder WithWinUIConverters(this IReactiveUIBuilder builder)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);

        return builder
            .WithConverter(new BooleanToVisibilityTypeConverter())
            .WithConverter(new VisibilityToBooleanTypeConverter())
            .WithFallbackConverter(new ComponentModelFallbackConverter());
    }
}
