// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder;

/// <summary>
/// WinForms-specific extensions for the ReactiveUI builder.
/// </summary>
public static class WinFormsReactiveUIBuilderExtensions
{
    /// <summary>
    /// Gets the win forms main thread scheduler.
    /// </summary>
    /// <value>
    /// The win forms main thread scheduler.
    /// </value>
    public static IScheduler WinFormsMainThreadScheduler { get; } = new WaitForDispatcherScheduler(static () => new SynchronizationContextScheduler(new WindowsFormsSynchronizationContext()));

    /// <summary>
    /// Configures ReactiveUI for WinForms platform with appropriate schedulers.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithWinForms(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder
            .WithMainThreadScheduler(WinFormsMainThreadScheduler)
            .WithPlatformModule<Winforms.Registrations>();
    }

    /// <summary>
    /// Withes the win UI scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithWinFormsScheduler(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithMainThreadScheduler(WinFormsMainThreadScheduler);
    }
}
