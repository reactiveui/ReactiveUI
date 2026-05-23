// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Hosting;
using ReactiveUI.Helpers;
using ReactiveUI.Maui;

namespace ReactiveUI.Builder;

/// <summary>
/// MAUI-specific extensions for the ReactiveUI builder.
/// </summary>
public static class MauiReactiveUIBuilderExtensions
{
#if WINUI_TARGET
    /// <summary>
    /// The lazily-initialized scheduler that marshals work onto the WinUI/MAUI main UI thread.
    /// </summary>
    private static readonly Lazy<IScheduler> LazyWinUIMauiMainThreadScheduler =
 new(() => new WaitForDispatcherScheduler(static () => DispatcherQueueScheduler.Current));
#endif

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
    public static IScheduler WinUIMauiMainThreadScheduler => LazyWinUIMauiMainThreadScheduler.Value;
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
    public static IScheduler AppleMainThreadScheduler { get; } =
 new WaitForDispatcherScheduler(static () => new NSRunloopScheduler());
#endif

    /// <summary>
    /// Configures ReactiveUI for MAUI platform with appropriate schedulers and platform services.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithMaui(this IReactiveUIBuilder builder) => builder.WithMaui(null);

    /// <summary>
    /// Configures ReactiveUI for MAUI platform with appropriate schedulers and platform services.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="dispatcher">The MAUI dispatcher to use for the main thread scheduler.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithMaui(this IReactiveUIBuilder builder, IDispatcher? dispatcher)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return ((IReactiveUIBuilder)builder.WithCoreServices())
            .WithMauiScheduler(dispatcher)
            .WithTaskPoolScheduler(TaskPoolScheduler.Default)
            .WithPlatformModule<Maui.Registrations>()
            .WithMauiConverters()
            .WithPlatformServices();
    }

    /// <summary>
    /// Uses the reactive UI.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="withReactiveUIBuilder">The reactive UI builder.</param>
    /// <returns>A The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">builder.</exception>
    public static MauiAppBuilder UseReactiveUI(
        this MauiAppBuilder builder,
        Action<IReactiveUIBuilder> withReactiveUIBuilder)
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
    public static MauiAppBuilder UseReactiveUI(this MauiAppBuilder builder, IDispatcher dispatcher)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        RxAppBuilder.CreateReactiveUIBuilder().WithMaui(dispatcher).BuildApp();
        return builder;
    }

    /// <summary>
    /// Adds the MAUI scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithMauiScheduler(this IReactiveUIBuilder builder) => builder.WithMauiScheduler(null);

    /// <summary>
    /// Adds the MAUI scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="dispatcher">Optional dispatcher instance to derive the scheduler from.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithMauiScheduler(this IReactiveUIBuilder builder, IDispatcher? dispatcher)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.WithTaskPoolScheduler(TaskPoolScheduler.Default);
        var scheduler = ResolveMainThreadScheduler(dispatcher);
        return builder.WithMainThreadScheduler(scheduler);
    }

    /// <summary>
    /// Registers Maui-specific converters to the ConverterService.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <remarks>
    /// This method registers Maui-specific converters (<see cref="BooleanToVisibilityTypeConverter"/>,
    /// <see cref="VisibilityToBooleanTypeConverter"/>) and the <see cref="ComponentModelFallbackConverter"/>
    /// to the <c>ConverterService</c> so they are available when using the builder pattern.
    /// </remarks>
    public static IReactiveUIBuilder WithMauiConverters(this IReactiveUIBuilder builder)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);

        return builder
            .WithConverter(new BooleanToVisibilityTypeConverter())
            .WithConverter(new VisibilityToBooleanTypeConverter())
            .WithFallbackConverter(new ComponentModelFallbackConverter());
    }

    /// <summary>
    /// Resolves the main thread scheduler to use based on the current platform and supplied dispatcher.
    /// </summary>
    /// <param name="dispatcher">Optional dispatcher to derive the scheduler from.</param>
    /// <returns>The resolved main thread scheduler.</returns>
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
    private sealed class MauiDispatcherScheduler(IDispatcher dispatcher) : LocalScheduler
    {
        /// <summary>
        /// The dispatcher used to marshal scheduled work onto the MAUI main thread.
        /// </summary>
        private readonly IDispatcher _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

        /// <summary>
        /// Gets the current timestamp for the scheduler.
        /// </summary>
        [SuppressMessage(
            "Major Code Smell",
            "S6354:Use a testable (date) time provider",
            Justification = "Scheduler intentionally uses real time.")]
        public override DateTimeOffset Now => DateTimeOffset.Now;

        /// <summary>
        /// Schedules immediate work on the dispatcher.
        /// </summary>
        /// <typeparam name="TState">The type of the state passed to the action.</typeparam>
        /// <param name="state">The state to pass to the action.</param>
        /// <param name="action">The action to execute.</param>
        /// <returns>A disposable that cancels the scheduled work.</returns>
        public override IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var disposable = new SingleAssignmentDisposable();

            void Execute()
            {
                if (disposable.IsDisposed)
                {
                    return;
                }

                disposable.Disposable = action(this, state);
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
        /// <typeparam name="TState">The type of the state passed to the action.</typeparam>
        /// <param name="state">The state to pass to the action.</param>
        /// <param name="dueTime">The relative delay before the action executes.</param>
        /// <param name="action">The action to execute.</param>
        /// <returns>A disposable that cancels the scheduled work.</returns>
        public override IDisposable Schedule<TState>(
            TState state,
            TimeSpan dueTime,
            Func<IScheduler, TState, IDisposable> action)
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
            handler = (_, _) =>
            {
                timer.Tick -= handler;
                timer.Stop();

                if (disposable.IsDisposed)
                {
                    return;
                }

                disposable.Disposable = action(this, state);
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
        /// <typeparam name="TState">The type of the state passed to the action.</typeparam>
        /// <param name="state">The state to pass to the action.</param>
        /// <param name="dueTime">The absolute time at which the action executes.</param>
        /// <param name="action">The action to execute.</param>
        /// <returns>A disposable that cancels the scheduled work.</returns>
        public override IDisposable Schedule<TState>(
            TState state,
            DateTimeOffset dueTime,
            Func<IScheduler, TState, IDisposable> action) =>
            Schedule(state, dueTime - Now, action);
    }
}
