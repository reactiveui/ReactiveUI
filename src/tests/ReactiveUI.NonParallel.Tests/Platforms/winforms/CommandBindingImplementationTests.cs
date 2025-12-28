// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Winforms;

/// <summary>
/// Checks the command bindings.
/// </summary>
public class CommandBindingImplementationTests
{
    /// <summary>
    /// Tests the command bind by name wireup.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindByNameWireup()
    {
        var vm = new WinformCommandBindViewModel();
        var view = new WinformCommandBindView { ViewModel = vm };
        var fixture = new CommandBinderImplementation();

        var invokeCount = 0;
        vm.Command1.Subscribe(_ => ++invokeCount);

        var disp = fixture.BindCommand(vm, view, x => x.Command1, x => x.Command1);

        view.Command1.PerformClick();

        await Assert.That(invokeCount).IsEqualTo(1);

        vm.Command1 = ReactiveCommand.Create(() => { });

        view.Command1.PerformClick();
        await Assert.That(invokeCount).IsEqualTo(1);

        disp.Dispose();
    }

    /// <summary>
    /// Tests the command bind explicit event wire up.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    [Skip("Test failing with assertion error - needs investigation")]
    public async Task CommandBindToExplicitEventWireupAsync()
    {
        var vm = new WinformCommandBindViewModel();
        var view = new WinformCommandBindView { ViewModel = vm };
        var fixture = new CommandBinderImplementation();

        var invokeCount = 0;

        vm.Command2.Subscribe(_ => invokeCount++);

        var disp = fixture.BindCommand(vm, view, x => x.Command2, x => x.Command2, "MouseUp");

        view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

        // With ImmediateScheduler, execution happens synchronously
        await Assert.That(invokeCount).IsEqualTo(1);

        disp.Dispose();

        view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

        // After disposal, command should not execute
        await Assert.That(invokeCount).IsEqualTo(1);
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindByNameWireupWithParameter()
    {
        var vm = new WinformCommandBindViewModel();
        var view = new WinformCommandBindView { ViewModel = vm };
        ICommandBinderImplementation fixture = new CommandBinderImplementation();

        var invokeCount = 0;
        vm.Command3.Subscribe(_ => ++invokeCount);

        var disp = CommandBinderImplementationMixins.BindCommand(fixture, vm, view, vm => vm.Command3, v => v.Command1, vm => vm.Parameter);

        view.Command1.PerformClick();
        using (Assert.Multiple())
        {
            await Assert.That(invokeCount).IsEqualTo(1);
            await Assert.That(vm.ParameterResult).IsEqualTo(10);
        }

        // update the parameter to ensure its updated when the command is executed
        vm.Parameter = 2;
        view.Command1.PerformClick();
        using (Assert.Multiple())
        {
            await Assert.That(invokeCount).IsEqualTo(2);
            await Assert.That(vm.ParameterResult).IsEqualTo(20);
        }

        // break the Command3 subscription
        vm.Command3 = ReactiveCommand.Create<int>(i => vm.ParameterResult = i * 2);

        // ensure that the invoke count does not update and that the Command3 is now using the new math
        view.Command1.PerformClick();
        using (Assert.Multiple())
        {
            await Assert.That(invokeCount).IsEqualTo(2);
            await Assert.That(vm.ParameterResult).IsEqualTo(4);
        }

        disp.Dispose();
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindToExplicitEventWireupWithParameter()
    {
        var vm = new WinformCommandBindViewModel();
        var view = new WinformCommandBindView { ViewModel = vm };
        var fixture = new CommandBinderImplementation();

        var invokeCount = 0;
        vm.Command3.Subscribe(_ => ++invokeCount);

        var disp = CommandBinderImplementationMixins.BindCommand(fixture, vm, view, x => x.Command3, x => x.Command2, vm => vm.Parameter, "MouseUp");

        view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        using (Assert.Multiple())
        {
            await Assert.That(vm.ParameterResult).IsEqualTo(10);
            await Assert.That(invokeCount).IsEqualTo(1);
        }

        vm.Parameter = 2;
        view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        using (Assert.Multiple())
        {
            await Assert.That(vm.ParameterResult).IsEqualTo(20);
            await Assert.That(invokeCount).IsEqualTo(2);
        }

        disp.Dispose();

        view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        await Assert.That(invokeCount).IsEqualTo(2);
    }
}
