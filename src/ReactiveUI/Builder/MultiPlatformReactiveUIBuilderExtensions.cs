// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder;

/// <summary>
/// Extensions for configuring multiple platforms with ReactiveUI.
/// </summary>
public static class MultiPlatformReactiveUIBuilderExtensions
{
    /// <summary>
    /// Configures ReactiveUI for multiple platforms simultaneously.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="platformConfigurations">The platform configuration actions.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static ReactiveUIBuilder ForPlatforms(this ReactiveUIBuilder builder, params Action<ReactiveUIBuilder>[] platformConfigurations)
    {
        if (platformConfigurations is null)
        {
            throw new ArgumentNullException(nameof(platformConfigurations));
        }

        foreach (var configurePlatform in platformConfigurations)
        {
            configurePlatform(builder);
        }

        return builder;
    }

    /// <summary>
    /// Configures a custom platform implementation for ReactiveUI.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="mainThreadScheduler">The main thread scheduler for the platform.</param>
    /// <param name="platformServices">The platform-specific service registrations.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static ReactiveUIBuilder ForCustomPlatform(
        this ReactiveUIBuilder builder,
        IScheduler mainThreadScheduler,
        Action<IMutableDependencyResolver> platformServices)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder
            .WithMainThreadScheduler(mainThreadScheduler)
            .WithRegistrationOnBuild(platformServices);
    }
}
