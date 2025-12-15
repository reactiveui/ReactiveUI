// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Hosting;

namespace ReactiveUI.Builder;

/// <summary>
/// MAUI-specific extensions for the ReactiveUI builder.
/// </summary>
public static partial class MauiReactiveUIBuilderExtensions
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
    /// Configures ReactiveUI for MAUI platform with appropriate schedulers and platform services.
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
            .WithMauiScheduler(dispatcher)
            .WithPlatformModule<Maui.Registrations>()
            .WithPlatformServices();
    }

    /// <summary>
    /// Uses the reactive UI.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="withReactiveUIBuilder">The reactive UI builder.</param>
    /// <returns>A The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">builder.</exception>
    public static MauiAppBuilder UseReactiveUI(this MauiAppBuilder builder, Action<IReactiveUIBuilder> withReactiveUIBuilder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var reactiveUIBuilder = RxAppBuilder.CreateReactiveUIBuilder();
        withReactiveUIBuilder?.Invoke(reactiveUIBuilder);
        reactiveUIBuilder.BuildApp();
        return builder;
    }

    /// <summary>
    /// Uses the reactive UI.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <returns>A The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">builder.</exception>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    public static MauiAppBuilder UseReactiveUI(this MauiAppBuilder builder, IDispatcher dispatcher)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var reactiveUIBuilder = RxAppBuilder.CreateReactiveUIBuilder().WithMaui(dispatcher).BuildApp();
        return builder;
    }

    /// <summary>
    /// Adds the MAUI scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="dispatcher">Optional dispatcher instance to derive the scheduler from.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithMauiScheduler(this IReactiveUIBuilder builder, IDispatcher? dispatcher = null)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.WithTaskPoolScheduler(TaskPoolScheduler.Default);
        var scheduler = ResolveMainThreadScheduler(dispatcher);
        return builder.WithMainThreadScheduler(scheduler);
    }

    private static IScheduler ResolveMainThreadScheduler(IDispatcher? dispatcher)
    {
        if (dispatcher is not null)
        {
            return new MauiDispatcherScheduler(dispatcher);
        }

        if (ModeDetector.InUnitTestRunner())
        {
            return CurrentThreadScheduler.Instance;
        }

#if ANDROID
        return AndroidMainThreadScheduler;
#elif MACCATALYST || IOS || MACOS || TVOS
        return AppleMainThreadScheduler;
#elif WINUI_TARGET
        return WinUIMauiMainThreadScheduler;
#else
        return MauiMainThreadScheduler;
#endif
    }

    /// <summary>
    /// Scheduler implementation that marshals work onto a provided MAUI dispatcher.
    /// </summary>
    private sealed partial class MauiDispatcherScheduler(IDispatcher dispatcher) : LocalScheduler
    {
        private readonly IDispatcher _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

        /// <summary>
        /// Gets the current timestamp for the scheduler.
        /// </summary>
        public override DateTimeOffset Now => DateTimeOffset.Now;

        /// <summary>
        /// Schedules immediate work on the dispatcher.
        /// </summary>
        public override IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var disposable = new SingleAssignmentDisposable();

            void Execute()
            {
                if (!disposable.IsDisposed)
                {
                    disposable.Disposable = action(this, state);
                }
            }

            if (_dispatcher.IsDispatchRequired)
            {
                _dispatcher.Dispatch(Execute);
            }
            else
            {
                Execute();
            }

            return disposable;
        }

        /// <summary>
        /// Schedules work to execute after the specified delay.
        /// </summary>
        public override IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var normalized = Scheduler.Normalize(dueTime);
            if (normalized == TimeSpan.Zero)
            {
                return Schedule(state, action);
            }

            var disposable = new SingleAssignmentDisposable();
            var timer = _dispatcher.CreateTimer();
            timer.IsRepeating = false;
            timer.Interval = normalized;

            EventHandler? handler = null;
            handler = (sender, args) =>
            {
                timer.Tick -= handler;
                timer.Stop();

                if (!disposable.IsDisposed)
                {
                    disposable.Disposable = action(this, state);
                }
            };

            timer.Tick += handler;
            timer.Start();

            return new CompositeDisposable(disposable, Disposable.Create(() =>
            {
                timer.Tick -= handler;
                timer.Stop();
            }));
        }

        /// <summary>
        /// Schedules work to execute at the specified absolute time.
        /// </summary>
        public override IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action) =>
            Schedule(state, dueTime - Now, action);
    }
}
