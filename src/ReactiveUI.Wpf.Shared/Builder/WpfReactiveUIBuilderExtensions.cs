// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Builder;
#else
namespace ReactiveUI.Builder;
#endif
/// <summary>WPF-specific extensions for the ReactiveUI builder.</summary>
public static class WpfReactiveUIBuilderExtensions
{
#if !NET462
    /// <summary>Lazily binds the shared WPF main-thread sequencer to the first UI dispatcher that requests it.</summary>
    private static readonly Lazy<ISequencer> LazyWpfMainThreadScheduler = new(static () =>
        new DispatcherSequencer(System.Windows.Threading.Dispatcher.CurrentDispatcher));
#endif

    /// <summary>Gets the WPF main thread scheduler.</summary>
    /// <value>
    /// The WPF main thread scheduler.
    /// </value>
    public static ISequencer WpfMainThreadScheduler =>
#if NET462
        // System.Reactive 6.x ships no net462 asset, and its netstandard2.0 facade (which net462 resolves to) does
        // not include DispatcherScheduler. Fall back to the current-thread scheduler so net462 compiles; use net472+
        // for true WPF dispatcher marshalling.
        Sequencer.CurrentThread;
#else
        LazyWpfMainThreadScheduler.Value;
#endif

    /// <summary>Provides ReactiveUI builder extension methods for WPF on <see cref="IAppBuilder"/>.</summary>
    /// <param name="builder">The application builder.</param>
    extension(IAppBuilder builder)
    {
        /// <summary>Configures ReactiveUI for WPF platform with appropriate schedulers.</summary>
        /// <returns>The builder instance for chaining.</returns>
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
