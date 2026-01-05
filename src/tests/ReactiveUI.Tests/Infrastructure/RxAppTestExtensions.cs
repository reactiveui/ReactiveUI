// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI.Tests.Infrastructure;

/// <summary>
/// Extension methods for testing ReactiveUI initialization and reset.
/// </summary>
internal static class RxAppTestExtensions
{
    /// <summary>
    /// Resets ReactiveUI state and reinitializes with core services using a fresh locator.
    /// </summary>
    /// <remarks>
    /// This method:
    /// 1. Resets the ReactiveUI initialization state.
    /// 2. Creates a new ModernDependencyResolver.
    /// 3. Initializes Splat with it.
    /// 4. Sets it as the current locator.
    /// 5. Initializes ReactiveUI with core services.
    /// </remarks>
    public static void ResetAndReinitialize()
    {
        // Reset the initialization flag
        RxAppBuilder.ResetForTesting();

        // Create a fresh dependency resolver
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        // Set it as the current locator
        AppLocator.SetLocator(resolver);

        // Initialize ReactiveUI with core services
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();
    }

    /// <summary>
    /// Resets ReactiveUI state only (does not reinitialize).
    /// </summary>
    /// <remarks>
    /// Use this when you want to manually control the initialization afterward,
    /// such as when creating a custom resolver for a specific test.
    /// </remarks>
    public static void ResetState()
    {
        RxAppBuilder.ResetForTesting();
    }
}
