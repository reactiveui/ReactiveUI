using ReactiveUI;
using ReactiveUI.Testing;
using Xunit;
using System;
using System.Linq;
using System.Concurrency;
using System.Collections.Generic;

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

                sched.Run();
            });

            // Note: Why doesn't the list match the above one? We're supposed
            // to suppress duplicate notifications, of course :)
            (new[] { -5, 1, 2, 3, 4 }).AssertAreEqual(output);
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

            sched.Run();
            Assert.Equal(4, fixture.Value);

            input.OnCompleted();
            sched.Run();
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

            sched.Run();

            Assert.Equal(4, fixture.Value);

            input.OnError(new Exception("Die!"));

            sched.Run();

            try {
                Assert.Equal(4, fixture.Value);
            } catch {
                return;
            }
            Assert.True(false, "We should've threw there");
        }
    }
}
