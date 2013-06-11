using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using ReactiveUI.Testing;
using Xunit;

using Microsoft.Reactive.Testing;

namespace ReactiveUI.Tests
{
    public class ObservableAsyncMRUCacheTest
    {
        [Fact]
        public void AsyncGetTest()
        {
            var input = new[] { 1, 1, 1, 1, 1 };
            var sched = new TestScheduler();

            var delay = TimeSpan.FromSeconds(1.0);
            var fixture = new ObservableAsyncMRUCache<int, int>(x => Observable.Return(x*5).Delay(delay, sched), 5, 2, null, sched);

            int result = 0;
            input.ToObservable(sched).SelectMany<int, int>(x => (IObservable<int>)fixture.AsyncGet(x)).Subscribe(x => result += x);

            sched.AdvanceTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(500)));
            Assert.Equal(0, result);

            sched.AdvanceTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(1200)));
            Assert.Equal(25, result);

            sched.Start();
            Assert.Equal(25, result);
        }

        [Fact]
        public void CacheShouldQueueOnceWeHitOurConcurrentLimit()
        {
            var input = new[] { 1, 2, 3, 4, 1 };
            var sched = new TestScheduler();

            var delay = TimeSpan.FromSeconds(1.0);
            var fixture = new ObservableAsyncMRUCache<int, int>(x => Observable.Return(x*5).Delay(delay, sched), 5, 2, null, sched);

            int result = 0;
            input.ToObservable(sched).SelectMany<int, int>(x => (IObservable<int>)fixture.AsyncGet(x)).Subscribe(x => result += x);

            sched.AdvanceTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(500)));
            Assert.Equal(0, result);

            sched.AdvanceTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(1500)));
            Assert.Equal(1*5 + 2*5 + 1*5, result);

            sched.AdvanceTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(2500)));
            Assert.Equal(1*5 + 2*5 + 3*5 + 4*5 + 1*5, result);

            sched.AdvanceTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(5000)));
            Assert.Equal(1*5 + 2*5 + 3*5 + 4*5 + 1*5, result);
        }
    }
}
