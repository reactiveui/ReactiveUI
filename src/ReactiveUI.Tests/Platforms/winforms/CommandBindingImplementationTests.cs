// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

using Xunit;

namespace ReactiveUI.Tests.Winforms
{
    /// <summary>
    /// Checks the command bindings.
    /// </summary>
    public class CommandBindingImplementationTests
    {
        /// <summary>
        /// Tests the command bind by name wireup.
        /// </summary>
        [Fact]
        public void CommandBindByNameWireup()
        {
            var vm = new WinformCommandBindViewModel();
            var view = new WinformCommandBindView { ViewModel = vm };
            var fixture = new CommandBinderImplementation();

            var invokeCount = 0;
            vm.Command1.Subscribe(_ => ++invokeCount);

            var disp = fixture.BindCommand(vm, view, x => x.Command1, x => x.Command1);

            view.Command1.PerformClick();

            Assert.Equal(1, invokeCount);

            var newCmd = ReactiveCommand.Create(() => { });
            vm.Command1 = newCmd;

            view.Command1.PerformClick();
            Assert.Equal(1, invokeCount);

            disp.Dispose();
        }

        /// <summary>
        /// Tests the command bind explicit event wire up.
        /// </summary>
        [Fact]
        public void CommandBindToExplicitEventWireup()
        {
            var vm = new WinformCommandBindViewModel();
            var view = new WinformCommandBindView { ViewModel = vm };
            var fixture = new CommandBinderImplementation();

            var invokeCount = 0;
            vm.Command2.Subscribe(_ => ++invokeCount);

            var disp = fixture.BindCommand(vm, view, x => x.Command2, x => x.Command2, "MouseUp");

            view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

            disp.Dispose();

            view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            Assert.Equal(1, invokeCount);
        }

        [Fact]
        public void CommandBindByNameWireupWithParameter()
        {
            var vm = new WinformCommandBindViewModel();
            var view = new WinformCommandBindView { ViewModel = vm };
            ICommandBinderImplementation fixture = new CommandBinderImplementation();

            var invokeCount = 0;
            vm.Command3.Subscribe(_ => ++invokeCount);

            var disp = CommandBinderImplementationMixins.BindCommand(fixture, vm, view, vm => vm.Command3, v => v.Command1, vm => vm.Parameter);

            view.Command1.PerformClick();
            Assert.Equal(1, invokeCount);
            Assert.Equal(10, vm.ParameterResult);

            // update the parameter to ensure its updated when the command is executed
            vm.Parameter = 2;
            view.Command1.PerformClick();
            Assert.Equal(2, invokeCount);
            Assert.Equal(20, vm.ParameterResult);

            // break the Command3 subscription
            var newCmd = ReactiveCommand.Create<int>(i => vm.ParameterResult = i * 2);
            vm.Command3 = newCmd;

            // ensure that the invoke count does not update and that the Command3 is now using the new math
            view.Command1.PerformClick();
            Assert.Equal(2, invokeCount);
            Assert.Equal(4, vm.ParameterResult);

            disp.Dispose();
        }

        [Fact]
        public void CommandBindToExplicitEventWireupWithParameter()
        {
            var vm = new WinformCommandBindViewModel();
            var view = new WinformCommandBindView { ViewModel = vm };
            var fixture = new CommandBinderImplementation();

            var invokeCount = 0;
            vm.Command3.Subscribe(_ => ++invokeCount);

            var disp = CommandBinderImplementationMixins.BindCommand(fixture, vm, view, x => x.Command3, x => x.Command2, vm => vm.Parameter, "MouseUp");

            view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            Assert.Equal(10, vm.ParameterResult);
            Assert.Equal(1, invokeCount);

            vm.Parameter = 2;
            view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            Assert.Equal(20, vm.ParameterResult);
            Assert.Equal(2, invokeCount);

            disp.Dispose();

            view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            Assert.Equal(2, invokeCount);
        }
    }
}
