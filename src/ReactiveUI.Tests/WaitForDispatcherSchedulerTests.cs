// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Tests the WaitForDispatcherSchedulerClass.
/// </summary>
public class WaitForDispatcherSchedulerTests
{
    /// <summary>
    /// Tests call scheduler factory on creation.
    /// </summary>
    [Fact]
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

        Assert.Equal(1, schedulerFactoryCalls);
    }

    /// <summary>
    /// Calls that factories throws argument null exception falls back to current thread.
    /// </summary>
    [Fact]
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

        Assert.Equal(CurrentThreadScheduler.Instance, schedulerExecutedOn);
    }

    /// <summary>
    /// Tests that factories throws exception re calls on schedule.
    /// </summary>
    [Fact]
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

        Assert.Equal(2, schedulerFactoryCalls);
    }

    /// <summary>
    /// Tests that factories throws invalid operation exception falls back to current thread.
    /// </summary>
    [Fact]
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

        Assert.Equal(CurrentThreadScheduler.Instance, schedulerExecutedOn);
    }

    /// <summary>
    /// Tests that factory uses cached scheduler.
    /// </summary>
    [Fact]
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

        Assert.Equal(1, schedulerFactoryCalls);
    }
}
