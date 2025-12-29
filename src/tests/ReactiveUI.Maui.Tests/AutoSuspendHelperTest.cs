// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Maui;

/// <summary>
/// Tests for <see cref="AutoSuspendHelper"/>.
/// </summary>
public class AutoSuspendHelperTest
{
    /// <summary>
    /// Tests that AutoSuspendHelper can be instantiated.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_CreatesInstance()
    {
        var helper = new AutoSuspendHelper();

        await Assert.That(helper).IsNotNull();
    }

    /// <summary>
    /// Tests that AutoSuspendHelper wires up suspension host observables.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_WiresUpSuspensionHost()
    {
        var helper = new AutoSuspendHelper();

        await Assert.That(RxApp.SuspensionHost.IsLaunchingNew).IsNotNull();
        await Assert.That(RxApp.SuspensionHost.IsUnpausing).IsNotNull();
        await Assert.That(RxApp.SuspensionHost.IsResuming).IsNotNull();
        await Assert.That(RxApp.SuspensionHost.ShouldPersistState).IsNotNull();
        await Assert.That(RxApp.SuspensionHost.ShouldInvalidateState).IsNotNull();
    }

    /// <summary>
    /// Tests that OnCreate triggers IsLaunchingNew.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnCreate_TriggersIsLaunchingNew()
    {
        var helper = new AutoSuspendHelper();
        var triggered = false;

        RxApp.SuspensionHost.IsLaunchingNew.Subscribe(_ => triggered = true);
        helper.OnCreate();

        await Assert.That(triggered).IsTrue();
    }

    /// <summary>
    /// Tests that OnStart triggers IsUnpausing.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnStart_TriggersIsUnpausing()
    {
        var helper = new AutoSuspendHelper();
        var triggered = false;

        RxApp.SuspensionHost.IsUnpausing.Subscribe(_ => triggered = true);
        helper.OnStart();

        await Assert.That(triggered).IsTrue();
    }

    /// <summary>
    /// Tests that OnResume triggers IsResuming.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnResume_TriggersIsResuming()
    {
        var helper = new AutoSuspendHelper();
        var triggered = false;

        RxApp.SuspensionHost.IsResuming.Subscribe(_ => triggered = true);
        helper.OnResume();

        await Assert.That(triggered).IsTrue();
    }

    /// <summary>
    /// Tests that OnSleep triggers ShouldPersistState.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnSleep_TriggersShouldPersistState()
    {
        var helper = new AutoSuspendHelper();
        var triggered = false;

        RxApp.SuspensionHost.ShouldPersistState.Subscribe(_ => triggered = true);
        helper.OnSleep();

        await Assert.That(triggered).IsTrue();
    }

    /// <summary>
    /// Tests that Dispose cleans up resources.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispose_CleansUpResources()
    {
        var helper = new AutoSuspendHelper();

        helper.Dispose();

        // Verify disposal completed without throwing
        await Task.CompletedTask;
    }

    /// <summary>
    /// Tests that UntimelyDemise property is accessible.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UntimelyDemise_IsAccessible()
    {
        var untimelyDemise = ReactiveUI.Maui.AutoSuspendHelper.UntimelyDemise;

        await Assert.That(untimelyDemise).IsNotNull();
    }
}
