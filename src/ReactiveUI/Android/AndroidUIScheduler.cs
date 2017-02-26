using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using Android.OS;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// HandlerScheduler is a scheduler that schedules items on a running Activity's main thread.
    /// This is the moral equivalent of DispatcherScheduler.
    /// </summary>
    /// <seealso cref="System.Reactive.Concurrency.IScheduler"/>
    /// <seealso cref="Splat.IEnableLogger"/>
    public class HandlerScheduler : IScheduler, IEnableLogger
    {
        /// <summary>
        /// The main thread scheduler
        /// </summary>
        public static IScheduler MainThreadScheduler = new HandlerScheduler(new Handler(Looper.MainLooper), Looper.MainLooper.Thread.Id);

        private Handler handler;
        private long looperId;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerScheduler"/> class.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="threadIdAssociatedWithHandler">The thread identifier associated with handler.</param>
        public HandlerScheduler(Handler handler, long? threadIdAssociatedWithHandler)
        {
            this.handler = handler;
            this.looperId = threadIdAssociatedWithHandler ?? -1;
        }

        /// <summary>
        /// Gets the scheduler's notion of current time.
        /// </summary>
        public DateTimeOffset Now
        {
            get { return DateTimeOffset.Now; }
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
            bool isCancelled = false;
            var innerDisp = new SerialDisposable() { Disposable = Disposable.Empty };

            if (this.looperId > 0 && this.looperId == Java.Lang.Thread.CurrentThread().Id) {
                return action(this, state);
            }

            this.handler.Post(() => {
                if (isCancelled) return;
                innerDisp.Disposable = action(this, state);
            });

            return new CompositeDisposable(
                Disposable.Create(() => isCancelled = true),
                innerDisp);
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
            bool isCancelled = false;
            var innerDisp = new SerialDisposable() { Disposable = Disposable.Empty };

            this.handler.PostDelayed(() => {
                if (isCancelled) return;
                innerDisp.Disposable = action(this, state);
            }, dueTime.Ticks / 10 / 1000);

            return new CompositeDisposable(
                Disposable.Create(() => isCancelled = true),
                innerDisp);
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
            if (dueTime <= this.Now) {
                return Schedule(state, action);
            }

            return Schedule(state, dueTime - this.Now, action);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :