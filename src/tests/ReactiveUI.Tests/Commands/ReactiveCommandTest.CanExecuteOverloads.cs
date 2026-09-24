// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace ReactiveUI.Tests.Commands;

/// <summary>
///     Covers the factory overloads that take a can-execute observable, and optionally a background
///     scheduler, but no output scheduler. Each test gates the command closed, confirms the gate is
///     honoured, then opens it and confirms the delegate runs.
/// </summary>
public partial class ReactiveCommandTest
{
    /// <summary>Verifies the parameterized action overload with a can-execute gate honours the gate and passes the parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Create_ActionWithParam_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var received = 0;
        var command = ReactiveCommand.Create<int>(param => received = param, canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        await command.Execute(ParameterValue).FirstAsync();

        await Assert.That(received).IsEqualTo(ParameterValue);
    }

    /// <summary>Verifies the parameterized function overload with a can-execute gate honours the gate and returns the result.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Create_FuncWithParam_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var command = ReactiveCommand.Create<int, string>(static param => param.ToString(), canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        var result = await command.Execute(ParameterValue).FirstAsync();

        await Assert.That(result).IsEqualTo(ParameterValueString);
    }

    /// <summary>Verifies the background action overload with a can-execute gate honours the gate and runs the action.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_Action_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var executed = false;
        var command = ReactiveCommand.CreateRunInBackground(Run, canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        await command.Execute().FirstAsync();

        await Assert.That(executed).IsTrue();

        // A void method group binds only to the Action overload; a lambda would bind to Func<bool>.
        void Run() => executed = true;
    }

    /// <summary>Verifies the background action overload with a can-execute gate and background scheduler honours the gate and runs the action.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_Action_CanExecuteAndBackgroundScheduler_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var executed = false;
        var command = ReactiveCommand.CreateRunInBackground(Run, canExecute, Sequencer.Immediate);

        await AssertGatedThenOpenAsync(command, canExecute);
        await command.Execute().FirstAsync();

        await Assert.That(executed).IsTrue();

        void Run() => executed = true;
    }

    /// <summary>Verifies the background action overload with background and output schedulers runs the action.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_Action_BackgroundAndOutputSchedulers_Executes()
    {
        var executed = false;
        var command = ReactiveCommand.CreateRunInBackground(Run, Sequencer.Immediate, Sequencer.Immediate);

        await command.Execute().FirstAsync();

        await Assert.That(executed).IsTrue();

        void Run() => executed = true;
    }

    /// <summary>Verifies the background function overload with a can-execute gate honours the gate and returns the result.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_Func_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var command = ReactiveCommand.CreateRunInBackground(static () => ParameterValue, canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        var result = await command.Execute().FirstAsync();

        await Assert.That(result).IsEqualTo(ParameterValue);
    }

    /// <summary>Verifies the background function overload with a can-execute gate and background scheduler honours the gate and returns the result.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_Func_CanExecuteAndBackgroundScheduler_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var command = ReactiveCommand.CreateRunInBackground(static () => ParameterValue, canExecute, Sequencer.Immediate);

        await AssertGatedThenOpenAsync(command, canExecute);
        var result = await command.Execute().FirstAsync();

        await Assert.That(result).IsEqualTo(ParameterValue);
    }

    /// <summary>Verifies the parameterized background action overload with a can-execute gate honours the gate and passes the parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_ActionWithParam_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var received = 0;
        var command = ReactiveCommand.CreateRunInBackground<int>(Run, canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        await command.Execute(ParameterValue).FirstAsync();

        await Assert.That(received).IsEqualTo(ParameterValue);

        void Run(int param) => received = param;
    }

    /// <summary>Verifies the parameterized background action overload with a can-execute gate and background scheduler honours the gate and passes the parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_ActionWithParam_CanExecuteAndBackgroundScheduler_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var received = 0;
        var command = ReactiveCommand.CreateRunInBackground<int>(Run, canExecute, Sequencer.Immediate);

        await AssertGatedThenOpenAsync(command, canExecute);
        await command.Execute(ParameterValue).FirstAsync();

        await Assert.That(received).IsEqualTo(ParameterValue);

        void Run(int param) => received = param;
    }

    /// <summary>Verifies the parameterized background function overload with a can-execute gate honours the gate and transforms the parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_FuncWithParam_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var command = ReactiveCommand.CreateRunInBackground<int, string>(static param => param.ToString(), canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        var result = await command.Execute(ParameterValue).FirstAsync();

        await Assert.That(result).IsEqualTo(ParameterValueString);
    }

    /// <summary>Verifies the parameterized background function overload with a can-execute gate and background scheduler honours the gate and transforms the parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_FuncWithParam_CanExecuteAndBackgroundScheduler_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var command = ReactiveCommand.CreateRunInBackground<int, string>(static param => param.ToString(), canExecute, Sequencer.Immediate);

        await AssertGatedThenOpenAsync(command, canExecute);
        var result = await command.Execute(ParameterValue).FirstAsync();

        await Assert.That(result).IsEqualTo(ParameterValueString);
    }

    /// <summary>Verifies the combined command overload with a can-execute gate honours the gate and runs every child.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateCombined_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var child1 = ReactiveCommand.Create<int, int>(static param => param, outputScheduler: Sequencer.Immediate);
        var child2 = ReactiveCommand.Create<int, int>(static param => param + 1, outputScheduler: Sequencer.Immediate);
        var command = ReactiveCommand.CreateCombined([child1, child2], canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        var results = await command.Execute(ParameterValue).FirstAsync();

        await Assert.That(results).IsEquivalentTo([ParameterValue, ParameterValue + 1]);
    }

    /// <summary>Verifies the parameterized observable overload with a can-execute gate honours the gate and surfaces the result.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromObservable_WithParam_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var command = ReactiveCommand.CreateFromObservable<int, string>(
            static param => new ReturnSignal<string>(param.ToString(), Sequencer.Immediate),
            canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        var result = await command.Execute(ParameterValue).FirstAsync();

        await Assert.That(result).IsEqualTo(ParameterValueString);
    }

    /// <summary>Verifies the result task overload with a can-execute gate honours the gate and returns the result.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_Result_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var command = ReactiveCommand.CreateFromTask(static () => Task.FromResult(ParameterValue), canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        var result = await command.Execute().FirstAsync();

        await Assert.That(result).IsEqualTo(ParameterValue);
    }

    /// <summary>Verifies the cancellable result task overload with a can-execute gate honours the gate and returns the result.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_CancellableResult_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var command = ReactiveCommand.CreateFromTask(static _ => Task.FromResult(ParameterValue), canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        var result = await command.Execute().FirstAsync();

        await Assert.That(result).IsEqualTo(ParameterValue);
    }

    /// <summary>Verifies the task overload with a can-execute gate honours the gate and runs the task.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_Unit_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var executed = false;
        var command = ReactiveCommand.CreateFromTask(
            () =>
            {
                executed = true;
                return Task.CompletedTask;
            },
            canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        await command.Execute().FirstAsync();

        await Assert.That(executed).IsTrue();
    }

    /// <summary>Verifies the cancellable task overload with a can-execute gate honours the gate and runs the task.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_CancellableUnit_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var executed = false;
        var command = ReactiveCommand.CreateFromTask(
            _=>
            {
                executed = true;
                return Task.CompletedTask;
            },
            canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        await command.Execute().FirstAsync();

        await Assert.That(executed).IsTrue();
    }

    /// <summary>Verifies the parameterized result task overload with a can-execute gate honours the gate and transforms the parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_ParamResult_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var command = ReactiveCommand.CreateFromTask<int, string>(static param => Task.FromResult(param.ToString()), canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        var result = await command.Execute(ParameterValue).FirstAsync();

        await Assert.That(result).IsEqualTo(ParameterValueString);
    }

    /// <summary>Verifies the cancellable parameterized result task overload with a can-execute gate honours the gate and transforms the parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_CancellableParamResult_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var command = ReactiveCommand.CreateFromTask<int, string>(static (param, _) => Task.FromResult(param.ToString()), canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        var result = await command.Execute(ParameterValue).FirstAsync();

        await Assert.That(result).IsEqualTo(ParameterValueString);
    }

    /// <summary>Verifies the parameterized task overload with a can-execute gate honours the gate and passes the parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_Param_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var received = 0;
        var command = ReactiveCommand.CreateFromTask<int>(
            param =>
            {
                received = param;
                return Task.CompletedTask;
            },
            canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        await command.Execute(ParameterValue).FirstAsync();

        await Assert.That(received).IsEqualTo(ParameterValue);
    }

    /// <summary>Verifies the cancellable parameterized task overload with a can-execute gate honours the gate and passes the parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_CancellableParam_CanExecute_GatesAndExecutes()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var received = 0;
        var command = ReactiveCommand.CreateFromTask<int>(
            (param, _) =>
            {
                received = param;
                return Task.CompletedTask;
            },
            canExecute);

        await AssertGatedThenOpenAsync(command, canExecute);
        await command.Execute(ParameterValue).FirstAsync();

        await Assert.That(received).IsEqualTo(ParameterValue);
    }

    /// <summary>Asserts the command is closed while the gate is false, then opens the gate and asserts it can execute.</summary>
    /// <param name="command">The command under test.</param>
    /// <param name="canExecute">The gate the command was created with.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private static async Task AssertGatedThenOpenAsync(ICommand command, BehaviorSignal<bool> canExecute)
    {
        await Assert.That(command.CanExecute(null)).IsFalse();

        canExecute.OnNext(true);

        await Assert.That(command.CanExecute(null)).IsTrue();
    }
}
