using System;
using System.Concurrency;
using System.Threading;
using ReactiveXaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReactiveXaml.Tests
{
    [TestClass()]
    public class StopwatchSchedulerTest
    {
        [TestMethod()]
        public void StopwatchSchedulerShouldFailLongrunningTasks()
        {
            var fixture = new StopwatchScheduler(TimeSpan.FromMilliseconds(500), null, RxApp.DeferredScheduler);

            fixture.Schedule(() => Console.WriteLine("Shouldn't fail"));

            bool should_die = true;
            try { 
                fixture.Schedule(() => Thread.Sleep(1000));
            } catch {
                should_die = false;
            }

            Assert.IsFalse(should_die);
        }
    }
}
