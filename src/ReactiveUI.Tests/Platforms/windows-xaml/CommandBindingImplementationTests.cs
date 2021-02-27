// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Xunit;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
using System.Windows.Input;
using FactAttribute = Xunit.WpfFactAttribute;
#endif

namespace ReactiveUI.Tests.Xaml
{
    /// <summary>
    /// Tests with the command binding implementation.
    /// </summary>
    public class CommandBindingImplementationTests
    {
        /// <summary>
        /// Tests the command bind by name wireup.
        /// </summary>
        [Fact]
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

        /// <summary>
        /// Tests the command bind nested command wireup.
        /// </summary>
        [Fact]
        public void CommandBindNestedCommandWireup()
        {
            var vm = new CommandBindViewModel
            {
                NestedViewModel = new FakeNestedViewModel()
            };

            var view = new CommandBindView { ViewModel = vm };

            view.BindCommand(vm, m => m.NestedViewModel.NestedCommand, x => x.Command1);

            Assert.Equal(vm.NestedViewModel.NestedCommand, view.Command1.Command);
        }

        /// <summary>
        /// Tests the command bind sets initial enabled state true.
        /// </summary>
        [Fact]
        public void CommandBindSetsInitialEnabledState_True()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView { ViewModel = vm };

            var canExecute1 = new BehaviorSubject<bool>(true);
            vm.Command1 = ReactiveCommand.Create<int>(_ => { }, canExecute1);

            view.BindCommand(vm, x => x.Command1, x => x.Command1);

            Assert.True(view.Command1.IsEnabled);
        }

        /// <summary>
        /// Tests the command bind sets disables command when can execute changed.
        /// </summary>
        [Fact]
        public void CommandBindSetsDisablesCommandWhenCanExecuteChanged()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView { ViewModel = vm };

            var canExecute1 = new BehaviorSubject<bool>(true);
            vm.Command1 = ReactiveCommand.Create<int>(_ => { }, canExecute1);

            view.BindCommand(vm, x => x.Command1, x => x.Command1);

            Assert.True(view.Command1.IsEnabled);

            canExecute1.OnNext(false);

            Assert.False(view.Command1.IsEnabled);
        }

        /// <summary>
        /// Tests the command bind sets initial enabled state false.
        /// </summary>
        [Fact]
        public void CommandBindSetsInitialEnabledState_False()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView { ViewModel = vm };

            var canExecute1 = new BehaviorSubject<bool>(false);
            vm.Command1 = ReactiveCommand.Create<int>(_ => { }, canExecute1);

            view.BindCommand(vm, x => x.Command1, x => x.Command1);

            Assert.False(view.Command1.IsEnabled);
        }

        /// <summary>
        /// Tests the command bind raises can execute changed on bind.
        /// </summary>
        [Fact]
        public void CommandBindRaisesCanExecuteChangedOnBind()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView { ViewModel = vm };

            var canExecute1 = new BehaviorSubject<bool>(true);
            vm.Command1 = ReactiveCommand.Create<int>(_ => { }, canExecute1);

            view.BindCommand(vm, x => x.Command1, x => x.Command1);

            Assert.True(view.Command1.IsEnabled);

            // Now  change to a disabled cmd
            var canExecute2 = new BehaviorSubject<bool>(false);
            vm.Command1 = ReactiveCommand.Create<int>(_ => { }, canExecute2);

            Assert.False(view.Command1.IsEnabled);
        }

        /// <summary>
        /// Tests the command bind with parameter expression.
        /// </summary>
        [Fact]
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

        /// <summary>
        /// Tests the command bind with delay set vm parameter expression.
        /// </summary>
        [Fact]
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

        /// <summary>
        /// Tests the command bind with delay set vm parameter no inpc expression.
        /// </summary>
        [Fact]
        public void CommandBindWithDelaySetVMParameterNoINPCExpression()
        {
            var vm = new CommandBindViewModel();
            var view = new CommandBindView();

            var received = 0;
            var cmd = ReactiveCommand.Create<int>(i => { received = i; });
            vm.Command1 = cmd;

            view.BindCommand(vm, x => x.Command1, x => x.Command1, x => x.Value, nameof(CustomClickButton.CustomClick));

            view.ViewModel = vm;

            vm.Value = 42;
            view.Command1.RaiseCustomClick();
            Assert.Equal(0, received);

            vm.Value = 13;
            view.Command1.RaiseCustomClick();
            Assert.Equal(0, received);
        }

        /// <summary>
        /// Tests the command bind with parameter observable.
        /// </summary>
        [Fact]
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
