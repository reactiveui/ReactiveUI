// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// This scheduler attempts to deal with some of the brain-dead defaults
/// on certain Microsoft platforms that make it difficult to access the
/// Dispatcher during startup. This class wraps a scheduler and if it
/// isn't available yet, it simply runs the scheduled item immediately.
/// </summary>
public class WaitForDispatcherScheduler : IScheduler
{
    private readonly Func<IScheduler> _schedulerFactory;
    private IScheduler? _scheduler;

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitForDispatcherScheduler"/> class.
    /// </summary>
    /// <param name="schedulerFactory">A func which will return a new scheduler.</param>
    public WaitForDispatcherScheduler(Func<IScheduler> schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;

        // NB: Creating a scheduler will fail on WinRT if we attempt to do
        // so on a non-UI thread, even if the underlying Dispatcher exists.
        // We assume (hope?) that WaitForDispatcherScheduler will be created
        // early enough that this won't be the case.
        AttemptToCreateScheduler();
    }

    /// <inheritdoc/>
    public DateTimeOffset Now => AttemptToCreateScheduler().Now;

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action) =>
        AttemptToCreateScheduler().Schedule(state, action);

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action) => // TODO: Create Test
        AttemptToCreateScheduler().Schedule(state, dueTime, action);

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action) => // TODO: Create Test
        AttemptToCreateScheduler().Schedule(state, dueTime, action);

    private IScheduler AttemptToCreateScheduler()
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
            // NB: Dispatcher's not ready yet. Keep using CurrentThread
            return CurrentThreadScheduler.Instance;
        }
        catch (ArgumentNullException)
        {
            // NB: Dispatcher's not ready yet. Keep using CurrentThread
            return CurrentThreadScheduler.Instance;
        }
    }
}
