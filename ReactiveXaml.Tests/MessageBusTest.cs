using System.Concurrency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using ReactiveXaml;
using System.IO;
using System.Text;
using ReactiveXaml.Tests;
using System.Runtime.Serialization.Json;
using System.Threading;

namespace ReactiveXaml.Tests
{
    [TestClass]
    public class MessageBusTest
    {
        [TestMethod]
        public void MessageBusSmokeTest()
        {
            var input = new[] {1, 2, 3, 4};

            var result = (new TestScheduler()).With(sched => {
                var source = new Subject<int>();
                var fixture = new MessageBus();

                fixture.RegisterMessageSource(source, "Test");
                Assert.IsFalse(fixture.IsRegistered(typeof (int)));
                Assert.IsFalse(fixture.IsRegistered(typeof (int), "Foo"));

                var output = fixture.Listen<int>("Test").CreateCollection();

                input.Run(source.OnNext);

                sched.Run();
                return output;
            });

            input.AssertAreEqual(result);
        }
    }
}
