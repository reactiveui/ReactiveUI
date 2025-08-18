// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

namespace ReactiveUI.WinUI;

/// <summary>
/// WinUI-specific extensions for ReactiveUIBuilder.
/// </summary>
public static class ReactiveUIBuilderWinUIExtensions
{
    /// <summary>
    /// Registers WinUI-specific services.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for method chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("WithWinUI uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("WithWinUI uses methods that may require unreferenced code")]
#endif
    public static AppBuilder WithWinUI(this Builder.ReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithPlatformModule<Registrations>();
    }

    /// <summary>
    /// Registers WinUI-specific services.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for method chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("WithWinUI uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("WithWinUI uses methods that may require unreferenced code")]
#endif
    public static AppBuilder WithWinUI(this AppBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (builder is not Builder.ReactiveUIBuilder reactiveUIBuilder)
        {
            throw new ArgumentException("The builder must be of type ReactiveUIBuilder.", nameof(builder));
        }

        return reactiveUIBuilder.WithPlatformModule<Registrations>();
    }
}
