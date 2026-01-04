// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder;

/// <summary>
/// Extension methods for configuring ReactiveUI with the Splat builder.
/// </summary>
public static class RxAppBuilder
{
    private static int _hasBeenInitialized; // 0 = false, 1 = true

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
        if (Volatile.Read(ref _hasBeenInitialized) == 0)
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

    /// <summary>
    /// Marks ReactiveUI as initialized. Called by ReactiveUIBuilder.BuildApp().
    /// </summary>
    internal static void MarkAsInitialized()
    {
        Interlocked.Exchange(ref _hasBeenInitialized, 1);
    }
}
