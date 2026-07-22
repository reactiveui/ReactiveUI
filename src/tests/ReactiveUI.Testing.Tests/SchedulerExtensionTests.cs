// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;
using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Testing.Tests;

/// <summary>Tests for SchedulerExtensions.</summary>
[NotInParallel]
[TestExecutor<AppBuilderTestExecutor>]
public sealed class SchedulerExtensionTests
{
    /// <summary>The message used when the scheduler is not set as expected.</summary>
    private const string SchedulerNotSetMessage = "Scheduler not set correctly";

    /// <summary>The value used in scheduler notification assertions.</summary>
    private const int ExpectedValue = 42;

    /// <summary>Tests that WithScheduler sets both RxApp and RxSchedulers schedulers.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithScheduler_ShouldSetBothRxAppAndRxSchedulersSchedulers()
    {
        var testScheduler = new VirtualTimeScheduler();
        var originalMainThread = RxSchedulers.MainThreadScheduler;
        var originalTaskpool = RxSchedulers.TaskpoolScheduler;

        using (SchedulerExtensions.WithScheduler(testScheduler))
        {
            // Verify schedulers
            using (Assert.Multiple())
            {
                // Verify RxApp schedulers are set
                await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(testScheduler);
                await Assert.That(RxSchedulers.TaskpoolScheduler).IsSameReferenceAs(testScheduler);
            }
        }

        // Verify schedulers are restored after disposal
        using (Assert.Multiple())
        {
            await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(originalMainThread);
            await Assert.That(RxSchedulers.TaskpoolScheduler).IsSameReferenceAs(originalTaskpool);
        }
    }

    /// <summary>Tests that nested WithScheduler calls work correctly (sequential access).</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithScheduler_NestedCalls_ShouldWorkSequentially()
    {
        var scheduler1 = new VirtualTimeScheduler();
        var scheduler2 = new VirtualTimeScheduler();
        var originalMainThread = RxSchedulers.MainThreadScheduler;

        using (SchedulerExtensions.WithScheduler(scheduler1))
        {
            await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(scheduler1);

            using (SchedulerExtensions.WithScheduler(scheduler2))
            {
                await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(scheduler2);
            }

            // After inner scope, should restore to scheduler1
            await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(scheduler1);
        }

        // After outer scope, should restore to original
        await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(originalMainThread);
    }

    /// <summary>Tests that WithScheduler properly releases the gate even when an exception is thrown.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithScheduler_ExceptionInCriticalSection_ShouldStillReleaseGate()
    {
        var scheduler1 = new VirtualTimeScheduler();
        var scheduler2 = new VirtualTimeScheduler();

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
            await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(scheduler2);
        }
    }

    /// <summary>Tests the With extension method with a function.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task With_Function_ShouldExecuteAndReturnValue()
    {
        var scheduler = new VirtualTimeScheduler();
        var originalScheduler = RxSchedulers.MainThreadScheduler;

        var result = scheduler.With(static s =>
        {
            // Inside the block, scheduler should be active
            if (RxSchedulers.MainThreadScheduler != s)
            {
                throw new InvalidOperationException(SchedulerNotSetMessage);
            }

            return ExpectedValue;
        });

        await Assert.That(result).IsEqualTo(ExpectedValue);

        // After the block, original scheduler should be restored
        await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(originalScheduler);
    }

    /// <summary>Tests the With extension method with an action.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task With_Action_ShouldExecute()
    {
        var scheduler = new VirtualTimeScheduler();
        var originalScheduler = RxSchedulers.MainThreadScheduler;
        var executed = false;

        scheduler.With(s =>
        {
            executed = true;

            // Inside the block, scheduler should be active
            if (RxSchedulers.MainThreadScheduler == s)
            {
                return;
            }

            throw new InvalidOperationException(SchedulerNotSetMessage);
        });

        await Assert.That(executed).IsTrue();
        await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(originalScheduler);
    }

    /// <summary>Tests the WithAsync extension method with a function.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithAsync_Function_ShouldExecuteAndReturnValue()
    {
        var scheduler = new VirtualTimeScheduler();
        var originalScheduler = RxSchedulers.MainThreadScheduler;

        var result = await scheduler.WithAsync(static s =>
        {
            // Inside the block, scheduler should be active
            if (RxSchedulers.MainThreadScheduler != s)
            {
                throw new InvalidOperationException(SchedulerNotSetMessage);
            }

            return Task.FromResult(ExpectedValue);
        });

        await Assert.That(result).IsEqualTo(ExpectedValue);
        await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(originalScheduler);
    }

    /// <summary>Tests the WithAsync extension method with an action.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithAsync_Action_ShouldExecute()
    {
        var scheduler = new VirtualTimeScheduler();
        var originalScheduler = RxSchedulers.MainThreadScheduler;
        var executed = false;

        await scheduler.WithAsync(s =>
        {
            executed = true;

            // Inside the block, scheduler should be active
            if (RxSchedulers.MainThreadScheduler != s)
            {
                throw new InvalidOperationException(SchedulerNotSetMessage);
            }

            return Task.CompletedTask;
        });

        await Assert.That(executed).IsTrue();
        await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(originalScheduler);
    }

    /// <summary>Tests that rapid sequential calls work correctly (stress test).</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithScheduler_RapidSequentialCalls_ShouldWork()
    {
        const int iterations = 100;

        for (var i = 0; i < iterations; i++)
        {
            var scheduler = new VirtualTimeScheduler();
            using (SchedulerExtensions.WithScheduler(scheduler))
            {
                await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(scheduler);
            }
        }

        // No assertions needed - if we get here without deadlock, the test passed
        await Task.CompletedTask;
    }
}
