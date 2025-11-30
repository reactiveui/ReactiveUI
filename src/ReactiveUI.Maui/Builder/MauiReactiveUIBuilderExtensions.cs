// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Dispatching;

namespace ReactiveUI.Builder;

/// <summary>
/// MAUI-specific extensions for the ReactiveUI builder.
/// </summary>
public static class MauiReactiveUIBuilderExtensions
{
    /// <summary>
    /// Gets the MAUI main thread scheduler.
    /// </summary>
    /// <value>
    /// The MAUI main thread scheduler.
    /// </value>
    public static IScheduler MauiMainThreadScheduler { get; } = DefaultScheduler.Instance;

#if WINUI_TARGET
    /// <summary>
    /// Gets a scheduler that schedules work on the WinUI or .NET MAUI main UI thread, if available.
    /// </summary>
    /// <remarks>Use this scheduler to ensure that actions are executed on the main thread in WinUI or .NET
    /// MAUI applications. This is useful for updating UI elements or performing operations that require main thread
    /// access. If called from a non-main thread, scheduled actions will be marshaled to the main UI thread.</remarks>
    public static IScheduler WinUIMauiMainThreadScheduler { get; } = new WaitForDispatcherScheduler(static () => DispatcherQueueScheduler.Current);
#endif

#if ANDROID
    /// <summary>
    /// Gets the scheduler that schedules work on the Android main (UI) thread.
    /// </summary>
    /// <remarks>Use this scheduler to execute actions that must run on the Android UI thread, such as
    /// updating user interface elements from background operations. This property is only available on Android
    /// platforms.</remarks>
    public static IScheduler AndroidMainThreadScheduler { get; } = HandlerScheduler.MainThreadScheduler;
#endif

#if MACCATALYST || IOS || MACOS || TVOS
    /// <summary>
    /// Gets the scheduler that schedules work on the Apple main (UI) thread.
    /// </summary>
    /// <remarks>Use this scheduler to execute actions that must run on the main UI thread of Apple platforms,
    /// such as updating user interface elements from background operations. This property is available on macOS, iOS,
    /// and Mac Catalyst platforms.</remarks>
    public static IScheduler AppleMainThreadScheduler { get; } = new WaitForDispatcherScheduler(static () => new NSRunloopScheduler());
#endif

    /// <summary>
    /// Configures ReactiveUI for MAUI platform with appropriate schedulers.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="dispatcher">The MAUI dispatcher to use for the main thread scheduler.</param>
    /// <returns>The builder instance for chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    public static IReactiveUIBuilder WithMaui(this IReactiveUIBuilder builder, IDispatcher? dispatcher = null)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder
            .WithMauiScheduler()
            .WithPlatformModule<Maui.Registrations>();
    }

    /// <summary>
    /// Adds the MAUI scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithMauiScheduler(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.WithTaskPoolScheduler(TaskPoolScheduler.Default);

        if (ModeDetector.InUnitTestRunner())
        {
            return builder.WithMainThreadScheduler(RxApp.UnitTestMainThreadScheduler);
        }

#if ANDROID
        return builder.WithMainThreadScheduler(AndroidMainThreadScheduler);
#elif MACCATALYST || IOS || MACOS || TVOS
        return builder.WithMainThreadScheduler(AppleMainThreadScheduler);
#elif WINUI_TARGET
        return builder.WithMainThreadScheduler(WinUIMauiMainThreadScheduler);
#else
        return builder.WithMainThreadScheduler(MauiMainThreadScheduler);
#endif
    }
}
