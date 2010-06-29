using ReactiveXaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Concurrency;
using System.Collections.Generic;

namespace ReactiveXaml.Tests
{
    [TestClass()]
    public class ObservableAsPropertyHelperTest
    {
        [TestMethod()]
        public void OAPHShouldFireChangeNotifications()
        {
            var output = new List<int>();

            var input = new[] { 1, 2, 3, 3, 4 }.ToObservable();
            var fixture = new ObservableAsPropertyHelper<int>(input,
                x => output.Add(x), -5);

            // Note: Why doesn't the list match the above one? We're supposed
            // to suppress duplicate notifications, of course :)
            (new[] { 1, 2, 3, 4 })
                .Zip(output, (expected, actual) => new { expected, actual })
                .Run(x => Assert.AreEqual(x.expected, x.actual));
        }

        [TestMethod()]
        public void OAPHShouldProvideLatestValue()
        {
            var input = new Subject<int>();

            var fixture = new ObservableAsPropertyHelper<int>(input,
                _ => { }, -5);

            Assert.AreEqual(-5, fixture.Value);
            (new[] { 1, 2, 3, 4 }).Run(x => input.OnNext(x));

            Assert.AreEqual(4, fixture.Value);

            input.OnCompleted();
            Assert.AreEqual(4, fixture.Value);
        }

        [TestMethod()]
        public void OAPHShouldRethrowErrors()
        {
            var input = new Subject<int>();

            var fixture = new ObservableAsPropertyHelper<int>(input,
                _ => { }, -5);

            Assert.AreEqual(-5, fixture.Value);
            (new[] { 1, 2, 3, 4 }).Run(x => input.OnNext(x));

            Assert.AreEqual(4, fixture.Value);

            input.OnError(new Exception("Die!"));
            try {
                Assert.AreEqual(4, fixture.Value);
            } catch {
                return;
            }
            Assert.Fail("We should've threw there");
        }
    }
}
