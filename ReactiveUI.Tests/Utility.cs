﻿using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Linq;
using Xunit;
using System.Collections;

namespace ReactiveUI.Tests
{
    public static class EnumerableTestMixin
    {
        public static void AssertAreEqual<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs)
        {
            var left = lhs.ToArray();
            var right = rhs.ToArray();

            try {
                Assert.Equal(left.Length, right.Length);
                for (int i = 0; i < left.Length; i++) {
                    Assert.Equal(left[i], right[i]);
                }
            } catch {
#if !WINRT
                Console.Error.WriteLine("lhs: [{0}]",
                    String.Join(",", lhs.ToArray()));
                Console.Error.WriteLine("rhs: [{0}]",
                    String.Join(",", rhs.ToArray()));
#endif
                throw;
            }
        }

        public static IEnumerable<T> DistinctUntilChanged<T>(this IEnumerable<T> This)
        {
            bool isFirst = true;
            T lastValue = default(T);

            foreach(var v in This) {
                if (isFirst) {
                    lastValue = v;
                    isFirst = false;
                    yield return v;
                    continue;
                }

                if (!EqualityComparer<T>.Default.Equals(v, lastValue)) {
                    yield return v;
                }
                lastValue = v;
            }
        }

#if WINRT
        public static IEnumerable<T> OfType<T>(this IEnumerable This)
        {
            foreach (T item in This) {
                yield return item;
            }
        }
#endif
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

        public DateTimeOffset Now {
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
            foreach (var v in This) {
                block(v); 
            }
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> This, int count)
        {
            return This.Take(This.Count() - count);
        }
    }
}