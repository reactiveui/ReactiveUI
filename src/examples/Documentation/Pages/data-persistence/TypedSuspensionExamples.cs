// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Documentation.DataPersistence;

/// <summary>
/// Shows the strongly-typed <see cref="SuspensionHost{TAppState}"/>: driving its lifecycle by hand, then the real
/// workflow through <see cref="SuspensionHostExtensions"/> and a file-backed <see cref="ISuspensionDriver"/>.
/// </summary>
public static class TypedSuspensionExamples
{
    /// <summary>Every lifecycle signal on a typed host behaves the same as the untyped <see cref="ISuspensionHost"/>, but <c>AppStateValue</c> is strongly typed.</summary>
    public static void DriveTypedLifecycleManually()
    {
        using SuspensionHost<GameSaveState> host = new();
        host.CreateNewAppStateTyped = static () => new GameSaveState(Level: 1, Score: 0);

        Signal<RxVoid> launching = new();
        Signal<RxVoid> resuming = new();
        Signal<RxVoid> unpausing = new();
        Signal<RxVoid> continuing = new();
        Signal<IDisposable> shouldPersist = new();
        Signal<RxVoid> shouldInvalidate = new();

        host.IsLaunchingNew = launching;
        host.IsResuming = resuming;
        host.IsUnpausing = unpausing;
        host.IsContinuing = continuing;
        host.ShouldPersistState = shouldPersist;
        host.ShouldInvalidateState = shouldInvalidate;

        List<GameSaveState?> stateChanges = [];
        using IDisposable stateChangeSubscription = host.AppStateValueChanged.Subscribe(stateChanges.Add);
        using IDisposable launchSubscription = host.IsLaunchingNew.Subscribe(_ =>
        {
            host.AppStateValue = host.CreateNewAppStateTyped!();
            Console.WriteLine("Launching new");
        });
        using IDisposable resumeSubscription = host.IsResuming.Subscribe(static _ => Console.WriteLine("Resuming"));
        using IDisposable unpauseSubscription = host.IsUnpausing.Subscribe(static _ => Console.WriteLine("Unpausing"));
        using IDisposable continueSubscription = host.IsContinuing.Subscribe(static _ => Console.WriteLine("Continuing"));
        using IDisposable persistSubscription = host.ShouldPersistState.Subscribe(token =>
        {
            Console.WriteLine($"Persisting level {host.AppStateValue!.Level}");
            token.Dispose();
        });
        using IDisposable invalidateSubscription = host.ShouldInvalidateState.Subscribe(static _ => Console.WriteLine("Invalidating"));

        launching.OnNext(RxVoid.Default);
        resuming.OnNext(RxVoid.Default);
        unpausing.OnNext(RxVoid.Default);
        continuing.OnNext(RxVoid.Default);
        shouldPersist.OnNext(new ActionDisposable(static () => Console.WriteLine("Persist token disposed")));
        shouldInvalidate.OnNext(RxVoid.Default);

        Console.WriteLine(stateChanges.Count);

        // Output:
        // Launching new
        // Resuming
        // Unpausing
        // Continuing
        // Persisting level 1
        // Persist token disposed
        // Invalidating
        // 1
    }

    /// <summary>
    /// The real workflow: a driver saved via a source-generated <c>JsonTypeInfo</c>. The no-driver overload resolves
    /// one from the service locator; the other overload takes it explicitly, for a second host that shares the same
    /// save file. Either way, <c>GetAppState</c> loads once and <c>ShouldPersistState</c> saves on demand.
    /// </summary>
    /// <returns>A task that completes once both hosts have saved to disk.</returns>
    public static async Task SetupDefaultSuspendResumeOverloads()
    {
        string savePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
        FileGameSaveDriver driver = new(savePath);

        // A platform's startup code registers its driver once; SetupDefaultSuspendResume(typeInfo) resolves it from here.
        AppLocator.CurrentMutable.RegisterConstant<ISuspensionDriver>(driver);

        try
        {
            using SuspensionHost<GameSaveState> resolvedHost = new()
            {
                CreateNewAppStateTyped = static () => new GameSaveState(Level: 1, Score: 0),
                IsLaunchingNew = Signal.Emit(RxVoid.Default),
                IsResuming = Signal.Silent<RxVoid>(),
                ShouldInvalidateState = Signal.Silent<RxVoid>(),
            };

            Signal<IDisposable> resolvedPersist = new();
            resolvedHost.ShouldPersistState = resolvedPersist;

            using IDisposable resolvedSubscription = resolvedHost.SetupDefaultSuspendResume(GameSaveJsonContext.Default.GameSaveState);

            GameSaveState loaded = resolvedHost.GetAppState();
            Console.WriteLine(loaded.Level);

            List<GameSaveState> observedStates = [];
            using IDisposable observeSubscription = resolvedHost.ObserveAppState().Subscribe(observedStates.Add);

            resolvedHost.AppStateValue = loaded with { Score = 250 };
            resolvedPersist.OnNext(new ActionDisposable(static () => { }));

            await WaitUntilSavedAsync(savePath, "250");

            using SuspensionHost<GameSaveState> explicitHost = new()
            {
                AppStateValue = new GameSaveState(Level: 9, Score: 0),
                IsLaunchingNew = Signal.Silent<RxVoid>(),
                IsResuming = Signal.Silent<RxVoid>(),
                ShouldInvalidateState = Signal.Silent<RxVoid>(),
            };

            Signal<IDisposable> explicitPersist = new();
            explicitHost.ShouldPersistState = explicitPersist;

            using IDisposable explicitSubscription = explicitHost.SetupDefaultSuspendResume(GameSaveJsonContext.Default.GameSaveState, driver);

            explicitPersist.OnNext(new ActionDisposable(static () => { }));

            await WaitUntilSavedAsync(savePath, "\"Level\":9");

            Console.WriteLine(observedStates.Count);
        }
        finally
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
        }

        // Output:
        // 1
        // 2
    }

    /// <summary>Polls the save file until it contains the expected text, since the driver saves on a background thread.</summary>
    /// <param name="path">The save file to poll.</param>
    /// <param name="expectedText">The text that must appear before the save has completed.</param>
    /// <returns>A task that completes once the file contains <paramref name="expectedText"/>.</returns>
    private static async Task WaitUntilSavedAsync(string path, string expectedText)
    {
        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(10));
        while (!File.Exists(path) || !(await File.ReadAllTextAsync(path)).Contains(expectedText, StringComparison.Ordinal))
        {
            _ = await timer.WaitForNextTickAsync();
        }
    }
}
