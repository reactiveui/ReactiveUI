using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI.Xaml;
using Xunit;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;

namespace ReactiveUI.Tests
{
    public class FakeViewModel : ReactiveObject
    {
        public ReactiveCommand Cmd { get; protected set; }

        public FakeViewModel()
        {
            Cmd = new ReactiveCommand();
        }
    }

    public class FakeView : IViewFor<FakeViewModel>
    {
        public TextBox TheTextBox { get; protected set; }

        public FakeView()
        {
            TheTextBox = new TextBox();
            ViewModel = new FakeViewModel();
        }

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (FakeViewModel)value; }
        }

        public FakeViewModel ViewModel { get; set; }
    }

    public class CreatesCommandBindingTests
    {
        [Fact]
        public void CommandBinderBindsToButton()
        {
            var fixture = new CreatesCommandBindingViaCommandParameter();
            var origCmd = new ReactiveAsyncCommand();
            var cmd = new ReactiveCommand();
            var input = new Button { Command = origCmd, CommandParameter = 42 };

            Assert.True(fixture.GetAffinityForObject(input.GetType(), true) <= 0);
            Assert.True(fixture.GetAffinityForObject(input.GetType(), false) > 0);

            var disp = fixture.BindCommandToObject(cmd, input, Observable.Return((object)5));

            Assert.Equal(cmd, input.Command);
            Assert.Equal(5, input.CommandParameter);

            disp.Dispose();

            Assert.Equal(origCmd, input.Command);
            Assert.Equal(42, input.CommandParameter);
        }

        [Fact]
        public void EventBinderBindsToExplicitEvent()
        {
            var input = new NonReactiveINPCObjectMadeReactive();
            var fixture = new CreatesCommandBindingViaEvent();
            var cmd = new ReactiveCommand();

            Assert.True(fixture.GetAffinityForObject(input.GetType(), true) > 0);
            Assert.False(fixture.GetAffinityForObject(input.GetType(), false) > 0);

            bool wasCalled = false;
            cmd.Subscribe(_ => wasCalled = true);

            var disp = fixture.BindCommandToObject<PropertyChangedEventArgs>(cmd, input, Observable.Return((object) 5), "PropertyChanged");
            input.InpcProperty = new TestFixture();
            Assert.True(wasCalled);

            wasCalled = false;
            disp.Dispose();
            input.InpcProperty = new TestFixture();
            Assert.False(wasCalled);
        }

        [Fact]
        public void EventBinderBindsToExplicitInheritedEvent()
        {
            var fixture = new FakeView();
            fixture.BindCommand(y=>y.ViewModel, x => x.Cmd, x => x.TheTextBox, "MouseDown");
        }


#if !SILVERLIGHT
        [Fact]
        public void EventBinderBindsToImplicitEvent()
        {
            var input = new Button();
            var fixture = new CreatesCommandBindingViaEvent();
            var cmd = new ReactiveCommand();

            Assert.True(fixture.GetAffinityForObject(input.GetType(), false) > 0);

            int invokeCount = 0;
            cmd.Subscribe(_ => invokeCount += 1);

            var disp = fixture.BindCommandToObject(cmd, input, Observable.Return((object) 5));
            Assert.NotNull(disp);
            Assert.Equal(0, invokeCount);

            input.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.Equal(1, invokeCount);

            disp.Dispose();
            input.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.Equal(1, invokeCount);
        }
#endif
    }

    public class CommandBindModel : ReactiveObject {
        int _Value;
        public int Value
        {
            get { return _Value; }
            set { this.RaiseAndSetIfChanged(ref _Value, value); }
        }
    }

    public class CommandBindViewModel : ReactiveObject
    {
        public ReactiveCommand _Command1 = new ReactiveCommand();
        public ReactiveCommand Command1 {
            get { return _Command1; }
            set { this.RaiseAndSetIfChanged(ref _Command1, value); }
        }

        public ReactiveCommand _Command2 = new ReactiveCommand();
        public ReactiveCommand Command2 {
            get { return _Command2; }
            set { this.RaiseAndSetIfChanged(ref _Command2, value); }
        }

        CommandBindModel _CommandBindModel;
        public CommandBindModel CommandBindModel
        {
            get { return _CommandBindModel; }
            set { this.RaiseAndSetIfChanged(ref _CommandBindModel, value); }
        }


        public CommandBindViewModel()
        {
            this.WhenAny(x=>x.CommandBindModel, x=> x.Value)
                .Subscribe(x=>{
                    Command1 = ReactiveCommand.Create(v=>true, v=>CommandBindModel.Value+=1 );
                    Command2 = ReactiveCommand.Create(v => true, v => CommandBindModel.Value += 2);
                 });
        }
    }

    // Make this subclass ReactiveObject but for WPF it
    // would be DependencyObject
    public class CommandBindView : ReactiveObject, IViewFor<CommandBindViewModel>
    {

        #region IViewFor
        object IViewFor.ViewModel
        {
            get { return _ViewModel; }
            set { this.RaiseAndSetIfChanged(ref _ViewModel, (CommandBindViewModel)value); }
        }
        CommandBindViewModel _ViewModel;
        public CommandBindViewModel ViewModel
        {
            get { return _ViewModel; }
            set { this.RaiseAndSetIfChanged(ref _ViewModel, (CommandBindViewModel)value); }
        }
        #endregion


        public Button Command1 { get; protected set; }

        public Image Command2 { get; protected set; }

        public CommandBindView()
        {
            Command1 = new Button();
            Command2 = new Image();
        }

    }

    public class CommandBindingImplementationTests
    {
        [Fact]
        public void CommandBindConventionWireup()
        {
            var model = new CommandBindModel();
            var viewmodel = new CommandBindViewModel() { CommandBindModel = model };
            var view = new CommandBindView() {ViewModel = viewmodel};
            var fixture = new CommandBinderImplementation();

            Assert.Null(view.Command1.Command);

            var disp = fixture.BindCommand(x=>x.ViewModel, view, x => x.Command1);
            Assert.Equal(view.ViewModel.Command1, view.Command1.Command);

            var newCmd = new ReactiveCommand();
            view.ViewModel.Command1 = newCmd;
            Assert.Equal(newCmd, view.Command1.Command);

            disp.Dispose();
            Assert.Null(view.Command1.Command);
        }

        [Fact]
        public void CommandBindByNameWireup()
        {
            (new TestScheduler()).With(sched =>
            {
                var model = new CommandBindModel();
                var viewmodel = new CommandBindViewModel() { CommandBindModel = model };
                var view = new CommandBindView() { ViewModel = viewmodel };
                var fixture = new CommandBinderImplementation();

                Assert.Null(view.Command1.Command);

                var disp = fixture.BindCommand(x => x.ViewModel, view, x => x.Command1, x => x.Command1);
                Assert.Equal(view.ViewModel.Command1, view.Command1.Command);

                var newCmd = new ReactiveCommand();
                view.ViewModel.Command1 = newCmd;
                Assert.Equal(newCmd, view.Command1.Command);

                disp.Dispose();
                Assert.Null(view.Command1.Command);
            });
        }

#if !SILVERLIGHT
        [Fact]
        public void CommandBindToExplicitEventWireup()
        {
            var model = new CommandBindModel();
            var viewmodel = new CommandBindViewModel() { CommandBindModel = model };
            var view = new CommandBindView() {ViewModel = viewmodel};
            var fixture = new CommandBinderImplementation();

            int invokeCount = 0;
            view.ViewModel.Command2.Subscribe(_ => invokeCount += 1);

            var disp = fixture.BindCommand(x=>x.ViewModel, view, x => x.Command2, x => x.Command2, "MouseUp");

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Image.MouseUpEvent });

            disp.Dispose();

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Image.MouseUpEvent });
            Assert.Equal(1, invokeCount);
        }
#endif
    }
}
