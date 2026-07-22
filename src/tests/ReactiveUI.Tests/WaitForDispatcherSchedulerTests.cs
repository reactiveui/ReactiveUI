// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>Tests for <see cref="WaitForDispatcherScheduler" />.</summary>
public class WaitForDispatcherSchedulerTests
{
    /// <summary>Tests call scheduler factory on creation.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CallSchedulerFactoryOnCreation()
    {
        var schedulerFactoryCalls = 0;
        var schedulerFactory = new Func<ISequencer>(() =>
        {
            schedulerFactoryCalls++;
            return null!;
        });

        _ = new WaitForDispatcherScheduler(schedulerFactory);

        await Assert.That(schedulerFactoryCalls).IsEqualTo(1);
    }

    /// <summary>Calls that factories throws argument null exception falls back to current thread.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task FactoryThrowsArgumentNullException_FallsBackToCurrentThread()
    {
        ISequencer? schedulerExecutedOn = null;
        var schedulerFactory = new Func<ISequencer>(static () => throw new ArgumentNullException());
        var sut = new WaitForDispatcherScheduler(schedulerFactory);
        _ = sut.Schedule<object>(
            null!,
            (scheduler, _) =>
            {
                schedulerExecutedOn = scheduler;
                return Scope.Empty;
            });

        await Assert.That(schedulerExecutedOn).IsEqualTo(Sequencer.CurrentThread);
    }

    /// <summary>Tests that factory throws exception re calls on schedule.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task FactoryThrowsException_ReCallsOnSchedule()
    {
        var schedulerFactoryCalls = 0;
        var schedulerFactory = new Func<ISequencer>(() =>
        {
            schedulerFactoryCalls++;
            throw new InvalidOperationException();
        });

        var sut = new WaitForDispatcherScheduler(schedulerFactory);
        _ = sut.Schedule(static () => { });

        const int ExpectedFactoryCalls = 2;
        await Assert.That(schedulerFactoryCalls).IsEqualTo(ExpectedFactoryCalls);
    }

    /// <summary>
    ///     Tests that when the factory first throws but later succeeds, the scheduler is cached and reused.
    ///     This covers the regression where WpfMainThreadScheduler falls back to CurrentThreadScheduler when
    ///     Schedule is called from a non-UI thread before the WPF Application Dispatcher is available.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task FactoryThrowsThenSucceeds_CachesSuccessfulScheduler()
    {
        var schedulerFactoryCalls = 0;
        var successScheduler = Sequencer.CurrentThread;
        var schedulerFactory = new Func<ISequencer>(() =>
        {
            schedulerFactoryCalls++;
            if (schedulerFactoryCalls == 1)
            {
                throw new InvalidOperationException("Dispatcher not ready yet.");
            }

            return successScheduler;
        });

        var sut = new WaitForDispatcherScheduler(schedulerFactory);

        // First Schedule call — factory throws, falls back to CurrentThreadScheduler (not cached)
        ISequencer? firstCallScheduler = null;
        _ = sut.Schedule<object>(
            null!,
            (scheduler, _) =>
            {
                firstCallScheduler = scheduler;
                return Scope.Empty;
            });

        // Second Schedule call — factory succeeds, result is cached
        ISequencer? secondCallScheduler = null;
        _ = sut.Schedule<object>(
            null!,
            (scheduler, _) =>
            {
                secondCallScheduler = scheduler;
                return Scope.Empty;
            });

        // Third Schedule call — uses cached scheduler, factory not called again
        var callsBeforeThird = schedulerFactoryCalls;
        _ = sut.Schedule<object>(
            null!,
            static (_, _) => Scope.Empty);

        await Assert.That(firstCallScheduler).IsEqualTo(Sequencer.CurrentThread);
        await Assert.That(secondCallScheduler).IsEqualTo(successScheduler);
        await Assert.That(schedulerFactoryCalls).IsEqualTo(callsBeforeThird);
    }

    /// <summary>Tests that factories throws invalid operation exception falls back to current thread.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task FactoryThrowsInvalidOperationException_FallsBackToCurrentThread()
    {
        ISequencer schedulerExecutedOn = null!;
        var schedulerFactory = new Func<ISequencer>(static () => throw new InvalidOperationException());

        var sut = new WaitForDispatcherScheduler(schedulerFactory);
        _ = sut.Schedule<object>(
            null!,
            (scheduler, _) =>
            {
                schedulerExecutedOn = scheduler;
                return Scope.Empty;
            });

        await Assert.That(schedulerExecutedOn).IsEqualTo(Sequencer.CurrentThread);
    }

    /// <summary>Tests that factory uses cached scheduler.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SuccessfulFactory_UsesCachedScheduler()
    {
        var schedulerFactoryCalls = 0;
        var schedulerFactory = new Func<ISequencer>(() =>
        {
            schedulerFactoryCalls++;
            return Sequencer.CurrentThread;
        });

        var sut = new WaitForDispatcherScheduler(schedulerFactory);
        _ = sut.Schedule(static () => { });

        await Assert.That(schedulerFactoryCalls).IsEqualTo(1);
    }
}
