using System;
using System.Reactive.Concurrency;

namespace ReactiveUI
{
    public class StopwatchScheduler : IScheduler, IEnableLogger
    {
        readonly TimeSpan maxAllowedTime;
        readonly IScheduler innerSched;
        readonly string errorMessage;

        public StopwatchScheduler(TimeSpan maxAllowedTime, string errorMessage = null, IScheduler innerSched = null)
        {
            this.maxAllowedTime = maxAllowedTime;
            this.errorMessage = errorMessage;
            this.innerSched = innerSched ?? RxApp.DeferredScheduler;
        }

        public DateTimeOffset Now {
            get { return innerSched.Now; }
        }

        public IDisposable Schedule(Action action, TimeSpan dueTime)
        {
            return innerSched.Schedule(() => {
                var start = Now;
                action();
                var end = Now;

                var elapsed = end - start;
                if (elapsed > maxAllowedTime) {
                    string error = String.Format("{0} Time elapsed: {1}, max allowed is {2}",
                        (errorMessage != null ? errorMessage + "\n" : ""), elapsed, maxAllowedTime);
                    this.Log().Error(error);
                    throw new Exception(error);
                }
            }, dueTime);
        }

        public IDisposable Schedule(Action action)
        {
            return Schedule(action, TimeSpan.Zero);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
