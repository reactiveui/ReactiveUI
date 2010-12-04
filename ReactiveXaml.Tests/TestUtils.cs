using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Disposables;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReactiveXaml.Tests
{
    public static class EnumerableTestMixin
    {
        public static void AssertAreEqual<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs)
        {
            var left = lhs.ToArray();
            var right = rhs.ToArray();

            Assert.AreEqual(left.Length, right.Length);
            for(int i=0; i < left.Length; i++) {
                Assert.AreEqual(left[i], right[i]);
            }
        }
    }

    public static class TestUtils
    {
        public static IDisposable WithTestScheduler(TestScheduler sched) {
            var prevDef = RxApp.DeferredScheduler;
            var prevTask = RxApp.TaskpoolScheduler;

            RxApp.DeferredScheduler = sched;
            RxApp.TaskpoolScheduler = sched;

            return Disposable.Create(() => {
                RxApp.DeferredScheduler = prevDef;
                RxApp.TaskpoolScheduler = prevTask;
            });
        }

        public static TRet With<TRet>(this TestScheduler sched, Func<TestScheduler, TRet> block)
        {
            TRet ret;
            using(WithTestScheduler(sched)) {
                ret = block(sched);
            }
            return ret;
        }

        public static void RunToMilliseconds(this TestScheduler sched, double milliseconds)
        {
            Console.WriteLine("Running to time t={0}", milliseconds);
            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)));
        }
    }
}
