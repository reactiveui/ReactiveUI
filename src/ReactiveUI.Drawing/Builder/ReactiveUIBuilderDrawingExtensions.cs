// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Builder;
#else
namespace ReactiveUI.Builder;
#endif

/// <summary>Drawing-specific extensions for ReactiveUIBuilder.</summary>
public static class ReactiveUIBuilderDrawingExtensions
{
    /// <summary>Provides ReactiveUI builder extension methods for Drawing.</summary>
    /// <param name="builder">The ReactiveUI builder.</param>
    extension(IReactiveUIBuilder builder)
    {
        /// <summary>Registers Drawing-specific services.</summary>
        /// <returns>The builder instance for method chaining.</returns>
        public IReactiveUIBuilder WithDrawing()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return builder.WithPlatformModule<Drawing.Registrations>();
        }
    }
}
