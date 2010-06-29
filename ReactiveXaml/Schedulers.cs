using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Concurrency;
using System.Diagnostics;
using System.Disposables;

namespace ReactiveXaml
{
    public class StopwatchTestScheduler : IScheduler, IEnableLogger
    {
        readonly TimeSpan maxAllowedTime;
        readonly IScheduler innerSched;

        public StopwatchTestScheduler(TimeSpan maxAllowedTime, IScheduler innerSched = null)
        {
            this.maxAllowedTime = maxAllowedTime;
            this.innerSched = innerSched ?? Scheduler.Immediate;
        }

        public DateTimeOffset Now {
            get { return innerSched.Now; }
        }

        public IDisposable Schedule(Action action, TimeSpan dueTime)
        {
            var sw = new Stopwatch();
            sw.Start();
            var inner_disp = innerSched.Schedule(action, dueTime);

            return Disposable.Create(() => {
                sw.Stop();
                inner_disp.Dispose();
                if (sw.Elapsed > maxAllowedTime)
                    throw new Exception(String.Format("Time elapsed: {0}, max allowed is {1}", sw.Elapsed, maxAllowedTime));
            });
        }

        public IDisposable Schedule(Action action)
        {
            return Schedule(action, TimeSpan.Zero);
        }
    }
}