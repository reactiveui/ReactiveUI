using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Linq;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Xunit;
using Xunit.Sdk;
using System.Diagnostics;

namespace ReactiveUI.Tests
{
    public static class EnumerableTestMixin
    {
        public static void AssertAreEqual<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs)
        {
            var left = lhs.ToArray();
            var right = rhs.ToArray();

            try
            {
                Assert.Equal(left.Length, right.Length);
                for (int i = 0; i < left.Length; i++)
                {
                    Assert.Equal(left[i], right[i]);
                }
            }
            catch
            {
                Debug.WriteLine("lhs: [{0}]", String.Join(",", lhs.ToArray()));
                Debug.WriteLine("rhs: [{0}]", String.Join(",", rhs.ToArray()));
                throw;
            }
        }

        public static IEnumerable<T> DistinctUntilChanged<T>(this IEnumerable<T> This)
        {
            bool isFirst = true;
            T lastValue = default(T);

            foreach (var v in This)
            {
                if (isFirst)
                {
                    lastValue = v;
                    isFirst = false;
                    yield return v;
                    continue;
                }

                if (!EqualityComparer<T>.Default.Equals(v, lastValue))
                {
                    yield return v;
                }
                lastValue = v;
            }
        }
    }

    public class CountingTestScheduler : IScheduler
    {
        public CountingTestScheduler(IScheduler innerScheduler)
        {
            InnerScheduler = innerScheduler;
            ScheduledItems = new List<Tuple<Action, TimeSpan?>>();
        }

        public IScheduler InnerScheduler { get; private set; }
        public List<Tuple<Action, TimeSpan?>> ScheduledItems { get; private set; }

        public DateTimeOffset Now
        {
            get { return InnerScheduler.Now; }
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            ScheduledItems.Add(new Tuple<Action, TimeSpan?>(() => action(this, state), null));
            return InnerScheduler.Schedule(state, dueTime, action);
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            ScheduledItems.Add(new Tuple<Action, TimeSpan?>(() => action(this, state), dueTime));
            return InnerScheduler.Schedule(state, dueTime, action);
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            ScheduledItems.Add(new Tuple<Action, TimeSpan?>(() => action(this, state), null));
            return InnerScheduler.Schedule(state, action);
        }
    }

    internal static class CompatMixins
    {
        public static void Run<T>(this IEnumerable<T> This, Action<T> block)
        {
            foreach (var v in This)
            {
                block(v);
            }
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> This, int count)
        {
            return This.Take(This.Count() - count);
        }
    }

    // run tests on invariant culture to avoid problems e.g with culture specific decimal separator
    public class UseInvariantCulture : BeforeAfterTestAttribute
    {
        private CultureInfo storedCulture;

        public override void Before(MethodInfo methodUnderTest)
        {
            storedCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public override void After(MethodInfo methodUnderTest)
        {
            Thread.CurrentThread.CurrentCulture = storedCulture;
        }
    }
}