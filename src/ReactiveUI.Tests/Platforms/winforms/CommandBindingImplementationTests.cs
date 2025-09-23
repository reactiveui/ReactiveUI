﻿// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;
using ReactiveUI.Testing;

namespace ReactiveUI.Tests.Winforms;

/// <summary>
/// Checks the command bindings.
/// </summary>
[TestFixture]
public class CommandBindingImplementationTests
{
    /// <summary>
    /// Tests the command bind by name wireup.
    /// </summary>
    [Test]
    public void CommandBindByNameWireup()
    {
        var vm = new WinformCommandBindViewModel();
        var view = new WinformCommandBindView { ViewModel = vm };
        var fixture = new CommandBinderImplementation();

        var invokeCount = 0;
        vm.Command1.Subscribe(_ => ++invokeCount);

        var disp = fixture.BindCommand(vm, view, x => x.Command1, x => x.Command1);

        view.Command1.PerformClick();

        Assert.That(invokeCount, Is.EqualTo(1));

        var newCmd = ReactiveCommand.Create(() => { });
        vm.Command1 = newCmd;

        view.Command1.PerformClick();
        Assert.That(invokeCount, Is.EqualTo(1));

        disp.Dispose();
    }

    /// <summary>
    /// Tests the command bind explicit event wire up.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CommandBindToExplicitEventWireupAsync()
    {
        using var testSequencer = new TestSequencer();
        var vm = new WinformCommandBindViewModel();
        var view = new WinformCommandBindView { ViewModel = vm };
        var fixture = new CommandBinderImplementation();

        var invokeCount = 0;
        vm.Command2.Subscribe(async _ =>
        {
            ++invokeCount;
            await testSequencer.AdvancePhaseAsync();
        });

        var disp = fixture.BindCommand(vm, view, x => x.Command2, x => x.Command2, "MouseUp");

        view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

        disp.Dispose();

        view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

        await testSequencer.AdvancePhaseAsync();
        Assert.That(invokeCount, Is.EqualTo(1));
    }

    [Test]
    public void CommandBindByNameWireupWithParameter()
    {
        var vm = new WinformCommandBindViewModel();
        var view = new WinformCommandBindView { ViewModel = vm };
        ICommandBinderImplementation fixture = new CommandBinderImplementation();

        var invokeCount = 0;
        vm.Command3.Subscribe(_ => ++invokeCount);

        var disp = CommandBinderImplementationMixins.BindCommand(fixture, vm, view, vm => vm.Command3, v => v.Command1, vm => vm.Parameter);

        view.Command1.PerformClick();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(invokeCount, Is.EqualTo(1));
            Assert.That(vm.ParameterResult, Is.EqualTo(10));
        }

        // update the parameter to ensure its updated when the command is executed
        vm.Parameter = 2;
        view.Command1.PerformClick();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(invokeCount, Is.EqualTo(2));
            Assert.That(vm.ParameterResult, Is.EqualTo(20));
        }

        // break the Command3 subscription
        var newCmd = ReactiveCommand.Create<int>(i => vm.ParameterResult = i * 2);
        vm.Command3 = newCmd;

        // ensure that the invoke count does not update and that the Command3 is now using the new math
        view.Command1.PerformClick();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(invokeCount, Is.EqualTo(2));
            Assert.That(vm.ParameterResult, Is.EqualTo(4));
        }

        disp.Dispose();
    }

    [Test]
    public void CommandBindToExplicitEventWireupWithParameter()
    {
        var vm = new WinformCommandBindViewModel();
        var view = new WinformCommandBindView { ViewModel = vm };
        var fixture = new CommandBinderImplementation();

        var invokeCount = 0;
        vm.Command3.Subscribe(_ => ++invokeCount);

        var disp = CommandBinderImplementationMixins.BindCommand(fixture, vm, view, x => x.Command3, x => x.Command2, vm => vm.Parameter, "MouseUp");

        view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(vm.ParameterResult, Is.EqualTo(10));
            Assert.That(invokeCount, Is.EqualTo(1));
        }

        vm.Parameter = 2;
        view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(vm.ParameterResult, Is.EqualTo(20));
            Assert.That(invokeCount, Is.EqualTo(2));
        }

        disp.Dispose();

        view.Command2.RaiseMouseUpEvent(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        Assert.That(invokeCount, Is.EqualTo(2));
    }
}
