// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Primitives.Concurrency;

namespace ReactiveUI.Documentation.Scheduling;

/// <summary>Shows a scheduler that falls back to running work in place until the real dispatcher is ready.</summary>
public static class WaitForDispatcherExamples
{
    /// <summary>Until the factory can build the real dispatcher scheduler, work runs on the current thread instead
    /// of failing; a music player's now-playing subject can use this scheduler from the moment the app starts.</summary>
    public static void RunBeforeTheDispatcherIsReady()
    {
        WaitForDispatcherScheduler scheduler = new(static () => throw new InvalidOperationException("Dispatcher not ready yet"));

        using IDisposable subscription = scheduler.Schedule("Bohemian Rhapsody", static (_, track) =>
        {
            Console.WriteLine($"Now playing: {track}");
            return EmptyDisposable.Instance;
        });

        Console.WriteLine(scheduler.Now > DateTimeOffset.MinValue);
        Console.WriteLine(scheduler.Timestamp > 0);

        // Output:
        // Now playing: Bohemian Rhapsody
        // True
        // True
    }

    /// <summary>A relative delay schedules through the same fallback path as an immediate call.</summary>
    public static void ScheduleAfterARelativeDelay()
    {
        WaitForDispatcherScheduler scheduler = new(static () => Sequencer.CurrentThread);

        using IDisposable subscription = scheduler.Schedule("Fix You", TimeSpan.FromMilliseconds(5), static (_, track) =>
        {
            Console.WriteLine($"Now playing: {track}");
            return EmptyDisposable.Instance;
        });

        // Output:
        // Now playing: Fix You
    }

    /// <summary>An absolute due time schedules the same way as a relative delay.</summary>
    public static void ScheduleAtAnAbsoluteTime()
    {
        WaitForDispatcherScheduler scheduler = new(static () => Sequencer.CurrentThread);
        DateTimeOffset dueTime = scheduler.Now.AddMilliseconds(5);

        using IDisposable subscription = scheduler.Schedule("Clocks", dueTime, static (_, track) =>
        {
            Console.WriteLine($"Now playing: {track}");
            return EmptyDisposable.Instance;
        });

        // Output:
        // Now playing: Clocks
    }

    /// <summary>A work item runs the same way a lambda does; this is the lower-level shape a sequencer accepts.</summary>
    public static void ScheduleAWorkItemDirectly()
    {
        WaitForDispatcherScheduler scheduler = new(static () => Sequencer.CurrentThread);
        NowPlayingWorkItem workItem = new("Yellow");

        scheduler.Schedule(workItem);

        // Output:
        // Now playing: Yellow
    }

    /// <summary>A work item can also target an absolute monotonic timestamp read from the scheduler.</summary>
    public static void ScheduleAWorkItemAtATimestamp()
    {
        WaitForDispatcherScheduler scheduler = new(static () => Sequencer.CurrentThread);
        NowPlayingWorkItem workItem = new("Viva la Vida");

        scheduler.Schedule(workItem, scheduler.Timestamp);

        // Output:
        // Now playing: Viva la Vida
    }
}
