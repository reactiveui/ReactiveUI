// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Builder;
#else
namespace ReactiveUI.Builder;
#endif

/// <summary>Reflection-based view registration extensions for <see cref="IReactiveUIBuilder"/>.</summary>
public static class UnsafeReactiveUIBuilderExtensions
{
    /// <summary>Provides reflection-based assembly scanning extensions for <see cref="IReactiveUIBuilder"/>.</summary>
    /// <param name="builder">The builder instance.</param>
    extension(IReactiveUIBuilder builder)
    {
        /// <summary>Automatically registers all views that implement <see cref="IViewFor"/> from the specified assembly.</summary>
        /// <param name="assembly">The assembly to scan for views.</param>
        /// <returns>The builder instance for method chaining.</returns>
        [RequiresUnreferencedCode(
            "Scans assembly for IViewFor implementations using reflection. For AOT compatibility, use the ReactiveUIBuilder pattern to RegisterView explicitly.")]
        public IReactiveUIBuilder WithViewsFromAssembly(Assembly assembly)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);
            ArgumentExceptionHelper.ThrowIfNull(assembly);

            return builder.WithRegistration(resolver => resolver.RegisterViewsForViewModels(assembly));
        }
    }
}
