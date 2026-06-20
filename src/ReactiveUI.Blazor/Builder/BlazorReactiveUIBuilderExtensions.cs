// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Builder;
#else
namespace ReactiveUI.Builder;
#endif
/// <summary>Blazor-specific extensions for the ReactiveUI builder.</summary>
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "ReactiveUI is the name of the product.")]
public static class BlazorReactiveUIBuilderExtensions
{
    /// <summary>Gets the blazor main thread scheduler.</summary>
    /// <value>
    /// The blazor main thread scheduler.
    /// </value>
    public static ISequencer BlazorMainThreadScheduler { get; } = Sequencer.CurrentThread;

    /// <summary>Gets the blazor wasm scheduler.</summary>
    /// <value>
    /// The blazor wasm scheduler.
    /// </value>
    public static ISequencer BlazorWasmScheduler { get; } =
#if REACTIVE_SHIM
        WasmScheduler.Default;
#else
        Sequencer.CurrentThread;
#endif

    /// <summary>Provides ReactiveUI builder extension methods for Blazor.</summary>
    /// <param name="builder">The ReactiveUI builder.</param>
    extension(IReactiveUIBuilder builder)
    {
        /// <summary>Configures ReactiveUI for Blazor platform with appropriate schedulers.</summary>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithBlazor()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return ((IReactiveUIBuilder)builder.WithCoreServices())
                .WithBlazorScheduler()
                .WithTaskPoolScheduler(TaskPoolSequencer.Default)
                .WithPlatformModule<Blazor.Registrations>();
        }

        /// <summary>Configures ReactiveUI for Blazor platform with appropriate schedulers.</summary>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithBlazorWasm()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return builder
                .WithBlazorWasmScheduler()
                .WithTaskPoolScheduler(TaskPoolSequencer.Default)
                .WithPlatformModule<Blazor.Registrations>();
        }

        /// <summary>Withes the blazor scheduler.</summary>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithBlazorScheduler()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return builder.WithMainThreadScheduler(BlazorMainThreadScheduler);
        }

        /// <summary>Withes the blazor scheduler.</summary>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithBlazorWasmScheduler()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return builder.WithMainThreadScheduler(BlazorWasmScheduler);
        }
    }
}
