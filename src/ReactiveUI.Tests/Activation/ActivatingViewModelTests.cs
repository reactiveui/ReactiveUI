// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Tests associated with activating view models.
    /// </summary>
    public class ActivatingViewModelTests
    {
        /// <summary>
        /// Tests for the activation to make sure it activates the appropriate number of times.
        /// </summary>
        [Fact]
        public void ActivationsGetRefCounted()
        {
            var fixture = new ActivatingViewModel();
            Assert.Equal(0, fixture.IsActiveCount);

            fixture.Activator.Activate();
            Assert.Equal(1, fixture.IsActiveCount);

            fixture.Activator.Activate();
            Assert.Equal(1, fixture.IsActiveCount);

            fixture.Activator.Deactivate();
            Assert.Equal(1, fixture.IsActiveCount);

            // RefCount drops to zero
            fixture.Activator.Deactivate();
            Assert.Equal(0, fixture.IsActiveCount);
        }

        /// <summary>
        /// Tests to make sure the activations of derived classes don't get stomped.
        /// </summary>
        [Fact]
        public void DerivedActivationsDontGetStomped()
        {
            var fixture = new DerivedActivatingViewModel();
            Assert.Equal(0, fixture.IsActiveCount);
            Assert.Equal(0, fixture.IsActiveCountAlso);

            fixture.Activator.Activate();
            Assert.Equal(1, fixture.IsActiveCount);
            Assert.Equal(1, fixture.IsActiveCountAlso);

            fixture.Activator.Activate();
            Assert.Equal(1, fixture.IsActiveCount);
            Assert.Equal(1, fixture.IsActiveCountAlso);

            fixture.Activator.Deactivate();
            Assert.Equal(1, fixture.IsActiveCount);
            Assert.Equal(1, fixture.IsActiveCountAlso);

            fixture.Activator.Deactivate();
            Assert.Equal(0, fixture.IsActiveCount);
            Assert.Equal(0, fixture.IsActiveCountAlso);
        }
    }
}
