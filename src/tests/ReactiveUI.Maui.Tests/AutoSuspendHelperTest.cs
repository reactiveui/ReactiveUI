// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui.Tests;

/// <summary>Tests for <see cref="AutoSuspendHelper"/>.</summary>
public sealed class AutoSuspendHelperTest
{
    /// <summary>Tests that AutoSuspendHelper can be instantiated.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_CreatesInstance()
    {
        var helper = new AutoSuspendHelper();

        await Assert.That(helper).IsNotNull();
    }

    /// <summary>Tests that AutoSuspendHelper wires up suspension host observables.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_WiresUpSuspensionHost()
    {
        _ = new AutoSuspendHelper();

        await Assert.That(RxSuspension.SuspensionHost.IsLaunchingNew).IsNotNull();
        await Assert.That(RxSuspension.SuspensionHost.IsUnpausing).IsNotNull();
        await Assert.That(RxSuspension.SuspensionHost.IsResuming).IsNotNull();
        await Assert.That(RxSuspension.SuspensionHost.ShouldPersistState).IsNotNull();
        await Assert.That(RxSuspension.SuspensionHost.ShouldInvalidateState).IsNotNull();
    }

    /// <summary>Tests that OnCreate triggers IsLaunchingNew.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnCreate_TriggersIsLaunchingNew()
    {
        var helper = new AutoSuspendHelper();
        var triggered = false;

        _ = RxSuspension.SuspensionHost.IsLaunchingNew.Subscribe(_ => triggered = true);
        helper.OnCreate();

        await Assert.That(triggered).IsTrue();
    }

    /// <summary>Tests that OnStart triggers IsUnpausing.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnStart_TriggersIsUnpausing()
    {
        var helper = new AutoSuspendHelper();
        var triggered = false;

        _ = RxSuspension.SuspensionHost.IsUnpausing.Subscribe(_ => triggered = true);
        helper.OnStart();

        await Assert.That(triggered).IsTrue();
    }

    /// <summary>Tests that OnResume triggers IsResuming.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnResume_TriggersIsResuming()
    {
        var helper = new AutoSuspendHelper();
        var triggered = false;

        _ = RxSuspension.SuspensionHost.IsResuming.Subscribe(_ => triggered = true);
        helper.OnResume();

        await Assert.That(triggered).IsTrue();
    }

    /// <summary>Tests that OnSleep triggers ShouldPersistState.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnSleep_TriggersShouldPersistState()
    {
        var helper = new AutoSuspendHelper();
        var triggered = false;

        _ = RxSuspension.SuspensionHost.ShouldPersistState.Subscribe(_ => triggered = true);
        helper.OnSleep();

        await Assert.That(triggered).IsTrue();
    }

    /// <summary>Tests that Dispose cleans up resources.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispose_CleansUpResources()
    {
        var helper = new AutoSuspendHelper();

        helper.Dispose();

        // Verify disposal completed without throwing
        await Task.CompletedTask;
    }

    /// <summary>Tests that UntimelyDemise property is accessible.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UntimelyDemise_IsAccessible() =>
        await Assert.That(AutoSuspendHelper.UntimelyDemise).IsNotNull();

    /// <summary>
    /// Tests that a second Dispose is a no-op: it does not throw, and it leaves the helper's lifecycle signals in the
    /// disposed state the first call put them in.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispose_CalledTwice_LeavesTheLifecycleSignalsDisposed()
    {
        var helper = new AutoSuspendHelper();
        helper.Dispose();

        await Assert.That(helper.Dispose).ThrowsNothing();

        await Assert.That(helper.OnCreate).Throws<ObjectDisposedException>();
        await Assert.That(helper.OnStart).Throws<ObjectDisposedException>();
        await Assert.That(helper.OnResume).Throws<ObjectDisposedException>();
        await Assert.That(helper.OnSleep).Throws<ObjectDisposedException>();
    }

    /// <summary>
    /// Tests that the non-disposing branch of the dispose pattern leaves the managed lifecycle signals alone, so the
    /// helper can still relay MAUI's notifications to the suspension host.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispose_WithoutDisposingManagedResources_KeepsTheLifecycleSignalsUsable()
    {
        using var helper = new TestAutoSuspendHelper();
        var unpaused = false;
        _ = RxSuspension.SuspensionHost.IsUnpausing.Subscribe(_ => unpaused = true);

        helper.InvokeDispose(false);

        await Assert.That(helper.OnStart).ThrowsNothing();
        await Assert.That(unpaused).IsTrue();
    }

    /// <summary>
    /// An <see cref="AutoSuspendHelper"/> that exposes the protected dispose overload, so the non-disposing branch the
    /// base class reserves for finalization can be driven from a test.
    /// </summary>
    private sealed class TestAutoSuspendHelper : AutoSuspendHelper
    {
        /// <summary>Runs the protected dispose overload.</summary>
        /// <param name="disposing"><see langword="true"/> to release managed resources.</param>
        public void InvokeDispose(bool disposing) => Dispose(disposing);
    }
}
