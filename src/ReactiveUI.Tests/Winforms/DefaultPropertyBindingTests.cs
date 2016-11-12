using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Windows.Forms;
using Xunit;
using ReactiveUI.Winforms;

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
            var output = fixture.GetNotificationForProperty(input, expression.Body).CreateCollection();
            Assert.Equal(0, output.Count);

            input.Text = "Foo";
            Assert.Equal(1, output.Count);
            Assert.Equal(input, output[0].Sender);
            Assert.Equal("Text", output[0].GetPropertyName());

            output.Dispose();

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
            var output = fixture.GetNotificationForProperty(input, expression.Body).CreateCollection();
            Assert.Equal(0, output.Count);

            input.Checked = true;
            Assert.Equal(1, output.Count);
            Assert.Equal(input, output[0].Sender);
            Assert.Equal("Checked", output[0].GetPropertyName());

            output.Dispose();

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
            var output = fixture.GetNotificationForProperty(input, expression.Body).CreateCollection();
            Assert.Equal(0, output.Count);

            input.Value = "Foo";
            Assert.Equal(1, output.Count);
            Assert.Equal(input, output[0].Sender);
            Assert.Equal("Value", output[0].GetPropertyName());

            output.Dispose();

            input.Value = "Bar";
            Assert.Equal(1, output.Count);
        }

        [Fact]
        public void CanBindViewModelToWinformControls()
        {
            var vm = new FakeWinformViewModel();
            var view = new FakeWinformsView(){ViewModel = vm};

            vm.SomeText = "Foo";
            Assert.NotEqual(vm.SomeText, view.Property3.Text);

            var disp = view.Bind(vm, x => x.SomeText, x => x.Property3.Text);
            vm.SomeText = "Bar";
            Assert.Equal(vm.SomeText,view.Property3.Text);

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
            var view = new FakeWinformsView() { ViewModel = vm };

            var disp = new CompositeDisposable(new[] {
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
        public string UrlPathSegment {
            get { return "fake"; }
        }

        public IScreen HostScreen { get; private set; }

        public FakeWinformViewModel(IScreen screen = null)
        {
            HostScreen = screen;
        }

        int someInteger;
        public int SomeInteger {
            get { return this.someInteger; }
            set { this.RaiseAndSetIfChanged(ref this.someInteger, value); }
        }

        string someText;
        public string SomeText {
            get { return this.someText; }
            set { this.RaiseAndSetIfChanged(ref this.someText, value); }
        }

        double someDouble;
        public double SomeDouble {
            get { return this.someDouble; }
            set { this.RaiseAndSetIfChanged(ref this.someDouble, value); }
        }

        string _property1;
        public string Property1 {
            get { return _property1; }
            set { this.RaiseAndSetIfChanged(ref _property1, value); }
        }

        string _property2;
        public string Property2 {
            get { return _property2; }
            set { this.RaiseAndSetIfChanged(ref _property2, value); }
        }

        string _property3;
        public string Property3 {
            get { return _property3; }
            set { this.RaiseAndSetIfChanged(ref _property3, value); }
        }

        string _property4;
        public string Property4 {
            get { return _property4; }
            set { this.RaiseAndSetIfChanged(ref _property4, value); }
        }

        bool _someBooleanProperty;
        public bool BooleanProperty {
            get { return _someBooleanProperty; }
            set { this.RaiseAndSetIfChanged(ref _someBooleanProperty, value); }
        }
    }

    public class FakeWinformsView : Control, IViewFor<FakeWinformViewModel>
    {
        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (FakeWinformViewModel)value; }
        }

        public FakeWinformViewModel ViewModel { get; set; }

        public Button Property1 { get; private set; }
        public Label Property2 { get; private set; }
        public TextBox Property3 { get; private set; }
        public RichTextBox Property4 { get; private set; }
        public CheckBox BooleanProperty { get; private set; }

        public TextBox SomeDouble { get; private set; }

        public FakeWinformsView()
        {
            this.Property1= new System.Windows.Forms.Button();
            this.Property2 = new Label();
            this.Property3 = new TextBox();
            this.Property4 = new RichTextBox();
            this.BooleanProperty = new CheckBox();
            SomeDouble = new TextBox();
        }
    }
}

namespace AThirdPartyNamespace
{
    public class ThirdPartyControl : Control
    {
        string value;

        public string Value {
            get { return this.value; }
            set {
                if (this.value != value) {
                    this.value = value;
                    this.OnValueChanged();
                }
            }
        }

        public event EventHandler ValueChanged;

        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null) {
                ValueChanged(this, EventArgs.Empty);
            }
        }
    }
}
