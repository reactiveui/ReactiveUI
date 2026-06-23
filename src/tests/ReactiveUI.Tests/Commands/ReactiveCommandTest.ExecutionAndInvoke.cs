// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using ReactiveUI.Tests.Commands.Mocks;
using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Commands;

/// <summary>
///     Comprehensive test suite for ReactiveCommand.
///     Tests cover all factory methods, behaviors, and edge cases.
///     Organized into logical test groups for maintainability.
/// </summary>
public partial class ReactiveCommandTest
{
    /// <summary>Verifies that an in-flight execution can be cancelled by disposing its subscription.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task Execute_CanBeCancelled()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var execute = SingleValueObservable.Void.Delay(TimeSpan.FromSeconds(1), scheduler);
        var command = ReactiveCommand.CreateFromObservable<RxVoid>(
            () => execute,
            outputScheduler: scheduler);
        var executed = command.Collect();

        var sub1 = command.Execute().Subscribe();
        _ = command.Execute().Subscribe();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        await Assert.That(await command.IsExecuting.FirstAsync()).IsTrue();
        await Assert.That(executed).IsEmpty();

        sub1.Dispose();
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));

        await Assert.That(executed).Count().IsEqualTo(1);
        await Assert.That(await command.IsExecuting.FirstAsync()).IsFalse();
    }

    /// <summary>Verifies that Execute is lazy and only runs the command when the returned observable is subscribed.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Execute_LazyEvaluation()
    {
        var executionCount = 0;
        var command = ReactiveCommand.Create(
            () => ++executionCount,
            outputScheduler: Sequencer.Immediate);

        var execution = command.Execute();
        await Assert.That(executionCount).IsEqualTo(0);

        _ = execution.Subscribe();
        await Assert.That(executionCount).IsEqualTo(1);
    }

    /// <summary>Verifies that Execute forwards each supplied parameter to the command.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Execute_PassesParameters()
    {
        const int ThirdParameter = 348;
        const int ExpectedCount = 3;
        const int ThirdIndex = 2;

        var parameters = new List<int>();
        var command = ReactiveCommand.Create<int>(
            parameters.Add,
            outputScheduler: Sequencer.Immediate);

        await command.Execute(1).FirstAsync();
        await command.Execute(ParameterValue).FirstAsync();
        await command.Execute(ThirdParameter).FirstAsync();

        using (Assert.Multiple())
        {
            await Assert.That(parameters).Count().IsEqualTo(ExpectedCount);
            await Assert.That(parameters[0]).IsEqualTo(1);
            await Assert.That(parameters[1]).IsEqualTo(ParameterValue);
            await Assert.That(parameters[ThirdIndex]).IsEqualTo(ThirdParameter);
        }
    }

    /// <summary>Verifies that the command becomes executable again after a successful execution completes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Execute_ReenablesAfterCompletion()
    {
        var command = ReactiveCommand.Create(
            () => { },
            outputScheduler: Sequencer.Immediate);
        var canExecute = command.CanExecute.Collect();

        await command.Execute().FirstAsync();

        const int ExpectedCount = 3;
        const int ThirdIndex = 2;

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(ExpectedCount);
            await Assert.That(canExecute[0]).IsTrue();
            await Assert.That(canExecute[1]).IsFalse();
            await Assert.That(canExecute[ThirdIndex]).IsTrue();
        }
    }

    /// <summary>Verifies that the command becomes executable again after an execution fails.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Execute_ReenablesAfterFailure()
    {
        var command = ReactiveCommand.CreateFromObservable(
            () => Signal.Fail<RxVoid>(new InvalidOperationException(TestErrorMessage)),
            outputScheduler: Sequencer.Immediate);
        var canExecute = command.CanExecute.Collect();
        _ = command.ThrownExceptions.Subscribe();

        _ = command.Execute().Subscribe(_ => { }, _ => { });

        const int ExpectedCount = 3;
        const int ThirdIndex = 2;

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(ExpectedCount);
            await Assert.That(canExecute[0]).IsTrue();
            await Assert.That(canExecute[1]).IsFalse();
            await Assert.That(canExecute[ThirdIndex]).IsTrue();
        }
    }

    /// <summary>Verifies that a single execution can tick multiple results from its observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task Execute_TicksMultipleResults()
    {
        const int SecondValue = 2;
        const int ThirdValue = 3;
        const int ExpectedCount = 3;
        const int ThirdIndex = 2;

        var command = ReactiveCommand.CreateFromObservable(
            () => new[] { 1, SecondValue, ThirdValue }.ToObservable(),
            outputScheduler: Sequencer.Immediate);
        var results = command.Collect();

        await command.Execute().FirstAsync();

        using (Assert.Multiple())
        {
            await Assert.That(results).Count().IsEqualTo(ExpectedCount);
            await Assert.That(results[0]).IsEqualTo(1);
            await Assert.That(results[1]).IsEqualTo(SecondValue);
            await Assert.That(results[ThirdIndex]).IsEqualTo(ThirdValue);
        }
    }

    /// <summary>Verifies that the ICommand CanExecute returns false while the command is executing.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task ICommand_CanExecute_IsFalseWhileExecuting()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var execute = SingleValueObservable.Void.Delay(TimeSpan.FromSeconds(1), scheduler);
        ICommand command = ReactiveCommand.CreateFromObservable<RxVoid>(
            () => execute,
            outputScheduler: scheduler);

        await Assert.That(command.CanExecute(null)).IsTrue();

        command.Execute(null);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));
        await Assert.That(command.CanExecute(null)).IsFalse();
    }

    /// <summary>Verifies that the ICommand CanExecute reflects the current can-execute state.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ICommand_CanExecute_ReturnsCorrectValue()
    {
        var canExecuteSubject = new BehaviorSignal<bool>(false);
        ICommand command = ReactiveCommand.Create(
            () => { },
            canExecuteSubject,
            Sequencer.Immediate);

        await Assert.That(command.CanExecute(null)).IsFalse();

        canExecuteSubject.OnNext(true);
        await Assert.That(command.CanExecute(null)).IsTrue();
    }

    /// <summary>Verifies that the ICommand raises CanExecuteChanged when its can-execute state changes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ICommand_CanExecuteChanged_RaisesEvents()
    {
        var canExecuteSubject = new BehaviorSignal<bool>(false);
        ICommand command = ReactiveCommand.Create(
            () => { },
            canExecuteSubject,
            Sequencer.Immediate);
        var canExecuteChanged = new List<bool>();
        command.CanExecuteChanged += (_, _) => canExecuteChanged.Add(command.CanExecute(null));

        canExecuteSubject.OnNext(true);
        canExecuteSubject.OnNext(false);

        const int ExpectedCount = 2;

        using (Assert.Multiple())
        {
            await Assert.That(canExecuteChanged).Count().IsEqualTo(ExpectedCount);
            await Assert.That(canExecuteChanged[0]).IsTrue();
            await Assert.That(canExecuteChanged[1]).IsFalse();
        }
    }

    /// <summary>Verifies that invoking the ICommand Execute runs the command.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ICommand_Execute_InvokesCommand()
    {
        var executed = false;
        ICommand command = ReactiveCommand.Create(
            () => executed = true,
            outputScheduler: Sequencer.Immediate);

        command.Execute(null);
        await Assert.That(executed).IsTrue();
    }

    /// <summary>Verifies that the ICommand Execute forwards its parameter to the command.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ICommand_Execute_PassesParameter()
    {
        var receivedParam = 0;
        ICommand command = ReactiveCommand.Create<int>(
            param => receivedParam = param,
            outputScheduler: Sequencer.Immediate);

        command.Execute(ParameterValue);
        await Assert.That(receivedParam).IsEqualTo(ParameterValue);
    }

    /// <summary>Verifies that the ICommand Execute throws when given a parameter of the wrong type.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ICommand_Execute_ThrowsOnIncorrectParameterType()
    {
        ICommand command = ReactiveCommand.Create<int>(
            _ => { },
            outputScheduler: Sequencer.Immediate);

        var ex = Assert.Throws<InvalidOperationException>(() => command.Execute("wrong type"));
        await Assert.That(ex.Message).Contains("System.Int32");
        await Assert.That(ex.Message).Contains("System.String");
    }

    /// <summary>Verifies that the ICommand Execute accepts nullable parameters including null values.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ICommand_Execute_WorksWithNullableParameters()
    {
        int? receivedValue = null;
        ICommand command = ReactiveCommand.Create<int?>(
            param => receivedValue = param,
            outputScheduler: Sequencer.Immediate);

        command.Execute(ParameterValue);
        await Assert.That(receivedValue).IsEqualTo(ParameterValue);

        command.Execute(null);
        await Assert.That(receivedValue).IsNull();
    }

    /// <summary>Verifies that InvokeCommand on an ICommand executes the command for each source value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ICommand_InvokesCommand()
    {
        var executionCount = 0;
        ICommand command = ReactiveCommand.Create(
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

    /// <summary>Verifies that InvokeCommand on an ICommand passes each source value as the command parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ICommand_PassesParameter()
    {
        const int SecondParameter = 100;
        const int ExpectedCount = 2;

        var receivedParams = new List<int>();
        ICommand command = ReactiveCommand.Create<int>(
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

    /// <summary>Verifies that InvokeCommand on an ICommand only executes when the command can execute.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ICommand_RespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSignal<bool>(false);
        ICommand command = ReactiveCommand.Create(
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

    /// <summary>Verifies that InvokeCommand on an ICommand works with a cold source observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ICommand_WorksWithColdObservable()
    {
        var executionCount = 0;
        ICommand command = ReactiveCommand.Create(
            () => ++executionCount,
            outputScheduler: Sequencer.Immediate);
        var source = Signal.Emit(RxVoid.Default);
        _ = source.InvokeCommand(command);

        await Assert.That(executionCount).IsEqualTo(1);
    }

    /// <summary>Verifies that InvokeCommand against a target's ICommand property executes the command.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ICommandInTarget_InvokesCommand()
    {
        var executionCount = 0;
        var target = new CommandHolder();
        var source = new Signal<RxVoid>();
        _ = source.InvokeCommand(target, x => x.TheCommand!);
        target.TheCommand = ReactiveCommand.Create(
            () => ++executionCount,
            outputScheduler: Sequencer.Immediate);

        const int ExpectedSecondCount = 2;

        source.OnNext(RxVoid.Default);
        await Assert.That(executionCount).IsEqualTo(1);

        source.OnNext(RxVoid.Default);
        await Assert.That(executionCount).IsEqualTo(ExpectedSecondCount);
    }

    /// <summary>Verifies that InvokeCommand against a target's ICommand property passes the source value as parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommand_ICommandInTarget_PassesParameter()
    {
        var target = new CommandHolder();
        var source = new Signal<int>();
        _ = source.InvokeCommand(target, x => x.TheCommand!);
        var command = new FakeCommand();
        target.TheCommand = command;

        source.OnNext(ParameterValue);

        using (Assert.Multiple())
        {
            await Assert.That(command.CanExecuteParameter).IsEqualTo(ParameterValue);
            await Assert.That(command.ExecuteParameter).IsEqualTo(ParameterValue);
        }
    }

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
