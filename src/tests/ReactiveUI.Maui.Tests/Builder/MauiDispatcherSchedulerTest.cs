// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using ReactiveUI.Builder;

namespace ReactiveUI.Maui.Tests.Builder;

/// <summary>
/// Tests for MauiDispatcherScheduler behavior.
/// </summary>
public class MauiDispatcherSchedulerTest
{

    /// <summary>
    /// Tests that dispatcher scheduler executes immediate work.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispatcher_ImmediateSchedule_ExecutesWork()
    {
        RxAppBuilder.ResetForTesting();
        var dispatcher = new TestDispatcher();
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        builder.WithMauiScheduler(dispatcher);
        builder.WithCoreServices();
        builder.BuildApp();

        var executed = false;
        RxSchedulers.MainThreadScheduler.Schedule(() => executed = true);

        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Tests that dispatcher scheduler with IsDispatchRequired false executes immediately.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispatcher_NoDispatchRequired_ExecutesImmediately()
    {
        RxAppBuilder.ResetForTesting();
        var dispatcher = new TestDispatcher { IsDispatchRequired = false };
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        builder.WithMauiScheduler(dispatcher);
        builder.WithCoreServices();
        builder.BuildApp();

        var executed = false;
        RxSchedulers.MainThreadScheduler.Schedule(() => executed = true);

        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Tests that dispatcher scheduler with IsDispatchRequired true executes work.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispatcher_DispatchRequired_ExecutesWork()
    {
        RxAppBuilder.ResetForTesting();
        var dispatcher = new TestDispatcher { IsDispatchRequired = true };
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        builder.WithMauiScheduler(dispatcher);
        builder.WithCoreServices();
        builder.BuildApp();

        var executed = false;
        RxSchedulers.MainThreadScheduler.Schedule(() => executed = true);

        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Test dispatcher for scheduler testing.
    /// </summary>
    private class TestDispatcher : Microsoft.Maui.Dispatching.IDispatcher
    {
        public bool IsDispatchRequired { get; set; }

        public bool Dispatch(Action action)
        {
            action();
            return true;
        }

        public bool DispatchDelayed(TimeSpan delay, Action action)
        {
            action();
            return true;
        }

        public Microsoft.Maui.Dispatching.IDispatcherTimer CreateTimer()
        {
            return new TestDispatcherTimer();
        }

        /// <summary>
        /// Test dispatcher timer for testing.
        /// </summary>
        private class TestDispatcherTimer : Microsoft.Maui.Dispatching.IDispatcherTimer
        {
            public event EventHandler? Tick;

            public TimeSpan Interval { get; set; }

            public bool IsRepeating { get; set; }

            public bool IsRunning { get; private set; }

            public void Start()
            {
                IsRunning = true;

                // Immediately fire the tick event for testing
                Tick?.Invoke(this, EventArgs.Empty);
            }

            public void Stop()
            {
                IsRunning = false;
            }
        }
    }
}
