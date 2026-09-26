// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Device.Tests;

/// <summary>Tests <see cref="AppSupportJsonSuspensionDriver"/> against the simulator's real Application Support folder.</summary>
public class AppSupportJsonSuspensionDriverTests
{
    /// <summary>The description of a save, used in timeout messages.</summary>
    private const string Saving = "SaveState";

    /// <summary>The description of a load, used in timeout messages.</summary>
    private const string Loading = "LoadState";

    /// <summary>The state the tests save.</summary>
    private static readonly SuspensionState SavedState = new("saved", 3);

    /// <summary>State saved with source-generated metadata loads back unchanged.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task SaveAndLoad_WithTypeInfo_RoundTrips()
    {
        var driver = new AppSupportJsonSuspensionDriver($"device-tests-{Guid.NewGuid():N}");

        _ = await driver.SaveState(SavedState, SuspensionStateJsonContext.Default.SuspensionState).FirstValueAsync(out _).WithTimeout(Saving);
        var loaded = await driver.LoadState(SuspensionStateJsonContext.Default.SuspensionState).FirstValueAsync(out _).WithTimeout(Loading);

        await Assert.That(loaded).IsEqualTo(SavedState);
    }

    /// <summary>Invalidating deletes the state, so the next load fails.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task InvalidateState_MakesLoadFail()
    {
        var driver = new AppSupportJsonSuspensionDriver($"device-tests-{Guid.NewGuid():N}");
        _ = await driver.SaveState(SavedState, SuspensionStateJsonContext.Default.SuspensionState).FirstValueAsync(out _).WithTimeout(Saving);

        _ = await driver.InvalidateState().FirstValueAsync(out _).WithTimeout("InvalidateState");
        var load = driver.LoadState(SuspensionStateJsonContext.Default.SuspensionState).FirstValueAsync(out _);

        await Assert.That(async () => await load.WithTimeout(Loading)).Throws<FileNotFoundException>();
    }

    /// <summary>Two drivers with different folders keep separate state.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task SeparateSubdirectories_KeepSeparateState()
    {
        var first = new AppSupportJsonSuspensionDriver($"device-tests-{Guid.NewGuid():N}");
        var second = new AppSupportJsonSuspensionDriver($"device-tests-{Guid.NewGuid():N}");
        _ = await first.SaveState(SavedState, SuspensionStateJsonContext.Default.SuspensionState).FirstValueAsync(out _).WithTimeout(Saving);

        var load = second.LoadState(SuspensionStateJsonContext.Default.SuspensionState).FirstValueAsync(out _);

        await Assert.That(async () => await load.WithTimeout(Loading)).Throws<FileNotFoundException>();
    }
}
