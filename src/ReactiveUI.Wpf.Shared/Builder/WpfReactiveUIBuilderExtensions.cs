// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Splat.Builder;

#if REACTIVE_SHIM
using WpfBindingModule = ReactiveUI.Binding.Reactive.Wpf.WpfBindingModule;

namespace ReactiveUI.Reactive.Builder;
#else
using WpfBindingModule = ReactiveUI.Binding.Wpf.WpfBindingModule;

namespace ReactiveUI.Builder;
#endif
/// <summary>WPF-specific extensions for the ReactiveUI builder.</summary>
public static class WpfReactiveUIBuilderExtensions
{
    /// <summary>Gets the WPF main thread scheduler.</summary>
    /// <value>
    /// The shared <see cref="DispatcherSequencer.Main"/> sequencer for the WPF UI thread.
    /// </value>
    /// <exception cref="InvalidOperationException">No application exists yet and the calling thread has no dispatcher.</exception>
    public static ISequencer WpfMainThreadScheduler => DispatcherSequencer.Main;

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
                .UsingSplatModule(new WpfBindingModule())
                .WithPlatformServices()
                .WithWpfConverters()
                .WithWpfScheduler()
                .WithTaskPoolScheduler(TaskPoolSequencer.Default);
        }

        /// <summary>Sets the main thread scheduler to the WPF UI thread.</summary>
        /// <returns>The builder instance for chaining.</returns>
        /// <remarks>
        /// When no <see cref="System.Windows.Application"/> exists yet, for example when a generic host is built first,
        /// the scheduler finds the UI thread when it is first used.
        /// </remarks>
        public IReactiveUIBuilder WithWpfScheduler()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return builder.WithMainThreadScheduler(
                System.Windows.Application.Current is null ? DeferredWpfMainThreadSequencer.Instance : WpfMainThreadScheduler);
        }

        /// <summary>Registers WPF-specific converters to the ConverterService.</summary>
        /// <returns>The builder instance for chaining.</returns>
        /// <remarks>
        /// This method registers WPF-specific converters (<see cref="BooleanToVisibilityTypeConverter"/> and
        /// <see cref="VisibilityToBooleanTypeConverter"/>) with the binding converter service.
        /// </remarks>
        public IReactiveUIBuilder WithWpfConverters()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return builder
                .WithConverter(new BooleanToVisibilityTypeConverter())
                .WithConverter(new VisibilityToBooleanTypeConverter());
        }
    }
}
