﻿using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using ReactiveUI.Testing;
using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Reactive.Testing;

namespace ReactiveUI.Tests
{
    public class ObservableAsPropertyHelperTest
    {
        [Fact]
        public void OAPHShouldFireChangeNotifications()
        {
            var input = new[] {1, 2, 3, 3, 4}.ToObservable();
            var output = new List<int>();

            (new TestScheduler()).With(sched => {
                var fixture = new ObservableAsPropertyHelper<int>(input,
                    x => output.Add(x), -5);

                sched.Start();

                // Note: Why doesn't the list match the above one? We're supposed
                // to suppress duplicate notifications, of course :)
                (new[] { -5, 1, 2, 3, 4 }).AssertAreEqual(output);
            });
        }

        [Fact]
        public void OAPHShouldProvideLatestValue()
        {
            var sched = new TestScheduler();
            var input = new Subject<int>();

            var fixture = new ObservableAsPropertyHelper<int>(input,
                _ => { }, -5, sched);

            Assert.Equal(-5, fixture.Value);
            (new[] { 1, 2, 3, 4 }).Run(x => input.OnNext(x));

            sched.Start();
            Assert.Equal(4, fixture.Value);

            input.OnCompleted();
            sched.Start();
            Assert.Equal(4, fixture.Value);
        }

        [Fact]
        public void OAPHShouldRethrowErrors()
        {
            var input = new Subject<int>();
            var sched = new TestScheduler();

            var fixture = new ObservableAsPropertyHelper<int>(input, _ => { }, -5, sched);
            var errors = new List<Exception>();

            Assert.Equal(-5, fixture.Value);
            (new[] { 1, 2, 3, 4 }).Run(x => input.OnNext(x));

            fixture.ThrownExceptions.Subscribe(errors.Add);

            sched.Start();

            Assert.Equal(4, fixture.Value);

            input.OnError(new Exception("Die!"));

            sched.Start();

            Assert.Equal(4, fixture.Value);
            Assert.Equal(1, errors.Count);
        }

        [Fact]
        public void NoThrownExceptionsSubscriberEqualsOAPHDeath()
        {
            (new TestScheduler()).With(sched => {
                var input = new Subject<int>();
                var fixture = new ObservableAsPropertyHelper<int>(input, _ => { }, -5);
    
                Assert.Equal(-5, fixture.Value);
                (new[] { 1, 2, 3, 4 }).Run(x => input.OnNext(x));
    
                input.OnError(new Exception("Die!"));
    
                bool failed = true;
                try {
                    sched.Start();
                } catch (Exception ex) {
                    failed = ex.InnerException.Message != "Die!";
                }
    
                Assert.False(failed);
                Assert.Equal(4, fixture.Value);
            });
        }


        [Fact]
        public void OAPHShouldBeObservable()
        {
            (new TestScheduler()).With(sched => {
                var input = sched.CreateHotObservable(
                    sched.OnNextAt(100, 5),
                    sched.OnNextAt(200, 10),
                    sched.OnNextAt(300, 15),
                    sched.OnNextAt(400, 20)
                );

                var result = new List<string>();

                var inputOaph = new ObservableAsPropertyHelper<int>(input, x => { }, 0);
                var fixture = new ObservableAsPropertyHelper<string>(inputOaph.Select(x => x.ToString()),
                    result.Add, "0");

                sched.AdvanceToMs(500);

                new[] {"0", "5", "10", "15", "20"}.AssertAreEqual(result);
            });
        }
    }
}