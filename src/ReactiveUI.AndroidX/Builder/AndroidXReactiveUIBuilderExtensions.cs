// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Builder;
#else
namespace ReactiveUI.Builder;
#endif
/// <summary>AndroidX-specific extensions for the ReactiveUI builder.</summary>
public static class AndroidXReactiveUIBuilderExtensions
{
    /// <summary>Gets the android x main thread scheduler.</summary>
    /// <value>
    /// The android x main thread scheduler.
    /// </value>
    public static ISequencer AndroidXMainThreadScheduler { get; } = HandlerSequencer.Main;

    /// <summary>Provides AndroidX configuration extension members for <see cref="IReactiveUIBuilder"/>.</summary>
    /// <param name="builder">The builder instance.</param>
    extension(IReactiveUIBuilder? builder)
    {
        /// <summary>Configures the builder to use AndroidX platform services and schedulers.</summary>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithAndroidX() =>
            builder is null
                ? throw new ArgumentNullException(nameof(builder))
                : ((IReactiveUIBuilder)builder.WithCoreServices())
                    .WithMainThreadScheduler(AndroidXMainThreadScheduler)
                    .WithTaskPoolScheduler(TaskPoolSequencer.Default)
                    .WithPlatformModule<AndroidX.Registrations>();

        /// <summary>Withes the android x scheduler.</summary>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithAndroidXScheduler() =>
            builder is null
                ? throw new ArgumentNullException(nameof(builder))
                : builder.WithMainThreadScheduler(AndroidXMainThreadScheduler);
    }
}
