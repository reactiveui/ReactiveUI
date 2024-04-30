﻿// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.OS;

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
    private readonly Handler _handler = handler;

    static HandlerScheduler() =>
        MainThreadScheduler = new HandlerScheduler(new(Looper.MainLooper!));

    /// <summary>
    /// Gets a common instance to avoid allocations to the MainThread for the HandlerScheduler.
    /// </summary>
    public static IScheduler MainThreadScheduler { get; }

    /// <inheritdoc/>
    public DateTimeOffset Now => DateTimeOffset.Now;

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
    {
        var isCancelled = false;
        var innerDisp = new SerialDisposable() { Disposable = Disposable.Empty };

        _handler.Post(() =>
        {
            if (isCancelled)
            {
                return;
            }

            innerDisp.Disposable = action(this, state);
        });

        return new CompositeDisposable(
                                       Disposable.Create(() => isCancelled = true),
                                       innerDisp);
    }

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action) // TODO: Create Test
    {
        var isCancelled = false;
        var innerDisp = new SerialDisposable() { Disposable = Disposable.Empty };

        _handler.PostDelayed(
                             () =>
                             {
                                 if (isCancelled)
                                 {
                                     return;
                                 }

                                 innerDisp.Disposable = action(this, state);
                             },
                             dueTime.Ticks / 10 / 1000);

        return new CompositeDisposable(
                                       Disposable.Create(() => isCancelled = true),
                                       innerDisp);
    }

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action) // TODO: Create Test
    {
        if (dueTime <= Now)
        {
            return Schedule(state, action);
        }

        return Schedule(state, dueTime - Now, action);
    }
}

// vim: tw=120 ts=4 sw=4 et :
