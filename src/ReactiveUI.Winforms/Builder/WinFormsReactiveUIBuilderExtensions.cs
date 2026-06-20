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

/// <summary>WinForms-specific extensions for the ReactiveUI builder.</summary>
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "ReactiveUI deliberate")]
public static class WinFormsReactiveUIBuilderExtensions
{
    /// <summary>Lazily creates the hidden control used to marshal the shared WinForms main-thread sequencer.</summary>
    private static readonly Lazy<ISequencer> LazyWinFormsMainThreadScheduler = new(static () =>
    {
        var control = new Control();

        // ControlSequencer needs a real handle so BeginInvoke can marshal work onto the UI thread.
        _ = control.Handle;
        return new ControlSequencer(control);
    });

    /// <summary>Gets the win forms main thread scheduler.</summary>
    /// <value>
    /// The win forms main thread scheduler.
    /// </value>
    public static ISequencer WinFormsMainThreadScheduler => LazyWinFormsMainThreadScheduler.Value;

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
                .WithPlatformModule<Winforms.Registrations>();
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
