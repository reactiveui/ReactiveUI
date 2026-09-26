// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using ReactiveUI.Primitives.Concurrency;

namespace ReactiveUI.Documentation.Scheduling;

/// <summary>Shows the schedulers ReactiveUI hands to every view model.</summary>
public static class RxSchedulerExamples
{
    /// <summary>A weather reading downloads on the task pool, then the dashboard shows it on the main thread; a
    /// console has no UI thread, so <c>MainThreadScheduler</c> defaults to running in place.</summary>
    /// <returns>A task that completes once the reading has downloaded and been shown.</returns>
    public static async Task DownloadOnTaskpoolShowOnMainThread()
    {
        const double Reading = 18.5;
        TaskCompletionSource<double> downloaded = new(TaskCreationOptions.RunContinuationsAsynchronously);

        using IDisposable download = RxSchedulers.TaskpoolScheduler.Schedule(downloaded, static (_, completion) =>
        {
            Console.WriteLine($"Downloaded reading: {Reading}°C");
            completion.SetResult(Reading);
            return EmptyDisposable.Instance;
        });

        double reading = await downloaded.Task;

        using IDisposable display = RxSchedulers.MainThreadScheduler.Schedule(reading, static (_, value) =>
        {
            Console.WriteLine($"Displayed on the main thread: {value}°C");
            return EmptyDisposable.Instance;
        });

        // Output:
        // Downloaded reading: 18.5°C
        // Displayed on the main thread: 18.5°C
    }

    /// <summary>Pointing both schedulers at the same sequencer, then restoring them, is how a quick script or a test makes asynchronous work run synchronously.</summary>
    public static void SwitchBothSchedulersForATest()
    {
        ISequencer originalMainThreadScheduler = RxSchedulers.MainThreadScheduler;
        ISequencer originalTaskpoolScheduler = RxSchedulers.TaskpoolScheduler;

        RxSchedulers.MainThreadScheduler = Sequencer.Immediate;
        RxSchedulers.TaskpoolScheduler = Sequencer.Immediate;

        Console.WriteLine(ReferenceEquals(RxSchedulers.MainThreadScheduler, RxSchedulers.TaskpoolScheduler));

        RxSchedulers.MainThreadScheduler = originalMainThreadScheduler;
        RxSchedulers.TaskpoolScheduler = originalTaskpoolScheduler;

        // Output:
        // True
    }

    /// <summary>A platform sets this to stop ReactiveUI logging a message every time a command binds to a control; a console host has no controls, so it silences the noise itself.</summary>
    public static void SuppressViewCommandBindingLogging()
    {
        bool original = RxSchedulers.SuppressViewCommandBindingMessage;

        RxSchedulers.SuppressViewCommandBindingMessage = true;
        Console.WriteLine(RxSchedulers.SuppressViewCommandBindingMessage);

        RxSchedulers.SuppressViewCommandBindingMessage = original;

        // Output:
        // True
    }

    /// <summary>ReactiveUI's internal memoizing caches use these two limits unless a builder configures its own with <c>WithCacheSizes</c>.</summary>
    public static void ReadTheCacheSizeLimits()
    {
        Console.WriteLine(RxCacheSize.SmallCacheLimit);
        Console.WriteLine(RxCacheSize.BigCacheLimit);

        // Output:
        // 64
        // 256
    }

    /// <summary>Building a second, isolated instance exposes the schedulers it was given without touching the
    /// global <see cref="RxSchedulers"/> values; a test host does this to keep its own schedulers out of the way.</summary>
    public static void InspectABuiltInstance()
    {
        ReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder();
        builder.WithMainThreadScheduler(Sequencer.Immediate, setRxApp: false);
        builder.WithTaskPoolScheduler(TaskPoolSequencer.Default, setRxApp: false);
        builder.WithCoreServices();
        IReactiveUIInstance instance = builder.BuildApp();

        Console.WriteLine(instance.MainThreadScheduler?.GetType().Name);
        Console.WriteLine(instance.TaskpoolScheduler?.GetType().Name);

        // Output:
        // ImmediateSequencer
        // TaskPoolSequencer
    }
}
