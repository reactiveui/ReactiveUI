using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI.Xaml;
using Xunit;

namespace ReactiveUI.Tests
{
    public class PropertyBindModel
    {
        public int AThing { get; set; }
        public string AnotherThing { get; set; }
    }

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

        public double _JustADouble;
        public double JustADouble {
            get { return _JustADouble; }
            set { this.RaiseAndSetIfChanged(x => x.JustADouble, value); }
        }

        public double? _NullableDouble;
        public double? NullableDouble {
            get { return _NullableDouble; }
            set { this.RaiseAndSetIfChanged(x => x.NullableDouble, value); }
        }

        public ReactiveCollection<string> SomeCollectionOfStrings { get; protected set; }

        public PropertyBindModel Model { get; protected set; }

        public PropertyBindViewModel()
        {
            Model = new PropertyBindModel() {AThing = 42, AnotherThing = "Baz"};
            SomeCollectionOfStrings = new ReactiveCollection<string>(new[] { "Foo", "Bar" });
        }
    }

    public class PropertyBindView : Control, IViewFor<PropertyBindViewModel>
    {
        public PropertyBindViewModel ViewModel {
            get { return (PropertyBindViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(PropertyBindViewModel), typeof(PropertyBindView), new PropertyMetadata(null));

        object IViewFor.ViewModel { 
            get { return ViewModel; }
            set { ViewModel = (PropertyBindViewModel)value; } 
        }
        
        public TextBox SomeTextBox;
        public ListBox SomeListBox;
        public TextBox Property2;
        public PropertyBindFakeControl FakeControl;
        public ItemsControl FakeItemsControl;

        public PropertyBindView()
        {
            SomeTextBox = new TextBox();
            SomeListBox = new ListBox();
            Property2 = new TextBox();
            FakeControl = new PropertyBindFakeControl();
            FakeItemsControl = new ItemsControl();
        }
    }

    public class PropertyBindFakeControl : Control
    {
        public double? NullableDouble {
            get { return (double?)GetValue(NullableDoubleProperty); }
            set { SetValue(NullableDoubleProperty, value); }
        }
        public static readonly DependencyProperty NullableDoubleProperty =
            DependencyProperty.Register("NullableDouble", typeof(double?), typeof(PropertyBindFakeControl), new PropertyMetadata(null));

        public double JustADouble {
            get { return (double)GetValue(JustADoubleProperty); }
            set { SetValue(JustADoubleProperty, value); }
        }
        public static readonly DependencyProperty JustADoubleProperty =
            DependencyProperty.Register("JustADouble", typeof(double), typeof(PropertyBindFakeControl), new PropertyMetadata(0.0));
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

        [Fact]
        public void BindingToItemsControl()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView() {ViewModel = vm};

            view.OneWayBind(view.ViewModel, x => x.SomeCollectionOfStrings, x => x.SomeListBox.ItemsSource);
            Assert.True(view.SomeListBox.ItemsSource.OfType<string>().Count() > 1);
        }

        [Fact]
        public void BindingIntoModelObjects()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView() {ViewModel = vm};

            view.OneWayBind(view.ViewModel, x => x.Model.AnotherThing, x => x.SomeTextBox.Text);
            Assert.Equal("Baz", view.SomeTextBox.Text);
        }

        [Fact]
        public void ImplicitBindPlusTypeConversion() 
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView() {ViewModel = vm};

            view.Bind(view.ViewModel, x => x.Property2);

            vm.Property2 = 42;
            Assert.Equal("42", view.Property2.Text);

            view.Property2.Text = "7";
            Assert.Equal(7, vm.Property2);
        }

        [Fact]
        public void ViewModelNullableToViewNonNullable()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView() {ViewModel = vm};

            view.Bind(view.ViewModel, x => x.NullableDouble, x => x.FakeControl.JustADouble);
            Assert.Equal(0.0, view.FakeControl.JustADouble);

            vm.NullableDouble = 4.0;
            Assert.Equal(4.0, view.FakeControl.JustADouble);

            vm.NullableDouble = null;
            Assert.Equal(4.0, view.FakeControl.JustADouble);

            vm.NullableDouble = 0.0;
            Assert.Equal(0.0, view.FakeControl.JustADouble);
        }

        [Fact]
        public void ViewModelNonNullableToViewNullable()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView() {ViewModel = vm};

            view.Bind(view.ViewModel, x => x.JustADouble, x => x.FakeControl.NullableDouble);
            Assert.Equal(0.0, vm.JustADouble);

            view.FakeControl.NullableDouble = 4.0;
            Assert.Equal(4.0, vm.JustADouble);

            view.FakeControl.NullableDouble = null;
            Assert.Equal(4.0, vm.JustADouble);

            view.FakeControl.NullableDouble = 0.0;
            Assert.Equal(0.0, vm.JustADouble);
        }

        [Fact]
        public void ViewModelNullableToViewNullable()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView() {ViewModel = vm};

            view.Bind(view.ViewModel, x => x.NullableDouble, x => x.FakeControl.NullableDouble);
            Assert.Equal(null, vm.NullableDouble);

            view.FakeControl.NullableDouble = 4.0;
            Assert.Equal(4.0, vm.NullableDouble);

            view.FakeControl.NullableDouble = null;
            Assert.Equal(null, vm.NullableDouble);

            view.FakeControl.NullableDouble = 0.0;
            Assert.Equal(0.0, vm.NullableDouble);
        }

        [Fact]
        public void ItemsControlShouldGetADataTemplate()
        {
            var vm = new PropertyBindViewModel();
            var view = new PropertyBindView() {ViewModel = vm};

            configureDummyServiceLocator();

            Assert.Null(view.FakeItemsControl.ItemTemplate);
            view.OneWayBind(vm, x => x.SomeCollectionOfStrings, x => x.FakeItemsControl.ItemsSource);

            Assert.NotNull(view.FakeItemsControl.ItemTemplate);
        }

        void configureDummyServiceLocator()
        {
            var types = new Dictionary<Tuple<Type, string>, List<Type>>();
            RxApp.ConfigureServiceLocator(
                (t, s) => Activator.CreateInstance(types[Tuple.Create(t, s)].First()),
                (t, s) => types[Tuple.Create(t, s)].Select(Activator.CreateInstance).ToArray(),
                (c, t, s) => {
                    var tuple = Tuple.Create(t, s);
                    if (!types.ContainsKey(tuple)) types[tuple] = new List<Type>();
                    types[tuple].Add(c);
                });
        }
    }
}