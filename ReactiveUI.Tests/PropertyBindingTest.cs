using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Windows.Controls;
using ReactiveUI.Xaml;
using Xunit;

namespace ReactiveUI.Tests
{
    public class PropertyBindViewModel : ReactiveObject
    {
        public string _Property1;
        public string Property1 {
            get { return _Property1; }
            set { this.RaiseAndSetIfChanged(x => x.Property1, value); }
        }

        public int _Property2;
        public int Property2 {
            get { return _Property2; }
            set { this.RaiseAndSetIfChanged(x => x.Property2, value); }
        }
    }

    public class PropertyBindView : IViewFor<PropertyBindViewModel>
    {
        object IViewFor.ViewModel { 
            get { return ViewModel; }
            set { ViewModel = (PropertyBindViewModel)value; } 
        }

        public PropertyBindViewModel ViewModel { get; set; }

        public TextBox SomeTextBox { get; protected set; }

        public PropertyBindView()
        {
            SomeTextBox = new TextBox();
        }
    }

    public class PropertyBindingTest
    {
        [Fact]
        public void TwoWayBindSmokeTest()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView() {ViewModel = vm};
            var fixture = new PropertyBinderImplementation();

            vm.Property1 = "Foo";
            Assert.NotEqual(vm.Property1, view.SomeTextBox.Text);

            var disp = fixture.Bind(vm, view, x => x.Property1, x => x.SomeTextBox.Text, (IObservable<Unit>)null, null);

            Assert.Equal(vm.Property1, view.SomeTextBox.Text);
            Assert.Equal("Foo", vm.Property1);

            view.SomeTextBox.Text = "Bar";
            Assert.Equal(vm.Property1, "Bar");

            disp.Dispose();
            vm.Property1 = "Baz";

            Assert.Equal("Baz", vm.Property1);
            Assert.NotEqual(vm.Property1, view.SomeTextBox.Text);
        }

        [Fact]
        public void TypeConvertedTwoWayBindSmokeTest()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView() { ViewModel = vm };
            var fixture = new PropertyBinderImplementation();

            vm.Property2 = 17;
            Assert.NotEqual(vm.Property2.ToString(), view.SomeTextBox.Text);

            var disp = fixture.Bind(vm, view, x => x.Property2, x => x.SomeTextBox.Text, (IObservable<Unit>)null, null);

            Assert.Equal(vm.Property2.ToString(), view.SomeTextBox.Text);
            Assert.Equal(17, vm.Property2);

            view.SomeTextBox.Text = "42";
            Assert.Equal(42, vm.Property2);

            disp.Dispose();
            vm.Property2 = 0;

            Assert.Equal(0, vm.Property2);
            Assert.NotEqual("0", view.SomeTextBox.Text);
        }
    }
}