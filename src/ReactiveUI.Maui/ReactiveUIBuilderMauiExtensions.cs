// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui;

/// <summary>
/// MAUI-specific extensions for ReactiveUIBuilder.
/// </summary>
public static class ReactiveUIBuilderMauiExtensions
{
    /// <summary>
    /// Registers MAUI-specific services.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public static Builder.ReactiveUIBuilder WithMaui(this Builder.ReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithPlatformModule<Registrations>();
    }
}
