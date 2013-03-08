using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;

namespace ReactiveUI.Xaml
{
    public class WaitForDispatcherScheduler : IScheduler
    {
        IScheduler _innerScheduler;
        readonly Func<IScheduler> _schedulerFactory;

        public WaitForDispatcherScheduler(Func<IScheduler> schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;

            // NB: Creating a scheduler will fail on WinRT if we attempt to do 
            // so on a non-UI thread, even if the underlying Dispatcher exists.
            // We assume (hope?) that WaitForDispatcherScheduler will be created 
            // early enough that this won't be the case.
            attemptToCreateScheduler();
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            return attemptToCreateScheduler().Schedule(state, action);
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            return attemptToCreateScheduler().Schedule(state, dueTime, action);
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            return attemptToCreateScheduler().Schedule(state, dueTime, action);
        }

        public DateTimeOffset Now {
            get { return attemptToCreateScheduler().Now; }
        }

        IScheduler attemptToCreateScheduler()
        {
            if (_innerScheduler != null) return _innerScheduler;
            try {
                _innerScheduler = _schedulerFactory();
                return _innerScheduler;
            } catch (Exception) {
                // NB: Dispatcher's not ready yet. Keep using CurrentThread
                return Scheduler.CurrentThread;
            }
        }
    }
}
