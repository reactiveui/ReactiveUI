// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Builder;
#else
namespace ReactiveUI.Builder;
#endif

/// <summary>WinUI-specific extensions for the ReactiveUI builder.</summary>
public static class WinUIReactiveUIBuilderExtensions
{
    /// <summary>Gets the win UI main thread scheduler.</summary>
    /// <value>
    /// The shared <see cref="DispatcherQueueSequencer.Main"/> sequencer for the WinUI UI thread.
    /// </value>
    /// <exception cref="InvalidOperationException">The sequencer is not bound yet and the calling thread has no dispatcher queue.</exception>
    public static ISequencer WinUIMainThreadScheduler => DispatcherQueueSequencer.Main;

    /// <summary>Provides ReactiveUI builder extension methods for WinUI.</summary>
    /// <param name="builder">The ReactiveUI builder.</param>
    extension(IReactiveUIBuilder builder)
    {
        /// <summary>Configures ReactiveUI for WinUI platform with appropriate schedulers.</summary>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithWinUI()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return ((IReactiveUIBuilder)builder.WithCoreServices())
                .WithWinUIScheduler()
                .WithTaskPoolScheduler(TaskPoolSequencer.Default)
                .WithWinUIConverters()
                .WithPlatformModule<WinUI.Registrations>();
        }

        /// <summary>Configures the builder to use the WinUI main thread scheduler for reactive operations.</summary>
        /// <returns>The same builder instance configured to use the WinUI main thread scheduler.</returns>
        /// <remarks>Use this method when building reactive applications targeting WinUI to ensure that main
        /// thread operations are scheduled appropriately for the WinUI environment.</remarks>
        public IReactiveUIBuilder WithWinUIScheduler()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return builder.WithMainThreadScheduler(WinUIMainThreadScheduler);
        }

        /// <summary>Registers WinUI-specific converters to the ConverterService.</summary>
        /// <returns>The builder instance for chaining.</returns>
        /// <remarks>
        /// This method registers WinUI-specific converters (<see cref="BooleanToVisibilityTypeConverter"/>,
        /// <see cref="VisibilityToBooleanTypeConverter"/>) and the <see cref="ComponentModelFallbackConverter"/>
        /// to the <c>ConverterService</c> so they are available when using the builder pattern.
        /// </remarks>
        public IReactiveUIBuilder WithWinUIConverters()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return builder
                .WithConverter(new BooleanToVisibilityTypeConverter())
                .WithConverter(new VisibilityToBooleanTypeConverter())
                .WithFallbackConverter(new ComponentModelFallbackConverter());
        }
    }
}
