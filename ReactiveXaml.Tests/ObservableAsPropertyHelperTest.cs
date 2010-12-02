using ReactiveXaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Concurrency;
using System.Collections.Generic;

namespace ReactiveXaml.Tests
{
    [TestClass()]
    public class ObservableAsPropertyHelperTest
    {
        [TestMethod()]
        public void OAPHShouldFireChangeNotifications()
        {
            var sched = new TestScheduler();
            var output = new List<int>();

            var input = new[] { 1, 2, 3, 3, 4 }.ToObservable();
            var fixture = new ObservableAsPropertyHelper<int>(input,
                x => output.Add(x), -5, sched);

            // Note: Why doesn't the list match the above one? We're supposed
            // to suppress duplicate notifications, of course :)
            sched.Run();
            (new[] { 1, 2, 3, 4 }).AssertAreEqual(output);
        }

        [TestMethod()]
        public void OAPHShouldProvideLatestValue()
        {
            var sched = new TestScheduler();
            var input = new Subject<int>();

            var fixture = new ObservableAsPropertyHelper<int>(input,
                _ => { }, -5, sched);

            Assert.AreEqual(-5, fixture.Value);
            (new[] { 1, 2, 3, 4 }).Run(x => input.OnNext(x));

            sched.Run();
            Assert.AreEqual(4, fixture.Value);

            input.OnCompleted();
            sched.Run();
            Assert.AreEqual(4, fixture.Value);
        }

        [TestMethod()]
        public void OAPHShouldRethrowErrors()
        {
            var input = new Subject<int>();
            var sched = new TestScheduler();

            var fixture = new ObservableAsPropertyHelper<int>(input,
                _ => { }, -5, sched);

            Assert.AreEqual(-5, fixture.Value);
            (new[] { 1, 2, 3, 4 }).Run(x => input.OnNext(x));

            sched.Run();

            Assert.AreEqual(4, fixture.Value);

            input.OnError(new Exception("Die!"));

            sched.Run();

            try {
                Assert.AreEqual(4, fixture.Value);
            } catch {
                return;
            }
            Assert.Fail("We should've threw there");
        }
    }
}
