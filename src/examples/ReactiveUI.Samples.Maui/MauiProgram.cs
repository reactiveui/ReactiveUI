// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Samples.Maui;

/// <summary>
/// MAUI application entry point demonstrating ReactiveUI builder initialization.
/// </summary>
public static class MauiProgram
{
    /// <summary>
    /// Creates and configures the MAUI application with ReactiveUI support.
    /// </summary>
    /// <returns>The configured <see cref="MauiApp"/>.</returns>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseReactiveUI(rxBuilder => rxBuilder.WithMaui());

        return builder.Build();
    }
}
