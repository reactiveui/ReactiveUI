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
    public class ObservableAsyncMRUCacheTest : IEnableLogger
    {
        [TestMethod()]
        public void GetTest()
        {
            var input = new[] {1, 1, 1, 1, 1};
            var sched = new TestScheduler();
            ObservableAsyncMRUCache<int, int> fixture;

            var delay = TimeSpan.FromSeconds(1.0);
            fixture = new ObservableAsyncMRUCache<int, int>(x => Observable.Return(x*5).Delay(delay, sched), 5, 2);

            int result = 0;
            var t = new Task(() => {
                foreach (int x in input.Select(x => fixture.Get(x))) {
                    result += x;
                }
            });
            t.Start();

            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(500)));

            Thread.Sleep(100);
            Assert.AreEqual(0, result);

            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(1200)));

            Thread.Sleep(100);
            Assert.AreEqual(25, result);

            this.Log().Debug("Running to end");
            sched.Run();
            t.Wait();
            Assert.AreEqual(25, result);
        }

        [TestMethod()]
        public void AsyncGetTest()
        {
            var input = new[] { 1, 1, 1, 1, 1 };
            var sched = new TestScheduler();
            var output = new ConcurrentQueue<int>();

            var delay = TimeSpan.FromSeconds(1.0);
            var fixture = new ObservableAsyncMRUCache<int, int>(x => Observable.Return(x*5).Delay(delay, sched), 5, 2);

            int result = 0;
            input.ToObservable(sched).SelectMany<int, int>(x => (IObservable<int>)fixture.AsyncGet(x)).Subscribe(x => result += x);

            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(500)));
            Assert.AreEqual(0, result);

            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(1200)));
            Assert.AreEqual(25, result);

            this.Log().Debug("Running to end");
            sched.Run();
            Assert.AreEqual(25, result);
        }

#if FALSE
        [TestMethod()]
        public void CachedValuesTest()
        {
            var input = new[] { 1, 2, 1, 3, 1 };
            var sched = new TestScheduler();

            var delay = TimeSpan.FromSeconds(1.0);
            var fixture = new ObservableAsyncMRUCache<int, int>(x => Observable.Return(x*5).Delay(delay, sched), 2, 2);

            var results = input.ToObservable().SelectMany(fixture.AsyncGet).CreateCollection();
            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(500)));

            Assert.AreEqual(0, fixture.CachedValues().Count());

            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(1500)));

            var output = fixture.CachedValues().ToArray();
            Assert.IsTrue(output.Length == 2);
            Assert.AreEqual(input.Length, results.Count);
        }
#endif

        [TestMethod()]
        public void CacheShouldQueueOnceWeHitOurConcurrentLimit()
        {
            var input = new[] { 1, 2, 3, 4, 1 };
            var sched = new TestScheduler();
            var output = new ConcurrentQueue<int>();

            var delay = TimeSpan.FromSeconds(1.0);
            var fixture = new ObservableAsyncMRUCache<int, int>(x => Observable.Return(x*5).Delay(delay, sched), 5, 2);

            int result = 0;
            input.ToObservable(sched).SelectMany<int, int>(x => (IObservable<int>)fixture.AsyncGet(x)).Subscribe(x => result += x);

            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(500)));
            Assert.AreEqual(0, result);

            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(1500)));
            Assert.AreEqual(1*5 + 2*5 + 1*5, result);

            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(2500)));
            Assert.AreEqual(1*5 + 2*5 + 3*5 + 4*5 + 1*5, result);

            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(5000)));
            Assert.AreEqual(1*5 + 2*5 + 3*5 + 4*5 + 1*5, result);
        }

        [TestMethod()]
        public void CacheShouldEatExceptionsAndMarshalThemToObservable()
        {
            /* This is a bit tricky:
             * 
             * 5,2 complete at t=1000 simultaneously
             * 10,0 get queued up, 0 fails immediately (delay() doesn't delay the OnError), 
             *    so 10 completes at t=2000
             * The 2nd 5 is cached, it returns at t=1000
             */
            var input = new[] { 5, 2, 10, 0/*boom!*/, 7 };
            var sched = new TestScheduler();

            Observable.Throw<int>(new Exception("Foo")).Subscribe(x => {
                Console.WriteLine(x);
            }, ex => {
                Console.WriteLine(ex);
            }, () => {
                Console.WriteLine("Completed");
            });

            var delay = TimeSpan.FromSeconds(1.0);
            var fixture = new ObservableAsyncMRUCache<int, int>(x => 
                (x == 0 ? Observable.Throw<int>(new Exception("Boom!")) : Observable.Return(10 * x)).Delay(delay, sched), 5, 2, sched);

            Exception exception = null;
            int completed = 0;
            input.ToObservable()
                .SelectMany(x => (IObservable<int>)fixture.AsyncGet(x))
                .Subscribe(x => {
                    this.Log().DebugFormat("Result = {0}", x);
                    completed++;
                }, ex => exception = exception ?? ex);

            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(500)));
            Assert.IsNull(exception);
            Assert.AreEqual(0, completed);

            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(1500)));
            Assert.IsNotNull(exception);
            Assert.AreEqual(2, completed);

            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(7500)));
            Assert.IsNotNull(exception);
            Assert.AreEqual(4, completed);
            this.Log().Debug(exception);
        }

        [TestMethod()]
        public void BlockingGetShouldRethrowExceptions()
        {
            var input = new[] { 5, 2, 10, 0/*boom!*/, 5 };
            var fixture = new QueuedAsyncMRUCache<int, int>(x => { Thread.Sleep(1000); return 50 / x; }, 5, 5);
            int[] output = {5, 2, 10, 5};

            bool did_throw = false;
            try {
                output = input.Select(x => fixture.Get(x)).ToArray();
            } catch(Exception ex) {
                did_throw = true;
                this.Log().Debug("Exception thrown", ex);
            }

            output.Run(x => this.Log().Debug(x));
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
