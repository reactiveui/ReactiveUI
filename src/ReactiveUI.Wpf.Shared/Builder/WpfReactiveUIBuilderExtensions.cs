// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Splat.Builder;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Builder;
#else
namespace ReactiveUI.Builder;
#endif
/// <summary>WPF-specific extensions for the ReactiveUI builder.</summary>
public static class WpfReactiveUIBuilderExtensions
{
    /// <summary>Gets the WPF main thread scheduler.</summary>
    /// <value>
    /// The WPF main thread scheduler.
    /// </value>
    public static ISequencer WpfMainThreadScheduler { get; } = new WaitForDispatcherScheduler(
        static () =>
        {
            var dispatcher = System.Windows.Application.Current?.Dispatcher
                ?? (System.Threading.Thread.CurrentThread.GetApartmentState() == System.Threading.ApartmentState.STA
                    ? System.Windows.Threading.Dispatcher.CurrentDispatcher
                    : System.Windows.Threading.Dispatcher.FromThread(System.Threading.Thread.CurrentThread)
                        ?? throw new InvalidOperationException("WPF Application has not been initialized yet."));
            return new DispatcherSequencer(dispatcher);
        });

    /// <summary>Provides ReactiveUI builder extension methods for WPF on <see cref="IAppBuilder"/>.</summary>
    /// <param name="builder">The application builder.</param>
    extension(IAppBuilder builder)
    {
        /// <summary>Configures ReactiveUI for WPF platform with appropriate schedulers.</summary>
        /// <returns>The builder instance for chaining.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReactiveUIBuilder WithWpf() => ((IReactiveUIBuilder)builder).WithWpf();
    }

    /// <summary>Provides ReactiveUI builder extension methods for WPF on <see cref="IReactiveUIBuilder"/>.</summary>
    /// <param name="builder">The ReactiveUI builder.</param>
    extension(IReactiveUIBuilder builder)
    {
        /// <summary>Configures ReactiveUI for WPF platform with appropriate schedulers.</summary>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithWpf()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return ((IReactiveUIBuilder)builder.WithCoreServices())
                .WithPlatformModule<Wpf.Registrations>()
                .WithPlatformServices()
                .WithWpfConverters()
                .WithWpfScheduler()
                .WithTaskPoolScheduler(TaskPoolSequencer.Default);
        }

        /// <summary>Withes the WPF scheduler.</summary>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithWpfScheduler()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return builder.WithMainThreadScheduler(WpfMainThreadScheduler);
        }

        /// <summary>Registers WPF-specific converters to the ConverterService.</summary>
        /// <returns>The builder instance for chaining.</returns>
        /// <remarks>
        /// This method registers WPF-specific converters (<see cref="BooleanToVisibilityTypeConverter"/>,
        /// <see cref="VisibilityToBooleanTypeConverter"/>) and the <see cref="ComponentModelFallbackConverter"/>
        /// to the <c>ConverterService</c> so they are available when using the builder pattern.
        /// </remarks>
        public IReactiveUIBuilder WithWpfConverters()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return builder
                .WithConverter(new BooleanToVisibilityTypeConverter())
                .WithConverter(new VisibilityToBooleanTypeConverter())
                .WithFallbackConverter(new ComponentModelFallbackConverter());
        }
    }
}
