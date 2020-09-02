﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

using Xunit;

namespace ReactiveUI.Tests
{
    public class WaitForDispatcherSchedulerTests
    {
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

        [Fact]
        public void FactoryThrowsArgumentNullException_FallsBackToCurrentThread()
        {
            IScheduler? schedulerExecutedOn = null;
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            var schedulerFactory = new Func<IScheduler>(() => throw new ArgumentNullException());
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

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
}
