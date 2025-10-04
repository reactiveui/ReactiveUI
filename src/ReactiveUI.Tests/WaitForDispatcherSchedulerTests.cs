// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Tests the WaitForDispatcherSchedulerClass.
/// </summary>
[TestFixture]
public class WaitForDispatcherSchedulerTests
{
    /// <summary>
    /// Tests call scheduler factory on creation.
    /// </summary>
    [Test]
    public void CallSchedulerFactoryOnCreation()
    {
        var schedulerFactoryCalls = 0;
        var schedulerFactory = new Func<IScheduler>(
                                                    () =>
                                                    {
                                                        schedulerFactoryCalls++;
                                                        return null!;
                                                    });

        var sut = new WaitForDispatcherScheduler(schedulerFactory);

        Assert.That(schedulerFactoryCalls, Is.EqualTo(1));
    }

    /// <summary>
    /// Calls that factories throws argument null exception falls back to current thread.
    /// </summary>
    [Test]
    public void FactoryThrowsArgumentNullException_FallsBackToCurrentThread()
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

        Assert.That(schedulerExecutedOn, Is.EqualTo(CurrentThreadScheduler.Instance));
    }

    /// <summary>
    /// Tests that factories throws exception re calls on schedule.
    /// </summary>
    [Test]
    public void FactoryThrowsException_ReCallsOnSchedule()
    {
        var schedulerFactoryCalls = 0;
        var schedulerFactory = new Func<IScheduler>(
                                                    () =>
                                                    {
                                                        schedulerFactoryCalls++;
                                                        throw new InvalidOperationException();
                                                    });

        var sut = new WaitForDispatcherScheduler(schedulerFactory);
        sut.Schedule(() => { });

        Assert.That(schedulerFactoryCalls, Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that factories throws invalid operation exception falls back to current thread.
    /// </summary>
    [Test]
    public void FactoryThrowsInvalidOperationException_FallsBackToCurrentThread()
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

        Assert.That(schedulerExecutedOn, Is.EqualTo(CurrentThreadScheduler.Instance));
    }

    /// <summary>
    /// Tests that factory uses cached scheduler.
    /// </summary>
    [Test]
    public void SuccessfulFactory_UsesCachedScheduler()
    {
        var schedulerFactoryCalls = 0;
        var schedulerFactory = new Func<IScheduler>(
                                                    () =>
                                                    {
                                                        schedulerFactoryCalls++;
                                                        return CurrentThreadScheduler.Instance;
                                                    });

        var sut = new WaitForDispatcherScheduler(schedulerFactory);
        sut.Schedule(() => { });

        Assert.That(schedulerFactoryCalls, Is.EqualTo(1));
    }
}
