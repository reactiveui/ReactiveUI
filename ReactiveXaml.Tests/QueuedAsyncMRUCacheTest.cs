using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ReactiveXaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Concurrency;
using System.Collections.Generic;
using System.Linq;

namespace ReactiveXaml.Tests
{
    [TestClass()]
    public class QueuedAsyncMRUCacheTest : IEnableLogger
    {
        [TestMethod()]
        public void GetTest()
        {
            var input = new[] { 1, 1, 1, 1, 1 };
            var sched = new TestScheduler();
            QueuedAsyncMRUCache<int, int> fixture;

            var delay = TimeSpan.FromSeconds(1.0);
            using (TestUtils.WithTestScheduler(sched)) {
                fixture = new QueuedAsyncMRUCache<int, int>(x => Observable.Return(x*5).Delay(delay, sched).First(), 5, 2);
            }

            int result = 0;
            var t = new Task(() => {
                foreach (int i in input) {
                    this.Log().InfoFormat("Counter is {0}", result);
                    result += fixture.Get(i);
                }
            });
            t.Start();

            Thread.Sleep(200);

            this.Log().Info("Running to t=0");
            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(500)));
            Assert.AreEqual(0, result);
            this.Log().Info("Running to t=1200");
            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(1200)));

            Thread.Sleep(200);

            Assert.AreEqual(25, result);

            this.Log().Info("Running to end");
            sched.Run();
            Assert.AreEqual(25, result);
        }

        [TestMethod()]
        public void AsyncGetTest()
        {
            var input = new[] { 1, 1, 1, 1, 1 };
            var fixture = new QueuedAsyncMRUCache<int, int>(x => { Thread.Sleep(1000); return x * 5; }, 5, 2);
            var output = new ConcurrentQueue<int>();

            assertStopwatch(new TimeSpan(0, 0, 0, 0, 1100), () => {
                var dontcare = input.Select(x => fixture.AsyncGet(x))
                    .Do(x => x.Subscribe(output.Enqueue))
                    .Merge()
                    .BufferWithCount(5).First();
            });

            Assert.IsTrue(output.ToArray().Length == 5);
        }

        [TestMethod()]
        public void CachedValuesTest()
        {
            var input = new[] { 1, 2, 1, 3, 1 };
            var fixture = new QueuedAsyncMRUCache<int, int>(x => x * 5, 2);

            var dontcare = input.Select(x => fixture.Get(x)).ToArray();

            var output = fixture.CachedValues().ToArray();
            Assert.IsTrue(output.Length == 2);
        }

        [TestMethod()]
        public void DisposeTest()
        {
            var input = new[] { 1, 1, 1, 1, 1 };
            var fixture = new QueuedAsyncMRUCache<int, int>(x => { Thread.Sleep(1000); return x * 5; }, 5, 2);

            input.Run(x => fixture.AsyncGet(x));
            fixture.Dispose();

            bool threw = false;
            try {
                fixture.AsyncGet(2);
            } catch(Exception ex) {
                this.Log().Info("Threw exception correctly", ex);
                threw = true;
            }
            Assert.IsTrue(threw);
        }

        [TestMethod()]
        public void CacheShouldBlockOnceWeHitOurConcurrentLimit()
        {
            var fixture = new QueuedAsyncMRUCache<int, int>(x => { Thread.Sleep(1000); return x * 5; }, 5, 3);

            assertStopwatch(new TimeSpan(0, 0, 0, 0, 200), () => {
                fixture.AsyncGet(1);
                fixture.AsyncGet(2);
            });

            assertStopwatch(new TimeSpan(0, 0, 0, 0, 2500), () => {
                Assert.AreEqual(15, fixture.Get(3));
            });
        }

        [TestMethod()]
        public void CacheShouldEatExceptionsAndMarshalThemToObservable()
        {
            var input = new[] { 5, 2, 10, 0/*boom!*/, 5 };
            var fixture = new QueuedAsyncMRUCache<int, int>(x => { Thread.Sleep(1000); return 50 / x; }, 5, 5);

            Exception exception = null;
            int completed = 0;
            input.Select(x => fixture.AsyncGet(x))
                 .Run(x => x.Subscribe(_ => { }, ex => exception = exception ?? ex, () => completed++));

            Thread.Sleep(5000);

            this.Log().Info(exception);
            Assert.AreEqual(4, completed);
            Assert.IsNotNull(exception);
        }

        [TestMethod()]
        public void BlockingGetShouldRethrowExceptions()
        {
            var input = new[] { 5, 2, 10, 0/*boom!*/, 5 };
            var fixture = new QueuedAsyncMRUCache<int, int>(x => { Thread.Sleep(1000); return 50 / x; }, 5, 5);
            int[] output = {0};

            bool did_throw = false;
            try {
                output = input.Select(x => fixture.Get(x)).ToArray();
            } catch(Exception ex) {
                did_throw = true;
                this.Log().Info("Exception thrown", ex);
            }

            output.Run(x => this.Log().Info(x));
            Assert.IsTrue(did_throw);
        }

        void assertStopwatch(TimeSpan max_time, Action block)
        {
            DateTime start = DateTime.Now;
            block();
            var delta = DateTime.Now - start;
            Assert.IsTrue(delta < max_time, delta.ToString());
        }
    }
}
