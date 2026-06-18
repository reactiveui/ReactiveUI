// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
using Microsoft.Reactive.Testing;
#else
using ReactiveUI.Primitives.Concurrency;
#endif

namespace ReactiveUI.Tests.Utilities.Schedulers;

/// <summary>Lightweight virtual time scheduler for testing. Provides deterministic time control without heavyweight dependencies.</summary>
public sealed class VirtualTimeScheduler : ISequencer
{
#if REACTIVE_SHIM
    /// <summary>The underlying reactive test scheduler.</summary>
    private readonly TestScheduler _scheduler = new();
#else
    /// <summary>The underlying deterministic virtual-time sequencer.</summary>
    private readonly VirtualClock _clock = new();
#endif

    /// <summary>Gets the current virtual time.</summary>
    public DateTimeOffset Now =>
#if REACTIVE_SHIM
        _scheduler.Now;
#else
        _clock.Now;
#endif

#if REACTIVE_SHIM
    /// <inheritdoc/>
    public IDisposable Schedule<TState>(TState state, Func<ISequencer, TState, IDisposable> action) =>
        _scheduler.Schedule(state, action);

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<ISequencer, TState, IDisposable> action) =>
        _scheduler.Schedule(state, dueTime, action);

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(
        TState state,
        DateTimeOffset dueTime,
        Func<ISequencer, TState, IDisposable> action) =>
        _scheduler.Schedule(state, dueTime, action);
#else
    /// <summary>Gets the current monotonic timestamp.</summary>
    public long Timestamp => _clock.Timestamp;

    /// <inheritdoc/>
    public void Schedule(IWorkItem item) => _clock.Schedule(item);

    /// <inheritdoc/>
    public void Schedule(IWorkItem item, long dueTimestamp) => _clock.Schedule(item, dueTimestamp);

    /// <summary>Schedules an action to be executed immediately.</summary>
    /// <typeparam name="TState">The type of state passed to the action.</typeparam>
    /// <param name="state">State passed to the action to be executed.</param>
    /// <param name="action">Action to be executed.</param>
    /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
    public IDisposable Schedule<TState>(TState state, Func<ISequencer, TState, IDisposable> action) =>
        _clock.Schedule(state, action);

    /// <summary>Schedules an action to be executed after the specified due time.</summary>
    /// <typeparam name="TState">The type of state passed to the action.</typeparam>
    /// <param name="state">State passed to the action to be executed.</param>
    /// <param name="dueTime">Relative time after which to execute the action.</param>
    /// <param name="action">Action to be executed.</param>
    /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<ISequencer, TState, IDisposable> action) =>
        _clock.Schedule(state, dueTime, action);

    /// <summary>Schedules an action to be executed at the specified due time.</summary>
    /// <typeparam name="TState">The type of state passed to the action.</typeparam>
    /// <param name="state">State passed to the action to be executed.</param>
    /// <param name="dueTime">Absolute time at which to execute the action.</param>
    /// <param name="action">Action to be executed.</param>
    /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
    public IDisposable Schedule<TState>(
        TState state,
        DateTimeOffset dueTime,
        Func<ISequencer, TState, IDisposable> action) =>
        _clock.Schedule(state, dueTime, action);
#endif

    /// <summary>Advances virtual time by the specified duration, executing all scheduled actions.</summary>
    /// <param name="time">The time span to advance.</param>
    public void AdvanceBy(TimeSpan time)
    {
#if REACTIVE_SHIM
        _scheduler.AdvanceBy(time.Ticks);
#else
        _clock.AdvanceBy(time);
#endif
    }

    /// <summary>Advances virtual time to the specified absolute time, executing all scheduled actions.</summary>
    /// <param name="time">The absolute time to advance to.</param>
    public void AdvanceTo(DateTimeOffset time)
    {
#if REACTIVE_SHIM
        _scheduler.AdvanceTo((time - DateTimeOffset.MinValue).Ticks);
#else
        _clock.AdvanceTo(time);
#endif
    }

    /// <summary>Runs all scheduled actions until there are no more.</summary>
    public void Start()
    {
#if REACTIVE_SHIM
        _scheduler.Start();
#else
        _clock.Start();
#endif
    }
}
