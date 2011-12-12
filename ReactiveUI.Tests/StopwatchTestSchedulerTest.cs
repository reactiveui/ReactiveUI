using System;
using System.Concurrency;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using ReactiveUI;
using Xunit;
using ReactiveUI.Testing;

namespace ReactiveUI.Tests
{
    public class StopwatchSchedulerTest
    {
        [Fact]
        public void StopwatchSchedulerShouldFailLongrunningTasks()
        {
            var sched = Scheduler.Immediate;
            var fixture = new StopwatchScheduler(TimeSpan.FromMilliseconds(500), null, sched);

            fixture.Schedule(() => Console.WriteLine("Shouldn't fail"));

            bool should_die = true;
            try {
                fixture.Schedule(() => Thread.Sleep(1000));
            } catch {
                should_die = false;
            }

            Assert.False(should_die);
        }
    }
}