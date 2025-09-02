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
    public static IScheduler WinUIMainThreadScheduler { get; } = new WaitForDispatcherScheduler(() => DispatcherQueueScheduler.Current);

    /// <summary>
    /// Configures ReactiveUI for WinUI platform with appropriate schedulers.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithWinUI(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder
            .WithWinUIScheduler()
            .WithPlatformModule<WinUI.Registrations>();
    }

    /// <summary>
    /// Withes the win UI scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithWinUIScheduler(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithMainThreadScheduler(WinUIMainThreadScheduler);
    }
}
