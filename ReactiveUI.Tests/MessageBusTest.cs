using System.Concurrency;
using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;
using ReactiveUI;
using System.IO;
using System.Text;
using ReactiveUI.Testing;
using ReactiveUI.Tests;
using System.Runtime.Serialization.Json;
using System.Threading;

namespace ReactiveUI.Tests
{
    public class MessageBusTest
    {
        [Fact]
        public void MessageBusSmokeTest()
        {
            var input = new[] {1, 2, 3, 4};

            var result = (new TestScheduler()).With(sched => {
                var source = new Subject<int>();
                var fixture = new MessageBus();

                fixture.RegisterMessageSource(source, "Test");
                Assert.False(fixture.IsRegistered(typeof (int)));
                Assert.False(fixture.IsRegistered(typeof (int), "Foo"));

                var output = fixture.Listen<int>("Test").CreateCollection();

                input.Run(source.OnNext);

                sched.Run();
                return output;
            });

            input.AssertAreEqual(result);
        }
    }
}
