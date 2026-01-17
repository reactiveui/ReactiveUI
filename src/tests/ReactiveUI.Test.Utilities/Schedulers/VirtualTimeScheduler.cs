// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reactive.Disposables;

namespace ReactiveUI.Tests.Utilities.Schedulers;

/// <summary>
///     Lightweight virtual time scheduler for testing.
///     Provides deterministic time control without heavyweight dependencies.
/// </summary>
public sealed class VirtualTimeScheduler : IScheduler
{
    private readonly SortedList<DateTimeOffset, List<Action>> _scheduledItems = [];
    private DateTimeOffset _now = DateTimeOffset.MinValue;

    /// <summary>
    ///     Gets the current virtual time.
    /// </summary>
    public DateTimeOffset Now => _now;

    /// <summary>
    ///     Schedules an action to be executed immediately.
    /// </summary>
    /// <typeparam name="TState">The type of state passed to the action.</typeparam>
    /// <param name="state">State passed to the action to be executed.</param>
    /// <param name="action">Action to be executed.</param>
    /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return Schedule(state, TimeSpan.Zero, action);
    }

    /// <summary>
    ///     Schedules an action to be executed after the specified due time.
    /// </summary>
    /// <typeparam name="TState">The type of state passed to the action.</typeparam>
    /// <param name="state">State passed to the action to be executed.</param>
    /// <param name="dueTime">Relative time after which to execute the action.</param>
    /// <param name="action">Action to be executed.</param>
    /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        var scheduledTime = _now.Add(dueTime);

        if (!_scheduledItems.TryGetValue(scheduledTime, out var actions))
        {
            actions = [];
            _scheduledItems[scheduledTime] = actions;
        }

        var cancelled = false;
        actions.Add(() =>
        {
            if (!cancelled)
            {
                action(this, state);
            }
        });

        return Disposable.Create(() => cancelled = true);
    }

    /// <summary>
    ///     Schedules an action to be executed at the specified due time.
    /// </summary>
    /// <typeparam name="TState">The type of state passed to the action.</typeparam>
    /// <param name="state">State passed to the action to be executed.</param>
    /// <param name="dueTime">Absolute time at which to execute the action.</param>
    /// <param name="action">Action to be executed.</param>
    /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (!_scheduledItems.TryGetValue(dueTime, out var actions))
        {
            actions = [];
            _scheduledItems[dueTime] = actions;
        }

        var cancelled = false;
        actions.Add(() =>
        {
            if (!cancelled)
            {
                action(this, state);
            }
        });

        return Disposable.Create(() => cancelled = true);
    }

    /// <summary>
    ///     Advances virtual time by the specified duration, executing all scheduled actions.
    /// </summary>
    /// <param name="time">The time span to advance.</param>
    public void AdvanceBy(TimeSpan time)
    {
        var targetTime = _now.Add(time);
        while (_scheduledItems.Count > 0 && _scheduledItems.Keys[0] <= targetTime)
        {
            var scheduledTime = _scheduledItems.Keys[0];
            var actions = _scheduledItems[scheduledTime];
            _scheduledItems.RemoveAt(0);

            _now = scheduledTime;

            // Execute all actions scheduled for this time
            foreach (var action in actions)
            {
                action();
            }
        }

        _now = targetTime;
    }

    /// <summary>
    ///     Advances virtual time to the specified absolute time, executing all scheduled actions.
    /// </summary>
    /// <param name="time">The absolute time to advance to.</param>
    public void AdvanceTo(DateTimeOffset time)
    {
        if (time < _now)
        {
            throw new ArgumentException("Cannot advance to a time in the past", nameof(time));
        }

        while (_scheduledItems.Count > 0 && _scheduledItems.Keys[0] <= time)
        {
            var scheduledTime = _scheduledItems.Keys[0];
            var actions = _scheduledItems[scheduledTime];
            _scheduledItems.RemoveAt(0);

            _now = scheduledTime;

            foreach (var action in actions)
            {
                action();
            }
        }

        _now = time;
    }

    /// <summary>
    ///     Runs all scheduled actions until there are no more.
    /// </summary>
    public void Start()
    {
        while (_scheduledItems.Count > 0)
        {
            var scheduledTime = _scheduledItems.Keys[0];
            var actions = _scheduledItems[scheduledTime];
            _scheduledItems.RemoveAt(0);

            _now = scheduledTime;

            foreach (var action in actions)
            {
                action();
            }
        }
    }
}
