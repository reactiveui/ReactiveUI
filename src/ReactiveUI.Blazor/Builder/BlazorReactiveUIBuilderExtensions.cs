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
    /// <summary>Gets the Blazor Server scheduler.</summary>
    /// <value>
    /// A current-thread scheduler. Renderer-affine work remains component-scoped and is marshalled through
    /// <c>ComponentBase.InvokeAsync</c> because Blazor Server has one renderer dispatcher per circuit.
    /// </value>
    public static ISequencer BlazorMainThreadScheduler { get; } = Sequencer.CurrentThread;

    /// <summary>Gets the browser WebAssembly event-loop scheduler.</summary>
    /// <value>
    /// The Primitives scheduler that yields work through the WebAssembly event loop without creating threads.
    /// </value>
    public static ISequencer BlazorWasmScheduler { get; } =
#if REACTIVE_SHIM
        WasmScheduler.Default;
#else
        WasmSequencer.Default;
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

            return ((IReactiveUIBuilder)builder.WithCoreServices())
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
