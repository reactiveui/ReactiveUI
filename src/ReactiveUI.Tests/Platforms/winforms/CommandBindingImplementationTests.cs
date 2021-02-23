// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows.Forms;
using Xunit;

namespace ReactiveUI.Tests.Winforms
{
    public class CommandBindingImplementationTests
    {
        [Fact]
        public void CommandBindByNameWireup()
        {
            var vm = new WinformCommandBindViewModel();
            var view = new WinformCommandBindView { ViewModel = vm };
            var fixture = new CommandBinderImplementation();

            var invokeCount = 0;
            vm.Command1.Subscribe(_ => invokeCount += 1);

            var disp = fixture.BindCommand(vm, view, x => x.Command1, x => x.Command1);

            view.Command1.PerformClick();

            Assert.Equal(1, invokeCount);

            var newCmd = ReactiveCommand.Create(() => { });
            vm.Command1 = newCmd;

            view.Command1.PerformClick();
            Assert.Equal(1, invokeCount);

            disp.Dispose();
        }

        [Fact]
        public void CommandBindToExplicitEventWireup()
        {
            var vm = new WinformCommandBindViewModel();
            var view = new WinformCommandBindView { ViewModel = vm };
            var fixture = new CommandBinderImplementation();

            var invokeCount = 0;
            vm.Command2.Subscribe(_ => invokeCount += 1);

            var disp = fixture.BindCommand(vm, view, x => x.Command2, x => x.Command2, "MouseUp");

            view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

            disp.Dispose();

            view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            Assert.Equal(1, invokeCount);
        }
    }
}
