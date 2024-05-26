// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Mopups.Hosting;
using ReactiveUI;
using ReactiveUI.Maui.Plugins.Popup;
using Splat;

namespace Microsoft.Maui.Hosting;

/// <summary>
/// INavigation Mixins.
/// </summary>
public static class MauiAppBuilderMixins
{
    /// <summary>
    /// Registers all the default registrations that are needed by the Splat module.
    /// Initialize resolvers with the default ReactiveUI types.
    /// Configures ReactiveUI Maui Mopups.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>MauiAppBuilder.</returns>
    public static MauiAppBuilder ConfigureReactiveUIPopup(this MauiAppBuilder builder)
    {
        builder.ConfigureMopups();
        var resolver = Locator.CurrentMutable;
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI(RegistrationNamespace.Maui);
        return builder;
    }

    /// <summary>
    /// Configures the reactive UI mopups.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="backPressHandler">The back press handler.</param>
    /// <returns>MauiAppBuilder.</returns>
    public static MauiAppBuilder ConfigureReactiveUIPopup(this MauiAppBuilder builder, Action? backPressHandler)
    {
        builder.ConfigureMopups(backPressHandler);
        var resolver = Locator.CurrentMutable;
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI(RegistrationNamespace.Maui);
        return builder;
    }
}
