// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

#if REACTIVE_SHIM
using WinFormsBindingModule = ReactiveUI.Binding.Reactive.WinForms.WinFormsBindingModule;

namespace ReactiveUI.Reactive.Builder;
#else
using WinFormsBindingModule = ReactiveUI.Binding.WinForms.WinFormsBindingModule;

namespace ReactiveUI.Builder;
#endif

/// <summary>WinForms-specific extensions for the ReactiveUI builder.</summary>
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "ReactiveUI deliberate")]
public static class WinFormsReactiveUIBuilderExtensions
{
    /// <summary>Gets the win forms main thread scheduler.</summary>
    /// <value>
    /// The shared <see cref="ControlSequencer.Main"/> sequencer for the Windows Forms UI thread.
    /// </value>
    /// <exception cref="InvalidOperationException">The sequencer is not bound yet and the calling thread is not an STA thread.</exception>
    public static ISequencer WinFormsMainThreadScheduler => ControlSequencer.Main;

    /// <summary>Provides ReactiveUI builder extension methods for WinForms.</summary>
    /// <param name="builder">The ReactiveUI builder.</param>
    extension(IReactiveUIBuilder builder)
    {
        /// <summary>Configures ReactiveUI for WinForms platform with appropriate schedulers.</summary>
        /// <returns>The builder instance for chaining.</returns>
#if NET6_0_OR_GREATER
        [SuppressMessage(
            "Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "Not using reflection")]
        [SuppressMessage(
            "AOT",
            "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "Not using reflection")]
#endif
        public IReactiveUIBuilder WithWinForms()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return ((IReactiveUIBuilder)builder.WithCoreServices())
                .WithMainThreadScheduler(WinFormsMainThreadScheduler)
                .WithTaskPoolScheduler(TaskPoolSequencer.Default)
                .WithPlatformModule<Winforms.Registrations>()
                .UsingSplatModule(new WinFormsBindingModule());
        }

        /// <summary>Withes the win UI scheduler.</summary>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithWinFormsScheduler()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            return builder.WithMainThreadScheduler(WinFormsMainThreadScheduler);
        }
    }
}
