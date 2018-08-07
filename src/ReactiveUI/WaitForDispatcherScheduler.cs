// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive.Concurrency;

namespace ReactiveUI
{
    /// <summary>
    /// This scheduler attempts to deal with some of the brain-dead defaults
    /// on certain Microsoft platforms that make it difficult to access the
    /// Dispatcher during startup. This class wraps a scheduler and if it
    /// isn't available yet, it simply runs the scheduled item immediately.
    /// </summary>
    public class WaitForDispatcherScheduler : IScheduler
    {
        private IScheduler scheduler;
        private readonly Func<IScheduler> schedulerFactory;

        public WaitForDispatcherScheduler(Func<IScheduler> schedulerFactory)
        {
            this.schedulerFactory = schedulerFactory;

            // NB: Creating a scheduler will fail on WinRT if we attempt to do
            // so on a non-UI thread, even if the underlying Dispatcher exists.
            // We assume (hope?) that WaitForDispatcherScheduler will be created
            // early enough that this won't be the case.
            AttemptToCreateScheduler();
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            return AttemptToCreateScheduler().Schedule(state, action);
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            return AttemptToCreateScheduler().Schedule(state, dueTime, action);
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            return AttemptToCreateScheduler().Schedule(state, dueTime, action);
        }

        public DateTimeOffset Now
        {
            get { return AttemptToCreateScheduler().Now; }
        }

        private IScheduler AttemptToCreateScheduler()
        {
            if (scheduler != null) return scheduler;
            try {
                scheduler = schedulerFactory();
                return scheduler;
            } catch (InvalidOperationException) {
                // NB: Dispatcher's not ready yet. Keep using CurrentThread
                return CurrentThreadScheduler.Instance;
            } catch (ArgumentNullException) {
                // NB: Dispatcher's not ready yet. Keep using CurrentThread
                return CurrentThreadScheduler.Instance;
            }
        }
    }
}
