using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xunit;

namespace ReactiveUI.Tests
{
    public class FakeViewModel : ReactiveObject
    {
        public ReactiveCommand Cmd { get; protected set; }

        public FakeViewModel()
        {
            Cmd = ReactiveCommand.Create(() => { });
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
            var origCmd = ReactiveCommand.Create(() => { });
            var cmd = ReactiveCommand.Create(() => { });
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
            var input = new TestFixture();
            var fixture = new CreatesCommandBindingViaEvent();
            bool wasCalled = false;
            var cmd = ReactiveCommand.Create<int>(x => wasCalled = true);

            Assert.True(fixture.GetAffinityForObject(input.GetType(), true) > 0);
            Assert.False(fixture.GetAffinityForObject(input.GetType(), false) > 0);

            var disp = fixture.BindCommandToObject<PropertyChangedEventArgs>(cmd, input, Observable.Return((object)5), "PropertyChanged");
            input.IsNotNullString = "Foo";
            Assert.True(wasCalled);

            wasCalled = false;
            disp.Dispose();
            input.IsNotNullString = "Bar";
            Assert.False(wasCalled);
        }

        [Fact]
        public void EventBinderBindsToExplicitInheritedEvent()
        {
            var fixture = new FakeView();
            fixture.BindCommand(fixture.ViewModel, x => x.Cmd, x => x.TheTextBox, "MouseDown");
        }

#if !SILVERLIGHT
        [Fact]
        public void EventBinderBindsToImplicitEvent()
        {
            var input = new Button();
            var fixture = new CreatesCommandBindingViaEvent();
            var cmd = ReactiveCommand.Create<int>(_ => { });

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

    public class CommandBindViewModel : ReactiveObject
    {
        public ReactiveCommand _Command1;
        public ReactiveCommand Command1 {
            get { return _Command1; }
            set { this.RaiseAndSetIfChanged(ref _Command1, value); }
        }

        public ReactiveCommand<Unit, Unit> _Command2;
        public ReactiveCommand<Unit, Unit> Command2 {
            get { return _Command2; }
            set { this.RaiseAndSetIfChanged(ref _Command2, value); }
        }

        public CommandBindViewModel()
        {
            Command1 = ReactiveCommand.Create(() => { });
            Command2 = ReactiveCommand.Create(() => { });
        }

        public FakeNestedViewModel NestedViewModel { get; set; }
    }

    public class FakeNestedViewModel : ReactiveObject
    {
        public FakeNestedViewModel()
        {
            NestedCommand = ReactiveCommand.Create(() => { });
        }

        public ReactiveCommand NestedCommand { get; protected set; }
    }


    public class CommandBindView : IViewFor<CommandBindViewModel>
    {
        object IViewFor.ViewModel { 
            get { return ViewModel; }
            set { ViewModel = (CommandBindViewModel)value; } 
        }

        public CommandBindViewModel ViewModel { get; set; }

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
        public void CommandBindByNameWireup()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView() {ViewModel = vm};

            Assert.Null(view.Command1.Command);

            var disp = view.BindCommand(vm, x => x.Command1, x => x.Command1);
            Assert.Equal(vm.Command1, view.Command1.Command);

            var newCmd = ReactiveCommand.Create(() => { });
            vm.Command1 = newCmd;
            Assert.Equal(newCmd, view.Command1.Command);

            disp.Dispose();
            Assert.Null(view.Command1.Command);
        }
        
        [Fact]
        public void CommandBindNestedCommandWireup()
        {
            var vm = new CommandBindViewModel()
            {
                NestedViewModel =  new FakeNestedViewModel()
            };

            var view = new CommandBindView { ViewModel = vm };

            var disp = view.BindCommand(vm, m => m.NestedViewModel.NestedCommand, x => x.Command1);

            Assert.Equal(vm.NestedViewModel.NestedCommand, view.Command1.Command);
        }

        [Fact]
        public void CommandBindSetsInitialEnabledState_True()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView() { ViewModel = vm };

            var canExecute1 = new BehaviorSubject<bool>(true);
            var cmd1 = ReactiveCommand.Create(() => { }, canExecute1);
            vm.Command1 = cmd1;

            var disp = view.BindCommand(vm, x => x.Command1, x => x.Command1);

            Assert.True(view.Command1.IsEnabled);
        }

        [Fact]
        public void CommandBindSetsDisablesCommandWhenCanExecuteChanged()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView() { ViewModel = vm };

            var canExecute1 = new BehaviorSubject<bool>(true);
            var cmd1 = ReactiveCommand.Create(() => { }, canExecute1);
            vm.Command1 = cmd1;

            var disp = view.BindCommand(vm, x => x.Command1, x => x.Command1);

            Assert.True(view.Command1.IsEnabled);

            canExecute1.OnNext(false);

            Assert.False(view.Command1.IsEnabled);
        }

        [Fact]
        public void CommandBindSetsInitialEnabledState_False()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView() { ViewModel = vm };

            var canExecute1 = new BehaviorSubject<bool>(false);
            var cmd1 = ReactiveCommand.Create(() => { }, canExecute1);
            vm.Command1 = cmd1;

            var disp = view.BindCommand(vm, x => x.Command1, x => x.Command1);

            Assert.False(view.Command1.IsEnabled);
        }


        [Fact]
        public void CommandBindRaisesCanExecuteChangedOnBind()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView() { ViewModel = vm };

            var canExecute1 = new BehaviorSubject<bool>(true);
            var cmd1 = ReactiveCommand.Create(() => { }, canExecute1);
            vm.Command1 = cmd1;

            var disp = view.BindCommand(vm, x => x.Command1, x => x.Command1);

            Assert.True(view.Command1.IsEnabled);

            // Now  change to a disabled cmd

            var canExecute2 = new BehaviorSubject<bool>(false);
            var cmd2 = ReactiveCommand.Create(() => { }, canExecute2);
            vm.Command1 = cmd2;

            Assert.False(view.Command1.IsEnabled);
        }

#if !SILVERLIGHT
        [Fact]
        public void CommandBindToExplicitEventWireup()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView() {ViewModel = vm};

            int invokeCount = 0;
            vm.Command2.Subscribe(_ => invokeCount += 1);

            var disp = view.BindCommand(vm, x => x.Command2, x => x.Command2, "MouseUp");

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Image.MouseUpEvent });

            disp.Dispose();

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Image.MouseUpEvent });
            Assert.Equal(1, invokeCount);
        }
#endif
    }
}
