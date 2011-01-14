using ReactiveXaml;
using Xunit;
using System;
using System.Linq;
using System.Concurrency;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using ReactiveXaml.Testing;

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

    public class DependencyObjectMixinTest : IEnableLogger
    {
        [Fact]
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
                    Assert.Equal(fixture, v.Sender);
                    Assert.Equal("TestString", v.PropertyName);
                }

                return new Unit();
            });
        }
    }
}
