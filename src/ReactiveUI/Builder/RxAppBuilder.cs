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
}
