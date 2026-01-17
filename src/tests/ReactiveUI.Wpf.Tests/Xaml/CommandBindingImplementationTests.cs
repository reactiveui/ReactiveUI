// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Mocks;
using ReactiveUI.Tests.Wpf;
using ReactiveUI.Tests.Xaml.Mocks;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests with the command binding implementation.
/// </summary>
[NotInParallel]
public class CommandBindingImplementationTests
{
    private WpfAppBuilderScope? _appBuilderScope;

    /// <summary>
    /// Sets up the WPF app builder scope for each test.
    /// </summary>
    [Before(Test)]
    public void Setup()
    {
        _appBuilderScope = new WpfAppBuilderScope();
    }

    /// <summary>
    /// Tears down the WPF app builder scope after each test.
    /// </summary>
    [After(Test)]
    public void TearDown()
    {
        _appBuilderScope?.Dispose();
    }

    /// <summary>
    /// Tests the command bind by name wireup.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
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

    /// <summary>
    /// Tests the command bind nested command wireup.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindNestedCommandWireup()
    {
        var vm = new CommandBindViewModel
        {
            NestedViewModel = new()
        };

        var view = new CommandBindView { ViewModel = vm };

        view.BindCommand(view.ViewModel, static m => m.NestedViewModel.NestedCommand, static x => x.Command1);

        await Assert.That(view.Command1.Command).IsEqualTo(view.ViewModel.NestedViewModel.NestedCommand);
    }

    /// <summary>
    /// Tests the command bind sets initial enabled state true.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindSetsInitialEnabledState_True()
    {
        var view = new CommandBindView { ViewModel = new() };

        var canExecute1 = new BehaviorSubject<bool>(true);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(static _ => { }, canExecute1);

        view.BindCommand(view.ViewModel, static x => x.Command1, static x => x.Command1);

        await Assert.That(view.Command1.IsEnabled).IsTrue();
    }

    /// <summary>
    /// Tests the command bind sets disables command when can execute changed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindSetsDisablesCommandWhenCanExecuteChanged()
    {
        var view = new CommandBindView { ViewModel = new() };

        var canExecute1 = new BehaviorSubject<bool>(true);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(static _ => { }, canExecute1);

        view.BindCommand(view.ViewModel, static x => x.Command1, static x => x.Command1);

        await Assert.That(view.Command1.IsEnabled).IsTrue();

        canExecute1.OnNext(false);

        await Assert.That(view.Command1.IsEnabled).IsFalse();
    }

    /// <summary>
    /// Tests the command bind sets initial enabled state false.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindSetsInitialEnabledState_False()
    {
        var view = new CommandBindView { ViewModel = new() };

        var canExecute1 = new BehaviorSubject<bool>(false);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(static _ => { }, canExecute1);

        view.BindCommand(view.ViewModel, static x => x.Command1, static x => x.Command1);

        await Assert.That(view.Command1.IsEnabled).IsFalse();
    }

    /// <summary>
    /// Tests the command bind raises can execute changed on bind.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindRaisesCanExecuteChangedOnBind()
    {
        var view = new CommandBindView { ViewModel = new() };

        var canExecute1 = new BehaviorSubject<bool>(true);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(static _ => { }, canExecute1);

        view.BindCommand(view.ViewModel, static x => x.Command1, static x => x.Command1);

        await Assert.That(view.Command1.IsEnabled).IsTrue();

        // Now  change to a disabled cmd
        var canExecute2 = new BehaviorSubject<bool>(false);
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(static _ => { }, canExecute2);

        await Assert.That(view.Command1.IsEnabled).IsFalse();
    }

    /// <summary>
    /// Tests the command bind with parameter expression.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindWithParameterExpression()
    {
        var view = new CommandBindView { ViewModel = new() };

        var received = 0;
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(i => received = i);

        var disp = view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1, x => x.Value, nameof(CustomClickButton.CustomClick));

        view.ViewModel.Value = 42;
        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(42);

        view.ViewModel.Value = 13;
        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(13);
    }

    /// <summary>
    /// Tests the command bind with delay set vm parameter expression.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindWithDelaySetVMParameterExpression()
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
        await Assert.That(received).IsEqualTo(42);

        view.ViewModel.Value = 13;
        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(13);
    }

    /// <summary>
    /// Tests the command bind with delay set vm parameter no inpc expression.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindWithDelaySetVMParameterNoINPCExpression()
    {
        var view = new CommandBindView { ViewModel = new() };

        var received = 0;
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(i => received = i);
        view.ViewModel.Value = 10;

        view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1, x => x.Value, nameof(CustomClickButton.CustomClick));

        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(10);

        view.ViewModel.Value = 42;
        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(42);

        view.ViewModel.Value = 13;
        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(13);
    }

    /// <summary>
    /// Tests the command bind with parameter observable.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CommandBindWithParameterObservable()
    {
        var view = new CommandBindView { ViewModel = new() };

        var received = 0;
        view.ViewModel.Command1 = ReactiveCommand.Create<int>(i => received = i);
        view.ViewModel.Value = 10;
        var value = view.ViewModel.WhenAnyValue(v => v.Value);
        var disp = view.BindCommand(view.ViewModel, x => x.Command1, x => x.Command1, value, nameof(CustomClickButton.CustomClick));

        view.Command1.RaiseCustomClick();
        await Assert.That(received).IsEqualTo(10);

        view.ViewModel.Value = 42;
        view.Command1.RaiseCustomClick();

        await Assert.That(received).IsEqualTo(42);
    }
}
