// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive.Concurrency;
using DynamicData;
using DynamicData.Binding;
using Xunit;


namespace ReactiveUI.Tests
{
    public class ViewModelActivatorTests
    {
        [Fact]
        public void ActivatingTicksActivatedObservable()
        {
            var viewModelActivator = new ViewModelActivator();
            viewModelActivator.Activated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

            viewModelActivator.Activate();

            Assert.Equal(1, activated.Count);
        }

        [Fact]
        public void DeactivatingIgnoringRefCountTicksDeactivatedObservable()
        {
            var viewModelActivator = new ViewModelActivator();
            viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated).Subscribe();

            viewModelActivator.Deactivate(true);

            Assert.Equal(1, deactivated.Count);
        }

        [Fact]
        public void DeactivatingCountDoesntTickDeactivatedObservable()
        {
            var viewModelActivator = new ViewModelActivator();
            viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated).Subscribe();

            viewModelActivator.Deactivate(false);

            Assert.Equal(0, deactivated.Count);
        }

        [Fact]
        public void DeactivatingFollowingActivatingTicksDeactivatedObservable()
        {
            var viewModelActivator = new ViewModelActivator();
            viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated).Subscribe();

            viewModelActivator.Activate();
            viewModelActivator.Deactivate(false);

            Assert.Equal(1, deactivated.Count);
        }

        [Fact]
        public void DisposingAfterActivationDeactivatesViewModel()
        {
            var viewModelActivator = new ViewModelActivator();
            viewModelActivator.Activated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var activated).Subscribe();
            viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated).Subscribe();

            using (viewModelActivator.Activate()) {
                Assert.Equal(1, activated.Count);
                Assert.Equal(0, deactivated.Count);
            }

            Assert.Equal(1, activated.Count);
            Assert.Equal(1, deactivated.Count);
        }
    }
}
