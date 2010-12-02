using ReactiveXaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Concurrency;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace ReactiveXaml.Tests
{
    public class DepObjFixture : FrameworkElement
    {
        public static readonly DependencyProperty TestStringProperty = 
            DependencyProperty.Register("TestString", typeof(string), typeof(DepObjFixture), new PropertyMetadata(null));

        public string TestString {
            get { return (string)GetValue(TestStringProperty); }
            set { SetValue(TestStringProperty, value); }
        }
    }

    [TestClass()]
    public class DependencyObjectMixinTest : IEnableLogger
    {
        [TestMethod()]
        public void ObservableFromDPSmokeTest()
        {
            (new TestScheduler()).With(sched => {
                var input = new[] {"Foo", "Bar", "Baz"};
                var fixture = new DepObjFixture();
                var output = fixture.ObservableFromDP(x => x.TestString).CreateCollection();

                foreach (var v in input) {
                    fixture.TestString = v;
                }

                sched.Run();
                input.AssertAreEqual(output.Select(x => x.Value));
                foreach (var v in output) {
                    Assert.AreEqual(fixture, v.Sender);
                    Assert.AreEqual("TestString", v.PropertyName);
                }

                return new Unit();
            });
        }
    }
}
