using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
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

    public class DerivedDepObjFixture : DepObjFixture
    {
        public string AnotherTestString {
            get { return (string)GetValue(AnotherTestStringProperty); }
            set { SetValue(AnotherTestStringProperty, value); }
        }
        public static readonly DependencyProperty AnotherTestStringProperty =
            DependencyProperty.Register("AnotherTestString", typeof(string), typeof(DerivedDepObjFixture), new PropertyMetadata(null));
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
            Expression<Func<DepObjFixture, object>> expression = x => x.TestString;
            var disp1 = binder.GetNotificationForProperty(fixture, expression.Body).Subscribe(results.Add);
            var disp2 = binder.GetNotificationForProperty(fixture, expression.Body).Subscribe(results.Add);

            fixture.TestString = "Foo";
            fixture.TestString = "Bar";

            Assert.Equal(4, results.Count);

            disp1.Dispose();
            disp2.Dispose();
        }

        [Fact]
        public void DerivedDependencyObjectObservableForPropertySmokeTest()
        {
            var fixture = new DerivedDepObjFixture();
            var binder = new DependencyObjectObservableForProperty();
            Assert.NotEqual(0, binder.GetAffinityForObject(typeof (DerivedDepObjFixture), "TestString"));
            Assert.Equal(0, binder.GetAffinityForObject(typeof (DerivedDepObjFixture), "DoesntExist"));

            var results = new List<IObservedChange<object, object>>();
            Expression<Func<DerivedDepObjFixture, object>> expression = x => x.TestString;
            var disp1 = binder.GetNotificationForProperty(fixture, expression.Body).Subscribe(results.Add);
            var disp2 = binder.GetNotificationForProperty(fixture, expression.Body).Subscribe(results.Add);

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

            var outputs = fixture.WhenAnyValue(x => x.TestString).CreateCollection();
            inputs.ForEach(x => fixture.TestString = x);

            Assert.Null(outputs.First());
            Assert.Equal(4, outputs.Count);
            Assert.True(inputs.Zip(outputs.Skip(1), (expected, actual) => expected == actual).All(x => x));
        }

        [Fact]
        public void ListBoxSelectedItemTest()
        {
            var input = new ListBox();
            input.Items.Add("Foo");
            input.Items.Add("Bar");
            input.Items.Add("Baz");

            var output = input.WhenAnyValue(x => x.SelectedItem).CreateCollection();
            Assert.Equal(1, output.Count);

            input.SelectedIndex = 1;
            Assert.Equal(2, output.Count);

            input.SelectedIndex = 2;
            Assert.Equal(3, output.Count);
        }
    }
}