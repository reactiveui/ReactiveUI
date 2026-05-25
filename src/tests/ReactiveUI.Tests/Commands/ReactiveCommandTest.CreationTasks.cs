// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;

namespace ReactiveUI.Tests.Commands;

/// <content>
///     Tests for the CreateFromTask and CreateRunInBackground factory methods, including
///     cancellation-token behavior and parameter passing.
/// </content>
public partial class ReactiveCommandTest
{
    /// <summary>
    ///     Verifies that disposing an in-flight cancellable task command cancels its execution.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_Cancellable_ProperlyCancelsExecution()
    {
        var tcsStarted = new TaskCompletionSource<Unit>();
        var tcsCaught = new TaskCompletionSource<Unit>();
        var tcsFinish = new TaskCompletionSource<Unit>();

        const int LongDelayMilliseconds = 10000;
        const int WaitTimeoutSeconds = 2;
        const int CompletionDelayMilliseconds = 100;

        var fixture = ReactiveCommand.CreateFromTask(
            async token =>
            {
                tcsStarted.TrySetResult(Unit.Default);
                try
                {
                    await Task.Delay(LongDelayMilliseconds, token);
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

        await tcsStarted.Task.WaitAsync(TimeSpan.FromSeconds(WaitTimeoutSeconds));
        disposable.Dispose();

        await tcsCaught.Task.WaitAsync(TimeSpan.FromSeconds(WaitTimeoutSeconds));
        tcsFinish.TrySetResult(Unit.Default);

        // Wait for cancellation to complete
        await Task.Delay(CompletionDelayMilliseconds);
    }

    /// <summary>
    ///     Verifies that a cancellable Unit task command without a parameter receives a cancellation token.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_Cancellable_Unit_WithoutParam_ReceivesCancellationToken()
    {
        const int DelayMilliseconds = 10;

        CancellationToken? receivedToken = null;
        var command = ReactiveCommand.CreateFromTask(
            async token =>
            {
                receivedToken = token;
                await Task.Delay(DelayMilliseconds, token);
            },
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute();
        await Assert.That(receivedToken).IsNotNull();
    }

    /// <summary>
    ///     Verifies that creating a cancellable Unit task command without a parameter from a null execute argument throws.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_Cancellable_Unit_WithoutParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateFromTask((Func<CancellationToken, Task>)null!);
            await Task.CompletedTask;
        });

    /// <summary>
    ///     Verifies that a parameterized cancellable Unit task command receives both the parameter and a token.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_Cancellable_Unit_WithParam_ReceivesParameterAndToken()
    {
        const int DelayMilliseconds = 10;

        var receivedParam = 0;
        CancellationToken? receivedToken = null;
        var command = ReactiveCommand.CreateFromTask<int>(
            async (param, token) =>
            {
                receivedParam = param;
                receivedToken = token;
                await Task.Delay(DelayMilliseconds, token);
            },
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute(ParameterValue);

        using (Assert.Multiple())
        {
            await Assert.That(receivedParam).IsEqualTo(ParameterValue);
            await Assert.That(receivedToken).IsNotNull();
        }
    }

    /// <summary>
    ///     Verifies that creating a parameterized cancellable Unit task command from a null execute argument throws.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_Cancellable_Unit_WithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateFromTask((Func<int, CancellationToken, Task>)null!);
            await Task.CompletedTask;
        });

    /// <summary>
    ///     Verifies that a cancellable result-returning task command without a parameter receives a cancellation token.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_Cancellable_WithoutParam_ReceivesCancellationToken()
    {
        const int DelayMilliseconds = 10;

        CancellationToken? receivedToken = null;
        var command = ReactiveCommand.CreateFromTask(
            async token =>
            {
                receivedToken = token;
                await Task.Delay(DelayMilliseconds, token);
                return ParameterValue;
            },
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute();
        await Assert.That(receivedToken).IsNotNull();
    }

    /// <summary>
    ///     Verifies that creating a cancellable result-returning task command without a parameter from a null execute
    ///     argument throws.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_Cancellable_WithoutParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateFromTask((Func<CancellationToken, Task<int>>)null!);
            await Task.CompletedTask;
        });

    /// <summary>
    ///     Verifies that a parameterized cancellable result-returning task command receives both the parameter and a token.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_Cancellable_WithParam_ReceivesParameterAndToken()
    {
        const int DelayMilliseconds = 10;

        var receivedParam = 0;
        CancellationToken? receivedToken = null;
        var command = ReactiveCommand.CreateFromTask<int, string>(
            async (param, token) =>
            {
                receivedParam = param;
                receivedToken = token;
                await Task.Delay(DelayMilliseconds, token);
                return param.ToString();
            },
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute(ParameterValue);

        using (Assert.Multiple())
        {
            await Assert.That(receivedParam).IsEqualTo(ParameterValue);
            await Assert.That(receivedToken).IsNotNull();
            await Assert.That(results[0]).IsEqualTo(ParameterValueString);
        }
    }

    /// <summary>
    ///     Verifies that creating a parameterized cancellable result-returning task command from a null execute argument
    ///     throws.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_Cancellable_WithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateFromTask((Func<int, CancellationToken, Task<string>>)null!);
            await Task.CompletedTask;
        });

    /// <summary>
    ///     Verifies that a Unit task command without a parameter completes its task successfully.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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

    /// <summary>
    ///     Verifies that creating a Unit task command without a parameter from a null execute argument throws.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_Unit_WithoutParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateFromTask((Func<Task>)null!);
            await Task.CompletedTask;
        });

    /// <summary>
    ///     Verifies that a parameterized Unit task command passes the parameter to its task.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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

        await command.Execute(ParameterValue);
        await Assert.That(receivedParam).IsEqualTo(ParameterValue);
    }

    /// <summary>
    ///     Verifies that creating a parameterized Unit task command from a null execute argument throws.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_Unit_WithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateFromTask((Func<int, Task>)null!);
            await Task.CompletedTask;
        });

    /// <summary>
    ///     Verifies that a task command without a parameter ticks the result returned by its task.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_WithoutParam_ReturnsTaskResult()
    {
        var command = ReactiveCommand.CreateFromTask(
            () => Task.FromResult(ParameterValue),
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute();
        await Assert.That(results[0]).IsEqualTo(ParameterValue);
    }

    /// <summary>
    ///     Verifies that creating a task command without a parameter from a null execute argument throws.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_WithoutParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateFromTask((Func<Task<int>>)null!);
            await Task.CompletedTask;
        });

    /// <summary>
    ///     Verifies that a parameterized task command passes the parameter to its task and ticks the result.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_WithParam_PassesParameterToTask()
    {
        var command = ReactiveCommand.CreateFromTask<int, string>(
            param => Task.FromResult(param.ToString()),
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute(ParameterValue);
        await Assert.That(results[0]).IsEqualTo(ParameterValueString);
    }

    /// <summary>
    ///     Verifies that creating a parameterized task command from a null execute argument throws.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateFromTask_WithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateFromTask((Func<int, Task<string>>)null!);
            await Task.CompletedTask;
        });

    /// <summary>
    ///     Verifies that a background action command executes on the supplied background scheduler.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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

    /// <summary>
    ///     Verifies that creating a background action command from a null execute argument throws.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_Action_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateRunInBackground(null!);
            await Task.CompletedTask;
        });

    /// <summary>
    ///     Verifies that a parameterized background action command receives the supplied parameter.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_ActionWithParam_PassesParameter()
    {
        var receivedParam = 0;
        var command = ReactiveCommand.CreateRunInBackground<int>(
            param => receivedParam = param,
            backgroundScheduler: ImmediateScheduler.Instance,
            outputScheduler: ImmediateScheduler.Instance);

        await command.Execute(ParameterValue);
        await Assert.That(receivedParam).IsEqualTo(ParameterValue);
    }

    /// <summary>
    ///     Verifies that creating a parameterized background action command from a null execute argument throws.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_ActionWithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateRunInBackground((Action<int>)null!);
            await Task.CompletedTask;
        });

    /// <summary>
    ///     Verifies that a background function command ticks its return value as a result.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_Func_ReturnsResult()
    {
        var command = ReactiveCommand.CreateRunInBackground(
            () => ParameterValue,
            backgroundScheduler: ImmediateScheduler.Instance,
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute();

        await Assert.That(results[0]).IsEqualTo(ParameterValue);
    }

    /// <summary>
    ///     Verifies that creating a background function command from a null execute argument throws.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_Func_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateRunInBackground((Func<int>)null!);
            await Task.CompletedTask;
        });

    /// <summary>
    ///     Verifies that creating a parameterized background function command from a null execute argument throws.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_FuncWithParam_ThrowsOnNullExecute() =>
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = ReactiveCommand.CreateRunInBackground((Func<int, string>)null!);
            await Task.CompletedTask;
        });

    /// <summary>
    ///     Verifies that a parameterized background function command transforms the parameter into a result.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackground_FuncWithParam_TransformsParameter()
    {
        var command = ReactiveCommand.CreateRunInBackground<int, string>(
            param => param.ToString(),
            backgroundScheduler: ImmediateScheduler.Instance,
            outputScheduler: ImmediateScheduler.Instance);
        command.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        await command.Execute(ParameterValue);
        await Assert.That(results[0]).IsEqualTo(ParameterValueString);
    }
}
