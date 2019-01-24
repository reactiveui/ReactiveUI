﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using Xunit;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using FactAttribute = Xunit.WpfFactAttribute;
using System.Windows.Controls;
using System.Windows.Input;
#endif

namespace ReactiveUI.Tests.Xaml
{
    public class CommandBindingImplementationTests
    {
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
