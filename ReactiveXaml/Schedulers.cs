using System;
using System.Concurrency;

namespace ReactiveXaml
{
    /// <summary>
    /// 
    /// </summary>
    public class StopwatchScheduler : IScheduler, IEnableLogger
    {
        readonly TimeSpan maxAllowedTime;
        readonly IScheduler innerSched;
        readonly string errorMessage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxAllowedTime"></param>
        /// <param name="errorMessage"></param>
        /// <param name="innerSched"></param>
        public StopwatchScheduler(TimeSpan maxAllowedTime, string errorMessage = null, IScheduler innerSched = null)
        {
            this.maxAllowedTime = maxAllowedTime;
            this.errorMessage = errorMessage;
            this.innerSched = innerSched ?? RxApp.DeferredScheduler;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset Now {
            get { return innerSched.Now; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="dueTime"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IDisposable Schedule(Action action)
        {
            return Schedule(action, TimeSpan.Zero);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :