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
                    (ref int field, int x) => { field = x; output.Add(x); }, -5);

                sched.Start();

                Assert.Equal(input.Last(), fixture.Value);

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
                null, -5, sched);

            sched.Start();
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

            var fixture = new ObservableAsPropertyHelper<int>(input, null, -5, sched);
            var errors = new List<Exception>();

            sched.Start();
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
                var fixture = new ObservableAsPropertyHelper<int>(input, null, -5);
    
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
    }
}