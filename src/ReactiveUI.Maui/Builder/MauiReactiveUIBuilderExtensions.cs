// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Hosting;
#if REACTIVE_SHIM
using ReactiveUI.Reactive.Maui;
#else
using ReactiveUI.Maui;
#endif
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Builder;
#else
namespace ReactiveUI.Builder;
#endif

/// <summary>MAUI-specific extensions for the ReactiveUI builder.</summary>
public static class MauiReactiveUIBuilderExtensions
{
    /// <summary>Gets the MAUI main thread scheduler.</summary>
    /// <value>
    /// The MAUI main thread scheduler.
    /// </value>
    public static ISequencer MauiMainThreadScheduler =>
        Dispatcher.GetForCurrentThread() is { } dispatcher ? dispatcher.ToSequencer() : Sequencer.Default;

    /// <summary>Provides MAUI-specific configuration extension members for <see cref="IReactiveUIBuilder"/>.</summary>
    /// <param name="builder">The builder instance.</param>
    extension(IReactiveUIBuilder builder)
    {
        /// <summary>Configures ReactiveUI for MAUI platform with appropriate schedulers and platform services.</summary>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithMaui() => builder.WithMaui(null);

        /// <summary>Configures ReactiveUI for MAUI platform with appropriate schedulers and platform services.</summary>
        /// <param name="dispatcher">The MAUI dispatcher to use for the main thread scheduler.</param>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithMaui(IDispatcher? dispatcher)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return ((IReactiveUIBuilder)builder.WithCoreServices())
                .WithMauiScheduler(dispatcher)
                .WithTaskPoolScheduler(TaskPoolSequencer.Default)
                .WithPlatformModule<Registrations>()
                .WithMauiConverters()
                .WithPlatformServices();
        }

        /// <summary>Adds the MAUI scheduler.</summary>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithMauiScheduler() => builder.WithMauiScheduler(null);

        /// <summary>Adds the MAUI scheduler.</summary>
        /// <param name="dispatcher">Optional dispatcher instance to derive the scheduler from.</param>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithMauiScheduler(IDispatcher? dispatcher)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            builder.WithTaskPoolScheduler(TaskPoolSequencer.Default);
            return builder.WithMainThreadScheduler(ResolveMainThreadScheduler(dispatcher));
        }

        /// <summary>Registers Maui-specific converters to the ConverterService.</summary>
        /// <returns>The builder instance for chaining.</returns>
        /// <remarks>
        /// This method registers Maui-specific converters (<see cref="BooleanToVisibilityTypeConverter"/>,
        /// <see cref="VisibilityToBooleanTypeConverter"/>) and the <see cref="ComponentModelFallbackConverter"/>
        /// to the <c>ConverterService</c> so they are available when using the builder pattern.
        /// </remarks>
        public IReactiveUIBuilder WithMauiConverters()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return builder
                .WithConverter(new BooleanToVisibilityTypeConverter())
                .WithConverter(new VisibilityToBooleanTypeConverter())
                .WithFallbackConverter(new ComponentModelFallbackConverter());
        }
    }

    /// <summary>Provides MAUI app-builder extension members for <see cref="MauiAppBuilder"/>.</summary>
    /// <param name="builder">The builder.</param>
    extension(MauiAppBuilder builder)
    {
        /// <summary>Uses the reactive UI.</summary>
        /// <param name="withReactiveUIBuilder">The reactive UI builder.</param>
        /// <returns>A The builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public MauiAppBuilder UseReactiveUI(Action<IReactiveUIBuilder> withReactiveUIBuilder)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            var reactiveUIBuilder = RxAppBuilder.CreateReactiveUIBuilder();
            withReactiveUIBuilder?.Invoke(reactiveUIBuilder);
            reactiveUIBuilder.BuildApp();
            return builder;
        }

        /// <summary>Uses the reactive UI.</summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <returns>A The builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public MauiAppBuilder UseReactiveUI(IDispatcher dispatcher)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            RxAppBuilder.CreateReactiveUIBuilder().WithMaui(dispatcher).BuildApp();
            return builder;
        }
    }

    /// <summary>Resolves the main thread scheduler to use based on the current platform and supplied dispatcher.</summary>
    /// <param name="dispatcher">Optional dispatcher to derive the scheduler from.</param>
    /// <returns>The resolved main thread scheduler.</returns>
    private static ISequencer ResolveMainThreadScheduler(IDispatcher? dispatcher)
    {
        if (dispatcher is not null)
        {
            return dispatcher.ToSequencer();
        }

        if (ModeDetector.InUnitTestRunner())
        {
            return Sequencer.CurrentThread;
        }

        return MauiMainThreadScheduler;
    }
}
