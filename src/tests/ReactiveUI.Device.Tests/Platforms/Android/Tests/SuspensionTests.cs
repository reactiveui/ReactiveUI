// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text.Json;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests <see cref="AutoSuspendHelper"/> against real activity lifecycles and <see cref="BundleSuspensionDriver"/> against real bundles.</summary>
public class SuspensionTests
{
    /// <summary>The description of a save, used in timeout messages.</summary>
    private const string Saving = "SaveState";

    /// <summary>The description of a load, used in timeout messages.</summary>
    private const string Loading = "LoadState";

    /// <summary>The state the round-trip tests save.</summary>
    private static readonly SuspensionState SavedState = new("saved", 3);

    /// <summary>A cold activity start signals that the app is launching new.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task AutoSuspendHelper_ColdStart_SignalsLaunchingNew()
    {
        var launching = RxSuspension.SuspensionHost.IsLaunchingNew.FirstValueAsync(out var subscription);
        using (subscription)
        {
            var activity = await ActivityLauncher.StartAsync<ActivatingActivity>();
            try
            {
                _ = await launching.WithTimeout("IsLaunchingNew");
                await Assert.That(activity.CreatedFromSavedState).IsFalse();
            }
            finally
            {
                await ActivityLauncher.FinishAsync(activity);
            }
        }
    }

    /// <summary>Pausing an activity signals that the app should persist its state.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task AutoSuspendHelper_Pause_SignalsShouldPersistState()
    {
        var activity = await ActivityLauncher.StartAsync<ActivatingActivity>();
        var persist = RxSuspension.SuspensionHost.ShouldPersistState.FirstValueAsync(out var subscription);
        using (subscription)
        {
            await ActivityLauncher.FinishAsync(activity);

            _ = await persist.WithTimeout("ShouldPersistState");
        }
    }

    /// <summary>Recreating an activity saves its state into the latest bundle and signals that the app is resuming.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task AutoSuspendHelper_Recreate_SavesTheBundleAndSignalsResuming()
    {
        var original = await ActivityLauncher.StartAsync<ActivatingActivity>();
        var resuming = RxSuspension.SuspensionHost.IsResuming.FirstValueAsync(out var subscription);
        try
        {
            using (subscription)
            {
                await MainThread.RunAsync(original.Recreate);

                _ = await resuming.WithTimeout("IsResuming");
            }

            await Eventually.TrueAsync(static () => ActivatingActivity.LastCreated is { CreatedFromSavedState: true }, "the recreated activity");
            await Assert.That(AutoSuspendHelper.LatestBundle).IsNotNull();
        }
        finally
        {
            if (ActivatingActivity.LastCreated is { } recreated)
            {
                await ActivityLauncher.FinishAsync(recreated);
            }
        }
    }

    /// <summary>State saved with source-generated metadata loads back unchanged.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task BundleDriver_SaveAndLoad_WithTypeInfo_RoundTrips()
    {
        AutoSuspendHelper.LatestBundle = new();
        var driver = new BundleSuspensionDriver();

        _ = await driver.SaveState(SavedState, SuspensionStateJsonContext.Default.SuspensionState).FirstValueAsync(out _).WithTimeout(Saving);
        var loaded = await driver.LoadState(SuspensionStateJsonContext.Default.SuspensionState).FirstValueAsync(out _).WithTimeout(Loading);

        await Assert.That(loaded).IsEqualTo(SavedState);
    }

    /// <summary>State saved by reflection loads back as JSON with the same values.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task BundleDriver_SaveAndLoad_ByReflection_RoundTrips()
    {
        AutoSuspendHelper.LatestBundle = new();
        var driver = new BundleSuspensionDriver();

        _ = await driver.SaveState(SavedState).FirstValueAsync(out _).WithTimeout(Saving);
        var loaded = await driver.LoadState().FirstValueAsync(out _).WithTimeout(Loading);

        await Assert.That(loaded).IsTypeOf<JsonElement>();
        var json = (JsonElement)loaded!;
        await Assert.That(json.GetProperty(nameof(SuspensionState.Name)).GetString()).IsEqualTo(SavedState.Name);
        await Assert.That(json.GetProperty(nameof(SuspensionState.Count)).GetInt32()).IsEqualTo(SavedState.Count);
    }

    /// <summary>Invalidated state no longer loads.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task BundleDriver_InvalidateState_MakesLoadFail()
    {
        AutoSuspendHelper.LatestBundle = new();
        var driver = new BundleSuspensionDriver();
        _ = await driver.SaveState(SavedState, SuspensionStateJsonContext.Default.SuspensionState).FirstValueAsync(out _).WithTimeout(Saving);

        _ = await driver.InvalidateState().FirstValueAsync(out _).WithTimeout("InvalidateState");
        var load = driver.LoadState(SuspensionStateJsonContext.Default.SuspensionState).FirstValueAsync(out _);

        await Assert.That(async () => await load.WithTimeout(Loading)).Throws<JsonException>();
    }

    /// <summary>Without a saved bundle there is no state to load.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task BundleDriver_WithoutABundle_LoadFails()
    {
        AutoSuspendHelper.LatestBundle = null;
        var load = new BundleSuspensionDriver().LoadState(SuspensionStateJsonContext.Default.SuspensionState).FirstValueAsync(out _);

        await Assert.That(async () => await load.WithTimeout(Loading)).Throws<InvalidOperationException>();
    }
}
