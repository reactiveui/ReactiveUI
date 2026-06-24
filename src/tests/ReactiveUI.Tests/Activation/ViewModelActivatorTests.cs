// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Activation;

/// <summary>Tests for the <see cref="ViewModelActivator" />.</summary>
public class ViewModelActivatorTests
{
    /// <summary>Tests the activating ticks activated observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TestActivatingTicksActivatedObservable()
    {
        var viewModelActivator = new ViewModelActivator();
        var activated = viewModelActivator.Activated.Collect();

        _ = viewModelActivator.Activate();

        await Assert.That(activated).Count().IsEqualTo(1);
    }

    /// <summary>Tests the deactivating count doesnt tick deactivated observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TestDeactivatingCountDoesntTickDeactivatedObservable()
    {
        var viewModelActivator = new ViewModelActivator();
        var deactivated = viewModelActivator.Deactivated.Collect();

        viewModelActivator.Deactivate();

        await Assert.That(deactivated).IsEmpty();
    }

    /// <summary>Tests the deactivating following activating ticks deactivated observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TestDeactivatingFollowingActivatingTicksDeactivatedObservable()
    {
        var viewModelActivator = new ViewModelActivator();
        var deactivated = viewModelActivator.Deactivated.Collect();

        _ = viewModelActivator.Activate();
        viewModelActivator.Deactivate();

        await Assert.That(deactivated).Count().IsEqualTo(1);
    }

    /// <summary>Tests the deactivating ignoring reference count ticks deactivated observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TestDeactivatingIgnoringRefCountTicksDeactivatedObservable()
    {
        var viewModelActivator = new ViewModelActivator();
        var deactivated = viewModelActivator.Deactivated.Collect();

        viewModelActivator.Deactivate(true);

        await Assert.That(deactivated).Count().IsEqualTo(1);
    }

    /// <summary>Tests the disposing after activation deactivates view model.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TestDisposingAfterActivationDeactivatesViewModel()
    {
        var viewModelActivator = new ViewModelActivator();
        var activated = viewModelActivator.Activated.Collect();
        var deactivated = viewModelActivator.Deactivated.Collect();

        using (viewModelActivator.Activate())
        using (Assert.Multiple())
        {
            await Assert.That(activated).Count().IsEqualTo(1);
            await Assert.That(deactivated).IsEmpty();
        }

        using (Assert.Multiple())
        {
            await Assert.That(activated).Count().IsEqualTo(1);
            await Assert.That(deactivated).Count().IsEqualTo(1);
        }
    }
}
