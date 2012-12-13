using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReactiveUI.Tests
{
    public class AwaiterTest
    {
        [Fact]
        public void AwaiterSmokeTest()
        {
            var fixture = awaitAnObservable();
            fixture.Wait();

            Assert.Equal(42, fixture.Result);
        }

        async Task<int> awaitAnObservable()
        {
            var o = Observable.Start(() => {
                Thread.Sleep(1000);
                return 42;
            }, RxApp.TaskpoolScheduler);

            var ret = await o;
            return ret;
        }
    }
}
