// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Windows.Forms;
using DynamicData;
using ReactiveUI.Winforms;
using Xunit;

#pragma warning disable SA1403 // FileMayOnlyContainASingleNamespace

namespace ReactiveUI.Tests.Winforms
{
    public class DefaultPropertyBindingTests
    {
        [Fact]
        public void WinformsCreatesObservableForPropertyWorksForTextboxes()
        {
            var input = new TextBox();
            var fixture = new WinformsCreatesObservableForProperty();

            Assert.NotEqual(0, fixture.GetAffinityForObject(typeof(TextBox), "Text"));

            Expression<Func<TextBox, string>> expression = x => x.Text;
            var propertyName = expression.Body.GetMemberInfo().Name;
            var dispose = fixture.GetNotificationForProperty(input, expression.Body, propertyName).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var output).Subscribe();
            Assert.Equal(0, output.Count);

            input.Text = "Foo";
            Assert.Equal(1, output.Count);
            Assert.Equal(input, output[0].Sender);
            Assert.Equal("Text", output[0].GetPropertyName());

            dispose.Dispose();

            input.Text = "Bar";
            Assert.Equal(1, output.Count);
        }

        [Fact]
        public void WinformsCreatesObservableForPropertyWorksForComponents()
        {
            var input = new ToolStripButton(); // ToolStripButton is a Component, not a Control
            var fixture = new WinformsCreatesObservableForProperty();

            Assert.NotEqual(0, fixture.GetAffinityForObject(typeof(ToolStripButton), "Checked"));

            Expression<Func<ToolStripButton, bool>> expression = x => x.Checked;
            var propertyName = expression.Body.GetMemberInfo().Name;
            var dispose = fixture.GetNotificationForProperty(input, expression.Body, propertyName).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var output).Subscribe();
            Assert.Equal(0, output.Count);

            input.Checked = true;
            Assert.Equal(1, output.Count);
            Assert.Equal(input, output[0].Sender);
            Assert.Equal("Checked", output[0].GetPropertyName());

            dispose.Dispose();

            // Since we disposed the derived list, we should no longer receive updates
            input.Checked = false;
            Assert.Equal(1, output.Count);
        }

        [Fact]
        public void WinformsCreatesObservableForPropertyWorksForThirdPartyControls()
        {
            var input = new AThirdPartyNamespace.ThirdPartyControl();
            var fixture = new WinformsCreatesObservableForProperty();

            Assert.NotEqual(0, fixture.GetAffinityForObject(typeof(AThirdPartyNamespace.ThirdPartyControl), "Value"));

            Expression<Func<AThirdPartyNamespace.ThirdPartyControl, string>> expression = x => x.Value;
            var propertyName = expression.Body.GetMemberInfo().Name;
            var dispose = fixture.GetNotificationForProperty(input, expression.Body, propertyName).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var output).Subscribe();
            Assert.Equal(0, output.Count);

            input.Value = "Foo";
            Assert.Equal(1, output.Count);
            Assert.Equal(input, output[0].Sender);
            Assert.Equal("Value", output[0].GetPropertyName());

            dispose.Dispose();

            input.Value = "Bar";
            Assert.Equal(1, output.Count);
        }

        [Fact]
        public void CanBindViewModelToWinformControls()
        {
            var vm = new FakeWinformViewModel();
            var view = new FakeWinformsView { ViewModel = vm };

            vm.SomeText = "Foo";
            Assert.NotEqual(vm.SomeText, view.Property3.Text);

            var disp = view.Bind(vm, x => x.SomeText, x => x.Property3.Text);
            vm.SomeText = "Bar";
            Assert.Equal(vm.SomeText, view.Property3.Text);

            view.Property3.Text = "Bar2";
            Assert.Equal(vm.SomeText, view.Property3.Text);

            var disp2 = view.Bind(vm, x => x.SomeDouble, x => x.Property3.Text);
            vm.SomeDouble = 123.4;

            Assert.Equal(vm.SomeDouble.ToString(), view.Property3.Text);
        }

        [Fact]
        public void SmokeTestWinformControls()
        {
            var vm = new FakeWinformViewModel();
            var view = new FakeWinformsView { ViewModel = vm };

            var disp = new CompositeDisposable(new[]
            {
                view.Bind(vm, x => x.Property1, x => x.Property1.Text),
                view.Bind(vm, x => x.Property2, x => x.Property2.Text),
                view.Bind(vm, x => x.Property3, x => x.Property3.Text),
                view.Bind(vm, x => x.Property4, x => x.Property4.Text),
                view.Bind(vm, x => x.BooleanProperty, x => x.BooleanProperty.Checked),
            });

            vm.Property1 = "FOOO";
            Assert.Equal(vm.Property1, view.Property1.Text);

            vm.Property2 = "FOOO1";
            Assert.Equal(vm.Property2, view.Property2.Text);

            vm.Property3 = "FOOO2";
            Assert.Equal(vm.Property3, view.Property3.Text);

            vm.Property4 = "FOOO3";
            Assert.Equal(vm.Property4, view.Property4.Text);

            vm.BooleanProperty = false;
            Assert.Equal(vm.BooleanProperty, view.BooleanProperty.Checked);
            vm.BooleanProperty = true;
            Assert.Equal(vm.BooleanProperty, view.BooleanProperty.Checked);

            disp.Dispose();
        }
    }

    public class FakeWinformViewModel : ReactiveObject, IRoutableViewModel
    {
        private bool _someBooleanProperty;
        private int _someInteger;
        private string _someText;
        private double _someDouble;
        private string _property1;
        private string _property2;
        private string _property3;
        private string _property4;

        public FakeWinformViewModel(IScreen screen = null)
        {
            HostScreen = screen;
        }

        public string UrlPathSegment => "fake";

        public IScreen HostScreen { get; }

        public int SomeInteger
        {
            get => _someInteger;
            set => this.RaiseAndSetIfChanged(ref _someInteger, value);
        }

        public string SomeText
        {
            get => _someText;
            set => this.RaiseAndSetIfChanged(ref _someText, value);
        }

        public double SomeDouble
        {
            get => _someDouble;
            set => this.RaiseAndSetIfChanged(ref _someDouble, value);
        }

        public string Property1
        {
            get => _property1;
            set => this.RaiseAndSetIfChanged(ref _property1, value);
        }

        public string Property2
        {
            get => _property2;
            set => this.RaiseAndSetIfChanged(ref _property2, value);
        }

        public string Property3
        {
            get => _property3;
            set => this.RaiseAndSetIfChanged(ref _property3, value);
        }

        public string Property4
        {
            get => _property4;
            set => this.RaiseAndSetIfChanged(ref _property4, value);
        }

        public bool BooleanProperty
        {
            get => _someBooleanProperty;
            set => this.RaiseAndSetIfChanged(ref _someBooleanProperty, value);
        }
    }

    public class FakeWinformsView : Control, IViewFor<FakeWinformViewModel>
    {
        public FakeWinformsView()
        {
            Property1 = new Button();
            Property2 = new Label();
            Property3 = new TextBox();
            Property4 = new RichTextBox();
            BooleanProperty = new CheckBox();
            SomeDouble = new TextBox();
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (FakeWinformViewModel)value;
        }

        public FakeWinformViewModel ViewModel { get; set; }

        public Button Property1 { get; }

        public Label Property2 { get; }

        public TextBox Property3 { get; }

        public RichTextBox Property4 { get; }

        public CheckBox BooleanProperty { get; }

        public TextBox SomeDouble { get; }
    }
}

namespace AThirdPartyNamespace
{
    public class ThirdPartyControl : Control
    {
        private string _value;

        public event EventHandler ValueChanged;

        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnValueChanged();
                }
            }
        }

        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
