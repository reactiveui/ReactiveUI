// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Maui.Tests;

/// <summary>
/// Tests for <see cref="MauiReactiveUIBuilderExtensions"/>.
/// </summary>
public class MauiReactiveUIBuilderExtensionsTest
{
    /// <summary>
    /// Tests that MauiMainThreadScheduler is not null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MauiMainThreadScheduler_IsNotNull()
    {
        await Assert.That(MauiReactiveUIBuilderExtensions.MauiMainThreadScheduler).IsNotNull();
    }

#if ANDROID
    /// <summary>
    /// Tests that AndroidMainThreadScheduler is not null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AndroidMainThreadScheduler_IsNotNull()
    {
        await Assert.That(MauiReactiveUIBuilderExtensions.AndroidMainThreadScheduler).IsNotNull();
    }
#endif

#if MACCATALYST || IOS || MACOS || TVOS
    /// <summary>
    /// Tests that AppleMainThreadScheduler is not null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AppleMainThreadScheduler_IsNotNull()
    {
        await Assert.That(MauiReactiveUIBuilderExtensions.AppleMainThreadScheduler).IsNotNull();
    }
#endif

#if WINUI_TARGET
    /// <summary>
    /// Tests that WinUIMauiMainThreadScheduler is not null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WinUIMauiMainThreadScheduler_IsNotNull()
    {
        await Assert.That(MauiReactiveUIBuilderExtensions.WinUIMauiMainThreadScheduler).IsNotNull();
    }
#endif

    /// <summary>
    /// Tests that UseReactiveUI with action does not throw.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UseReactiveUI_WithAction_DoesNotThrow()
    {
        var builder = Microsoft.Maui.Hosting.MauiApp.CreateBuilder();

        var result = builder.UseReactiveUI(rxBuilder => { });

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsEqualTo(builder);
    }

    /// <summary>
    /// Tests that UseReactiveUI throws for null builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UseReactiveUI_NullBuilder_Throws()
    {
        Microsoft.Maui.Hosting.MauiAppBuilder builder = null!;

        await Assert.That(() => builder.UseReactiveUI(rxBuilder => { }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that WithMauiScheduler throws for null builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithMauiScheduler_NullBuilder_Throws()
    {
        IReactiveUIBuilder builder = null!;

        await Assert.That(() => builder.WithMauiScheduler())
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that WithMaui throws for null builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithMaui_NullBuilder_Throws()
    {
        IReactiveUIBuilder builder = null!;

        await Assert.That(() => builder.WithMaui())
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that UseReactiveUI with dispatcher does not throw.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UseReactiveUI_WithDispatcher_DoesNotThrow()
    {
        var builder = Microsoft.Maui.Hosting.MauiApp.CreateBuilder();
        var dispatcher = new MockDispatcher();

        var result = builder.UseReactiveUI(dispatcher);

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsEqualTo(builder);
    }

    /// <summary>
    /// Tests that UseReactiveUI with dispatcher throws for null builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UseReactiveUI_WithDispatcher_NullBuilder_Throws()
    {
        Microsoft.Maui.Hosting.MauiAppBuilder builder = null!;
        var dispatcher = new MockDispatcher();

        await Assert.That(() => builder.UseReactiveUI(dispatcher))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that WithMauiScheduler registers the correct scheduler.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithMauiScheduler_RegistersScheduler()
    {
        var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(resolver, resolver);
        var dispatcher = new MockDispatcher();

        builder.WithMauiScheduler(dispatcher);

        await Assert.That(builder.MainThreadScheduler).IsNotNull();
        await Assert.That(builder.MainThreadScheduler!.GetType().Name).IsEqualTo("MauiDispatcherScheduler");
    }

    /// <summary>
    /// Tests that MauiDispatcherScheduler schedules actions on the dispatcher.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MauiDispatcherScheduler_Schedule_DispatchesAction()
    {
        var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(resolver, resolver);
        var dispatcher = new MockDispatcher();
        builder.WithMauiScheduler(dispatcher);
        var scheduler = builder.MainThreadScheduler!;

        bool executed = false;
        scheduler.Schedule(() => executed = true);

        // MockDispatcher executes immediately if Dispatch is called
        await Assert.That(executed).IsTrue();
        await Assert.That(dispatcher.DispatchCount).IsEqualTo(1);
    }

    /// <summary>
    /// Tests that MauiDispatcherScheduler schedules delayed actions using a timer.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MauiDispatcherScheduler_ScheduleWithDelay_UsesTimer()
    {
        var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(resolver, resolver);
        var dispatcher = new MockDispatcher();
        builder.WithMauiScheduler(dispatcher);
        var scheduler = builder.MainThreadScheduler!;

        bool executed = false;
        scheduler.Schedule(TimeSpan.FromMilliseconds(100), () => executed = true);

        await Assert.That(dispatcher.CreatedTimers.Count).IsEqualTo(1);
        var timer = dispatcher.CreatedTimers[0];

        await Assert.That(timer.IsStarted).IsTrue();
        await Assert.That(timer.Interval).IsEqualTo(TimeSpan.FromMilliseconds(100));
        await Assert.That(timer.IsRepeating).IsFalse();

        // Simulate timer tick
        timer.FireTick();

        await Assert.That(executed).IsTrue();
        await Assert.That(timer.IsStarted).IsFalse(); // Should stop after tick
    }

    private class MockDispatcher : Microsoft.Maui.Dispatching.IDispatcher
    {
        public int DispatchCount { get; private set; }

        public List<MockDispatcherTimer> CreatedTimers { get; } = new();

        public bool IsDispatchRequired => true; // Force Dispatch call

        public bool Dispatch(Action action)
        {
            DispatchCount++;
            action();
            return true;
        }

        public bool DispatchDelayed(TimeSpan delay, Action action)
        {
            throw new NotImplementedException();
        }

        public Microsoft.Maui.Dispatching.IDispatcherTimer CreateTimer()
        {
            var timer = new MockDispatcherTimer();
            CreatedTimers.Add(timer);
            return timer;
        }
    }

    private class MockDispatcherTimer : Microsoft.Maui.Dispatching.IDispatcherTimer
    {
        public event EventHandler? Tick;

        public TimeSpan Interval { get; set; }

        public bool IsRepeating { get; set; }

        public bool IsRunning { get; private set; }

        public bool IsStarted { get; private set; }

        public void Start()
        {
            IsStarted = true;
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
            IsStarted = false;
        }

        public void FireTick()
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }
    }
}
