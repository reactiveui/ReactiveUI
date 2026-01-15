// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Threading;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing.Reactive;

using TUnit.Core.Executors;

namespace ReactiveUI.Testing.Tests;

/// <summary>
/// Tests for SchedulerExtensions.
/// </summary>
[NotInParallel]
[TestExecutor<AppBuilderTestExecutor>]
public sealed class SchedulerExtensionTests
{
    /// <summary>
    /// Tests that WithScheduler sets both RxApp and RxSchedulers schedulers.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithScheduler_ShouldSetBothRxAppAndRxSchedulersSchedulers()
    {
        var testScheduler = new TestScheduler();
        var originalMainThread = RxSchedulers.MainThreadScheduler;
        var originalTaskpool = RxSchedulers.TaskpoolScheduler;

        using (SchedulerExtensions.WithScheduler(testScheduler))
        {
            // Verify schedulers
            using (Assert.Multiple())
            {
                // Verify RxApp schedulers are set
                await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(testScheduler);
                await Assert.That(RxSchedulers.TaskpoolScheduler).IsEqualTo(testScheduler);
            }
        }

        // Verify schedulers are restored after disposal
        using (Assert.Multiple())
        {
            await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(originalMainThread);
            await Assert.That(RxSchedulers.TaskpoolScheduler).IsEqualTo(originalTaskpool);
        }
    }

    /// <summary>
    /// Tests that nested WithScheduler calls work correctly (sequential access).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithScheduler_NestedCalls_ShouldWorkSequentially()
    {
        var scheduler1 = new TestScheduler();
        var scheduler2 = new TestScheduler();
        var originalMainThread = RxSchedulers.MainThreadScheduler;

        using (SchedulerExtensions.WithScheduler(scheduler1))
        {
            await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(scheduler1);

            using (SchedulerExtensions.WithScheduler(scheduler2))
            {
                await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(scheduler2);
            }

            // After inner scope, should restore to scheduler1
            await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(scheduler1);
        }

        // After outer scope, should restore to original
        await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(originalMainThread);
    }

    /// <summary>
    /// Tests that WithScheduler properly releases the gate even when an exception is thrown.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithScheduler_ExceptionInCriticalSection_ShouldStillReleaseGate()
    {
        var scheduler1 = new TestScheduler();
        var scheduler2 = new TestScheduler();

        // First call throws an exception
        try
        {
            using (SchedulerExtensions.WithScheduler(scheduler1))
            {
                throw new InvalidOperationException("Test exception");
            }
        }
        catch (InvalidOperationException)
        {
            // Expected
        }

        // Second call should succeed (gate was released despite exception)
        using (SchedulerExtensions.WithScheduler(scheduler2))
        {
            await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(scheduler2);
        }
    }

    /// <summary>
    /// Tests the With extension method with a function.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task With_Function_ShouldExecuteAndReturnValue()
    {
        var scheduler = new TestScheduler();
        var originalScheduler = RxSchedulers.MainThreadScheduler;

        var result = scheduler.With(s =>
        {
            // Inside the block, scheduler should be active
            if (RxSchedulers.MainThreadScheduler != s)
            {
                throw new InvalidOperationException("Scheduler not set correctly");
            }

            return 42;
        });

        await Assert.That(result).IsEqualTo(42);

        // After the block, original scheduler should be restored
        await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(originalScheduler);
    }

    /// <summary>
    /// Tests the With extension method with an action.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task With_Action_ShouldExecute()
    {
        var scheduler = new TestScheduler();
        var originalScheduler = RxSchedulers.MainThreadScheduler;
        var executed = false;

        scheduler.With(s =>
        {
            executed = true;

            // Inside the block, scheduler should be active
            if (RxSchedulers.MainThreadScheduler != s)
            {
                throw new InvalidOperationException("Scheduler not set correctly");
            }
        });

        await Assert.That(executed).IsTrue();
        await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(originalScheduler);
    }

    /// <summary>
    /// Tests the WithAsync extension method with a function.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithAsync_Function_ShouldExecuteAndReturnValue()
    {
        var scheduler = new TestScheduler();
        var originalScheduler = RxSchedulers.MainThreadScheduler;

        var result = await scheduler.WithAsync(s =>
        {
            // Inside the block, scheduler should be active
            if (RxSchedulers.MainThreadScheduler != s)
            {
                throw new InvalidOperationException("Scheduler not set correctly");
            }

            return Task.FromResult(42);
        });

        await Assert.That(result).IsEqualTo(42);
        await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(originalScheduler);
    }

    /// <summary>
    /// Tests the WithAsync extension method with an action.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithAsync_Action_ShouldExecute()
    {
        var scheduler = new TestScheduler();
        var originalScheduler = RxSchedulers.MainThreadScheduler;
        var executed = false;

        await scheduler.WithAsync(s =>
        {
            executed = true;

            // Inside the block, scheduler should be active
            if (RxSchedulers.MainThreadScheduler != s)
            {
                throw new InvalidOperationException("Scheduler not set correctly");
            }

            return Task.CompletedTask;
        });

        await Assert.That(executed).IsTrue();
        await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(originalScheduler);
    }

    /// <summary>
    /// Tests that rapid sequential calls work correctly (stress test).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithScheduler_RapidSequentialCalls_ShouldWork()
    {
        const int iterations = 100;

        for (var i = 0; i < iterations; i++)
        {
            var scheduler = new TestScheduler();
            using (SchedulerExtensions.WithScheduler(scheduler))
            {
                await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(scheduler);
            }
        }

        // No assertions needed - if we get here without deadlock, the test passed
        await Task.CompletedTask;
    }

    /// <summary>
    /// Tests that AdvanceToMs advances the scheduler to the specified time.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AdvanceToMs_ShouldAdvanceToSpecifiedTime()
    {
        var scheduler = new TestScheduler();
        scheduler.AdvanceToMs(1000);

        var expectedTicks = TimeSpan.FromMilliseconds(1000).Ticks;
        await Assert.That(scheduler.Clock).IsEqualTo(expectedTicks);
    }

    /// <summary>
    /// Tests that AdvanceByMs advances the scheduler by the specified time.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AdvanceByMs_ShouldAdvanceBySpecifiedTime()
    {
        var scheduler = new TestScheduler();
        var initialTime = scheduler.Clock;

        scheduler.AdvanceByMs(500);

        var expectedTime = initialTime + TimeSpan.FromMilliseconds(500).Ticks;
        await Assert.That(scheduler.Clock).IsEqualTo(expectedTime);
    }

    /// <summary>
    /// Tests that OnNextAt creates a notification at the specified time.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnNextAt_ShouldCreateNotificationAtSpecifiedTime()
    {
        var scheduler = new TestScheduler();
        var recorded = scheduler.OnNextAt(100, 42);

        var expectedTime = scheduler.FromTimeSpan(TimeSpan.FromMilliseconds(100));

        using (Assert.Multiple())
        {
            await Assert.That(recorded.Time).IsEqualTo(expectedTime);
            await Assert.That(recorded.Value.Kind).IsEqualTo(NotificationKind.OnNext);
            await Assert.That(recorded.Value.Value).IsEqualTo(42);
        }
    }

    /// <summary>
    /// Tests that OnErrorAt creates an error notification at the specified time.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnErrorAt_ShouldCreateErrorNotificationAtSpecifiedTime()
    {
        var scheduler = new TestScheduler();
        var exception = new InvalidOperationException("Test error");
        var recorded = scheduler.OnErrorAt<int>(200, exception);

        var expectedTime = scheduler.FromTimeSpan(TimeSpan.FromMilliseconds(200));

        using (Assert.Multiple())
        {
            await Assert.That(recorded.Time).IsEqualTo(expectedTime);
            await Assert.That(recorded.Value.Kind).IsEqualTo(NotificationKind.OnError);
            await Assert.That(recorded.Value.Exception).IsEqualTo(exception);
        }
    }

    /// <summary>
    /// Tests that OnCompletedAt creates a completion notification at the specified time.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnCompletedAt_ShouldCreateCompletionNotificationAtSpecifiedTime()
    {
        var scheduler = new TestScheduler();
        var recorded = scheduler.OnCompletedAt<int>(300);

        var expectedTime = scheduler.FromTimeSpan(TimeSpan.FromMilliseconds(300));

        using (Assert.Multiple())
        {
            await Assert.That(recorded.Time).IsEqualTo(expectedTime);
            await Assert.That(recorded.Value.Kind).IsEqualTo(NotificationKind.OnCompleted);
        }
    }

    /// <summary>
    /// Tests that FromTimeSpan converts TimeSpan to ticks correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task FromTimeSpan_ShouldConvertToTicks()
    {
        var scheduler = new TestScheduler();
        var timeSpan = TimeSpan.FromMilliseconds(250);
        var ticks = scheduler.FromTimeSpan(timeSpan);

        await Assert.That(ticks).IsEqualTo(timeSpan.Ticks);
    }
}
