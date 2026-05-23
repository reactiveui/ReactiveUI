// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities;

/// <summary>
/// A scheduler that records every scheduled action while delegating execution to an inner scheduler.
/// </summary>
/// <param name="innerScheduler">The scheduler that actually performs the scheduling.</param>
public class CountingTestScheduler(IScheduler innerScheduler) : IScheduler
{
    /// <summary>
    /// Gets the scheduler that scheduling is delegated to.
    /// </summary>
    public IScheduler InnerScheduler { get; } = innerScheduler;

    /// <inheritdoc />
    public DateTimeOffset Now => InnerScheduler.Now;

    /// <summary>
    /// Gets the list of recorded scheduled actions and their due times.
    /// </summary>
    public List<(Action action, TimeSpan? dueTime)> ScheduledItems { get; } = [];

    /// <inheritdoc />
    public IDisposable Schedule<TState>(
        TState state,
        DateTimeOffset dueTime,
        Func<IScheduler, TState, IDisposable> action)
    {
        ScheduledItems.Add((() => action(this, state), null));
        return InnerScheduler.Schedule(state, dueTime, action);
    }

    /// <inheritdoc />
    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        ScheduledItems.Add((() => action(this, state), dueTime));
        return InnerScheduler.Schedule(state, dueTime, action);
    }

    /// <inheritdoc />
    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
    {
        ScheduledItems.Add((() => action(this, state), null));
        return InnerScheduler.Schedule(state, action);
    }
}
