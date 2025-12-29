// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Windows;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="AutoSuspendHelper"/>.
/// </summary>
[NotInParallel]
public class AutoSuspendHelperTest
{
    /// <summary>
    /// Tests that AutoSuspendHelper can be instantiated.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_CreatesInstance()
    {
        var app = new Application();
        var helper = new AutoSuspendHelper(app);

        await Assert.That(helper).IsNotNull();
        await Assert.That(helper.IdleTimeout).IsEqualTo(TimeSpan.FromSeconds(15.0));
    }

    /// <summary>
    /// Tests that IdleTimeout property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IdleTimeout_CanBeSetAndRetrieved()
    {
        var app = new Application();
        var helper = new AutoSuspendHelper(app);
        var timeout = TimeSpan.FromSeconds(30);

        helper.IdleTimeout = timeout;

        await Assert.That(helper.IdleTimeout).IsEqualTo(timeout);
    }

    /// <summary>
    /// Tests that AutoSuspendHelper wires up suspension host observables.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_WiresUpSuspensionHost()
    {
        var app = new Application();
        var helper = new AutoSuspendHelper(app);

        await Assert.That(RxApp.SuspensionHost.IsLaunchingNew).IsNotNull();
        await Assert.That(RxApp.SuspensionHost.IsUnpausing).IsNotNull();
        await Assert.That(RxApp.SuspensionHost.IsResuming).IsNotNull();
        await Assert.That(RxApp.SuspensionHost.ShouldPersistState).IsNotNull();
        await Assert.That(RxApp.SuspensionHost.ShouldInvalidateState).IsNotNull();
    }

    /// <summary>
    /// Tests that IsResuming is set to Never observable.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_SetsIsResumingToNever()
    {
        var app = new Application();
        var helper = new AutoSuspendHelper(app);

        // IsResuming should be Observable.Never
        var triggered = false;
        var subscription = RxApp.SuspensionHost.IsResuming.Subscribe(_ => triggered = true);

        await Task.Delay(100);
        await Assert.That(triggered).IsFalse();
        subscription.Dispose();
    }

    /// <summary>
    /// Tests that default IdleTimeout is 15 seconds.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_SetsDefaultIdleTimeout()
    {
        var app = new Application();
        var helper = new AutoSuspendHelper(app);

        await Assert.That(helper.IdleTimeout).IsEqualTo(TimeSpan.FromSeconds(15.0));
    }
}
