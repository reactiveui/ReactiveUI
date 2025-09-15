// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;

namespace ReactiveUI.Builder;

/// <summary>
/// AndroidX-specific extensions for the ReactiveUI builder.
/// </summary>
public static class AndroidXReactiveUIBuilderExtensions
{
    /// <summary>
    /// Gets the android x main thread scheduler.
    /// </summary>
    /// <value>
    /// The android x main thread scheduler.
    /// </value>
    public static IScheduler AndroidXMainThreadScheduler { get; } = HandlerScheduler.MainThreadScheduler;

    /// <summary>
    /// Configures ReactiveUI for AndroidX platform with appropriate schedulers.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Uses reflection to create instances of types.")]
    [RequiresDynamicCode("Uses reflection to create instances of types.")]
#endif
    public static IReactiveUIBuilder WithAndroidX(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder
            .WithMainThreadScheduler(HandlerScheduler.MainThreadScheduler)
            .WithPlatformModule<AndroidX.Registrations>();
    }

    /// <summary>
    /// Withes the android x scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithAndroidXScheduler(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithMainThreadScheduler(AndroidXMainThreadScheduler);
    }
}
