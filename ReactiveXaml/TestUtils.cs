using System;
using System.Concurrency;
using System.Disposables;
using System.Reflection;
using System.Text;

namespace ReactiveXaml.Testing
{
    public static class TestUtils
    {
        public static IDisposable WithScheduler(IScheduler sched) {
            var prevDef = RxApp.DeferredScheduler;
            var prevTask = RxApp.TaskpoolScheduler;

            RxApp.DeferredScheduler = sched;
            RxApp.TaskpoolScheduler = sched;

            return Disposable.Create(() => {
                RxApp.DeferredScheduler = prevDef;
                RxApp.TaskpoolScheduler = prevTask;
            });
        }

        public static TRet With<TRet>(this IScheduler sched, Func<IScheduler, TRet> block)
        {
            TRet ret;
            using(WithScheduler(sched)) {
                ret = block(sched);
            }
            return ret;
        }

        public static void With(this IScheduler sched, Action<IScheduler> block)
        {
            sched.With(x => { block(x); return 0; });
        }

        public static TRet With<TRet>(this TestScheduler sched, Func<TestScheduler, TRet> block)
        {
            TRet ret;
            using(WithScheduler(sched)) {
                ret = block(sched);
            }
            return ret;
        }

        public static void With(this TestScheduler sched, Action<TestScheduler> block)
        {
            sched.With(x => { block(x); return 0; });
        }

        public static void RunToMilliseconds(this TestScheduler sched, double milliseconds)
        {
            Console.WriteLine("Running to time t={0}", milliseconds);
            sched.RunTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)));
        }
    }
}
