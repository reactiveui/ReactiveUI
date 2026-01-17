// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

namespace ReactiveUI.Tests.Activation;

public class ViewModelActivatorTests
{
    /// <summary>
    ///     Tests the activating ticks activated observable.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TestActivatingTicksActivatedObservable()
    {
        var viewModelActivator = new ViewModelActivator();
        viewModelActivator.Activated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var activated)
            .Subscribe();

        viewModelActivator.Activate();

        await Assert.That(activated).Count().IsEqualTo(1);
    }

    /// <summary>
    ///     Tests the deactivating count doesnt tick deactivated observable.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TestDeactivatingCountDoesntTickDeactivatedObservable()
    {
        var viewModelActivator = new ViewModelActivator();
        viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated)
            .Subscribe();

        viewModelActivator.Deactivate();

        await Assert.That(deactivated).IsEmpty();
    }

    /// <summary>
    ///     Tests the deactivating following activating ticks deactivated observable.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TestDeactivatingFollowingActivatingTicksDeactivatedObservable()
    {
        var viewModelActivator = new ViewModelActivator();
        viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated)
            .Subscribe();

        viewModelActivator.Activate();
        viewModelActivator.Deactivate();

        await Assert.That(deactivated).Count().IsEqualTo(1);
    }

    /// <summary>
    ///     Tests the deactivating ignoring reference count ticks deactivated observable.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TestDeactivatingIgnoringRefCountTicksDeactivatedObservable()
    {
        var viewModelActivator = new ViewModelActivator();
        viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated)
            .Subscribe();

        viewModelActivator.Deactivate(true);

        await Assert.That(deactivated).Count().IsEqualTo(1);
    }

    /// <summary>
    ///     Tests the disposing after activation deactivates view model.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TestDisposingAfterActivationDeactivatesViewModel()
    {
        var viewModelActivator = new ViewModelActivator();
        viewModelActivator.Activated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var activated)
            .Subscribe();
        viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated)
            .Subscribe();

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
