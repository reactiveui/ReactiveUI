﻿// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Dispatching;

namespace ReactiveUI.Builder;

/// <summary>
/// MAUI-specific extensions for the ReactiveUI builder.
/// </summary>
public static class MauiReactiveUIBuilderExtensions
{
    /// <summary>
    /// Gets the maui main thread scheduler.
    /// </summary>
    /// <value>
    /// The maui main thread scheduler.
    /// </value>
    public static IScheduler MauiMainThreadScheduler { get; } = DefaultScheduler.Instance;

    /// <summary>
    /// Configures ReactiveUI for MAUI platform with appropriate schedulers.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="dispatcher">The MAUI dispatcher to use for the main thread scheduler.</param>
    /// <returns>The builder instance for chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    public static IReactiveUIBuilder WithMaui(this IReactiveUIBuilder builder, IDispatcher? dispatcher = null)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder
            .WithMauiScheduler()
            .WithPlatformModule<Maui.Registrations>();
    }

    /// <summary>
    /// Withes the maui scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithMauiScheduler(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithMainThreadScheduler(MauiMainThreadScheduler);
    }
}
