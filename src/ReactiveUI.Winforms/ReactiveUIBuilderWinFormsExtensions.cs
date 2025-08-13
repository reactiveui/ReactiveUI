// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Winforms;

/// <summary>
/// WinForms-specific extensions for ReactiveUIBuilder.
/// </summary>
public static class ReactiveUIBuilderWinFormsExtensions
{
    /// <summary>
    /// Registers WinForms-specific services.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for method chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("WithWinForms uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("WithWinForms uses methods that may require unreferenced code")]
#endif
    public static Builder.ReactiveUIBuilder WithWinForms(this Builder.ReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithPlatformModule<Registrations>();
    }
}
