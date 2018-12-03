// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xunit;

namespace ReactiveUI.Tests
{
    public class FakeViewModel : ReactiveObject
    {
        public FakeViewModel()
        {
            Cmd = ReactiveCommand.Create(() => { });
        }

        public ReactiveCommand<Unit, Unit> Cmd { get; protected set; }
    }

    public class FakeView : IViewFor<FakeViewModel>
    {
        public FakeView()
        {
            TheTextBox = new TextBox();
            ViewModel = new FakeViewModel();
        }

        public TextBox TheTextBox { get; protected set; }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (FakeViewModel)value;
        }

        public FakeViewModel ViewModel { get; set; }
    }

    public class CreatesCommandBindingTests
    {
        [WpfFact]
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
            var wasCalled = false;
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

        [WpfFact]
        public void EventBinderBindsToExplicitInheritedEvent()
        {
            var fixture = new FakeView();
            fixture.BindCommand(fixture.ViewModel, x => x.Cmd, x => x.TheTextBox, "MouseDown");
        }

        [WpfFact]
        public void EventBinderBindsToImplicitEvent()
        {
            var input = new Button();
            var fixture = new CreatesCommandBindingViaEvent();
            var cmd = ReactiveCommand.Create<int>(_ => { });

            Assert.True(fixture.GetAffinityForObject(input.GetType(), false) > 0);

            var invokeCount = 0;
            cmd.Subscribe(_ => invokeCount += 1);

            var disp = fixture.BindCommandToObject(cmd, input, Observable.Return((object)5));
            Assert.NotNull(disp);
            Assert.Equal(0, invokeCount);

            input.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            Assert.Equal(1, invokeCount);

            disp.Dispose();
            input.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            Assert.Equal(1, invokeCount);
        }
    }

    public class CommandBindViewModel : ReactiveObject
    {
        public ReactiveCommand<int, Unit> _Command1;
        public ReactiveCommand<Unit, Unit> _Command2;

        private int _value;

        public CommandBindViewModel()
        {
            Command1 = ReactiveCommand.Create<int, Unit>(_ => Unit.Default);
            Command2 = ReactiveCommand.Create(() => { });
        }

        public ReactiveCommand<int, Unit> Command1
        {
            get => _Command1;
            set => this.RaiseAndSetIfChanged(ref _Command1, value);
        }

        public ReactiveCommand<Unit, Unit> Command2
        {
            get => _Command2;
            set => this.RaiseAndSetIfChanged(ref _Command2, value);
        }

        public FakeNestedViewModel NestedViewModel { get; set; }

        public int Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }
    }

    public class FakeNestedViewModel : ReactiveObject
    {
        public FakeNestedViewModel()
        {
            NestedCommand = ReactiveCommand.Create(() => { });
        }

        public ReactiveCommand<Unit, Unit> NestedCommand { get; protected set; }
    }

    public class CustomClickButton : Button
    {
        public event EventHandler<EventArgs> CustomClick;

        public void RaiseCustomClick() =>
            CustomClick?.Invoke(this, EventArgs.Empty);
    }

    public class CommandBindView : IViewFor<CommandBindViewModel>
    {
        public CommandBindView()
        {
            Command1 = new CustomClickButton();
            Command2 = new Image();
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CommandBindViewModel)value;
        }

        public CommandBindViewModel ViewModel { get; set; }

        public CustomClickButton Command1 { get; protected set; }

        public Image Command2 { get; protected set; }
    }

    public class ReactiveObjectCommandBindView : ReactiveObject, IViewFor<CommandBindViewModel>
    {
        private CommandBindViewModel _vm;

        public ReactiveObjectCommandBindView()
        {
            Command1 = new CustomClickButton();
            Command2 = new Image();
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CommandBindViewModel)value;
        }

        public CommandBindViewModel ViewModel
        {
            get => _vm;
            set => this.RaiseAndSetIfChanged(ref _vm, value);
        }

        public CustomClickButton Command1 { get; protected set; }

        public Image Command2 { get; protected set; }
    }

    public class CommandBindingImplementationTests
    {
        [WpfFact]
        public void CommandBindByNameWireup()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView { ViewModel = vm };

            Assert.Null(view.Command1.Command);

            var disp = view.BindCommand(vm, x => x.Command1, x => x.Command1);
            Assert.Equal(vm.Command1, view.Command1.Command);

            var newCmd = ReactiveCommand.Create<int>(_ => { });
            vm.Command1 = newCmd;
            Assert.Equal(newCmd, view.Command1.Command);

            disp.Dispose();
            Assert.Null(view.Command1.Command);
        }

        [WpfFact]
        public void CommandBindNestedCommandWireup()
        {
            var vm = new CommandBindViewModel
            {
                NestedViewModel = new FakeNestedViewModel()
            };

            var view = new CommandBindView { ViewModel = vm };

            var disp = view.BindCommand(vm, m => m.NestedViewModel.NestedCommand, x => x.Command1);

            Assert.Equal(vm.NestedViewModel.NestedCommand, view.Command1.Command);
        }

        [WpfFact]
        public void CommandBindSetsInitialEnabledState_True()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView { ViewModel = vm };

            var canExecute1 = new BehaviorSubject<bool>(true);
            var cmd1 = ReactiveCommand.Create<int>(_ => { }, canExecute1);
            vm.Command1 = cmd1;

            var disp = view.BindCommand(vm, x => x.Command1, x => x.Command1);

            Assert.True(view.Command1.IsEnabled);
        }

        [WpfFact]
        public void CommandBindSetsDisablesCommandWhenCanExecuteChanged()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView { ViewModel = vm };

            var canExecute1 = new BehaviorSubject<bool>(true);
            var cmd1 = ReactiveCommand.Create<int>(_ => { }, canExecute1);
            vm.Command1 = cmd1;

            var disp = view.BindCommand(vm, x => x.Command1, x => x.Command1);

            Assert.True(view.Command1.IsEnabled);

            canExecute1.OnNext(false);

            Assert.False(view.Command1.IsEnabled);
        }

        [WpfFact]
        public void CommandBindSetsInitialEnabledState_False()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView { ViewModel = vm };

            var canExecute1 = new BehaviorSubject<bool>(false);
            var cmd1 = ReactiveCommand.Create<int>(_ => { }, canExecute1);
            vm.Command1 = cmd1;

            var disp = view.BindCommand(vm, x => x.Command1, x => x.Command1);

            Assert.False(view.Command1.IsEnabled);
        }

        [WpfFact]
        public void CommandBindRaisesCanExecuteChangedOnBind()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView { ViewModel = vm };

            var canExecute1 = new BehaviorSubject<bool>(true);
            var cmd1 = ReactiveCommand.Create<int>(_ => { }, canExecute1);
            vm.Command1 = cmd1;

            var disp = view.BindCommand(vm, x => x.Command1, x => x.Command1);

            Assert.True(view.Command1.IsEnabled);

            // Now  change to a disabled cmd
            var canExecute2 = new BehaviorSubject<bool>(false);
            var cmd2 = ReactiveCommand.Create<int>(_ => { }, canExecute2);
            vm.Command1 = cmd2;

            Assert.False(view.Command1.IsEnabled);
        }

        [WpfFact]
        public void CommandBindToExplicitEventWireup()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView { ViewModel = vm };

            var invokeCount = 0;
            vm.Command2.Subscribe(_ => invokeCount += 1);

            var disp = view.BindCommand(vm, x => x.Command2, x => x.Command2, "MouseUp");

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });

            disp.Dispose();

            view.Command2.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseUpEvent });
            Assert.Equal(1, invokeCount);
        }

        [WpfFact]
        public void CommandBindWithParameterExpression()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView { ViewModel = vm };

            var received = 0;
            var cmd = ReactiveCommand.Create<int>(i => { received = i; });
            vm.Command1 = cmd;

            var disp = view.BindCommand(vm, x => x.Command1, x => x.Command1, x => x.Value, nameof(CustomClickButton.CustomClick));

            vm.Value = 42;
            view.Command1.RaiseCustomClick();
            Assert.Equal(42, received);

            vm.Value = 13;
            view.Command1.RaiseCustomClick();
            Assert.Equal(13, received);
        }

        [WpfFact]
        public void CommandBindWithDelaySetVMParameterExpression()
        {
            var vm = new CommandBindViewModel();
            var view = new ReactiveObjectCommandBindView();

            var received = 0;
            var cmd = ReactiveCommand.Create<int>(i => { received = i; });
            vm.Command1 = cmd;

            var disp = view.BindCommand(vm, x => x.Command1, x => x.Command1, x => x.Value, nameof(CustomClickButton.CustomClick));

            view.ViewModel = vm;

            vm.Value = 42;
            view.Command1.RaiseCustomClick();
            Assert.Equal(42, received);

            vm.Value = 13;
            view.Command1.RaiseCustomClick();
            Assert.Equal(13, received);
        }

        [WpfFact]
        public void CommandBindWithDelaySetVMParameterNoINPCExpression()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView();

            var received = 0;
            var cmd = ReactiveCommand.Create<int>(i => { received = i; });
            vm.Command1 = cmd;

            var disp = view.BindCommand(vm, x => x.Command1, x => x.Command1, x => x.Value, nameof(CustomClickButton.CustomClick));

            view.ViewModel = vm;

            vm.Value = 42;
            view.Command1.RaiseCustomClick();
            Assert.Equal(0, received);

            vm.Value = 13;
            view.Command1.RaiseCustomClick();
            Assert.Equal(0, received);
        }

        [WpfFact]
        public void CommandBindWithParameterObservable()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView { ViewModel = vm };

            var received = 0;
            var cmd = ReactiveCommand.Create<int>(i => { received = i; });
            vm.Command1 = cmd;

            var value = Observable.Return(42);
            var disp = view.BindCommand(vm, x => x.Command1, x => x.Command1, value, nameof(CustomClickButton.CustomClick));

            vm.Value = 42;
            view.Command1.RaiseCustomClick();

            Assert.Equal(42, received);
        }
    }
}
