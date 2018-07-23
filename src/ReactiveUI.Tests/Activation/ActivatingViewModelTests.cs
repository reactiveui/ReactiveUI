// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace ReactiveUI.Tests
{
    public class ActivatingViewModelTests
    {
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

            // Refcount drops to zero
            fixture.Activator.Deactivate();
            Assert.Equal(0, fixture.IsActiveCount);
        }

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
