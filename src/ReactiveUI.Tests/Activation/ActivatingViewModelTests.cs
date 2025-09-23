// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Tests associated with activating view models.
/// </summary>
[TestFixture]
public class ActivatingViewModelTests
{
    /// <summary>
    /// Tests for the activation to make sure it activates the appropriate number of times.
    /// </summary>
    [Test]
    public void ActivationsGetRefCounted()
    {
        var fixture = new ActivatingViewModel();
        Assert.That(fixture.IsActiveCount, Is.Zero);

        fixture.Activator.Activate();
        Assert.That(fixture.IsActiveCount, Is.EqualTo(1));

        fixture.Activator.Activate();
        Assert.That(fixture.IsActiveCount, Is.EqualTo(1));

        fixture.Activator.Deactivate();
        Assert.That(fixture.IsActiveCount, Is.EqualTo(1));

        // RefCount drops to zero
        fixture.Activator.Deactivate();
        Assert.That(fixture.IsActiveCount, Is.Zero);
    }

    /// <summary>
    /// Tests to make sure the activations of derived classes don't get stomped.
    /// </summary>
    [Test]
    public void DerivedActivationsDontGetStomped()
    {
        var fixture = new DerivedActivatingViewModel();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.IsActiveCount, Is.Zero);
            Assert.That(fixture.IsActiveCountAlso, Is.Zero);
        }

        fixture.Activator.Activate();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.IsActiveCount, Is.EqualTo(1));
            Assert.That(fixture.IsActiveCountAlso, Is.EqualTo(1));
        }

        fixture.Activator.Activate();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.IsActiveCount, Is.EqualTo(1));
            Assert.That(fixture.IsActiveCountAlso, Is.EqualTo(1));
        }

        fixture.Activator.Deactivate();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.IsActiveCount, Is.EqualTo(1));
            Assert.That(fixture.IsActiveCountAlso, Is.EqualTo(1));
        }

        fixture.Activator.Deactivate();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.IsActiveCount, Is.Zero);
            Assert.That(fixture.IsActiveCountAlso, Is.Zero);
        }
    }
}
