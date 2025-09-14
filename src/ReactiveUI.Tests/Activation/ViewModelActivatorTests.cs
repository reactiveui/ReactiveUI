// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the view model activator.
/// </summary>
[TestFixture]
public class ViewModelActivatorTests
{
    /// <summary>
    /// Tests the activating ticks activated observable.
    /// </summary>
    [Test]
    public void TestActivatingTicksActivatedObservable()
    {
        var viewModelActivator = new ViewModelActivator();
        viewModelActivator.Activated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

        viewModelActivator.Activate();

        Assert.That(activated, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests the deactivating ignoring reference count ticks deactivated observable.
    /// </summary>
    [Test]
    public void TestDeactivatingIgnoringRefCountTicksDeactivatedObservable()
    {
        var viewModelActivator = new ViewModelActivator();
        viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated).Subscribe();

        viewModelActivator.Deactivate(true);

        Assert.That(deactivated, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests the deactivating count doesnt tick deactivated observable.
    /// </summary>
    [Test]
    public void TestDeactivatingCountDoesntTickDeactivatedObservable()
    {
        var viewModelActivator = new ViewModelActivator();
        viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated).Subscribe();

        viewModelActivator.Deactivate(false);

        Assert.That(deactivated, Is.Empty);
    }

    /// <summary>
    /// Tests the deactivating following activating ticks deactivated observable.
    /// </summary>
    [Test]
    public void TestDeactivatingFollowingActivatingTicksDeactivatedObservable()
    {
        var viewModelActivator = new ViewModelActivator();
        viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated).Subscribe();

        viewModelActivator.Activate();
        viewModelActivator.Deactivate(false);

        Assert.That(deactivated, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests the disposing after activation deactivates view model.
    /// </summary>
    [Test]
    public void TestDisposingAfterActivationDeactivatesViewModel()
    {
        var viewModelActivator = new ViewModelActivator();
        viewModelActivator.Activated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var activated).Subscribe();
        viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated).Subscribe();

        using (viewModelActivator.Activate())
        using (Assert.EnterMultipleScope())
        {
            Assert.That(activated, Has.Count.EqualTo(1));
            Assert.That(deactivated, Is.Empty);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(activated, Has.Count.EqualTo(1));
            Assert.That(deactivated, Has.Count.EqualTo(1));
        }
    }
}
