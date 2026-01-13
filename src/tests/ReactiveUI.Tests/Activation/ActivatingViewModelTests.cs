// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Activation;

public class ActivatingViewModelTests
{
    /// <summary>
    ///     Tests for the activation to make sure it activates the appropriate number of times.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ActivationsGetRefCounted()
    {
        var fixture = new ActivatingViewModel();
        await Assert.That(fixture.IsActiveCount).IsEqualTo(0);

        fixture.Activator.Activate();
        await Assert.That(fixture.IsActiveCount).IsEqualTo(1);

        fixture.Activator.Activate();
        await Assert.That(fixture.IsActiveCount).IsEqualTo(1);

        fixture.Activator.Deactivate();
        await Assert.That(fixture.IsActiveCount).IsEqualTo(1);

        // RefCount drops to zero
        fixture.Activator.Deactivate();
        await Assert.That(fixture.IsActiveCount).IsEqualTo(0);
    }

    /// <summary>
    ///     Tests to make sure the activations of derived classes don't get stomped.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DerivedActivationsDontGetStomped()
    {
        var fixture = new DerivedActivatingViewModel();
        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsActiveCount).IsEqualTo(0);
            await Assert.That(fixture.IsActiveCountAlso).IsEqualTo(0);
        }

        fixture.Activator.Activate();
        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsActiveCount).IsEqualTo(1);
            await Assert.That(fixture.IsActiveCountAlso).IsEqualTo(1);
        }

        fixture.Activator.Activate();
        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsActiveCount).IsEqualTo(1);
            await Assert.That(fixture.IsActiveCountAlso).IsEqualTo(1);
        }

        fixture.Activator.Deactivate();
        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsActiveCount).IsEqualTo(1);
            await Assert.That(fixture.IsActiveCountAlso).IsEqualTo(1);
        }

        fixture.Activator.Deactivate();
        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsActiveCount).IsEqualTo(0);
            await Assert.That(fixture.IsActiveCountAlso).IsEqualTo(0);
        }
    }
}
