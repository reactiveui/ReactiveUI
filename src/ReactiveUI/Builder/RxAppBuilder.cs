// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Builder;

/// <summary>
/// Provides static methods for configuring and initializing ReactiveUI using the builder pattern.
/// </summary>
/// <remarks>RxAppBuilder enables applications to set up ReactiveUI with explicit dependency resolver
/// configuration and initialization. It replaces the legacy RxApp.EnsureInitialized() approach, encouraging use of the
/// builder pattern for clearer and more reliable application startup. For migration guidance and usage examples, see
/// the ReactiveUI documentation.</remarks>
public static class RxAppBuilder
{
#if NET9_0_OR_GREATER
    /// <summary>Synchronizes access to initialization state across threads.</summary>
    private static readonly Lock _resetLock = new();
#else
    /// <summary>Synchronizes access to initialization state across threads.</summary>
    private static readonly object _resetLock = new();
#endif

    /// <summary>Initialization flag: 0 means not initialized, 1 means initialized.</summary>
    private static int _hasBeenInitialized;

    /// <summary>
    /// Creates a ReactiveUI builder with the Splat Locator instance.
    /// </summary>
    /// <returns>The ReactiveUI builder instance.</returns>
    public static ReactiveUIBuilder CreateReactiveUIBuilder() =>
        new(AppLocator.CurrentMutable, AppLocator.Current);

    /// <summary>
    /// Creates a ReactiveUI builder with the specified dependency resolver.
    /// </summary>
    /// <param name="resolver">The dependency resolver to use.</param>
    /// <returns>The ReactiveUI builder instance.</returns>
    public static ReactiveUIBuilder CreateReactiveUIBuilder(this IMutableDependencyResolver resolver)
    {
        if (resolver is null)
        {
            throw new ArgumentNullException(nameof(resolver));
        }

        var readonlyResolver = resolver as IReadonlyDependencyResolver ?? AppLocator.Current;
        return new(resolver, readonlyResolver);
    }

    /// <summary>
    /// Ensures ReactiveUI has been initialized via the builder pattern.
    /// Throws an exception if BuildApp() has not been called.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if ReactiveUI has not been initialized via the builder pattern.</exception>
    /// <remarks>
    /// <para>
    /// This method replaces the old RxApp.EnsureInitialized() pattern.
    /// Call this method at the start of your application or in test setup to verify ReactiveUI is properly initialized.
    /// </para>
    /// <para>
    /// To initialize ReactiveUI, call:
    /// <code>
    /// RxAppBuilder.CreateReactiveUIBuilder()
    ///     .WithCoreServices()
    ///     .BuildApp();
    /// </code>
    /// </para>
    /// </remarks>
    public static void EnsureInitialized()
    {
        lock (_resetLock)
        {
            if (_hasBeenInitialized == 0)
            {
                throw new InvalidOperationException(
                    "ReactiveUI has not been initialized. You must initialize ReactiveUI using the builder pattern. " +
                    "See https://www.reactiveui.net/docs/handbook/rxappbuilder.html for migration guidance.\n\n" +
                    "Example:\n" +
                    "RxAppBuilder.CreateReactiveUIBuilder()\n" +
                    "    .WithCoreServices()\n" +
                    "    .WithPlatformServices()\n" +
                    "    .BuildApp();");
            }
        }
    }

    /// <summary>
    /// Resets the initialization state of ReactiveUI.
    /// This method is intended for testing purposes only.
    /// </summary>
    /// <remarks>
    /// WARNING: This method should ONLY be used in unit tests to reset state between test runs.
    /// Never call this in production code as it can lead to inconsistent application state.
    /// This method is thread-safe and performs all reset operations atomically.
    /// </remarks>
    internal static void ResetForTesting()
    {
        lock (_resetLock)
        {
            RxSchedulers.ResetForTesting();

            Splat.Builder.AppBuilder.ResetBuilderStateForTests();

            AppLocator.SetLocator(new ModernDependencyResolver());

            ViewForMixins.ResetActivationFetcherCacheForTesting();

            _hasBeenInitialized = 0;
        }
    }

    /// <summary>
    /// Marks ReactiveUI as initialized. Called by ReactiveUIBuilder.BuildApp().
    /// </summary>
    internal static void MarkAsInitialized()
    {
        lock (_resetLock)
        {
            _hasBeenInitialized = 1;
        }
    }
}
