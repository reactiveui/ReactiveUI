// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.WinForms.Tests.Winforms.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>
/// Checks the command bindings.
/// </summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]
public class CommandBindingImplementationTests
{
    private const int InitialParameter = 2;
    private const int ExpectedSingleParameterResult = 10;
    private const int ExpectedDoubleParameterResult = 20;
    private const int ExpectedRebindParameterResult = 4;
    private const int ExpectedInvokeCountTwo = 2;

    /// <summary>
    /// Tests the command bind by name wireup.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
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
    /// Tests the command bind by name wireup with a parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindByNameWireupWithParameter()
    {
        var vm = new WinformCommandBindViewModel();
        var view = new WinformCommandBindView { ViewModel = vm };
        var fixture = new CommandBinderImplementation();

        var invokeCount = 0;
        vm.Command3.Subscribe(_ => ++invokeCount);

        var disp = fixture.BindCommand(vm, view, vm => vm.Command3, v => v.Command1, vm => vm.Parameter);

        view.Command1.PerformClick();
        using (Assert.Multiple())
        {
            await Assert.That(invokeCount).IsEqualTo(1);
            await Assert.That(vm.ParameterResult).IsEqualTo(ExpectedSingleParameterResult);
        }

        // update the parameter to ensure its updated when the command is executed
        vm.Parameter = InitialParameter;
        view.Command1.PerformClick();
        using (Assert.Multiple())
        {
            await Assert.That(invokeCount).IsEqualTo(ExpectedInvokeCountTwo);
            await Assert.That(vm.ParameterResult).IsEqualTo(ExpectedDoubleParameterResult);
        }

        // break the Command3 subscription
        vm.Command3 = ReactiveCommand.Create<int>(i => vm.ParameterResult = i * InitialParameter);

        // ensure that the invoke count does not update and that the Command3 is now using the new math
        view.Command1.PerformClick();
        using (Assert.Multiple())
        {
            await Assert.That(invokeCount).IsEqualTo(ExpectedInvokeCountTwo);
            await Assert.That(vm.ParameterResult).IsEqualTo(ExpectedRebindParameterResult);
        }

        disp.Dispose();
    }

    /// <summary>
    /// Tests the command bind to an explicit event wireup with a parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindToExplicitEventWireupWithParameter()
    {
        var vm = new WinformCommandBindViewModel();
        var view = new WinformCommandBindView { ViewModel = vm };
        var fixture = new CommandBinderImplementation();

        var invokeCount = 0;
        vm.Command3.Subscribe(_ => ++invokeCount);

        var disp = fixture.BindCommand(vm, view, x => x.Command3, x => x.Command2, vm => vm.Parameter, "MouseUp");

        view.Command2.RaiseMouseUpEvent(new(MouseButtons.Left, 1, 0, 0, 0));
        using (Assert.Multiple())
        {
            await Assert.That(vm.ParameterResult).IsEqualTo(ExpectedSingleParameterResult);
            await Assert.That(invokeCount).IsEqualTo(1);
        }

        vm.Parameter = InitialParameter;
        view.Command2.RaiseMouseUpEvent(new(MouseButtons.Left, 1, 0, 0, 0));
        using (Assert.Multiple())
        {
            await Assert.That(vm.ParameterResult).IsEqualTo(ExpectedDoubleParameterResult);
            await Assert.That(invokeCount).IsEqualTo(ExpectedInvokeCountTwo);
        }

        disp.Dispose();

        view.Command2.RaiseMouseUpEvent(new(MouseButtons.Left, 1, 0, 0, 0));
        await Assert.That(invokeCount).IsEqualTo(ExpectedInvokeCountTwo);
    }
}
