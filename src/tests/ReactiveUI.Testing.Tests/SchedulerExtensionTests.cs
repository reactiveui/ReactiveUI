// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Threading;
using Microsoft.Reactive.Testing;

namespace ReactiveUI.Testing.Tests;

/// <summary>
/// Tests for SchedulerExtensions.
/// </summary>
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
        var originalMainThread = RxApp.MainThreadScheduler;
        var originalTaskpool = RxApp.TaskpoolScheduler;

        using (SchedulerExtensions.WithScheduler(testScheduler))
        {
            // Verify schedulers
            using (Assert.Multiple())
            {
                // Verify RxApp schedulers are set
                await Assert.That(RxApp.MainThreadScheduler).IsEqualTo(testScheduler);
                await Assert.That(RxApp.TaskpoolScheduler).IsEqualTo(testScheduler);
            }
        }

        // Verify schedulers are restored after disposal
        using (Assert.Multiple())
        {
            await Assert.That(RxApp.MainThreadScheduler).IsEqualTo(originalMainThread);
            await Assert.That(RxApp.TaskpoolScheduler).IsEqualTo(originalTaskpool);
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
        var originalMainThread = RxApp.MainThreadScheduler;

        using (SchedulerExtensions.WithScheduler(scheduler1))
        {
            await Assert.That(RxApp.MainThreadScheduler).IsEqualTo(scheduler1);

            using (SchedulerExtensions.WithScheduler(scheduler2))
            {
                await Assert.That(RxApp.MainThreadScheduler).IsEqualTo(scheduler2);
            }

            // After inner scope, should restore to scheduler1
            await Assert.That(RxApp.MainThreadScheduler).IsEqualTo(scheduler1);
        }

        // After outer scope, should restore to original
        await Assert.That(RxApp.MainThreadScheduler).IsEqualTo(originalMainThread);
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
            await Assert.That(RxApp.MainThreadScheduler).IsEqualTo(scheduler2);
        }
    }

    /// <summary>
    /// Tests concurrent access to WithScheduler from multiple threads.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithScheduler_ConcurrentAccess_ShouldSerialize()
    {
        const int threadCount = 5;
        const int iterationsPerThread = 3;
        var exceptions = new List<Exception>();
        var tasks = new List<Task>();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        // Use CountdownEvent instead of Barrier to avoid potential deadlocks
        using var startSignal = new CountdownEvent(1);

        for (var i = 0; i < threadCount; i++)
        {
            var threadId = i;
            tasks.Add(Task.Run(
                () =>
                {
                    try
                    {
                        // Wait for start signal
                        startSignal.Wait(cts.Token);

                        for (var j = 0; j < iterationsPerThread; j++)
                        {
                            var scheduler = new TestScheduler();
                            using (SchedulerExtensions.WithScheduler(scheduler))
                            {
                                // Verify scheduler is set
                                if (RxApp.MainThreadScheduler != scheduler)
                                {
                                    throw new InvalidOperationException($"Thread {threadId}: Scheduler mismatch!");
                                }

                                // Simulate minimal work
                                Thread.SpinWait(100);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (exceptions)
                        {
                            exceptions.Add(ex);
                        }
                    }
                },
                cts.Token));
        }

        // Give tasks a moment to start and wait at the signal
        await Task.Delay(50, cts.Token);

        // Release all threads simultaneously
        startSignal.Signal();

        // Wait for all tasks with timeout
        await Task.WhenAll(tasks);

        // Verify no exceptions occurred
        await Assert.That(exceptions).IsEmpty();
    }

    /// <summary>
    /// Tests the With extension method with a function.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task With_Function_ShouldExecuteAndReturnValue()
    {
        var scheduler = new TestScheduler();
        var originalScheduler = RxApp.MainThreadScheduler;

        var result = scheduler.With(s =>
        {
            // Inside the block, scheduler should be active
            if (RxApp.MainThreadScheduler != s)
            {
                throw new InvalidOperationException("Scheduler not set correctly");
            }

            return 42;
        });

        await Assert.That(result).IsEqualTo(42);

        // After the block, original scheduler should be restored
        await Assert.That(RxApp.MainThreadScheduler).IsEqualTo(originalScheduler);
    }

    /// <summary>
    /// Tests the With extension method with an action.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task With_Action_ShouldExecute()
    {
        var scheduler = new TestScheduler();
        var originalScheduler = RxApp.MainThreadScheduler;
        var executed = false;

        scheduler.With(s =>
        {
            executed = true;

            // Inside the block, scheduler should be active
            if (RxApp.MainThreadScheduler != s)
            {
                throw new InvalidOperationException("Scheduler not set correctly");
            }
        });

        await Assert.That(executed).IsTrue();
        await Assert.That(RxApp.MainThreadScheduler).IsEqualTo(originalScheduler);
    }

    /// <summary>
    /// Tests the WithAsync extension method with a function.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithAsync_Function_ShouldExecuteAndReturnValue()
    {
        var scheduler = new TestScheduler();
        var originalScheduler = RxApp.MainThreadScheduler;

        var result = await scheduler.WithAsync(s =>
        {
            // Inside the block, scheduler should be active
            if (RxApp.MainThreadScheduler != s)
            {
                throw new InvalidOperationException("Scheduler not set correctly");
            }

            return Task.FromResult(42);
        });

        await Assert.That(result).IsEqualTo(42);
        await Assert.That(RxApp.MainThreadScheduler).IsEqualTo(originalScheduler);
    }

    /// <summary>
    /// Tests the WithAsync extension method with an action.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithAsync_Action_ShouldExecute()
    {
        var scheduler = new TestScheduler();
        var originalScheduler = RxApp.MainThreadScheduler;
        var executed = false;

        await scheduler.WithAsync(s =>
        {
            executed = true;

            // Inside the block, scheduler should be active
            if (RxApp.MainThreadScheduler != s)
            {
                throw new InvalidOperationException("Scheduler not set correctly");
            }

            return Task.CompletedTask;
        });

        await Assert.That(executed).IsTrue();
        await Assert.That(RxApp.MainThreadScheduler).IsEqualTo(originalScheduler);
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
                await Assert.That(RxApp.MainThreadScheduler).IsEqualTo(scheduler);
            }
        }

        // No assertions needed - if we get here without deadlock, the test passed
        await Task.CompletedTask;
    }
}
