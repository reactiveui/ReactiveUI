// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using Android.OS;

using ReactiveUI.Internal;
using Splat;

namespace ReactiveUI;

/// <summary>
/// HandlerScheduler is a scheduler that schedules items on a running
/// Activity's main thread. This is the moral equivalent of
/// DispatcherScheduler.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="HandlerScheduler"/> class.
/// </remarks>
/// <param name="handler">The handler.</param>
public class HandlerScheduler(Handler handler) : IScheduler, IEnableLogger
{
    /// <summary>
    /// The number of 100-nanosecond ticks per microsecond, used to convert
    /// a <see cref="TimeSpan"/> tick count into microseconds.
    /// </summary>
    private const long TicksPerMicrosecond = 10;

    /// <summary>
    /// The number of microseconds per millisecond, used to convert microseconds
    /// into the millisecond delay expected by <see cref="Handler.PostDelayed(Action, long)"/>.
    /// </summary>
    private const long MicrosecondsPerMillisecond = 1000;

    /// <summary>
    /// The Android handler used to post work to the target thread.
    /// </summary>
    private readonly Handler _handler = handler;

    /// <summary>
    /// Initializes static members of the <see cref="HandlerScheduler"/> class.
    /// </summary>
    static HandlerScheduler() =>
        MainThreadScheduler = new HandlerScheduler(new(Looper.MainLooper!));

    /// <summary>
    /// Gets a common instance to avoid allocations to the MainThread for the HandlerScheduler.
    /// </summary>
    public static IScheduler MainThreadScheduler { get; }

    /// <inheritdoc/>
    [SuppressMessage(
        "Major Code Smell",
        "S6354:Use a testable (date) time provider",
        Justification = "Scheduler intentionally uses real time.")]
    public DateTimeOffset Now => DateTimeOffset.Now;

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
    {
        var isCancelled = false;
        SwapDisposable innerDisp = new() { Disposable = EmptyDisposable.Instance };

        _handler.Post(() =>
        {
            if (isCancelled)
            {
                return;
            }

            innerDisp.Disposable = action(this, state);
        });

        return new CompositeDisposable(
            new ActionDisposable(() => isCancelled = true),
            innerDisp);
    }

    /// <inheritdoc/>
    public IDisposable
        Schedule<TState>(
        TState state,
        TimeSpan dueTime,
        Func<IScheduler, TState, IDisposable> action)
    {
        var isCancelled = false;
        SwapDisposable innerDisp = new() { Disposable = EmptyDisposable.Instance };

        _handler.PostDelayed(
            () =>
            {
                if (isCancelled)
                {
                    return;
                }

                innerDisp.Disposable = action(this, state);
            },
            dueTime.Ticks / TicksPerMicrosecond / MicrosecondsPerMillisecond);

        return new CompositeDisposable(
            new ActionDisposable(() => isCancelled = true),
            innerDisp);
    }

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(
        TState state,
        DateTimeOffset dueTime,
        Func<IScheduler, TState, IDisposable> action)
    {
        if (dueTime <= Now)
        {
            return Schedule(state, action);
        }

        return Schedule(state, dueTime - Now, action);
    }
}
