// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using DynamicData;
using ReactiveUI.Tests.Commands.Mocks;
using ReactiveUI.Tests.Mocks;

namespace ReactiveUI.Tests.Commands;

/// <summary>
///     Comprehensive test suite for ReactiveCommand.
///     Tests cover all factory methods, behaviors, and edge cases.
///     Organized into logical test groups for maintainability.
/// </summary>
[NotInParallel]
[TestExecutor<WithSchedulerExecutor>]
public class ReactiveCommandTest
{
    [Test]
    public async Task CanExecute_IsBehavioral()
    {
        var command = ReactiveCommand.Create(
            () => { },
            outputScheduler: ImmediateScheduler.Instance);
        command.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(1);
            await Assert.That(canExecute[0]).IsTrue();
        }
    }

    [Test]

    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task CanExecute_IsFalseWhileExecuting()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var execute = Observables.Unit.Delay(TimeSpan.FromSeconds(1), scheduler);
        var command = ReactiveCommand.CreateFromObservable(
            () => execute,
            outputScheduler: scheduler);
        command.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        command.Execute().Subscribe();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(2);
            await Assert.That(canExecute[0]).IsTrue();
            await Assert.That(canExecute[1]).IsFalse();
        }
    }

    [Test]
    public async Task CanExecute_OnlyTicksDistinctValues()
    {
        var canExecuteSubject = new BehaviorSubject<bool>(false);
        var command = ReactiveCommand.Create(
            () => { },
            canExecuteSubject,
            ImmediateScheduler.Instance);
        command.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        canExecuteSubject.OnNext(false);
        canExecuteSubject.OnNext(false);
        canExecuteSubject.OnNext(true);
        canExecuteSubject.OnNext(true);

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(2);
            await Assert.That(canExecute[0]).IsFalse();
            await Assert.That(canExecute[1]).IsTrue();
        }
    }

    [Test]
    public async Task CanExecute_RespectsProvidedObservable()
    {
        var canExecuteSubject = new BehaviorSubject<bool>(false);
        var command = ReactiveCommand.Create(
            () => { },
            canExecuteSubject,
            ImmediateScheduler.Instance);
        command.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        canExecuteSubject.OnNext(true);
        canExecuteSubject.OnNext(false);

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(3);
            await Assert.That(canExecute[0]).IsFalse();
            await Assert.That(canExecute[1]).IsTrue();
            await Assert.That(canExecute[2]).IsFalse();
        }
    }

    [Test]
    public async Task CanExecute_TicksExceptionsThroughThrownExceptions()
    {
        var canExecuteSubject = new Subject<bool>();
        var command = ReactiveCommand.Create(
            () => { },
            canExecuteSubject,
            ImmediateScheduler.Instance);
        command.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var exceptions)
            .Subscribe();

        canExecuteSubject.OnError(new InvalidOperationException("Test error"));

        await Assert.That(exceptions).Count().IsEqualTo(1);
        await Assert.That(exceptions[0]).IsTypeOf<InvalidOperationException>();
    }

    [Test]
    public async Task CanExecute_UnsubscribesOnDisposal()
    {
        var canExecuteSubject = new BehaviorSubject<bool>(true);
        var command = ReactiveCommand.Create(
            () => { },
            canExecuteSubject,
            ImmediateScheduler.Instance);

        await Assert.That(canExecuteSubject.HasObservers).IsTrue();

        command.Dispose();

        await Assert.That(canExecuteSubject.HasObservers).IsFalse();
    }

    [Test]
    public async Task Create_Action_ExecutesSuccessfully()
    {
        var executed = false;
        var command = ReactiveCommand.Create(
            () => executed = true,
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute();
        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task Create_Action_RespectsCanExecute()
    {
        var canExecute = new BehaviorSubject<bool>(false);
        var executed = false;
        var command = ReactiveCommand.Create(
            () => executed = true,
            canExecute,
            ImmediateScheduler.Instance);
        var source = new Subject<Unit>();

        source.InvokeCommand(command);

        source.OnNext(Unit.Default);
        await Assert.That(executed).IsFalse();

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task Create_Action_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.Create(null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task Create_ActionWithParam_HandlesMultipleExecutions()
    {
        var parameters = new List<int>();
        var command = ReactiveCommand.Create<int>(
            param => parameters.Add(param),
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute(1);
        await command.Execute(2);
        await command.Execute(3);

        using (Assert.Multiple())
        {
            await Assert.That(parameters).Count().IsEqualTo(3);
            await Assert.That(parameters[0]).IsEqualTo(1);
            await Assert.That(parameters[1]).IsEqualTo(2);
            await Assert.That(parameters[2]).IsEqualTo(3);
        }
    }

    [Test]
    public async Task Create_ActionWithParam_PassesParameterCorrectly()
    {
        var receivedParam = 0;
        var command = ReactiveCommand.Create<int>(
            param => receivedParam = param,
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute(42);
        await Assert.That(receivedParam).IsEqualTo(42);
    }

    [Test]
    public async Task Create_ActionWithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.Create((Action<int>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task Create_Func_ReturnsResult()
    {
        var command = ReactiveCommand.Create(
            () => 42,
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute();

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]).IsEqualTo(42);
    }

    [Test]
    public async Task Create_Func_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.Create((Func<int>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task Create_Func_TicksMultipleResults()
    {
        var counter = 0;
        var command = ReactiveCommand.Create(
            () => ++counter,
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute();
        await command.Execute();
        await command.Execute();

        using (Assert.Multiple())
        {
            await Assert.That(results).Count().IsEqualTo(3);
            await Assert.That(results[0]).IsEqualTo(1);
            await Assert.That(results[1]).IsEqualTo(2);
            await Assert.That(results[2]).IsEqualTo(3);
        }
    }

    [Test]
    public async Task Create_FuncWithParam_ReturnsResultFromParameter()
    {
        var command = ReactiveCommand.Create<int, string>(
            param => param.ToString(),
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute(42);

        await Assert.That(results[0]).IsEqualTo("42");
    }

    [Test]
    public async Task Create_FuncWithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.Create((Func<int, string>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task Create_FuncWithParam_TransformsParameters()
    {
        var command = ReactiveCommand.Create<int, int>(
            param => param * 2,
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute(5);
        await command.Execute(10);
        await command.Execute(15);

        using (Assert.Multiple())
        {
            await Assert.That(results).Count().IsEqualTo(3);
            await Assert.That(results[0]).IsEqualTo(10);
            await Assert.That(results[1]).IsEqualTo(20);
            await Assert.That(results[2]).IsEqualTo(30);
        }
    }

    [Test]
    public async Task CreateCombined_CanExecuteIsFalseIfAnyChildCannotExecute()
    {
        var canExecute1 = new BehaviorSubject<bool>(true);
        var canExecute2 = new BehaviorSubject<bool>(false);

        var cmd1 = ReactiveCommand.Create<int, int>(
            x => x,
            canExecute1,
            ImmediateScheduler.Instance);
        var cmd2 = ReactiveCommand.Create<int, int>(
            x => x,
            canExecute2,
            ImmediateScheduler.Instance);

        var combined = ReactiveCommand.CreateCombined(
            [cmd1, cmd2],
            outputScheduler: ImmediateScheduler.Instance);

        var canExecuteValue = await combined.CanExecute.FirstAsync();
        await Assert.That(canExecuteValue).IsFalse();

        canExecute2.OnNext(true);
        canExecuteValue = await combined.CanExecute.FirstAsync();
        await Assert.That(canExecuteValue).IsTrue();
    }

    [Test]
    public async Task CreateCombined_ExecutesAllChildCommands()
    {
        var executed1 = false;
        var executed2 = false;
        var executed3 = false;

        var cmd1 = ReactiveCommand.Create<int, int>(
            x =>
            {
                executed1 = true;
                return x * 2;
            },
            outputScheduler: ImmediateScheduler.Instance);
        var cmd2 = ReactiveCommand.Create<int, int>(
            x =>
            {
                executed2 = true;
                return x * 3;
            },
            outputScheduler: ImmediateScheduler.Instance);
        var cmd3 = ReactiveCommand.Create<int, int>(
            x =>
            {
                executed3 = true;
                return x * 4;
            },
            outputScheduler: ImmediateScheduler.Instance);

        var combined = ReactiveCommand.CreateCombined(
            [cmd1, cmd2, cmd3],
            outputScheduler: ImmediateScheduler.Instance);

        combined.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await combined.Execute(5);

        using (Assert.Multiple())
        {
            await Assert.That(executed1).IsTrue();
            await Assert.That(executed2).IsTrue();
            await Assert.That(executed3).IsTrue();
            await Assert.That(results).Count().IsEqualTo(1);
            await Assert.That(results[0]).Count().IsEqualTo(3);
            await Assert.That(results[0][0]).IsEqualTo(10);
            await Assert.That(results[0][1]).IsEqualTo(15);
            await Assert.That(results[0][2]).IsEqualTo(20);
        }
    }

    [Test]
    public async Task CreateCombined_PropagatesChildExceptions()
    {
        var cmd1 = ReactiveCommand.Create<int, int>(
            x => x,
            outputScheduler: ImmediateScheduler.Instance);
        var cmd2 = ReactiveCommand.Create<int, int>(
            x => throw new InvalidOperationException("Test exception"),
            outputScheduler: ImmediateScheduler.Instance);

        var combined = ReactiveCommand.CreateCombined(
            [cmd1, cmd2],
            outputScheduler: ImmediateScheduler.Instance);

        combined.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var exceptions)
            .Subscribe();

        combined.Execute(5).Subscribe(_ => { }, _ => { });

        await Assert.That(exceptions).Count().IsEqualTo(1);
        await Assert.That(exceptions[0]).IsTypeOf<InvalidOperationException>();
    }

    [Test]
    public async Task CreateCombined_ThrowsOnEmptyChildCommands() =>
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            ReactiveCommand.CreateCombined<int, int>([]);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateCombined_ThrowsOnNullChildCommands() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateCombined<int, int>(null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateFromObservable_WithoutParam_EmitsMultipleValues()
    {
        var command = ReactiveCommand.CreateFromObservable(
            () => new[] { 1, 2, 3 }.ToObservable(),
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute();

        using (Assert.Multiple())
        {
            await Assert.That(results).Count().IsEqualTo(3);
            await Assert.That(results[0]).IsEqualTo(1);
            await Assert.That(results[1]).IsEqualTo(2);
            await Assert.That(results[2]).IsEqualTo(3);
        }
    }

    [Test]
    public async Task CreateFromObservable_WithoutParam_EmitsObservableResults()
    {
        var command = ReactiveCommand.CreateFromObservable(
            () => Observable.Return(42),
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute();

        await Assert.That(results[0]).IsEqualTo(42);
    }

    [Test]
    public async Task CreateFromObservable_WithoutParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateFromObservable((Func<IObservable<int>>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateFromObservable_WithParam_PassesParameterToObservable()
    {
        var command = ReactiveCommand.CreateFromObservable<int, string>(
            param => Observable.Return(param.ToString()),
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute(42);
        await Assert.That(results[0]).IsEqualTo("42");
    }

    [Test]
    public async Task CreateFromObservable_WithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateFromObservable((Func<int, IObservable<string>>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateFromTask_Cancellable_ProperlyCancelsExecution()
    {
        var tcsStarted = new TaskCompletionSource<Unit>();
        var tcsCaught = new TaskCompletionSource<Unit>();
        var tcsFinish = new TaskCompletionSource<Unit>();

        var fixture = ReactiveCommand.CreateFromTask(
            async token =>
            {
                tcsStarted.TrySetResult(Unit.Default);
                try
                {
                    await Task.Delay(10000, token);
                }
                catch (OperationCanceledException)
                {
                    tcsCaught.TrySetResult(Unit.Default);
                    await tcsFinish.Task;
                    throw;
                }
            },
            outputScheduler: ImmediateScheduler.Instance);

        fixture.ThrownExceptions.Subscribe(_ => { });

        var disposable = fixture.Execute().Subscribe();

        await tcsStarted.Task.WaitAsync(TimeSpan.FromSeconds(2));
        disposable.Dispose();

        await tcsCaught.Task.WaitAsync(TimeSpan.FromSeconds(2));
        tcsFinish.TrySetResult(Unit.Default);

        // Wait for cancellation to complete
        await Task.Delay(100);
    }

    [Test]
    public async Task CreateFromTask_Cancellable_Unit_WithoutParam_ReceivesCancellationToken()
    {
        CancellationToken? receivedToken = null;
        var command = ReactiveCommand.CreateFromTask(
            async token =>
            {
                receivedToken = token;
                await Task.Delay(10, token);
            },
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute();
        await Assert.That(receivedToken).IsNotNull();
    }

    [Test]
    public async Task CreateFromTask_Cancellable_Unit_WithoutParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateFromTask((Func<CancellationToken, Task>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateFromTask_Cancellable_Unit_WithParam_ReceivesParameterAndToken()
    {
        var receivedParam = 0;
        CancellationToken? receivedToken = null;
        var command = ReactiveCommand.CreateFromTask<int>(
            async (param, token) =>
            {
                receivedParam = param;
                receivedToken = token;
                await Task.Delay(10, token);
            },
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute(42);

        using (Assert.Multiple())
        {
            await Assert.That(receivedParam).IsEqualTo(42);
            await Assert.That(receivedToken).IsNotNull();
        }
    }

    [Test]
    public async Task CreateFromTask_Cancellable_Unit_WithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateFromTask((Func<int, CancellationToken, Task>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateFromTask_Cancellable_WithoutParam_ReceivesCancellationToken()
    {
        CancellationToken? receivedToken = null;
        var command = ReactiveCommand.CreateFromTask(
            async token =>
            {
                receivedToken = token;
                await Task.Delay(10, token);
                return 42;
            },
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute();
        await Assert.That(receivedToken).IsNotNull();
    }

    [Test]
    public async Task CreateFromTask_Cancellable_WithoutParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateFromTask((Func<CancellationToken, Task<int>>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateFromTask_Cancellable_WithParam_ReceivesParameterAndToken()
    {
        var receivedParam = 0;
        CancellationToken? receivedToken = null;
        var command = ReactiveCommand.CreateFromTask<int, string>(
            async (param, token) =>
            {
                receivedParam = param;
                receivedToken = token;
                await Task.Delay(10, token);
                return param.ToString();
            },
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute(42);

        using (Assert.Multiple())
        {
            await Assert.That(receivedParam).IsEqualTo(42);
            await Assert.That(receivedToken).IsNotNull();
            await Assert.That(results[0]).IsEqualTo("42");
        }
    }

    [Test]
    public async Task CreateFromTask_Cancellable_WithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateFromTask((Func<int, CancellationToken, Task<string>>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateFromTask_Unit_WithoutParam_CompletesSuccessfully()
    {
        var executed = false;
        var command = ReactiveCommand.CreateFromTask(
            async () =>
            {
                await Task.CompletedTask;
                executed = true;
            },
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute();
        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task CreateFromTask_Unit_WithoutParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateFromTask((Func<Task>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateFromTask_Unit_WithParam_PassesParameter()
    {
        var receivedParam = 0;
        var command = ReactiveCommand.CreateFromTask<int>(
            async param =>
            {
                await Task.CompletedTask;
                receivedParam = param;
            },
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute(42);
        await Assert.That(receivedParam).IsEqualTo(42);
    }

    [Test]
    public async Task CreateFromTask_Unit_WithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateFromTask((Func<int, Task>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateFromTask_WithoutParam_ReturnsTaskResult()
    {
        var command = ReactiveCommand.CreateFromTask(
            () => Task.FromResult(42),
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute();
        await Assert.That(results[0]).IsEqualTo(42);
    }

    [Test]
    public async Task CreateFromTask_WithoutParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateFromTask((Func<Task<int>>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateFromTask_WithParam_PassesParameterToTask()
    {
        var command = ReactiveCommand.CreateFromTask<int, string>(
            param => Task.FromResult(param.ToString()),
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute(42);
        await Assert.That(results[0]).IsEqualTo("42");
    }

    [Test]
    public async Task CreateFromTask_WithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateFromTask((Func<int, Task<string>>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateRunInBackground_Action_ExecutesOnBackgroundScheduler()
    {
        var executed = false;
        var command = ReactiveCommand.CreateRunInBackground(
            () => executed = true,
            backgroundScheduler: ImmediateScheduler.Instance,
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute();
        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task CreateRunInBackground_Action_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateRunInBackground(null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateRunInBackground_ActionWithParam_PassesParameter()
    {
        var receivedParam = 0;
        var command = ReactiveCommand.CreateRunInBackground<int>(
            param => receivedParam = param,
            backgroundScheduler: ImmediateScheduler.Instance,
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute(42);
        await Assert.That(receivedParam).IsEqualTo(42);
    }

    [Test]
    public async Task CreateRunInBackground_ActionWithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateRunInBackground((Action<int>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateRunInBackground_Func_ReturnsResult()
    {
        var command = ReactiveCommand.CreateRunInBackground(
            () => 42,
            backgroundScheduler: ImmediateScheduler.Instance,
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute();

        await Assert.That(results[0]).IsEqualTo(42);
    }

    [Test]
    public async Task CreateRunInBackground_Func_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateRunInBackground((Func<int>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateRunInBackground_FuncWithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateRunInBackground((Func<int, string>)null!);
            await Task.CompletedTask;
        });

    [Test]
    public async Task CreateRunInBackground_FuncWithParam_TransformsParameter()
    {
        var command = ReactiveCommand.CreateRunInBackground<int, string>(
            param => param.ToString(),
            backgroundScheduler: ImmediateScheduler.Instance,
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute(42);
        await Assert.That(results[0]).IsEqualTo("42");
    }

    [Test]

    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task Execute_CanBeCancelled()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var execute = Observables.Unit.Delay(TimeSpan.FromSeconds(1), scheduler);
        var command = ReactiveCommand.CreateFromObservable(
            () => execute,
            outputScheduler: scheduler);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var executed).Subscribe();

        var sub1 = command.Execute().Subscribe();
        var sub2 = command.Execute().Subscribe();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        await Assert.That(command.IsExecuting.FirstAsync().Wait()).IsTrue();
        await Assert.That(executed).IsEmpty();

        sub1.Dispose();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        await Assert.That(executed).Count().IsEqualTo(1);
        await Assert.That(command.IsExecuting.FirstAsync().Wait()).IsFalse();
    }

    [Test]
    public async Task Execute_LazyEvaluation()
    {
        var executionCount = 0;
        var command = ReactiveCommand.Create(
            () => ++executionCount,
            outputScheduler: ImmediateScheduler.Instance);

        var execution = command.Execute();
        await Assert.That(executionCount).IsEqualTo(0);

        execution.Subscribe();
        await Assert.That(executionCount).IsEqualTo(1);
    }

    [Test]
    public async Task Execute_PassesParameters()
    {
        var parameters = new List<int>();
        var command = ReactiveCommand.Create<int>(
            param => parameters.Add(param),
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute(1);
        await command.Execute(42);
        await command.Execute(348);

        using (Assert.Multiple())
        {
            await Assert.That(parameters).Count().IsEqualTo(3);
            await Assert.That(parameters[0]).IsEqualTo(1);
            await Assert.That(parameters[1]).IsEqualTo(42);
            await Assert.That(parameters[2]).IsEqualTo(348);
        }
    }

    [Test]
    public async Task Execute_ReenablesAfterCompletion()
    {
        var command = ReactiveCommand.Create(
            () => { },
            outputScheduler: ImmediateScheduler.Instance);
        command.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        await command.Execute();

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(3);
            await Assert.That(canExecute[0]).IsTrue();
            await Assert.That(canExecute[1]).IsFalse();
            await Assert.That(canExecute[2]).IsTrue();
        }
    }

    [Test]
    public async Task Execute_ReenablesAfterFailure()
    {
        var command = ReactiveCommand.CreateFromObservable(
            () => Observable.Throw<Unit>(new InvalidOperationException("Test error")),
            outputScheduler: ImmediateScheduler.Instance);
        command.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();
        command.ThrownExceptions.Subscribe();

        command.Execute().Subscribe(_ => { }, _ => { });

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(3);
            await Assert.That(canExecute[0]).IsTrue();
            await Assert.That(canExecute[1]).IsFalse();
            await Assert.That(canExecute[2]).IsTrue();
        }
    }

    [Test]
    public async Task Execute_TicksMultipleResults()
    {
        var command = ReactiveCommand.CreateFromObservable(
            () => new[] { 1, 2, 3 }.ToObservable(),
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute();

        using (Assert.Multiple())
        {
            await Assert.That(results).Count().IsEqualTo(3);
            await Assert.That(results[0]).IsEqualTo(1);
            await Assert.That(results[1]).IsEqualTo(2);
            await Assert.That(results[2]).IsEqualTo(3);
        }
    }

    [Test]

    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task ICommand_CanExecute_IsFalseWhileExecuting()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var execute = Observables.Unit.Delay(TimeSpan.FromSeconds(1), scheduler);
        ICommand command = ReactiveCommand.CreateFromObservable(
            () => execute,
            outputScheduler: scheduler);

        await Assert.That(command.CanExecute(null)).IsTrue();

        command.Execute(null);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));
        await Assert.That(command.CanExecute(null)).IsFalse();
    }

    [Test]
    public async Task ICommand_CanExecute_ReturnsCorrectValue()
    {
        var canExecuteSubject = new BehaviorSubject<bool>(false);
        ICommand command = ReactiveCommand.Create(
            () => { },
            canExecuteSubject,
            ImmediateScheduler.Instance);

        await Assert.That(command.CanExecute(null)).IsFalse();

        canExecuteSubject.OnNext(true);
        await Assert.That(command.CanExecute(null)).IsTrue();
    }

    [Test]
    public async Task ICommand_CanExecuteChanged_RaisesEvents()
    {
        var canExecuteSubject = new BehaviorSubject<bool>(false);
        ICommand command = ReactiveCommand.Create(
            () => { },
            canExecuteSubject,
            ImmediateScheduler.Instance);
        var canExecuteChanged = new List<bool>();
        command.CanExecuteChanged += (_, __) => canExecuteChanged.Add(command.CanExecute(null));

        canExecuteSubject.OnNext(true);
        canExecuteSubject.OnNext(false);

        using (Assert.Multiple())
        {
            await Assert.That(canExecuteChanged).Count().IsEqualTo(2);
            await Assert.That(canExecuteChanged[0]).IsTrue();
            await Assert.That(canExecuteChanged[1]).IsFalse();
        }
    }

    [Test]
    public async Task ICommand_Execute_InvokesCommand()
    {
        var executed = false;
        ICommand command = ReactiveCommand.Create(
            () => executed = true,
            outputScheduler: ImmediateScheduler.Instance);

        command.Execute(null);
        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task ICommand_Execute_PassesParameter()
    {
        var receivedParam = 0;
        ICommand command = ReactiveCommand.Create<int>(
            param => receivedParam = param,
            outputScheduler: ImmediateScheduler.Instance);

        command.Execute(42);
        await Assert.That(receivedParam).IsEqualTo(42);
    }

    [Test]
    public async Task ICommand_Execute_ThrowsOnIncorrectParameterType()
    {
        ICommand command = ReactiveCommand.Create<int>(
            _ => { },
            outputScheduler: ImmediateScheduler.Instance);

        var ex = Assert.Throws<InvalidOperationException>(() => command.Execute("wrong type"));
        await Assert.That(ex!.Message).Contains("System.Int32");
        await Assert.That(ex.Message).Contains("System.String");
    }

    [Test]
    public async Task ICommand_Execute_WorksWithNullableParameters()
    {
        int? receivedValue = null;
        ICommand command = ReactiveCommand.Create<int?>(
            param => receivedValue = param,
            outputScheduler: ImmediateScheduler.Instance);

        command.Execute(42);
        await Assert.That(receivedValue).IsEqualTo(42);

        command.Execute(null);
        await Assert.That(receivedValue).IsNull();
    }

    [Test]
    public async Task InvokeCommand_ICommand_InvokesCommand()
    {
        var executionCount = 0;
        ICommand command = ReactiveCommand.Create(
            () => ++executionCount,
            outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(command);

        source.OnNext(Unit.Default);
        await Assert.That(executionCount).IsEqualTo(1);

        source.OnNext(Unit.Default);
        await Assert.That(executionCount).IsEqualTo(2);
    }

    [Test]
    public async Task InvokeCommand_ICommand_PassesParameter()
    {
        var receivedParams = new List<int>();
        ICommand command = ReactiveCommand.Create<int>(
            param => receivedParams.Add(param),
            outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<int>();
        source.InvokeCommand(command);

        source.OnNext(42);
        source.OnNext(100);

        using (Assert.Multiple())
        {
            await Assert.That(receivedParams).Count().IsEqualTo(2);
            await Assert.That(receivedParams[0]).IsEqualTo(42);
            await Assert.That(receivedParams[1]).IsEqualTo(100);
        }
    }

    [Test]
    public async Task InvokeCommand_ICommand_RespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        ICommand command = ReactiveCommand.Create(
            () => executed = true,
            canExecute,
            ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(command);

        source.OnNext(Unit.Default);
        await Assert.That(executed).IsFalse();

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task InvokeCommand_ICommand_WorksWithColdObservable()
    {
        var executionCount = 0;
        ICommand command = ReactiveCommand.Create(
            () => ++executionCount,
            outputScheduler: ImmediateScheduler.Instance);
        var source = Observable.Return(Unit.Default);
        source.InvokeCommand(command);

        await Assert.That(executionCount).IsEqualTo(1);
    }

    [Test]
    public async Task InvokeCommand_ICommandInTarget_InvokesCommand()
    {
        var executionCount = 0;
        var target = new ICommandHolder();
        var source = new Subject<Unit>();
        source.InvokeCommand(target, x => x.TheCommand!);
        target.TheCommand = ReactiveCommand.Create(
            () => ++executionCount,
            outputScheduler: ImmediateScheduler.Instance);

        source.OnNext(Unit.Default);
        await Assert.That(executionCount).IsEqualTo(1);

        source.OnNext(Unit.Default);
        await Assert.That(executionCount).IsEqualTo(2);
    }

    [Test]
    public async Task InvokeCommand_ICommandInTarget_PassesParameter()
    {
        var target = new ICommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(target, x => x.TheCommand!);
        var command = new FakeCommand();
        target.TheCommand = command;

        source.OnNext(42);

        using (Assert.Multiple())
        {
            await Assert.That(command.CanExecuteParameter).IsEqualTo(42);
            await Assert.That(command.ExecuteParameter).IsEqualTo(42);
        }
    }

    [Test]
    public async Task InvokeCommand_ICommandInTarget_RespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var target = new ICommandHolder();
        var source = new Subject<Unit>();
        source.InvokeCommand(target, x => x.TheCommand!);
        target.TheCommand = ReactiveCommand.Create(
            () => executed = true,
            canExecute,
            ImmediateScheduler.Instance);

        source.OnNext(Unit.Default);
        await Assert.That(executed).IsFalse();

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task InvokeCommand_ICommandInTarget_RespectsCanExecuteWindow()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var target = new ICommandHolder();
        var source = new Subject<Unit>();
        source.InvokeCommand(target, x => x.TheCommand!);
        target.TheCommand = ReactiveCommand.Create(
            () => executed = true,
            canExecute,
            ImmediateScheduler.Instance);

        source.OnNext(Unit.Default);
        await Assert.That(executed).IsFalse();

        // When window reopens, previous requests should NOT execute
        canExecute.OnNext(true);
        await Assert.That(executed).IsFalse();
    }

    [Test]
    public async Task InvokeCommand_ICommandInTarget_SwallowsExceptions()
    {
        var count = 0;
        var target = new ICommandHolder();
        var command = ReactiveCommand.Create(
            () =>
            {
                ++count;
                throw new InvalidOperationException();
            },
            outputScheduler: ImmediateScheduler.Instance);
        command.ThrownExceptions.Subscribe();
        target.TheCommand = command;
        var source = new Subject<Unit>();
        source.InvokeCommand(target, x => x.TheCommand!);

        source.OnNext(Unit.Default);
        source.OnNext(Unit.Default);

        await Assert.That(count).IsEqualTo(2);
    }

    [Test]
    public async Task InvokeCommand_ReactiveCommand_InvokesCommand()
    {
        var executionCount = 0;
        var command = ReactiveCommand.Create(
            () => ++executionCount,
            outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(command);

        source.OnNext(Unit.Default);
        await Assert.That(executionCount).IsEqualTo(1);

        source.OnNext(Unit.Default);
        await Assert.That(executionCount).IsEqualTo(2);
    }

    [Test]
    public async Task InvokeCommand_ReactiveCommand_PassesParameter()
    {
        var receivedParams = new List<int>();
        var command = ReactiveCommand.Create<int>(
            param => receivedParams.Add(param),
            outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<int>();
        source.InvokeCommand(command);

        source.OnNext(42);
        source.OnNext(100);

        using (Assert.Multiple())
        {
            await Assert.That(receivedParams).Count().IsEqualTo(2);
            await Assert.That(receivedParams[0]).IsEqualTo(42);
            await Assert.That(receivedParams[1]).IsEqualTo(100);
        }
    }

    [Test]
    public async Task InvokeCommand_ReactiveCommand_RespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var command = ReactiveCommand.Create(
            () => executed = true,
            canExecute,
            ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(command);

        source.OnNext(Unit.Default);
        await Assert.That(executed).IsFalse();

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task InvokeCommand_ReactiveCommand_RespectsCanExecuteWindow()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var command = ReactiveCommand.Create(
            () => executed = true,
            canExecute,
            ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(command);

        source.OnNext(Unit.Default);
        await Assert.That(executed).IsFalse();

        // When window reopens, previous requests should NOT execute
        canExecute.OnNext(true);
        await Assert.That(executed).IsFalse();
    }

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
            outputScheduler: ImmediateScheduler.Instance);
        command.ThrownExceptions.Subscribe();
        var source = new Subject<Unit>();
        source.InvokeCommand(command);

        source.OnNext(Unit.Default);
        source.OnNext(Unit.Default);

        await Assert.That(count).IsEqualTo(2);
    }

    [Test]
    public async Task InvokeCommand_ReactiveCommandInTarget_InvokesCommand()
    {
        var executionCount = 0;
        var target = new ReactiveCommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(target, x => x.TheCommand!);
        target.TheCommand = ReactiveCommand.Create<int>(
            _ => ++executionCount,
            outputScheduler: ImmediateScheduler.Instance);

        source.OnNext(0);
        await Assert.That(executionCount).IsEqualTo(1);

        source.OnNext(0);
        await Assert.That(executionCount).IsEqualTo(2);
    }

    [Test]
    public async Task InvokeCommand_ReactiveCommandInTarget_PassesParameter()
    {
        var receivedParam = 0;
        var target = new ReactiveCommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(target, x => x.TheCommand!);
        target.TheCommand = ReactiveCommand.Create<int>(
            param => receivedParam = param,
            outputScheduler: ImmediateScheduler.Instance);

        source.OnNext(42);
        await Assert.That(receivedParam).IsEqualTo(42);
    }

    [Test]
    public async Task InvokeCommand_ReactiveCommandInTarget_RespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var target = new ReactiveCommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(target, x => x.TheCommand!);
        target.TheCommand = ReactiveCommand.Create<int>(
            _ => executed = true,
            canExecute,
            ImmediateScheduler.Instance);

        source.OnNext(0);
        await Assert.That(executed).IsFalse();

        canExecute.OnNext(true);
        source.OnNext(0);
        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task InvokeCommand_ReactiveCommandInTarget_RespectsCanExecuteWindow()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var target = new ReactiveCommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(target, x => x.TheCommand!);
        target.TheCommand = ReactiveCommand.Create<int>(
            _ => executed = true,
            canExecute,
            ImmediateScheduler.Instance);

        source.OnNext(0);
        await Assert.That(executed).IsFalse();

        // When window reopens, previous requests should NOT execute
        canExecute.OnNext(true);
        await Assert.That(executed).IsFalse();
    }

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
                outputScheduler: ImmediateScheduler.Instance)
        };
        target.TheCommand.ThrownExceptions.Subscribe();
        var source = new Subject<int>();
        source.InvokeCommand(target, x => x.TheCommand!);

        source.OnNext(0);
        source.OnNext(0);

        await Assert.That(count).IsEqualTo(2);
    }

    [Test]

    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task IsExecuting_HandlesMultipleInFlightExecutions()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var execute = Observables.Unit.Delay(TimeSpan.FromMilliseconds(500), scheduler);
        var command = ReactiveCommand.CreateFromObservable(
            () => execute,
            outputScheduler: scheduler);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var executed).Subscribe();

        var sub1 = command.Execute().Subscribe();
        var sub2 = command.Execute().Subscribe();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        await Assert.That(command.IsExecuting.FirstAsync().Wait()).IsTrue();
        await Assert.That(executed).IsEmpty();
    }

    [Test]
    public async Task IsExecuting_IsBehavioral()
    {
        var command = ReactiveCommand.Create(
            () => { },
            outputScheduler: ImmediateScheduler.Instance);
        command.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(isExecuting).Count().IsEqualTo(1);
            await Assert.That(isExecuting[0]).IsFalse();
        }
    }

    [Test]
    public async Task IsExecuting_RemainsTrue_UntilExecutionCompletes()
    {
        var executeSubject = new Subject<Unit>();
        var command = ReactiveCommand.CreateFromObservable(
            () => executeSubject,
            outputScheduler: ImmediateScheduler.Instance);

        command.Execute().Subscribe();

        await Assert.That(command.IsExecuting.FirstAsync().Wait()).IsTrue();

        executeSubject.OnNext(Unit.Default);
        await Assert.That(command.IsExecuting.FirstAsync().Wait()).IsTrue();

        executeSubject.OnCompleted();
        await Assert.That(command.IsExecuting.FirstAsync().Wait()).IsFalse();
    }

    [Test]

    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task IsExecuting_TicksWhileExecuting()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var execute = Observables.Unit.Delay(TimeSpan.FromSeconds(1), scheduler);
        var command = ReactiveCommand.CreateFromObservable(
            () => execute,
            outputScheduler: scheduler);
        command.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting).Subscribe();

        command.Execute().Subscribe();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        using (Assert.Multiple())
        {
            await Assert.That(isExecuting).Count().IsEqualTo(2);
            await Assert.That(isExecuting[0]).IsFalse();
            await Assert.That(isExecuting[1]).IsTrue();
        }
    }

    [Test]
    public async Task Observable_Subscription_ProperLifecycle()
    {
        var executed = 0;
        var command = ReactiveCommand.Create(
            () => ++executed,
            outputScheduler: ImmediateScheduler.Instance);

        var subscription = command.Subscribe(_ => { });
        await command.Execute();

        await Assert.That(executed).IsEqualTo(1);

        subscription.Dispose();
        await command.Execute();

        // Should still execute even after subscription disposal
        await Assert.That(executed).IsEqualTo(2);
    }

    [Test]

    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task ReactiveSetpoint_AsyncMethodExecution()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        RxSchedulers.TaskpoolScheduler = scheduler;

        var fooVm = new FooViewModel(new Foo());

        await Assert.That(fooVm.Foo.Value).IsEqualTo(42);

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(11));
        await Assert.That(fooVm.Foo.Value).IsEqualTo(0);

        fooVm.Setpoint = 123;
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(5));
        await Assert.That(fooVm.Foo.Value).IsEqualTo(0);

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(6));
        await Assert.That(fooVm.Foo.Value).IsEqualTo(123);
    }

    [Test]
    public async Task Scheduler_BackgroundCommandUsesBackgroundScheduler()
    {
        var backgroundScheduler = ImmediateScheduler.Instance;
        var executed = false;
        var command = ReactiveCommand.CreateRunInBackground(
            () => executed = true,
            backgroundScheduler: backgroundScheduler,
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute();
        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task Scheduler_ResultsDeliveredOnOutputScheduler()
    {
        var scheduler = TestContext.Current!.GetScheduler();
        var command = ReactiveCommand.CreateFromObservable(
            () => Observables.Unit,
            outputScheduler: scheduler);
        var executed = false;

        command.Execute().ObserveOn(scheduler).Subscribe(_ => executed = true);

        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task Task_Cancellation_HandlesProperCancellationFlow()
    {
        var tcsStarted = new TaskCompletionSource<Unit>();
        var tcsCaught = new TaskCompletionSource<Unit>();
        var tcsFinish = new TaskCompletionSource<Unit>();
        var statusTrail = new List<(int Position, string Status)>();
        var position = 0;

        var command = ReactiveCommand.CreateFromTask(
            async token =>
            {
                statusTrail.Add((Interlocked.Increment(ref position) - 1, "started command"));
                tcsStarted.TrySetResult(Unit.Default);
                try
                {
                    await Task.Delay(10000, token);
                }
                catch (OperationCanceledException)
                {
                    statusTrail.Add((Interlocked.Increment(ref position) - 1, "cancelling command"));
                    tcsCaught.TrySetResult(Unit.Default);
                    await tcsFinish.Task;
                    statusTrail.Add((Interlocked.Increment(ref position) - 1, "finished cancelling"));
                    throw;
                }

                return Unit.Default;
            },
            outputScheduler: ImmediateScheduler.Instance);

        Exception? exception = null;
        command.ThrownExceptions.Subscribe(ex => exception = ex);
        var latestIsExecutingValue = false;
        command.IsExecuting.Subscribe(isExec =>
        {
            statusTrail.Add((Interlocked.Increment(ref position) - 1, $"executing = {isExec}"));
            Volatile.Write(ref latestIsExecutingValue, isExec);
        });

        var disposable = command.Execute().Subscribe();

        await tcsStarted.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();

        disposable.Dispose();

        await tcsCaught.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();

        tcsFinish.TrySetResult(Unit.Default);
        await Task.Delay(100);

        using (Assert.Multiple())
        {
            await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsFalse();
            await Assert.That(exception).IsTypeOf<TaskCanceledException>();
            await Assert.That(statusTrail).IsEquivalentTo(
            [
                (0, "executing = False"),
                (1, "executing = True"),
                (2, "started command"),
                (3, "cancelling command"),
                (4, "finished cancelling"),
                (5, "executing = False")
            ]);
        }
    }

    [Test]
    public async Task Task_Completion_HandlesProperCompletionFlow()
    {
        var tcsStarted = new TaskCompletionSource<Unit>();
        var tcsFinished = new TaskCompletionSource<Unit>();
        var tcsContinue = new TaskCompletionSource<Unit>();
        var statusTrail = new List<(int Position, string Status)>();
        var position = 0;

        var command = ReactiveCommand.CreateFromTask(
            async cts =>
            {
                statusTrail.Add((Interlocked.Increment(ref position) - 1, "started command"));
                tcsStarted.TrySetResult(Unit.Default);
                try
                {
                    await Task.Delay(1000, cts);
                }
                catch (OperationCanceledException)
                {
                    statusTrail.Add((Interlocked.Increment(ref position) - 1, "cancelling command"));
                    await Task.Delay(5000, CancellationToken.None);
                    statusTrail.Add((Interlocked.Increment(ref position) - 1, "finished cancelling"));
                    throw;
                }

                statusTrail.Add((Interlocked.Increment(ref position) - 1, "finished command"));
                tcsFinished.TrySetResult(Unit.Default);
                await tcsContinue.Task;
                return Unit.Default;
            },
            outputScheduler: ImmediateScheduler.Instance);

        Exception? exception = null;
        command.ThrownExceptions.Subscribe(ex => exception = ex);
        var latestIsExecutingValue = false;
        command.IsExecuting.Subscribe(isExec =>
        {
            statusTrail.Add((Interlocked.Increment(ref position) - 1, $"executing = {isExec}"));
            Volatile.Write(ref latestIsExecutingValue, isExec);
        });

        var result = false;
        command.Execute().Subscribe(_ => result = true);

        await tcsStarted.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();

        await tcsFinished.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();

        tcsContinue.TrySetResult(Unit.Default);
        await Task.Delay(100);

        using (Assert.Multiple())
        {
            await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsFalse();
            await Assert.That(result).IsTrue();
            await Assert.That(exception).IsNull();
            await Assert.That(statusTrail).IsEquivalentTo(
            [
                (0, "executing = False"),
                (1, "executing = True"),
                (2, "started command"),
                (3, "finished command"),
                (4, "executing = False")
            ]);
        }
    }

    [Test]
    public async Task Task_Exception_HandlesExceptionFlow()
    {
        var tcsStart = new TaskCompletionSource<Unit>();
        var command = ReactiveCommand.CreateFromTask(
            async _ =>
            {
                await tcsStart.Task;
                throw new Exception("Task exception");
            },
            outputScheduler: ImmediateScheduler.Instance);
        command.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting).Subscribe();
        command.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var exceptions)
            .Subscribe();

        command.Execute().Subscribe();

        await Task.Delay(100);
        tcsStart.SetResult(Unit.Default);
        await Task.Delay(100);

        using (Assert.Multiple())
        {
            await Assert.That(isExecuting[0]).IsFalse();
            await Assert.That(isExecuting[1]).IsTrue();
            await Assert.That(exceptions).Count().IsEqualTo(1);
            await Assert.That(exceptions[0].Message).IsEqualTo("Task exception");
        }
    }

    [Test]
    public async Task ThrownExceptions_CapturesLambdaExceptions()
    {
        var command = ReactiveCommand.CreateFromObservable<Unit>(
            () => throw new InvalidOperationException("Lambda error"),
            outputScheduler: ImmediateScheduler.Instance);
        command.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var exceptions)
            .Subscribe();

        command.Execute().Subscribe(_ => { }, _ => { });

        await Assert.That(exceptions).Count().IsEqualTo(1);
        await Assert.That(exceptions[0]).IsTypeOf<InvalidOperationException>();
        await Assert.That(exceptions[0].Message).IsEqualTo("Lambda error");
    }

    [Test]
    public async Task ThrownExceptions_CapturesObservableExceptions()
    {
        var command = ReactiveCommand.CreateFromObservable(
            () => Observable.Throw<Unit>(new InvalidOperationException("Test error")),
            outputScheduler: ImmediateScheduler.Instance);
        command.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var exceptions)
            .Subscribe();

        command.Execute().Subscribe(_ => { }, _ => { });

        await Assert.That(exceptions).Count().IsEqualTo(1);
        await Assert.That(exceptions[0]).IsTypeOf<InvalidOperationException>();
        await Assert.That(exceptions[0].Message).IsEqualTo("Test error");
    }

    [Test]
    public async Task ThrownExceptions_DeliveredOnOutputScheduler()
    {
        var scheduler = TestContext.Current!.GetScheduler();
        var command = ReactiveCommand.CreateFromObservable(
            () => Observable.Throw<Unit>(new InvalidOperationException()),
            outputScheduler: scheduler);
        Exception? exception = null;
        command.ThrownExceptions.Subscribe(ex => exception = ex);

        command.Execute().Subscribe(_ => { }, _ => { });

        await Assert.That(exception).IsTypeOf<InvalidOperationException>();
    }

    [Test]
    public async Task ThrownExceptions_PropagatesTaskExceptions()
    {
        var tcsStart = new TaskCompletionSource<Unit>();
        var command = ReactiveCommand.CreateFromTask(
            async _ =>
            {
                await tcsStart.Task;
                throw new Exception("Task exception");
            },
            outputScheduler: ImmediateScheduler.Instance);
        command.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var exceptions)
            .Subscribe();

        command.Execute().Subscribe();

        await Task.Delay(100);
        tcsStart.SetResult(Unit.Default);
        await Task.Delay(100);

        await Assert.That(exceptions).Count().IsEqualTo(1);
        await Assert.That(exceptions[0].Message).IsEqualTo("Task exception");
    }
}
