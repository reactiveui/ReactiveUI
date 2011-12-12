using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
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

            var fixture = new ObservableAsPropertyHelper<int>(input,
                _ => { }, -5, sched);

            Assert.Equal(-5, fixture.Value);
            (new[] { 1, 2, 3, 4 }).Run(x => input.OnNext(x));

            sched.Start();

            Assert.Equal(4, fixture.Value);

            input.OnError(new Exception("Die!"));

            sched.Start();

            try {
                Assert.Equal(4, fixture.Value);
            } catch {
                return;
            }
            Assert.True(false, "We should've threw there");
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

                sched.RunToMilliseconds(500);

                new[] {"0", "5", "10", "15", "20"}.AssertAreEqual(result);
            });
        }
    }
}