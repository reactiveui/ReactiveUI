// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

public class WaitForDispatcherSchedulerTests
{
    /// <summary>
    ///     Tests call scheduler factory on creation.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CallSchedulerFactoryOnCreation()
    {
        var schedulerFactoryCalls = 0;
        var schedulerFactory = new Func<IScheduler>(() =>
        {
            schedulerFactoryCalls++;
            return null!;
        });

        var sut = new WaitForDispatcherScheduler(schedulerFactory);

        await Assert.That(schedulerFactoryCalls).IsEqualTo(1);
    }

    /// <summary>
    ///     Calls that factories throws argument null exception falls back to current thread.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task FactoryThrowsArgumentNullException_FallsBackToCurrentThread()
    {
        IScheduler? schedulerExecutedOn = null;
        var schedulerFactory = new Func<IScheduler>(() => throw new ArgumentNullException());
        var sut = new WaitForDispatcherScheduler(schedulerFactory);
        sut.Schedule<object>(
            null!,
            (scheduler, state) =>
            {
                schedulerExecutedOn = scheduler;
                return Disposable.Empty;
            });

        await Assert.That(schedulerExecutedOn).IsEqualTo(CurrentThreadScheduler.Instance);
    }

    /// <summary>
    ///     Tests that factories throws exception re calls on schedule.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task FactoryThrowsException_ReCallsOnSchedule()
    {
        var schedulerFactoryCalls = 0;
        var schedulerFactory = new Func<IScheduler>(() =>
        {
            schedulerFactoryCalls++;
            throw new InvalidOperationException();
        });

        var sut = new WaitForDispatcherScheduler(schedulerFactory);
        sut.Schedule(() => { });

        await Assert.That(schedulerFactoryCalls).IsEqualTo(2);
    }

    /// <summary>
    ///     Tests that factories throws invalid operation exception falls back to current thread.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task FactoryThrowsInvalidOperationException_FallsBackToCurrentThread()
    {
        IScheduler schedulerExecutedOn = null!;
        var schedulerFactory = new Func<IScheduler>(() => throw new InvalidOperationException());

        var sut = new WaitForDispatcherScheduler(schedulerFactory);
        sut.Schedule<object>(
            null!,
            (scheduler, state) =>
            {
                schedulerExecutedOn = scheduler;
                return Disposable.Empty;
            });

        await Assert.That(schedulerExecutedOn).IsEqualTo(CurrentThreadScheduler.Instance);
    }

    /// <summary>
    ///     Tests that factory uses cached scheduler.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SuccessfulFactory_UsesCachedScheduler()
    {
        var schedulerFactoryCalls = 0;
        var schedulerFactory = new Func<IScheduler>(() =>
        {
            schedulerFactoryCalls++;
            return CurrentThreadScheduler.Instance;
        });

        var sut = new WaitForDispatcherScheduler(schedulerFactory);
        sut.Schedule(() => { });

        await Assert.That(schedulerFactoryCalls).IsEqualTo(1);
    }
}
