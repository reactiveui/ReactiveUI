// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Mocks;
using ReactiveUI.Tests.Wpf;
using ReactiveUI.Tests.Xaml.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Xaml;

/// <summary>Tests with the command binding implementation.</summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class CommandBindingImplementationTests
{
    /// <summary>The first value used when exercising command parameter wireup.</summary>
    private const int FirstValue = 42;

    /// <summary>The second value used when exercising command parameter wireup.</summary>
    private const int SecondValue = 13;

    /// <summary>The initial value used when exercising command parameter wireup.</summary>
    private const int InitialValue = 10;

    /// <summary>Tests the command bind by name wireup.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindByNameWireup()
    {
        var view = new CommandBindView { ViewModel = new() };

        await Assert.That(view.Command1.Command).IsNull();

        var disp = view.BindCommand(view.ViewModel, static x => x.Command1, static x => x.Command1);
        await Assert.That(view.Command1.Command).IsEqualTo(view.ViewModel.Command1);

        var newCmd = ReactiveCommand.Create<int>(static _ => { });
        view.ViewModel.Command1 = newCmd;
        await Assert.That(view.Command1.Command).IsEqualTo(newCmd);

        disp.Dispose();
        await Assert.That(view.Command1.Command).IsNull();
    }

    /// <summary>Tests the command bind nested command wireup.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindNestedCommandWireup()
    {
        var vm = new CommandBindViewModel
        {
            NestedViewModel = new()
        };

        var view = new CommandBindView { ViewModel = vm };

        _ = view.BindCommand(view.ViewModel, static m => m.NestedViewModel.NestedCommand, static x => x.Command1);

        await Assert.That(view.Command1.Command).IsEqualTo(view.ViewModel.NestedViewModel.NestedCommand);
    }

    /// <summary>Tests the command bind sets initial enabled state true.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindSetsInitialEnabledState_True()
    {
        var view = new CommandBindView { ViewModel = new() };

        var canExecute1 = new BehaviorSignal<bool>(true);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(static _ => { }, canExecute1);

        _ = view.BindCommand(view.ViewModel, static x => x.Command1, static x => x.Command1);

        await Assert.That(view.Command1.IsEnabled).IsTrue();
    }

    /// <summary>Tests the command bind sets disables command when can execute changed.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindSetsDisablesCommandWhenCanExecuteChanged()
    {
        var view = new CommandBindView { ViewModel = new() };

        var canExecute1 = new BehaviorSignal<bool>(true);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(static _ => { }, canExecute1);

        _ = view.BindCommand(view.ViewModel, static x => x.Command1, static x => x.Command1);

        await Assert.That(view.Command1.IsEnabled).IsTrue();

        canExecute1.OnNext(false);

        await Assert.That(view.Command1.IsEnabled).IsFalse();
    }

    /// <summary>Tests the command bind sets initial enabled state false.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindSetsInitialEnabledState_False()
    {
        var view = new CommandBindView { ViewModel = new() };

        var canExecute1 = new BehaviorSignal<bool>(false);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(static _ => { }, canExecute1);

        _ = view.BindCommand(view.ViewModel, static x => x.Command1, static x => x.Command1);

        await Assert.That(view.Command1.IsEnabled).IsFalse();
    }

    /// <summary>Tests the command bind raises can execute changed on bind.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindRaisesCanExecuteChangedOnBind()
    {
        var view = new CommandBindView { ViewModel = new() };

        var canExecute1 = new BehaviorSignal<bool>(true);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(static _ => { }, canExecute1);

        _ = view.BindCommand(view.ViewModel, static x => x.Command1, static x => x.Command1);

        await Assert.That(view.Command1.IsEnabled).IsTrue();

        // Now  change to a disabled cmd
        var canExecute2 = new BehaviorSignal<bool>(false);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(static _ => { }, canExecute2);

        await Assert.That(view.Command1.IsEnabled).IsFalse();
    }

    /// <summary>Tests the command bind with parameter expression.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindWithParameterExpression()
    {
        var view = new CommandBindView { ViewModel = new() };

        var received = 0;
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(i => received = i);

        _ = view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1, x => x.Value, nameof(CustomClickButton.CustomClick));

        view.ViewModel.Value = FirstValue;
        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(FirstValue);

        view.ViewModel.Value = SecondValue;
        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(SecondValue);
    }

    /// <summary>Tests the command bind with delay set vm parameter expression.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindWithDelaySetVmParameterExpression()
    {
        var view = new ReactiveObjectCommandBindView
        {
            ViewModel = new()
        };

        var received = 0;
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(i => received = i);

        _ = view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1, x => x.Value, nameof(CustomClickButton.CustomClick));

        view.ViewModel.Value = FirstValue;
        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(FirstValue);

        view.ViewModel.Value = SecondValue;
        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(SecondValue);
    }

    /// <summary>Tests the command bind with delay set vm parameter no inpc expression.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindWithDelaySetVmParameterNoInpcExpression()
    {
        var view = new CommandBindView { ViewModel = new() };

        var received = 0;
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(i => received = i);
        view.ViewModel.Value = InitialValue;

        _ = view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1, x => x.Value, nameof(CustomClickButton.CustomClick));

        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(InitialValue);

        view.ViewModel.Value = FirstValue;
        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(FirstValue);

        view.ViewModel.Value = SecondValue;
        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(SecondValue);
    }

    /// <summary>Tests the command bind with parameter observable.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBindWithParameterObservable()
    {
        var view = new CommandBindView { ViewModel = new() };

        var received = 0;
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(i => received = i);
        view.ViewModel.Value = InitialValue;
        var value = view.ViewModel.WhenAnyValue(v => v.Value);
        _ = view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1, value, nameof(CustomClickButton.CustomClick));

        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(InitialValue);

        view.ViewModel.Value = FirstValue;
        view.Command1.RaiseCustomClick();

        await Assert.That(received).IsEqualTo(FirstValue);
    }
}
