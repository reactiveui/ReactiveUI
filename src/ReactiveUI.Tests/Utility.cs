// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Threading;
using Xunit;
using Xunit.Sdk;

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
                for (var i = 0; i < left.Length; i++)
                {
                    Assert.Equal(left[i], right[i]);
                }
            }
            catch
            {
                Debug.WriteLine("lhs: [{0}]", string.Join(",", lhs.ToArray()));
                Debug.WriteLine("rhs: [{0}]", string.Join(",", rhs.ToArray()));
                throw;
            }
        }

        public static IEnumerable<T> DistinctUntilChanged<T>(this IEnumerable<T> @this)
        {
            var isFirst = true;
            var lastValue = default(T);

            foreach (var v in @this)
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

        public DateTimeOffset Now => InnerScheduler.Now;

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

    // run tests on invariant culture to avoid problems e.g with culture specific decimal separator
    public class UseInvariantCulture : BeforeAfterTestAttribute
    {
        private CultureInfo _storedCulture;

        public override void Before(MethodInfo methodUnderTest)
        {
            _storedCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public override void After(MethodInfo methodUnderTest)
        {
            Thread.CurrentThread.CurrentCulture = _storedCulture;
        }
    }

    internal static class CompatMixins
    {
        public static void Run<T>(this IEnumerable<T> @this, Action<T> block)
        {
            foreach (var v in @this)
            {
                block(v);
            }
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> @this, int count)
        {
            return @this.Take(@this.Count() - count);
        }
    }
}
