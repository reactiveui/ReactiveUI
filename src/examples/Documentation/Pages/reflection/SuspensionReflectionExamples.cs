// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Documentation.Reflection;

/// <summary>
/// Shows the untyped <see cref="ISuspensionDriver"/> and <see cref="ISuspensionHost"/> members, which serialize
/// whatever object <see cref="ISuspensionHost.AppState"/> holds using reflection. Giving the host a
/// <c>JsonTypeInfo&lt;TAppState&gt;</c> and a strongly-typed <c>ISuspensionHost&lt;TAppState&gt;</c> — shown on the
/// <c>data-persistence</c> page — reaches the same result without reflecting at run time.
/// </summary>
public static class SuspensionReflectionExamples
{
    /// <summary><see cref="DummySuspensionDriver"/> saves and loads nothing; both calls go through the untyped, reflection-based members.</summary>
    /// <returns>A task that completes once both driver calls have been awaited.</returns>
    public static async Task SaveAndLoadThroughTheUntypedDriver()
    {
        DummySuspensionDriver driver = new();

        _ = await driver.SaveState(new PlayerState("Africa", 210));
        object? loaded = await driver.LoadState();

        Console.WriteLine(loaded is null);

        // Output:
        // True
    }

    /// <summary>
    /// <c>GetAppState</c> casts <see cref="ISuspensionHost.AppState"/> to the requested type; <c>ObserveAppState</c>
    /// watches it the same way, through the untyped, reflection-based <c>WhenAny</c>.
    /// </summary>
    public static void ReadAndObserveTheUntypedAppState()
    {
        ISuspensionHost host = RxSuspension.SuspensionHost;
        host.AppState = new PlayerState("Bohemian Rhapsody", 0);

        List<PlayerState> observed = [];
        using IDisposable subscription = host.ObserveAppState<PlayerState>().Subscribe(observed.Add);

        PlayerState current = host.GetAppState<PlayerState>();
        host.AppState = new PlayerState("Africa", 45);

        Console.WriteLine(current.Track);
        Console.WriteLine(string.Join(", ", observed.Select(static state => state.Track)));

        // Output:
        // Bohemian Rhapsody
        // Bohemian Rhapsody, Africa
    }

    /// <summary>
    /// <c>SetupDefaultSuspendResume</c> wires <see cref="ISuspensionHost"/> to a driver, either resolved from the
    /// service locator or passed explicitly; both overloads load once and save through the same untyped driver calls.
    /// </summary>
    public static void SetUpSuspendAndResumeWithAndWithoutAnExplicitDriver()
    {
        ISuspensionHost host = RxSuspension.SuspensionHost;
        host.AppState = null;
        host.CreateNewAppState = static () => new PlayerState("Nothing queued", 0);
        host.IsLaunchingNew = Signal.Emit(RxVoid.Default);
        host.IsResuming = Signal.Silent<RxVoid>();
        host.IsUnpausing = Signal.Silent<RxVoid>();
        host.ShouldPersistState = Signal.Silent<IDisposable>();
        host.ShouldInvalidateState = Signal.Silent<RxVoid>();

        DummySuspensionDriver driver = new();
        AppLocator.CurrentMutable.RegisterConstant<ISuspensionDriver>(driver);

        // No driver given: resolves the one just registered and loads the app state immediately, because
        // IsLaunchingNew already has a value.
        using IDisposable resolvedSubscription = host.SetupDefaultSuspendResume();
        PlayerState createdState = host.GetAppState<PlayerState>();
        Console.WriteLine(createdState.Track);

        // The same driver, passed explicitly instead of resolved.
        using IDisposable explicitSubscription = host.SetupDefaultSuspendResume(driver);
        Console.WriteLine(explicitSubscription is not null);

        // Output:
        // Nothing queued
        // True
    }
}
