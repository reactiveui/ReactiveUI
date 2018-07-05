// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System.Reactive.Concurrency;
using Xunit;

namespace ReactiveUI.Tests
{
    public class ViewModelActivatorTests
    {
        [Fact]
        public void ActivatingTicksActivatedObservable()
        {
            var viewModelActivator = new ViewModelActivator();
            var activated = viewModelActivator.Activated.CreateCollection(scheduler: ImmediateScheduler.Instance);

            viewModelActivator.Activate();

            Assert.Equal(1, activated.Count);
        }

        [Fact]
        public void DeactivatingIgnoringRefCountTicksDeactivatedObservable()
        {
            var viewModelActivator = new ViewModelActivator();
            var deactivated = viewModelActivator.Deactivated.CreateCollection(scheduler: ImmediateScheduler.Instance);

            viewModelActivator.Deactivate(true);

            Assert.Equal(1, deactivated.Count);
        }

        [Fact]
        public void DeactivatingCountDoesntTickDeactivatedObservable()
        {
            var viewModelActivator = new ViewModelActivator();
            var deactivated = viewModelActivator.Deactivated.CreateCollection(scheduler: ImmediateScheduler.Instance);

            viewModelActivator.Deactivate(false);

            Assert.Equal(0, deactivated.Count);
        }

        [Fact]
        public void DeactivatingFollowingActivatingTicksDeactivatedObservable()
        {
            var viewModelActivator = new ViewModelActivator();
            var deactivated = viewModelActivator.Deactivated.CreateCollection(scheduler: ImmediateScheduler.Instance);

            viewModelActivator.Activate();
            viewModelActivator.Deactivate(false);

            Assert.Equal(1, deactivated.Count);
        }

        [Fact]
        public void DisposingAfterActivationDeactivatesViewModel()
        {
            var viewModelActivator = new ViewModelActivator();
            var activated = viewModelActivator.Activated.CreateCollection();
            var deactivated = viewModelActivator.Deactivated.CreateCollection();

            using (viewModelActivator.Activate()) {
                Assert.Equal(1, activated.Count);
                Assert.Equal(0, deactivated.Count);
            }

            Assert.Equal(1, activated.Count);
            Assert.Equal(1, deactivated.Count);
        }
    }
}
