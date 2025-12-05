// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Helper class which will get the currently registered IViewLocator interface inside
/// the Splat dependency injection container.
/// </summary>
public static class ViewLocator
{
    /// <summary>
    /// Gets the currently registered IViewLocator interface.
    /// </summary>
    /// <exception cref="Exception">
    /// If there is no IViewLocator registered.
    /// Can happen due to using your own DI container and don't rerun the
    /// DependencyResolverMixins.InitializeReactiveUI() method.
    /// Also can happen if you don't include all the NuGet packages.
    /// </exception>
    [SuppressMessage("Microsoft.Reliability", "CA1065", Justification = "Exception required to keep interface same.")]
    public static IViewLocator Current =>
        AppLocator.Current.GetService<IViewLocator>() ?? throw new ViewLocatorNotFoundException("Could not find a default ViewLocator. This should never happen, your dependency resolver is broken");
}
