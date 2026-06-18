// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Commands;

/// <summary>
///     Comprehensive test suite for ReactiveCommand.
///     Tests cover all factory methods, behaviors, and edge cases.
///     Organized into logical test groups for maintainability.
/// </summary>
[NotInParallel]
[TestExecutor<WithSchedulerExecutor>]
public partial class ReactiveCommandTest
{
    /// <summary>The numeric parameter value passed to commands under test.</summary>
    private const int ParameterValue = 42;

    /// <summary>The string form of the parameter value passed to commands under test.</summary>
    private const string ParameterValueString = "42";

    /// <summary>The error message used when simulating a command failure.</summary>
    private const string TestErrorMessage = "Test error";

    /// <summary>The exception message used when simulating a task failure.</summary>
    private const string TaskExceptionMessage = "Task exception";

    /// <summary>The status text recorded when a command has started executing.</summary>
    private const string StartedCommandStatus = "started command";

    /// <summary>The status text recorded when IsExecuting transitions to false.</summary>
    private const string ExecutingFalseStatus = "executing = False";

    /// <summary>The status text recorded when a command has finished cancelling.</summary>
    private const string FinishedCancellingStatus = "finished cancelling";

    /// <summary>The status text recorded when a command is in the process of cancelling.</summary>
    private const string CancellingCommandStatus = "cancelling command";

    /// <summary>Verifies that CanExecute behaves as a behavioral observable, immediately yielding its current value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecute_IsBehavioral()
    {
        var command = ReactiveCommand.Create(
            () => { },
            outputScheduler: Sequencer.Immediate);
        var canExecute = command.CanExecute.Collect();

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(1);
            await Assert.That(canExecute[0]).IsTrue();
        }
    }

    /// <summary>Verifies that CanExecute reports false while the command is executing.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task CanExecute_IsFalseWhileExecuting()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var execute = SingleValueObservable.Void.Delay(TimeSpan.FromSeconds(1), scheduler);
        var command = ReactiveCommand.CreateFromObservable<RxVoid>(
            () => execute,
            outputScheduler: scheduler);
        var canExecute = command.CanExecute.Collect();

        command.Execute().Subscribe();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        const int ExpectedCount = 2;

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(ExpectedCount);
            await Assert.That(canExecute[0]).IsTrue();
            await Assert.That(canExecute[1]).IsFalse();
        }
    }

    /// <summary>Verifies that CanExecute only ticks when its value changes, suppressing duplicates.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecute_OnlyTicksDistinctValues()
    {
        var canExecuteSubject = new BehaviorSignal<bool>(false);
        var command = ReactiveCommand.Create(
            () => { },
            canExecuteSubject,
            Sequencer.Immediate);
        var canExecute = command.CanExecute.Collect();

        canExecuteSubject.OnNext(false);
        canExecuteSubject.OnNext(false);
        canExecuteSubject.OnNext(true);
        canExecuteSubject.OnNext(true);

        const int ExpectedCount = 2;

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(ExpectedCount);
            await Assert.That(canExecute[0]).IsFalse();
            await Assert.That(canExecute[1]).IsTrue();
        }
    }

    /// <summary>Verifies that CanExecute reflects the values from the provided can-execute observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecute_RespectsProvidedObservable()
    {
        var canExecuteSubject = new BehaviorSignal<bool>(false);
        var command = ReactiveCommand.Create(
            () => { },
            canExecuteSubject,
            Sequencer.Immediate);
        var canExecute = command.CanExecute.Collect();

        canExecuteSubject.OnNext(true);
        canExecuteSubject.OnNext(false);

        const int ExpectedCount = 3;
        const int ThirdIndex = 2;

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(ExpectedCount);
            await Assert.That(canExecute[0]).IsFalse();
            await Assert.That(canExecute[1]).IsTrue();
            await Assert.That(canExecute[ThirdIndex]).IsFalse();
        }
    }

    /// <summary>Verifies that errors from the can-execute observable are surfaced through ThrownExceptions.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecute_TicksExceptionsThroughThrownExceptions()
    {
        var canExecuteSubject = new Signal<bool>();
        var command = ReactiveCommand.Create(
            () => { },
            canExecuteSubject,
            Sequencer.Immediate);
        var exceptions = command.ThrownExceptions.Collect();

        canExecuteSubject.OnError(new InvalidOperationException(TestErrorMessage));

        await Assert.That(exceptions).Count().IsEqualTo(1);
        await Assert.That(exceptions[0]).IsTypeOf<InvalidOperationException>();
    }

    /// <summary>Verifies that disposing the command unsubscribes from the can-execute observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecute_UnsubscribesOnDisposal()
    {
        var canExecuteSubject = new BehaviorSignal<bool>(true);
        var command = ReactiveCommand.Create(
            () => { },
            canExecuteSubject,
            Sequencer.Immediate);

        await Assert.That(canExecuteSubject.HasObservers).IsTrue();

        command.Dispose();

        await Assert.That(canExecuteSubject.HasObservers).IsFalse();
    }

    /// <summary>Verifies that a command created from an action executes the action successfully.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Create_Action_ExecutesSuccessfully()
    {
        var executed = false;
        var command = ReactiveCommand.Create(
            () => executed = true,
            outputScheduler: Sequencer.Immediate);

        await command.Execute().FirstAsync();
        await Assert.That(executed).IsTrue();
    }

    /// <summary>Verifies that an action command only executes when its can-execute observable allows it.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Create_Action_RespectsCanExecute()
    {
        var canExecute = new BehaviorSignal<bool>(false);
        var executed = false;
        var command = ReactiveCommand.Create(
            () => executed = true,
            canExecute,
            Sequencer.Immediate);
        var source = new Signal<RxVoid>();

        source.InvokeCommand(command);

        source.OnNext(RxVoid.Default);
        await Assert.That(executed).IsFalse();

        canExecute.OnNext(true);
        source.OnNext(RxVoid.Default);
        await Assert.That(executed).IsTrue();
    }

    /// <summary>Verifies that creating an action command with a null execute argument throws.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Create_Action_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.Create(null!);
            await Task.CompletedTask;
        });

    /// <summary>Verifies that a parameterized action command handles multiple executions in order.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Create_ActionWithParam_HandlesMultipleExecutions()
    {
        var parameters = new List<int>();
        var command = ReactiveCommand.Create<int>(
            parameters.Add,
            outputScheduler: Sequencer.Immediate);

        const int SecondParameter = 2;
        const int ThirdParameter = 3;
        const int ExpectedCount = 3;
        const int ThirdIndex = 2;

        await command.Execute(1).FirstAsync();
        await command.Execute(SecondParameter).FirstAsync();
        await command.Execute(ThirdParameter).FirstAsync();

        using (Assert.Multiple())
        {
            await Assert.That(parameters).Count().IsEqualTo(ExpectedCount);
            await Assert.That(parameters[0]).IsEqualTo(1);
            await Assert.That(parameters[1]).IsEqualTo(SecondParameter);
            await Assert.That(parameters[ThirdIndex]).IsEqualTo(ThirdParameter);
        }
    }

    /// <summary>Verifies that a parameterized action command receives the supplied parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Create_ActionWithParam_PassesParameterCorrectly()
    {
        var receivedParam = 0;
        var command = ReactiveCommand.Create<int>(
            param => receivedParam = param,
            outputScheduler: Sequencer.Immediate);

        await command.Execute(ParameterValue).FirstAsync();
        await Assert.That(receivedParam).IsEqualTo(ParameterValue);
    }

    /// <summary>Verifies that creating a parameterized action command with a null execute argument throws.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Create_ActionWithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.Create((Action<int>)null!);
            await Task.CompletedTask;
        });

    /// <summary>Verifies that a command created from a function ticks its return value as a result.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Create_Func_ReturnsResult()
    {
        var command = ReactiveCommand.Create(
            () => ParameterValue,
            outputScheduler: Sequencer.Immediate);
        var results = command.Collect();

        await command.Execute().FirstAsync();

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]).IsEqualTo(ParameterValue);
    }

    /// <summary>Verifies that creating a function command with a null execute argument throws.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Create_Func_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.Create((Func<int>)null!);
            await Task.CompletedTask;
        });

    /// <summary>Verifies that a function command ticks a fresh result for each execution.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Create_Func_TicksMultipleResults()
    {
        var counter = 0;
        var command = ReactiveCommand.Create(
            () => ++counter,
            outputScheduler: Sequencer.Immediate);
        var results = command.Collect();

        await command.Execute().FirstAsync();
        await command.Execute().FirstAsync();
        await command.Execute().FirstAsync();

        const int SecondResult = 2;
        const int ThirdResult = 3;
        const int ExpectedCount = 3;
        const int ThirdIndex = 2;

        using (Assert.Multiple())
        {
            await Assert.That(results).Count().IsEqualTo(ExpectedCount);
            await Assert.That(results[0]).IsEqualTo(1);
            await Assert.That(results[1]).IsEqualTo(SecondResult);
            await Assert.That(results[ThirdIndex]).IsEqualTo(ThirdResult);
        }
    }

    /// <summary>Verifies that a parameterized function command returns a result derived from the parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Create_FuncWithParam_ReturnsResultFromParameter()
    {
        var command = ReactiveCommand.Create<int, string>(
            param => param.ToString(),
            outputScheduler: Sequencer.Immediate);
        var results = command.Collect();

        await command.Execute(ParameterValue).FirstAsync();

        await Assert.That(results[0]).IsEqualTo(ParameterValueString);
    }

    /// <summary>Verifies that creating a parameterized function command with a null execute argument throws.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Create_FuncWithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.Create((Func<int, string>)null!);
            await Task.CompletedTask;
        });

    /// <summary>Verifies that a parameterized function command transforms each parameter into a result.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Create_FuncWithParam_TransformsParameters()
    {
        const int Multiplier = 2;
        const int FirstParameter = 5;
        const int SecondParameter = 10;
        const int ThirdParameter = 15;
        const int ExpectedCount = 3;
        const int FirstExpected = 10;
        const int SecondExpected = 20;
        const int ThirdExpected = 30;
        const int ThirdIndex = 2;

        var command = ReactiveCommand.Create<int, int>(
            param => param * Multiplier,
            outputScheduler: Sequencer.Immediate);
        var results = command.Collect();

        await command.Execute(FirstParameter).FirstAsync();
        await command.Execute(SecondParameter).FirstAsync();
        await command.Execute(ThirdParameter).FirstAsync();

        using (Assert.Multiple())
        {
            await Assert.That(results).Count().IsEqualTo(ExpectedCount);
            await Assert.That(results[0]).IsEqualTo(FirstExpected);
            await Assert.That(results[1]).IsEqualTo(SecondExpected);
            await Assert.That(results[ThirdIndex]).IsEqualTo(ThirdExpected);
        }
    }

    /// <summary>Verifies that a combined command cannot execute while any child command cannot execute.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateCombined_CanExecuteIsFalseIfAnyChildCannotExecute()
    {
        var canExecute1 = new BehaviorSignal<bool>(true);
        var canExecute2 = new BehaviorSignal<bool>(false);

        var cmd1 = ReactiveCommand.Create<int, int>(
            x => x,
            canExecute1,
            Sequencer.Immediate);
        var cmd2 = ReactiveCommand.Create<int, int>(
            x => x,
            canExecute2,
            Sequencer.Immediate);

        var combined = ReactiveCommand.CreateCombined(
            [cmd1, cmd2],
            outputScheduler: Sequencer.Immediate);

        var canExecuteValue = await combined.CanExecute.FirstAsync();
        await Assert.That(canExecuteValue).IsFalse();

        canExecute2.OnNext(true);
        canExecuteValue = await combined.CanExecute.FirstAsync();
        await Assert.That(canExecuteValue).IsTrue();
    }

    /// <summary>Verifies that executing a combined command executes all child commands and collects their results.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateCombined_ExecutesAllChildCommands()
    {
        var executed1 = false;
        var executed2 = false;
        var executed3 = false;

        const int FirstMultiplier = 2;
        const int SecondMultiplier = 3;
        const int ThirdMultiplier = 4;
        const int Parameter = 5;
        const int ExpectedChildCount = 3;
        const int FirstExpected = 10;
        const int SecondExpected = 15;
        const int ThirdExpected = 20;
        const int ThirdIndex = 2;

        var cmd1 = ReactiveCommand.Create<int, int>(
            x =>
            {
                executed1 = true;
                return x * FirstMultiplier;
            },
            outputScheduler: Sequencer.Immediate);
        var cmd2 = ReactiveCommand.Create<int, int>(
            x =>
            {
                executed2 = true;
                return x * SecondMultiplier;
            },
            outputScheduler: Sequencer.Immediate);
        var cmd3 = ReactiveCommand.Create<int, int>(
            x =>
            {
                executed3 = true;
                return x * ThirdMultiplier;
            },
            outputScheduler: Sequencer.Immediate);

        var combined = ReactiveCommand.CreateCombined(
            [cmd1, cmd2, cmd3],
            outputScheduler: Sequencer.Immediate);

        var results = combined.Collect();

        await combined.Execute(Parameter).FirstAsync();

        using (Assert.Multiple())
        {
            await Assert.That(executed1).IsTrue();
            await Assert.That(executed2).IsTrue();
            await Assert.That(executed3).IsTrue();
            await Assert.That(results).Count().IsEqualTo(1);
            await Assert.That(results[0]).Count().IsEqualTo(ExpectedChildCount);
            await Assert.That(results[0][0]).IsEqualTo(FirstExpected);
            await Assert.That(results[0][1]).IsEqualTo(SecondExpected);
            await Assert.That(results[0][ThirdIndex]).IsEqualTo(ThirdExpected);
        }
    }

    /// <summary>Verifies that exceptions thrown by a child command propagate through the combined command's ThrownExceptions.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateCombined_PropagatesChildExceptions()
    {
        var cmd1 = ReactiveCommand.Create<int, int>(
            x => x,
            outputScheduler: Sequencer.Immediate);
        var cmd2 = ReactiveCommand.Create<int, int>(
            x => throw new InvalidOperationException("Test exception"),
            outputScheduler: Sequencer.Immediate);

        var combined = ReactiveCommand.CreateCombined(
            [cmd1, cmd2],
            outputScheduler: Sequencer.Immediate);

        var exceptions = combined.ThrownExceptions.Collect();

        const int Parameter = 5;

        combined.Execute(Parameter).Subscribe(_ => { }, _ => { });

        await Assert.That(exceptions).Count().IsEqualTo(1);
        await Assert.That(exceptions[0]).IsTypeOf<InvalidOperationException>();
    }

    /// <summary>Verifies that creating a combined command from an empty child collection throws.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateCombined_ThrowsOnEmptyChildCommands() =>
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            _ = ReactiveCommand.CreateCombined<int, int>([]);
            await Task.CompletedTask;
        });

    /// <summary>Verifies that creating a combined command from a null child collection throws.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateCombined_ThrowsOnNullChildCommands() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateCombined<int, int>(null!);
            await Task.CompletedTask;
        });

    /// <summary>Verifies that an observable command without a parameter emits all values from its observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromObservable_WithoutParam_EmitsMultipleValues()
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

    /// <summary>Verifies that an observable command without a parameter emits the value produced by its observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromObservable_WithoutParam_EmitsObservableResults()
    {
        var command = ReactiveCommand.CreateFromObservable(
            () => Signal.Emit(ParameterValue),
            outputScheduler: Sequencer.Immediate);
        var results = command.Collect();

        await command.Execute().FirstAsync();

        await Assert.That(results[0]).IsEqualTo(ParameterValue);
    }

    /// <summary>Verifies that creating an observable command without a parameter from a null execute argument throws.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromObservable_WithoutParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateFromObservable((Func<IObservable<int>>)null!);
            await Task.CompletedTask;
        });

    /// <summary>Verifies that a parameterized observable command passes the parameter to its observable factory.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromObservable_WithParam_PassesParameterToObservable()
    {
        var command = ReactiveCommand.CreateFromObservable<int, string>(
            param => Signal.Emit(param.ToString()),
            outputScheduler: Sequencer.Immediate);
        var results = command.Collect();

        await command.Execute(ParameterValue).FirstAsync();
        await Assert.That(results[0]).IsEqualTo(ParameterValueString);
    }

    /// <summary>Verifies that creating a parameterized observable command from a null execute argument throws.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromObservable_WithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateFromObservable((Func<int, IObservable<string>>)null!);
            await Task.CompletedTask;
        });
}
