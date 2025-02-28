// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using FactAttribute = Xunit.WpfFactAttribute;
#endif

namespace ReactiveUI.Tests.Xaml;

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
        var view = new CommandBindView { ViewModel = new() };

        Assert.Null(view.Command1.Command);

        var disp = view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1);
        Assert.Equal(view.ViewModel.Command1, view.Command1.Command);

        var newCmd = ReactiveCommand.Create<int>(_ => { });
        view.ViewModel.Command1 = newCmd;
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
            NestedViewModel = new()
        };

        var view = new CommandBindView { ViewModel = vm };

        view.BindCommand(view.ViewModel, m => m.NestedViewModel.NestedCommand, x => x.Command1);

        Assert.Equal(view.ViewModel.NestedViewModel.NestedCommand, view.Command1.Command);
    }

    /// <summary>
    /// Tests the command bind sets initial enabled state true.
    /// </summary>
    [Fact]
    public void CommandBindSetsInitialEnabledState_True()
    {
        var view = new CommandBindView { ViewModel = new() };

        var canExecute1 = new BehaviorSubject<bool>(true);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(_ => { }, canExecute1);

        view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1);

        Assert.True(view.Command1.IsEnabled);
    }

    /// <summary>
    /// Tests the command bind sets disables command when can execute changed.
    /// </summary>
    [Fact]
    public void CommandBindSetsDisablesCommandWhenCanExecuteChanged()
    {
        var view = new CommandBindView { ViewModel = new() };

        var canExecute1 = new BehaviorSubject<bool>(true);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(_ => { }, canExecute1);

        view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1);

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
        var view = new CommandBindView { ViewModel = new() };

        var canExecute1 = new BehaviorSubject<bool>(false);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(_ => { }, canExecute1);

        view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1);

        Assert.False(view.Command1.IsEnabled);
    }

    /// <summary>
    /// Tests the command bind raises can execute changed on bind.
    /// </summary>
    [Fact]
    public void CommandBindRaisesCanExecuteChangedOnBind()
    {
        var view = new CommandBindView { ViewModel = new() };

        var canExecute1 = new BehaviorSubject<bool>(true);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(_ => { }, canExecute1);

        view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1);

        Assert.True(view.Command1.IsEnabled);

        // Now  change to a disabled cmd
        var canExecute2 = new BehaviorSubject<bool>(false);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(_ => { }, canExecute2);

        Assert.False(view.Command1.IsEnabled);
    }

    /// <summary>
    /// Tests the command bind with parameter expression.
    /// </summary>
    [Fact]
    public void CommandBindWithParameterExpression()
    {
        var view = new CommandBindView { ViewModel = new() };

        var received = 0;
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(i => received = i);

        var disp = view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1, x => x.Value, nameof(CustomClickButton.CustomClick));

        view.ViewModel.Value = 42;
        view.Command1.RaiseCustomClick();
        Assert.Equal(42, received);

        view.ViewModel.Value = 13;
        view.Command1.RaiseCustomClick();
        Assert.Equal(13, received);
    }

    /// <summary>
    /// Tests the command bind with delay set vm parameter expression.
    /// </summary>
    [Fact]
    public void CommandBindWithDelaySetVMParameterExpression()
    {
        var view = new ReactiveObjectCommandBindView
        {
            ViewModel = new()
        };

        var received = 0;
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(i => received = i);

        var disp = view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1, x => x.Value, nameof(CustomClickButton.CustomClick));

        view.ViewModel.Value = 42;
        view.Command1.RaiseCustomClick();
        Assert.Equal(42, received);

        view.ViewModel.Value = 13;
        view.Command1.RaiseCustomClick();
        Assert.Equal(13, received);
    }

    /// <summary>
    /// Tests the command bind with delay set vm parameter no inpc expression.
    /// </summary>
    [Fact]
    public void CommandBindWithDelaySetVMParameterNoINPCExpression()
    {
        var view = new CommandBindView { ViewModel = new() };

        var received = 0;
        var cmd = ReactiveCommand.Create<int>(i => received = i);
        view.ViewModel.Command1 = cmd;
        view.ViewModel.Value = 10;

        view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1, x => x.Value, nameof(CustomClickButton.CustomClick));

        view.Command1.RaiseCustomClick();
        Assert.Equal(10, received);

        view.ViewModel.Value = 42;
        view.Command1.RaiseCustomClick();
        Assert.Equal(42, received);

        view.ViewModel.Value = 13;
        view.Command1.RaiseCustomClick();
        Assert.Equal(13, received);
    }

    /// <summary>
    /// Tests the command bind with parameter observable.
    /// </summary>
    [Fact]
    public void CommandBindWithParameterObservable()
    {
        var view = new CommandBindView { ViewModel = new() };

        var received = 0;
        var cmd = ReactiveCommand.Create<int>(i => received = i);
        view.ViewModel.Command1 = cmd;
        view.ViewModel.Value = 10;
        var value = view.ViewModel.WhenAnyValue(v => v.Value);
        var disp = view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1, value, nameof(CustomClickButton.CustomClick));

        view.Command1.RaiseCustomClick();
        Assert.Equal(10, received);

        view.ViewModel.Value = 42;
        view.Command1.RaiseCustomClick();

        Assert.Equal(42, received);
    }
}
