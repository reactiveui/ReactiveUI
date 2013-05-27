using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ReactiveUI.Xaml;
using Xunit;

namespace ReactiveUI.Tests
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

    public class DependencyObjectObservableForPropertyTest
    {
        [Fact]
        public void DependencyObjectObservableForPropertySmokeTest()
        {
            var fixture = new DepObjFixture();
            var binder = new DependencyObjectObservableForProperty();
            Assert.NotEqual(0, binder.GetAffinityForObject(typeof (DepObjFixture), "TestString"));
            Assert.Equal(0, binder.GetAffinityForObject(typeof (DepObjFixture), "DoesntExist"));

            var results = new List<IObservedChange<object, object>>();
            var disp1 = binder.GetNotificationForProperty(fixture, "TestString").Subscribe(results.Add);
            var disp2 = binder.GetNotificationForProperty(fixture, "TestString").Subscribe(results.Add);

            fixture.TestString = "Foo";
            fixture.TestString = "Bar";

            Assert.Equal(4, results.Count);

            disp1.Dispose();
            disp2.Dispose();
        }

        [Fact]
        public void WhenAnyWithDependencyObjectTest()
        {
            var inputs = new[] {"Foo", "Bar", "Baz"};
            var fixture = new DepObjFixture();

            var outputs = fixture.WhenAny(x => x.TestString, x => x.Value).CreateCollection();
            inputs.ForEach(x => fixture.TestString = x);

            Assert.Null(outputs.First());
            Assert.Equal(4, outputs.Count);
            Assert.True(inputs.Zip(outputs.Skip(1), (expected, actual) => expected == actual).All(x => x));
        }
    }
}