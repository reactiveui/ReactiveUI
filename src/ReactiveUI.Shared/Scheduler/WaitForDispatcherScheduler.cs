// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// This scheduler attempts to deal with some of the brain-dead defaults
/// on certain Microsoft platforms that make it difficult to access the
/// Dispatcher during startup. This class wraps a scheduler and if it
/// isn't available yet, it simply runs the scheduled item immediately.
/// </summary>
[System.Diagnostics.DebuggerDisplay("InnerScheduler = {_scheduler}")]
public class WaitForDispatcherScheduler : ISequencer
{
    /// <summary>Factory function used to create the underlying dispatcher scheduler on demand.</summary>
    private readonly Func<ISequencer> _schedulerFactory;

    /// <summary>Cached scheduler instance created by the factory, or null if creation has not yet succeeded.</summary>
    private ISequencer? _scheduler;

    /// <summary>Initializes a new instance of the <see cref="WaitForDispatcherScheduler"/> class.</summary>
    /// <param name="schedulerFactory">A func which will return a new scheduler.</param>
    public WaitForDispatcherScheduler(Func<ISequencer> schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;

        AttemptToCreateScheduler();
    }

    /// <inheritdoc/>
    public DateTimeOffset Now => AttemptToCreateScheduler().Now;

#if REACTIVE_SHIM
    /// <inheritdoc/>
    public IDisposable Schedule<TState>(TState state, Func<ISequencer, TState, IDisposable> action) =>
        AttemptToCreateScheduler().Schedule(state, action);

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(
        TState state,
        TimeSpan dueTime,
        Func<ISequencer, TState, IDisposable> action) =>
        AttemptToCreateScheduler().Schedule(state, dueTime, action);

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(
        TState state,
        DateTimeOffset dueTime,
        Func<ISequencer, TState, IDisposable> action) =>
        AttemptToCreateScheduler().Schedule(state, dueTime, action);
#else
    /// <inheritdoc/>
    public long Timestamp => AttemptToCreateScheduler().Timestamp;

    /// <summary>Schedules <paramref name="action"/> on the underlying scheduler, falling back to the current-thread
    /// scheduler when the dispatcher is not yet available.</summary>
    /// <typeparam name="TState">The type of the state passed to the action.</typeparam>
    /// <param name="state">The state passed to the action.</param>
    /// <param name="action">The action to run.</param>
    /// <returns>A disposable that cancels the scheduled work.</returns>
    public IDisposable Schedule<TState>(TState state, Func<ISequencer, TState, IDisposable> action) =>
        AttemptToCreateScheduler().Schedule(state, action);

    /// <summary>Schedules <paramref name="action"/> after <paramref name="dueTime"/> on the underlying scheduler,
    /// falling back to the current-thread scheduler when the dispatcher is not yet available.</summary>
    /// <typeparam name="TState">The type of the state passed to the action.</typeparam>
    /// <param name="state">The state passed to the action.</param>
    /// <param name="dueTime">The relative delay before the action runs.</param>
    /// <param name="action">The action to run.</param>
    /// <returns>A disposable that cancels the scheduled work.</returns>
    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<ISequencer, TState, IDisposable> action) =>
        AttemptToCreateScheduler().Schedule(state, dueTime, action);

    /// <summary>Schedules <paramref name="action"/> at <paramref name="dueTime"/> on the underlying scheduler,
    /// falling back to the current-thread scheduler when the dispatcher is not yet available.</summary>
    /// <typeparam name="TState">The type of the state passed to the action.</typeparam>
    /// <param name="state">The state passed to the action.</param>
    /// <param name="dueTime">The absolute time at which the action runs.</param>
    /// <param name="action">The action to run.</param>
    /// <returns>A disposable that cancels the scheduled work.</returns>
    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<ISequencer, TState, IDisposable> action) =>
        AttemptToCreateScheduler().Schedule(state, dueTime, action);

    /// <inheritdoc/>
    public void Schedule(IWorkItem item) => AttemptToCreateScheduler().Schedule(item);

    /// <inheritdoc/>
    public void Schedule(IWorkItem item, long dueTimestamp) => AttemptToCreateScheduler().Schedule(item, dueTimestamp);
#endif

    /// <summary>
    /// Attempts to create and return an instance of the scheduler. If the scheduler cannot be created, returns a
    /// fallback scheduler instance.
    /// </summary>
    /// <remarks>This method caches the created scheduler instance for future calls. If the underlying
    /// scheduler factory throws an <see cref="InvalidOperationException"/> or <see cref="ArgumentNullException"/>, the
    /// method returns a scheduler that executes work on the current thread instead.</remarks>
    /// <returns>An <see cref="ISequencer"/> instance. If the scheduler cannot be created due to the dispatcher not being ready,
    /// returns the current-thread scheduler as a fallback.</returns>
    private ISequencer AttemptToCreateScheduler()
    {
        if (_scheduler is not null)
        {
            return _scheduler;
        }

        try
        {
            _scheduler = _schedulerFactory();
            return _scheduler;
        }
        catch (InvalidOperationException)
        {
            return Sequencer.CurrentThread;
        }
        catch (ArgumentNullException)
        {
            return Sequencer.CurrentThread;
        }
    }
}
