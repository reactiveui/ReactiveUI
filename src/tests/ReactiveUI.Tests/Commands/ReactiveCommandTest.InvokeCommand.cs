// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Commands.Mocks;

namespace ReactiveUI.Tests.Commands;

/// <summary>InvokeCommand extension-method tests for <see cref="ReactiveCommand"/>.</summary>
public partial class ReactiveCommandTest
{
    /// <summary>Verifies that InvokeCommand against a target's ICommand property respects the command's can-execute state.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ICommandInTarget_RespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSignal<bool>(false);
        var target = new CommandHolder();
        var source = new Signal<RxVoid>();
        _ = source.InvokeCommand(target, x => x.TheCommand!);
        target.TheCommand = ReactiveCommand.Create(
            () => executed = true,
            canExecute,
            Sequencer.Immediate);

        source.OnNext(RxVoid.Default);
        await Assert.That(executed).IsFalse();

        canExecute.OnNext(true);
        source.OnNext(RxVoid.Default);
        await Assert.That(executed).IsTrue();
    }

    /// <summary>Verifies that source values arriving while a target ICommand cannot execute are not replayed when it reopens.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ICommandInTarget_RespectsCanExecuteWindow()
    {
        var executed = false;
        var canExecute = new BehaviorSignal<bool>(false);
        var target = new CommandHolder();
        var source = new Signal<RxVoid>();
        _ = source.InvokeCommand(target, x => x.TheCommand!);
        target.TheCommand = ReactiveCommand.Create(
            () => executed = true,
            canExecute,
            Sequencer.Immediate);

        source.OnNext(RxVoid.Default);
        await Assert.That(executed).IsFalse();

        // When window reopens, previous requests should NOT execute
        canExecute.OnNext(true);
        await Assert.That(executed).IsFalse();
    }

    /// <summary>Verifies that exceptions thrown by a target ICommand do not break the InvokeCommand subscription.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ICommandInTarget_SwallowsExceptions()
    {
        var count = 0;
        var target = new CommandHolder();
        var command = ReactiveCommand.Create(
            () =>
            {
                ++count;
                throw new InvalidOperationException();
            },
            outputScheduler: Sequencer.Immediate);
        _ = command.ThrownExceptions.Subscribe();
        target.TheCommand = command;
        var source = new Signal<RxVoid>();
        _ = source.InvokeCommand(target, x => x.TheCommand!);

        const int ExpectedCount = 2;

        source.OnNext(RxVoid.Default);
        source.OnNext(RxVoid.Default);

        await Assert.That(count).IsEqualTo(ExpectedCount);
    }

    /// <summary>Verifies that InvokeCommand on a ReactiveCommand executes the command for each source value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ReactiveCommand_InvokesCommand()
    {
        var executionCount = 0;
        var command = ReactiveCommand.Create(
            () => ++executionCount,
            outputScheduler: Sequencer.Immediate);
        var source = new Signal<RxVoid>();
        _ = source.InvokeCommand(command);

        const int ExpectedSecondCount = 2;

        source.OnNext(RxVoid.Default);
        await Assert.That(executionCount).IsEqualTo(1);

        source.OnNext(RxVoid.Default);
        await Assert.That(executionCount).IsEqualTo(ExpectedSecondCount);
    }

    /// <summary>Verifies that InvokeCommand on a ReactiveCommand passes each source value as the command parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ReactiveCommand_PassesParameter()
    {
        const int SecondParameter = 100;
        const int ExpectedCount = 2;

        var receivedParams = new List<int>();
        var command = ReactiveCommand.Create<int>(
            receivedParams.Add,
            outputScheduler: Sequencer.Immediate);
        var source = new Signal<int>();
        _ = source.InvokeCommand(command);

        source.OnNext(ParameterValue);
        source.OnNext(SecondParameter);

        using (Assert.Multiple())
        {
            await Assert.That(receivedParams).Count().IsEqualTo(ExpectedCount);
            await Assert.That(receivedParams[0]).IsEqualTo(ParameterValue);
            await Assert.That(receivedParams[1]).IsEqualTo(SecondParameter);
        }
    }

    /// <summary>Verifies that InvokeCommand on a ReactiveCommand only executes when the command can execute.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ReactiveCommand_RespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSignal<bool>(false);
        var command = ReactiveCommand.Create(
            () => executed = true,
            canExecute,
            Sequencer.Immediate);
        var source = new Signal<RxVoid>();
        _ = source.InvokeCommand(command);

        source.OnNext(RxVoid.Default);
        await Assert.That(executed).IsFalse();

        canExecute.OnNext(true);
        source.OnNext(RxVoid.Default);
        await Assert.That(executed).IsTrue();
    }

    /// <summary>Verifies that source values arriving while a ReactiveCommand cannot execute are not replayed when it reopens.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ReactiveCommand_RespectsCanExecuteWindow()
    {
        var executed = false;
        var canExecute = new BehaviorSignal<bool>(false);
        var command = ReactiveCommand.Create(
            () => executed = true,
            canExecute,
            Sequencer.Immediate);
        var source = new Signal<RxVoid>();
        _ = source.InvokeCommand(command);

        source.OnNext(RxVoid.Default);
        await Assert.That(executed).IsFalse();

        // When window reopens, previous requests should NOT execute
        canExecute.OnNext(true);
        await Assert.That(executed).IsFalse();
    }

    /// <summary>Verifies that exceptions thrown by a ReactiveCommand do not break the InvokeCommand subscription.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ReactiveCommand_SwallowsExceptions()
    {
        var count = 0;
        var command = ReactiveCommand.Create(
            () =>
            {
                ++count;
                throw new InvalidOperationException();
            },
            outputScheduler: Sequencer.Immediate);
        _ = command.ThrownExceptions.Subscribe();
        var source = new Signal<RxVoid>();
        _ = source.InvokeCommand(command);

        const int ExpectedCount = 2;

        source.OnNext(RxVoid.Default);
        source.OnNext(RxVoid.Default);

        await Assert.That(count).IsEqualTo(ExpectedCount);
    }

    /// <summary>Verifies that InvokeCommand against a target's ReactiveCommand property executes the command.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ReactiveCommandInTarget_InvokesCommand()
    {
        var executionCount = 0;
        var target = new ReactiveCommandHolder();
        var source = new Signal<int>();
        _ = source.InvokeCommand(target, x => x.TheCommand!);
        target.TheCommand = ReactiveCommand.Create<int>(
            _ => ++executionCount,
            outputScheduler: Sequencer.Immediate);

        const int ExpectedSecondCount = 2;

        source.OnNext(0);
        await Assert.That(executionCount).IsEqualTo(1);

        source.OnNext(0);
        await Assert.That(executionCount).IsEqualTo(ExpectedSecondCount);
    }

    /// <summary>Verifies that InvokeCommand against a target's ReactiveCommand property passes the source value as parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ReactiveCommandInTarget_PassesParameter()
    {
        var receivedParam = 0;
        var target = new ReactiveCommandHolder();
        var source = new Signal<int>();
        _ = source.InvokeCommand(target, x => x.TheCommand!);
        target.TheCommand = ReactiveCommand.Create<int>(
            param => receivedParam = param,
            outputScheduler: Sequencer.Immediate);

        source.OnNext(ParameterValue);
        await Assert.That(receivedParam).IsEqualTo(ParameterValue);
    }

    /// <summary>Verifies that InvokeCommand against a target's ReactiveCommand property respects its can-execute state.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ReactiveCommandInTarget_RespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSignal<bool>(false);
        var target = new ReactiveCommandHolder();
        var source = new Signal<int>();
        _ = source.InvokeCommand(target, x => x.TheCommand!);
        target.TheCommand = ReactiveCommand.Create<int>(
            _ => executed = true,
            canExecute,
            Sequencer.Immediate);

        source.OnNext(0);
        await Assert.That(executed).IsFalse();

        canExecute.OnNext(true);
        source.OnNext(0);
        await Assert.That(executed).IsTrue();
    }

    /// <summary>Verifies that source values arriving while a target ReactiveCommand cannot execute are not replayed when it reopens.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ReactiveCommandInTarget_RespectsCanExecuteWindow()
    {
        var executed = false;
        var canExecute = new BehaviorSignal<bool>(false);
        var target = new ReactiveCommandHolder();
        var source = new Signal<int>();
        _ = source.InvokeCommand(target, x => x.TheCommand!);
        target.TheCommand = ReactiveCommand.Create<int>(
            _ => executed = true,
            canExecute,
            Sequencer.Immediate);

        source.OnNext(0);
        await Assert.That(executed).IsFalse();

        // When window reopens, previous requests should NOT execute
        canExecute.OnNext(true);
        await Assert.That(executed).IsFalse();
    }

    /// <summary>Verifies that exceptions thrown by a target ReactiveCommand do not break the InvokeCommand subscription.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ReactiveCommandInTarget_SwallowsExceptions()
    {
        var count = 0;
        var target = new ReactiveCommandHolder
        {
            TheCommand = ReactiveCommand.Create<int>(
                _ =>
                {
                    ++count;
                    throw new InvalidOperationException();
                },
                outputScheduler: Sequencer.Immediate)
        };
        _ = target.TheCommand.ThrownExceptions.Subscribe();
        var source = new Signal<int>();
        _ = source.InvokeCommand(target, x => x.TheCommand!);

        const int ExpectedCount = 2;

        source.OnNext(0);
        source.OnNext(0);

        await Assert.That(count).IsEqualTo(ExpectedCount);
    }
}
