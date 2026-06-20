// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
#if WINUI_TARGET

using Microsoft.UI.Dispatching;

namespace System.Reactive.Concurrency;

/// <summary>Represents an object that schedules units of work on a <see cref="Microsoft.UI.Dispatching.DispatcherQueue"/>.</summary>
public partial class DispatcherQueueScheduler : LocalScheduler, ISchedulerPeriodic
{
    /// <summary>Initializes a new instance of the <see cref="DispatcherQueueScheduler"/> class.</summary>
    /// <param name="dispatcherQueue"><see cref="Microsoft.UI.Dispatching.DispatcherQueue"/> to schedule work on.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dispatcherQueue"/> is <c>null</c>.</exception>
    public DispatcherQueueScheduler(DispatcherQueue dispatcherQueue)
    {
        DispatcherQueue = dispatcherQueue ?? throw new ArgumentNullException(nameof(dispatcherQueue));
        Priority = DispatcherQueuePriority.Normal;
    }

    /// <summary>Initializes a new instance of the <see cref="DispatcherQueueScheduler"/> class.</summary>
    /// <param name="dispatcherQueue"><see cref="Microsoft.UI.Dispatching.DispatcherQueue"/> to schedule work on.</param>
    /// <param name="priority">Priority at which units of work are scheduled.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dispatcherQueue"/> is <c>null</c>.</exception>
    public DispatcherQueueScheduler(DispatcherQueue dispatcherQueue, DispatcherQueuePriority priority)
    {
        DispatcherQueue = dispatcherQueue ?? throw new ArgumentNullException(nameof(dispatcherQueue));
        Priority = priority;
    }

    /// <summary>Gets the scheduler that schedules work on the <see cref="Microsoft.UI.Dispatching.DispatcherQueue"/> for the current thread.</summary>
    public static DispatcherQueueScheduler Current
    {
        get
        {
            var dispatcher = DispatcherQueue.GetForCurrentThread() ?? throw new InvalidOperationException("There is no current dispatcher thread");
            return new(dispatcher);
        }
    }

    /// <summary>Gets the <see cref="Microsoft.UI.Dispatching.DispatcherQueue"/> associated with the <see cref="DispatcherQueueScheduler"/>.</summary>
    public DispatcherQueue DispatcherQueue { get; }

    /// <summary>Gets the priority at which work items will be dispatched.</summary>
    public DispatcherQueuePriority Priority { get; }

    /// <summary>Schedules an action to be executed on the dispatcher.</summary>
    /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
    /// <param name="state">State passed to the action to be executed.</param>
    /// <param name="action">Action to be executed.</param>
    /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <c>null</c>.</exception>
    public override IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var d = new OnceDisposable();

        DispatcherQueue.TryEnqueue(
            Priority,
            () =>
            {
                if (d.IsDisposed)
                {
                    return;
                }

                d.Disposable = action(this, state);
            });

        return d;
    }

    /// <summary>Schedules an action to be executed after <paramref name="dueTime"/> on the dispatcherQueue, using a <see cref="Microsoft.UI.Dispatching.DispatcherQueueTimer"/> object.</summary>
    /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
    /// <param name="state">State passed to the action to be executed.</param>
    /// <param name="dueTime">Relative time after which to execute the action.</param>
    /// <param name="action">Action to be executed.</param>
    /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <c>null</c>.</exception>
    public override IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var dt = Scheduler.Normalize(dueTime);
        if (dt.Ticks == 0)
        {
            return Schedule(state, action);
        }

        return ScheduleSlow(state, dt, action);
    }

    /// <summary>Schedules a periodic piece of work on the dispatcherQueue, using a <see cref="Microsoft.UI.Dispatching.DispatcherQueueTimer"/> object.</summary>
    /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
    /// <param name="state">Initial state passed to the action upon the first iteration.</param>
    /// <param name="period">Period for running the work periodically.</param>
    /// <param name="action">Action to be executed, potentially updating the state.</param>
    /// <returns>The disposable object used to cancel the scheduled recurring action (best effort).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="period"/> is less than <see cref="TimeSpan.Zero"/>.</exception>
    public IDisposable SchedulePeriodic<TState>(TState state, TimeSpan period, Func<TState, TState> action)
    {
        if (period < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(period));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var timer = DispatcherQueue.CreateTimer();

        var state1 = state;

        timer.Tick +=
            (_, _) => state1 = action(state1);

        timer.Interval = period;
        timer.Start();

        return new ActionDisposable(() =>
        {
            var t = Interlocked.Exchange(ref timer, null);
            if (t is null)
            {
                return;
            }

            t.Stop();
            action = static _ => _;
        });
    }

    /// <summary>Schedules an action to be executed after <paramref name="dueTime"/> using a <see cref="Microsoft.UI.Dispatching.DispatcherQueueTimer"/> object.</summary>
    /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
    /// <param name="state">State passed to the action to be executed.</param>
    /// <param name="dueTime">Relative time after which to execute the action.</param>
    /// <param name="action">Action to be executed.</param>
    /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
    private MutableDisposable ScheduleSlow<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        var d = new MutableDisposable();

        var timer = DispatcherQueue.CreateTimer();

        timer.Tick += (_, _) =>
        {
            var t = Interlocked.Exchange(ref timer, null);
            if (t is null)
            {
                return;
            }

            try
            {
                d.Disposable = action(this, state);
            }
            finally
            {
                t.Stop();
                action = static (_, _) => EmptyDisposable.Instance;
            }
        };

        timer.Interval = dueTime;
        timer.Start();

        d.Disposable = new ActionDisposable(() =>
        {
            var t = Interlocked.Exchange(ref timer, null);
            if (t is null)
            {
                return;
            }

            t.Stop();
            action = static (_, _) => EmptyDisposable.Instance;
        });

        return d;
    }
}
#endif
