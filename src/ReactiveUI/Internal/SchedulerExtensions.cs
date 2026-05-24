// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;

namespace ReactiveUI.Internal;

/// <summary>
/// Scheduler helpers shared by the reactive command and routing pipelines.
/// </summary>
internal static class SchedulerExtensions
{
    /// <summary>
    /// Dispatches <paramref name="action"/> on <paramref name="scheduler"/>, or invokes it inline when the scheduler
    /// is <see cref="ImmediateScheduler.Instance"/>. The immediate scheduler's stateful <c>Schedule</c> overload
    /// allocates a fresh trampoline (an <c>AsyncLockScheduler</c> + <c>LocalScheduler</c> + priority queue, plus a
    /// <c>SystemClock</c> registration) on every call; running inline is semantically identical because the immediate
    /// scheduler always executes synchronously on the calling thread.
    /// </summary>
    /// <typeparam name="TState">The state threaded to the action.</typeparam>
    /// <param name="scheduler">The scheduler to dispatch on.</param>
    /// <param name="state">The state passed to the action.</param>
    /// <param name="action">The action to run on the scheduler.</param>
    public static void ScheduleOrInline<TState>(this IScheduler scheduler, TState state, Func<IScheduler, TState, IDisposable> action)
    {
        if (ReferenceEquals(scheduler, ImmediateScheduler.Instance))
        {
            _ = action(scheduler, state);
            return;
        }

        scheduler.Schedule(state, action);
    }
}
