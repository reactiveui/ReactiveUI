using System;
using System.Reactive.Concurrency;

namespace ReactiveUI
{
    /// <summary>
    /// This scheduler attempts to deal with some of the brain-dead defaults on certain Microsoft
    /// platforms that make it difficult to access the Dispatcher during startup. This class wraps a
    /// scheduler and if it isn't available yet, it simply runs the scheduled item immediately.
    /// </summary>
    public class WaitForDispatcherScheduler : IScheduler
    {
        private readonly Func<IScheduler> _schedulerFactory;
        private IScheduler _innerScheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForDispatcherScheduler"/> class.
        /// </summary>
        /// <param name="schedulerFactory">The scheduler factory.</param>
        public WaitForDispatcherScheduler(Func<IScheduler> schedulerFactory)
        {
            this._schedulerFactory = schedulerFactory;

            // NB: Creating a scheduler will fail on WinRT if we attempt to do so on a non-UI thread,
            // even if the underlying Dispatcher exists. We assume (hope?) that
            // WaitForDispatcherScheduler will be created early enough that this won't be the case.
            attemptToCreateScheduler();
        }

        /// <summary>
        /// Gets the scheduler's notion of current time.
        /// </summary>
        public DateTimeOffset Now
        {
            get { return attemptToCreateScheduler().Now; }
        }

        /// <summary>
        /// Schedules an action to be executed.
        /// </summary>
        /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
        /// <param name="state">State passed to the action to be executed.</param>
        /// <param name="action">Action to be executed.</param>
        /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            return attemptToCreateScheduler().Schedule(state, action);
        }

        /// <summary>
        /// Schedules an action to be executed after dueTime.
        /// </summary>
        /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
        /// <param name="state">State passed to the action to be executed.</param>
        /// <param name="dueTime">Relative time after which to execute the action.</param>
        /// <param name="action">Action to be executed.</param>
        /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            return attemptToCreateScheduler().Schedule(state, dueTime, action);
        }

        /// <summary>
        /// Schedules an action to be executed at dueTime.
        /// </summary>
        /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
        /// <param name="state">State passed to the action to be executed.</param>
        /// <param name="dueTime">Absolute time at which to execute the action.</param>
        /// <param name="action">Action to be executed.</param>
        /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            return attemptToCreateScheduler().Schedule(state, dueTime, action);
        }

        private IScheduler attemptToCreateScheduler()
        {
            if (this._innerScheduler != null) return this._innerScheduler;
            try {
                this._innerScheduler = this._schedulerFactory();
                return this._innerScheduler;
            } catch (InvalidOperationException) {

                // NB: Dispatcher's not ready yet. Keep using CurrentThread
                return CurrentThreadScheduler.Instance;
            }
        }
    }
}