// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Mocks;
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
    /// <summary>Verifies that IsExecuting reports true while multiple executions are in flight.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task IsExecuting_HandlesMultipleInFlightExecutions()
    {
        const int DelayMilliseconds = 500;

        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var execute = SingleValueObservable.Void.Delay(TimeSpan.FromMilliseconds(DelayMilliseconds), scheduler);
        var command = ReactiveCommand.CreateFromObservable<RxVoid>(
            () => execute,
            outputScheduler: scheduler);
        var executed = command.Collect();

        _ = command.Execute().Subscribe();
        _ = command.Execute().Subscribe();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        await Assert.That(await command.IsExecuting.FirstAsync()).IsTrue();
        await Assert.That(executed).IsEmpty();
    }

    /// <summary>Verifies that IsExecuting behaves as a behavioral observable, immediately yielding its current value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsExecuting_IsBehavioral()
    {
        var command = ReactiveCommand.Create(
            () => { },
            outputScheduler: Sequencer.Immediate);
        var isExecuting = command.IsExecuting.Collect();

        using (Assert.Multiple())
        {
            await Assert.That(isExecuting).Count().IsEqualTo(1);
            await Assert.That(isExecuting[0]).IsFalse();
        }
    }

    /// <summary>Verifies that IsExecuting stays true until the execution observable completes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsExecuting_RemainsTrue_UntilExecutionCompletes()
    {
        var executeSubject = new Signal<RxVoid>();
        var command = ReactiveCommand.CreateFromObservable(
            () => executeSubject,
            outputScheduler: Sequencer.Immediate);

        command.Execute().Subscribe();

        await Assert.That(await command.IsExecuting.FirstAsync()).IsTrue();

        executeSubject.OnNext(RxVoid.Default);
        await Assert.That(await command.IsExecuting.FirstAsync()).IsTrue();

        executeSubject.OnCompleted();
        await Assert.That(await command.IsExecuting.FirstAsync()).IsFalse();
    }

    /// <summary>Verifies that IsExecuting ticks true once execution begins.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task IsExecuting_TicksWhileExecuting()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var execute = SingleValueObservable.Void.Delay(TimeSpan.FromSeconds(1), scheduler);
        var command = ReactiveCommand.CreateFromObservable<RxVoid>(
            () => execute,
            outputScheduler: scheduler);
        var isExecuting = command.IsExecuting.Collect();

        command.Execute().Subscribe();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        const int ExpectedCount = 2;

        using (Assert.Multiple())
        {
            await Assert.That(isExecuting).Count().IsEqualTo(ExpectedCount);
            await Assert.That(isExecuting[0]).IsFalse();
            await Assert.That(isExecuting[1]).IsTrue();
        }
    }

    /// <summary>Verifies that disposing a result subscription does not prevent the command from continuing to execute.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Observable_Subscription_ProperLifecycle()
    {
        var executed = 0;
        var command = ReactiveCommand.Create(
            () => ++executed,
            outputScheduler: Sequencer.Immediate);

        var subscription = command.Subscribe(_ => { });
        await command.Execute().FirstAsync();

        await Assert.That(executed).IsEqualTo(1);

        const int ExpectedSecondCount = 2;

        subscription.Dispose();
        await command.Execute().FirstAsync();

        // Should still execute even after subscription disposal
        await Assert.That(executed).IsEqualTo(ExpectedSecondCount);
    }

    /// <summary>Verifies that an async setpoint-driven view model produces the expected debounced values over virtual time.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task ReactiveSetpoint_AsyncMethodExecution()
    {
        const int InitialFooValue = 42;
        const int SetpointValue = 123;
        const int InitialAdvanceMilliseconds = 11;
        const int PartialAdvanceMilliseconds = 5;
        const int RemainingAdvanceMilliseconds = 6;

        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        RxSchedulers.TaskpoolScheduler = scheduler;

        var fooVm = new FooViewModel(new());

        await Assert.That(fooVm.Foo.Value).IsEqualTo(InitialFooValue);

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));
        await Assert.That(fooVm.Foo.Value).IsEqualTo(0);

        fooVm.Setpoint = SetpointValue;
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PartialAdvanceMilliseconds));
        await Assert.That(fooVm.Foo.Value).IsEqualTo(0);

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(RemainingAdvanceMilliseconds));
        await Assert.That(fooVm.Foo.Value).IsEqualTo(SetpointValue);
    }

    /// <summary>Verifies that a background command runs its work on the supplied background scheduler.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Scheduler_BackgroundCommandUsesBackgroundScheduler()
    {
        var backgroundScheduler = Sequencer.Immediate;
        var executed = false;
        var command = ReactiveCommand.CreateRunInBackground(
            () => executed = true,
            backgroundScheduler: backgroundScheduler,
            outputScheduler: Sequencer.Immediate);

        await command.Execute().FirstAsync();
        await Assert.That(executed).IsTrue();
    }

    /// <summary>Verifies that command results are delivered on the configured output scheduler.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Scheduler_ResultsDeliveredOnOutputScheduler()
    {
        var scheduler = TestContext.Current!.GetScheduler();
        var command = ReactiveCommand.CreateFromObservable(
            () => SingleValueObservable.Void,
            outputScheduler: scheduler);
        var executed = false;

        command.Execute().Subscribe(_ => executed = true);

        await Assert.That(executed).IsTrue();
    }

    /// <summary>Verifies the IsExecuting and exception sequence when a task command is cancelled mid-execution.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Task_Cancellation_HandlesProperCancellationFlow()
    {
        var tcsStarted = new TaskCompletionSource<RxVoid>();
        var tcsCaught = new TaskCompletionSource<RxVoid>();
        var tcsFinish = new TaskCompletionSource<RxVoid>();
        var statusTrail = new List<(int Position, string Status)>();
        var position = 0;

        const int LongDelayMilliseconds = 10_000;
        const int WaitTimeoutSeconds = 2;
        const int CompletionDelayMilliseconds = 100;

        var command = ReactiveCommand.CreateFromTask(
            async token =>
            {
                statusTrail.Add((Interlocked.Increment(ref position) - 1, StartedCommandStatus));
                tcsStarted.TrySetResult(RxVoid.Default);
                try
                {
                    await Task.Delay(LongDelayMilliseconds, token);
                }
                catch (OperationCanceledException)
                {
                    statusTrail.Add((Interlocked.Increment(ref position) - 1, CancellingCommandStatus));
                    tcsCaught.TrySetResult(RxVoid.Default);
                    await tcsFinish.Task;
                    statusTrail.Add((Interlocked.Increment(ref position) - 1, FinishedCancellingStatus));
                    throw;
                }

                return RxVoid.Default;
            },
            outputScheduler: Sequencer.Immediate);

        Exception? exception = null;
        command.ThrownExceptions.Subscribe(ex => exception = ex);
        var latestIsExecutingValue = false;
        command.IsExecuting.Subscribe(isExec =>
        {
            statusTrail.Add((Interlocked.Increment(ref position) - 1, $"executing = {isExec}"));
            Volatile.Write(ref latestIsExecutingValue, isExec);
        });

        var disposable = command.Execute().Subscribe();

        await tcsStarted.Task.WaitAsync(TimeSpan.FromSeconds(WaitTimeoutSeconds));
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();

        disposable.Dispose();

        await tcsCaught.Task.WaitAsync(TimeSpan.FromSeconds(WaitTimeoutSeconds));
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();

        tcsFinish.TrySetResult(RxVoid.Default);
        await Task.Delay(CompletionDelayMilliseconds);

        const int StartedCommandPosition = 2;
        const int CancellingCommandPosition = 3;
        const int FinishedCancellingPosition = 4;
        const int FinalExecutingPosition = 5;

        using (Assert.Multiple())
        {
            await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsFalse();
            await Assert.That(exception).IsTypeOf<TaskCanceledException>();
            await Assert.That(statusTrail).IsEquivalentTo([
                (0, ExecutingFalseStatus),
                (1, "executing = True"),
                (StartedCommandPosition, StartedCommandStatus),
                (CancellingCommandPosition, CancellingCommandStatus),
                (FinishedCancellingPosition, FinishedCancellingStatus),
                (FinalExecutingPosition, ExecutingFalseStatus)
            ]);
        }
    }

    /// <summary>Verifies the IsExecuting and result sequence when a task command runs to normal completion.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Task_Completion_HandlesProperCompletionFlow()
    {
        var tcsStarted = new TaskCompletionSource<RxVoid>();
        var tcsFinished = new TaskCompletionSource<RxVoid>();
        var tcsContinue = new TaskCompletionSource<RxVoid>();
        var statusTrail = new List<(int Position, string Status)>();
        var position = 0;

        const int DelayMilliseconds = 1_000;
        const int CancelDelayMilliseconds = 5_000;
        const int CompletionDelayMilliseconds = 100;
        const int WaitTimeoutSeconds = 2;

        var command = ReactiveCommand.CreateFromTask(
            async cts =>
            {
                statusTrail.Add((Interlocked.Increment(ref position) - 1, StartedCommandStatus));
                tcsStarted.TrySetResult(RxVoid.Default);
                try
                {
                    await Task.Delay(DelayMilliseconds, cts);
                }
                catch (OperationCanceledException)
                {
                    statusTrail.Add((Interlocked.Increment(ref position) - 1, CancellingCommandStatus));
                    await Task.Delay(CancelDelayMilliseconds, CancellationToken.None);
                    statusTrail.Add((Interlocked.Increment(ref position) - 1, FinishedCancellingStatus));
                    throw;
                }

                statusTrail.Add((Interlocked.Increment(ref position) - 1, "finished command"));
                tcsFinished.TrySetResult(RxVoid.Default);
                await tcsContinue.Task;
                return RxVoid.Default;
            },
            outputScheduler: Sequencer.Immediate);

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

        await tcsStarted.Task.WaitAsync(TimeSpan.FromSeconds(WaitTimeoutSeconds));
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();

        await tcsFinished.Task.WaitAsync(TimeSpan.FromSeconds(WaitTimeoutSeconds));
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();

        tcsContinue.TrySetResult(RxVoid.Default);
        await Task.Delay(CompletionDelayMilliseconds);

        const int StartedCommandPosition = 2;
        const int FinishedCommandPosition = 3;
        const int FinalExecutingPosition = 4;

        using (Assert.Multiple())
        {
            await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsFalse();
            await Assert.That(result).IsTrue();
            await Assert.That(exception).IsNull();
            await Assert.That(statusTrail).IsEquivalentTo([
                (0, ExecutingFalseStatus),
                (1, "executing = True"),
                (StartedCommandPosition, StartedCommandStatus),
                (FinishedCommandPosition, "finished command"),
                (FinalExecutingPosition, ExecutingFalseStatus)
            ]);
        }
    }

    /// <summary>Verifies the IsExecuting and ThrownExceptions sequence when a task command throws.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Task_Exception_HandlesExceptionFlow()
    {
        var tcsStart = new TaskCompletionSource<RxVoid>();
        var command = ReactiveCommand.CreateFromTask(
            async _ =>
            {
                await tcsStart.Task;
                throw new InvalidOperationException(TaskExceptionMessage);
            },
            outputScheduler: Sequencer.Immediate);
        var isExecuting = command.IsExecuting.Collect();
        var exceptions = command.ThrownExceptions.Collect();

        const int DelayMilliseconds = 100;

        command.Execute().Subscribe();

        await Task.Delay(DelayMilliseconds);
        tcsStart.SetResult(RxVoid.Default);
        await Task.Delay(DelayMilliseconds);

        using (Assert.Multiple())
        {
            await Assert.That(isExecuting[0]).IsFalse();
            await Assert.That(isExecuting[1]).IsTrue();
            await Assert.That(exceptions).Count().IsEqualTo(1);
            await Assert.That(exceptions[0].Message).IsEqualTo(TaskExceptionMessage);
        }
    }

    /// <summary>Verifies that exceptions thrown synchronously by the execute lambda are surfaced through ThrownExceptions.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ThrownExceptions_CapturesLambdaExceptions()
    {
        var command = ReactiveCommand.CreateFromObservable<RxVoid>(
            () => throw new InvalidOperationException("Lambda error"),
            outputScheduler: Sequencer.Immediate);
        var exceptions = command.ThrownExceptions.Collect();

        command.Execute().Subscribe(_ => { }, _ => { });

        await Assert.That(exceptions).Count().IsEqualTo(1);
        await Assert.That(exceptions[0]).IsTypeOf<InvalidOperationException>();
        await Assert.That(exceptions[0].Message).IsEqualTo("Lambda error");
    }

    /// <summary>Verifies that errors emitted by the execution observable are surfaced through ThrownExceptions.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ThrownExceptions_CapturesObservableExceptions()
    {
        var command = ReactiveCommand.CreateFromObservable(
            () => Signal.Fail<RxVoid>(new InvalidOperationException(TestErrorMessage)),
            outputScheduler: Sequencer.Immediate);
        var exceptions = command.ThrownExceptions.Collect();

        command.Execute().Subscribe(_ => { }, _ => { });

        await Assert.That(exceptions).Count().IsEqualTo(1);
        await Assert.That(exceptions[0]).IsTypeOf<InvalidOperationException>();
        await Assert.That(exceptions[0].Message).IsEqualTo(TestErrorMessage);
    }

    /// <summary>Verifies that thrown exceptions are delivered on the configured output scheduler.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ThrownExceptions_DeliveredOnOutputScheduler()
    {
        var scheduler = TestContext.Current!.GetScheduler();
        var command = ReactiveCommand.CreateFromObservable(
            () => Signal.Fail<RxVoid>(new InvalidOperationException()),
            outputScheduler: scheduler);
        Exception? exception = null;
        command.ThrownExceptions.Subscribe(ex => exception = ex);

        command.Execute().Subscribe(_ => { }, _ => { });

        await Assert.That(exception).IsTypeOf<InvalidOperationException>();
    }

    /// <summary>Verifies that exceptions thrown by a task command are propagated through ThrownExceptions.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ThrownExceptions_PropagatesTaskExceptions()
    {
        var tcsStart = new TaskCompletionSource<RxVoid>();
        var command = ReactiveCommand.CreateFromTask(
            async _ =>
            {
                await tcsStart.Task;
                throw new InvalidOperationException(TaskExceptionMessage);
            },
            outputScheduler: Sequencer.Immediate);
        var exceptions = command.ThrownExceptions.Collect();

        const int DelayMilliseconds = 100;

        command.Execute().Subscribe();

        await Task.Delay(DelayMilliseconds);
        tcsStart.SetResult(RxVoid.Default);
        await Task.Delay(DelayMilliseconds);

        await Assert.That(exceptions).Count().IsEqualTo(1);
        await Assert.That(exceptions[0].Message).IsEqualTo(TaskExceptionMessage);
    }
}
