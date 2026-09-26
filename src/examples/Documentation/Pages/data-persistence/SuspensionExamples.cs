// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.DataPersistence;

/// <summary>Shows the process-wide <see cref="ISuspensionHost"/>, driven by hand, and the fallback <see cref="DummySuspensionDriver"/>.</summary>
public static class SuspensionExamples
{
    /// <summary><see cref="RxSuspension.SuspensionHost"/> always returns the same instance for the whole app.</summary>
    public static void SingletonIsSharedAcrossTheApp()
    {
        Console.WriteLine(ReferenceEquals(RxSuspension.SuspensionHost, RxSuspension.SuspensionHost));

        // Output:
        // True
    }

    /// <summary>
    /// A platform host pushes one signal into each lifecycle property; view models subscribe to react to launch,
    /// resume, persist and invalidate. This drives every property by hand, the way <c>AutoSuspendHelper</c> would
    /// on a real platform.
    /// </summary>
    public static void DriveLifecycleManually()
    {
        ISuspensionHost suspensionHost = RxSuspension.SuspensionHost;
        suspensionHost.CreateNewAppState = static () => new GameSaveState(Level: 1, Score: 0);

        Signal<RxVoid> launching = new();
        Signal<RxVoid> resuming = new();
        Signal<RxVoid> unpausing = new();
        Signal<IDisposable> shouldPersist = new();
        Signal<RxVoid> shouldInvalidate = new();

        suspensionHost.IsLaunchingNew = launching;
        suspensionHost.IsResuming = resuming;
        suspensionHost.IsUnpausing = unpausing;
        suspensionHost.ShouldPersistState = shouldPersist;
        suspensionHost.ShouldInvalidateState = shouldInvalidate;

        using IDisposable launchSubscription = suspensionHost.IsLaunchingNew.Subscribe(_ =>
        {
            suspensionHost.AppState = suspensionHost.CreateNewAppState!();
            Console.WriteLine("Launching new");
        });
        using IDisposable resumeSubscription = suspensionHost.IsResuming.Subscribe(static _ => Console.WriteLine("Resuming"));
        using IDisposable unpauseSubscription = suspensionHost.IsUnpausing.Subscribe(static _ => Console.WriteLine("Unpausing"));
        using IDisposable persistSubscription = suspensionHost.ShouldPersistState.Subscribe(token =>
        {
            GameSaveState state = (GameSaveState)suspensionHost.AppState!;
            Console.WriteLine($"Persisting level {state.Level}");
            token.Dispose();
        });
        using IDisposable invalidateSubscription = suspensionHost.ShouldInvalidateState.Subscribe(static _ => Console.WriteLine("Invalidating"));

        launching.OnNext(RxVoid.Default);
        resuming.OnNext(RxVoid.Default);
        unpausing.OnNext(RxVoid.Default);
        shouldPersist.OnNext(new ActionDisposable(static () => Console.WriteLine("Persist token disposed")));
        shouldInvalidate.OnNext(RxVoid.Default);

        // Output:
        // Launching new
        // Resuming
        // Unpausing
        // Persisting level 1
        // Persist token disposed
        // Invalidating
    }

    /// <summary><see cref="DummySuspensionDriver"/> saves and loads nothing; it is a stand-in for tests and for apps with no storage yet.</summary>
    /// <returns>A task that completes once every driver call has been awaited.</returns>
    public static async Task DummyDriverDoesNothing()
    {
        DummySuspensionDriver driver = new();

        _ = await driver.SaveState(new GameSaveState(Level: 5, Score: 900), GameSaveJsonContext.Default.GameSaveState);
        GameSaveState? loaded = await driver.LoadState(GameSaveJsonContext.Default.GameSaveState);
        _ = await driver.InvalidateState();

        Console.WriteLine(loaded is null);

        // Output:
        // True
    }
}
