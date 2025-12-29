// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Hosting;
using ReactiveUI.Builder;

namespace ReactiveUI.Tests.Maui.Builder;

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
    /// Tests that AndroidMainThreadScheduler is not null on Android.
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
    /// Tests that AppleMainThreadScheduler is not null on Apple platforms.
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
    /// Tests that WinUIMauiMainThreadScheduler is not null on WinUI.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WinUIMauiMainThreadScheduler_IsNotNull()
    {
        await Assert.That(MauiReactiveUIBuilderExtensions.WinUIMauiMainThreadScheduler).IsNotNull();
    }
#endif

    /// <summary>
    /// Tests that WithMauiScheduler returns the builder instance.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithMauiScheduler_ReturnsBuilder()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithMauiScheduler();

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsEqualTo(builder);
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
    /// Tests that UseReactiveUI with action throws for null builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UseReactiveUI_WithAction_NullBuilder_Throws()
    {
        MauiAppBuilder builder = null!;

        await Assert.That(() => builder.UseReactiveUI(_ => { }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that UseReactiveUI with dispatcher throws for null builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UseReactiveUI_WithDispatcher_NullBuilder_Throws()
    {
        MauiAppBuilder builder = null!;

        await Assert.That(() => builder.UseReactiveUI(Microsoft.Maui.Dispatching.Dispatcher.GetForCurrentThread()!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that WithMauiScheduler with custom dispatcher works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithMauiScheduler_WithCustomDispatcher_ConfiguresScheduler()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        var dispatcher = new TestDispatcher();

        var result = builder.WithMauiScheduler(dispatcher);

        await Assert.That(result).IsNotNull();
    }

    /// <summary>
    /// Tests that WithMaui configures the builder properly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithMaui_ConfiguresBuilder()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithMaui();

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsEqualTo(builder);
    }

    /// <summary>
    /// Test dispatcher for testing.
    /// </summary>
    private class TestDispatcher : Microsoft.Maui.Dispatching.IDispatcher
    {
        public bool IsDispatchRequired => false;

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
