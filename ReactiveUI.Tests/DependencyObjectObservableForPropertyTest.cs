using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ReactiveUI.Xaml;
using Xunit;
using System.Windows.Controls;

namespace ReactiveUI.Tests
{
    public class ViewModelClass : ReactiveObject{
        string _TestString;
        public string TestString
        {
            get { return _TestString; }
            set { this.RaiseAndSetIfChanged(value); }
        }

        string _TestString1;
        public string TestString1
        {
            get { return _TestString1; }
            set { this.RaiseAndSetIfChanged(value); }
        }
    }

    public class DepObjFixture : UserControl, IViewFor<ViewModelClass>
    {

        public string TestString
        {
            get { return (string)GetValue(TestStringProperty); }
            set { SetValue(TestStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TestString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TestStringProperty =
            DependencyProperty.Register
            ("TestString",
            typeof(string), 
            typeof(DepObjFixture ), 
            new FrameworkPropertyMetadata(
                null, 
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault ));

        public string TestString2
        {
            get { return (string)GetValue(TestString2Property); }
            set { SetValue(TestString2Property, value); }
        }

        // Using a DependencyProperty as the backing store for TestString2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TestString2Property =
            DependencyProperty.Register
            ("TestString2",
            typeof(string), 
            typeof(DepObjFixture ), 
            new FrameworkPropertyMetadata(
                null, 
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault ));

        #region IViewFor<ViewModelClass>
        public ViewModelClass ViewModel
        {
            get { return (ViewModelClass)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ViewModelClass), typeof(DepObjFixture ), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ViewModelClass)value; }
        }
        #endregion

    }

    public class DependencyObjectObservableForPropertyTest
    {
        [Fact]
        public void DependencyObjectObservableForPropertySmokeTest()
        {
            var fixture = new DepObjFixture();
            var binder = new DependencyObjectObservableForProperty();
            Assert.NotEqual(0, binder.GetAffinityForObject(typeof (DepObjFixture)));

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
            var outputs2 = fixture.WhenAny(x => x.TestString2, x => x.Value).CreateCollection();

            inputs.ForEach(x => fixture.TestString2 = x);
            inputs.ForEach(x => fixture.TestString = x);

            Assert.Null(outputs.First());
            Assert.Null(outputs2.First());
            Assert.Equal(4, outputs.Count);
            Assert.Equal(4, outputs2.Count);
            Assert.True(inputs.Zip(outputs.Skip(1), (expected, actual) => expected == actual).All(x => x));
            Assert.True(inputs.Zip(outputs2.Skip(1), (expected, actual) => expected == actual).All(x => x));
        }

        [Fact]
        public void WhenAnyWithDependencyObject2()
        {
            var fixture = new DepObjFixture();
            var o0 = fixture.WhenAny(x => x.TestString, x => x.Value);

            int c0 = 0;
            Assert.Equal(0,c0);

            o0.Subscribe(x => c0++);

            Assert.Equal(1,c0);

            fixture.TestString = "hello";
            Assert.Equal(2,c0);

            fixture.TestString = "hello1";
            Assert.Equal(3,c0);

            fixture.TestString = "hello2";
            Assert.Equal(4,c0);

            fixture.TestString = "hello3";
            Assert.Equal(5,c0);
            
        }

        [Fact]
        public void WhenAnyWithDependencyObject3()
        {
            var fixture = new DepObjFixture();
            var o0 = fixture.WhenAny(x => x.TestString, x => x.Value);
            var o1 = fixture.WhenAny(x => x.TestString2, x => x.Value);

            int c0 = 0;
            int c1 = 0;
            int c2 = 0;
            Assert.Equal(0,c0);
            Assert.Equal(0,c1);

            o0.Subscribe(x => c0++);
            o1.Subscribe(x => c1++);
            o1.Subscribe(x => c2++);

            Assert.Equal(1,c0);
            Assert.Equal(1,c1);

            fixture.TestString = "hello";
            Assert.Equal(2,c0);
            Assert.Equal(1,c1);

            fixture.TestString2 = "hello";
            Assert.Equal(2,c0);
            Assert.Equal(2,c1);

            fixture.TestString2 = "hello1";
            Assert.Equal(2,c0);
            Assert.Equal(3,c1);

            fixture.TestString = "hello1";
            Assert.Equal(3,c0);
            Assert.Equal(3,c1);

            fixture.TestString = "hello2";
            Assert.Equal(4,c0);
            Assert.Equal(3,c1);

            fixture.TestString = "hello3";
            Assert.Equal(5,c0);
            Assert.Equal(3,c1);

            fixture.TestString2 = "hello3";
            Assert.Equal(5,c0);
            Assert.Equal(4,c1);
            
        }

    }
}