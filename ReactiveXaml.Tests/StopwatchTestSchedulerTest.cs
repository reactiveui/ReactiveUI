using System;
using System.Concurrency;
using System.Linq;
using System.Threading;
using ReactiveXaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveXaml.Testing;

namespace ReactiveXaml.Tests
{
    [TestClass()]
    public class StopwatchSchedulerTest
    {
        [TestMethod()]
        public void StopwatchSchedulerShouldFailLongrunningTasks()
        {
            var sched = new TestScheduler();
            var fixture = new StopwatchScheduler(TimeSpan.FromMilliseconds(500), null, sched);

            fixture.Schedule(() => Console.WriteLine("Shouldn't fail"));

            bool should_die = true;
            try {
                fixture.Schedule(() => Observable.Return(4).Delay(TimeSpan.FromMilliseconds(2000)));
            } catch {
                should_die = false;
            }

            sched.RunToMilliseconds(2500);

            Assert.IsFalse(should_die);
        }
    }
}
