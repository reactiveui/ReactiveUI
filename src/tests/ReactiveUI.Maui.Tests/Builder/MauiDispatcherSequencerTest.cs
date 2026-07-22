// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using ReactiveUI.Builder;

namespace ReactiveUI.Maui.Tests.Builder;

/// <summary>Tests for MauiDispatcherSequencer behavior.</summary>
public class MauiDispatcherSequencerTest
{
/// <summary>Tests that dispatcher sequencer executes immediate work.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispatcher_ImmediateSchedule_ExecutesWork()
    {
        RxAppBuilder.ResetForTesting();
        var dispatcher = new TestDispatcher();
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        _ = builder.WithMauiScheduler(dispatcher);
        _ = builder.WithCoreServices();
        _ = builder.BuildApp();

        var executed = new StrongBox<bool>();
        _ = RxSchedulers.MainThreadScheduler.Schedule(executed, static state => state.Value = true);

        await Assert.That(executed.Value).IsTrue();
    }

    /// <summary>Tests that dispatcher sequencer with IsDispatchRequired false executes immediately.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispatcher_NoDispatchRequired_ExecutesImmediately()
    {
        RxAppBuilder.ResetForTesting();
        var dispatcher = new TestDispatcher { IsDispatchRequired = false };
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        _ = builder.WithMauiScheduler(dispatcher);
        _ = builder.WithCoreServices();
        _ = builder.BuildApp();

        var executed = new StrongBox<bool>();
        _ = RxSchedulers.MainThreadScheduler.Schedule(executed, static state => state.Value = true);

        await Assert.That(executed.Value).IsTrue();
    }

    /// <summary>Tests that dispatcher sequencer with IsDispatchRequired true executes work.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispatcher_DispatchRequired_ExecutesWork()
    {
        RxAppBuilder.ResetForTesting();
        var dispatcher = new TestDispatcher { IsDispatchRequired = true };
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        _ = builder.WithMauiScheduler(dispatcher);
        _ = builder.WithCoreServices();
        _ = builder.BuildApp();

        var executed = new StrongBox<bool>();
        _ = RxSchedulers.MainThreadScheduler.Schedule(executed, static state => state.Value = true);

        await Assert.That(executed.Value).IsTrue();
    }

    /// <summary>Test dispatcher for sequencer testing.</summary>
    private sealed class TestDispatcher : IDispatcher
    {
        /// <inheritdoc/>
        public bool IsDispatchRequired { get; set; }

        /// <inheritdoc/>
        public bool Dispatch(Action action)
        {
            action();
            return true;
        }

        /// <inheritdoc/>
        public bool DispatchDelayed(TimeSpan delay, Action action)
        {
            action();
            return true;
        }

        /// <inheritdoc/>
        public IDispatcherTimer CreateTimer() => new TestDispatcherTimer();

        /// <summary>Test dispatcher timer for testing.</summary>
        private sealed class TestDispatcherTimer : IDispatcherTimer
        {
            /// <inheritdoc/>
            public event EventHandler? Tick;

            /// <inheritdoc/>
            public TimeSpan Interval { get; set; }

            /// <inheritdoc/>
            public bool IsRepeating { get; set; }

            /// <inheritdoc/>
            public bool IsRunning { get; private set; }

            /// <inheritdoc/>
            public void Start()
            {
                IsRunning = true;

                // Immediately fire the tick event for testing
                Tick?.Invoke(this, EventArgs.Empty);
            }

            /// <inheritdoc/>
            public void Stop() => IsRunning = false;
        }
    }
}
