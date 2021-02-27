// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using DynamicData;
using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Tests for the view model activator.
    /// </summary>
    public class ViewModelActivatorTests
    {
        /// <summary>
        /// Tests the activating ticks activated observable.
        /// </summary>
        [Fact]
        public void TestActivatingTicksActivatedObservable()
        {
            var viewModelActivator = new ViewModelActivator();
            viewModelActivator.Activated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

            viewModelActivator.Activate();

            Assert.Equal(1, activated.Count);
        }

        /// <summary>
        /// Tests the deactivating ignoring reference count ticks deactivated observable.
        /// </summary>
        [Fact]
        public void TestDeactivatingIgnoringRefCountTicksDeactivatedObservable()
        {
            var viewModelActivator = new ViewModelActivator();
            viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated).Subscribe();

            viewModelActivator.Deactivate(true);

            Assert.Equal(1, deactivated.Count);
        }

        /// <summary>
        /// Tests the deactivating count doesnt tick deactivated observable.
        /// </summary>
        [Fact]
        public void TestDeactivatingCountDoesntTickDeactivatedObservable()
        {
            var viewModelActivator = new ViewModelActivator();
            viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated).Subscribe();

            viewModelActivator.Deactivate(false);

            Assert.Equal(0, deactivated.Count);
        }

        /// <summary>
        /// Tests the deactivating following activating ticks deactivated observable.
        /// </summary>
        [Fact]
        public void TestDeactivatingFollowingActivatingTicksDeactivatedObservable()
        {
            var viewModelActivator = new ViewModelActivator();
            viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated).Subscribe();

            viewModelActivator.Activate();
            viewModelActivator.Deactivate(false);

            Assert.Equal(1, deactivated.Count);
        }

        /// <summary>
        /// Tests the disposing after activation deactivates view model.
        /// </summary>
        [Fact]
        public void TestDisposingAfterActivationDeactivatesViewModel()
        {
            var viewModelActivator = new ViewModelActivator();
            viewModelActivator.Activated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var activated).Subscribe();
            viewModelActivator.Deactivated.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var deactivated).Subscribe();

            using (viewModelActivator.Activate())
            {
                Assert.Equal(1, activated.Count);
                Assert.Equal(0, deactivated.Count);
            }

            Assert.Equal(1, activated.Count);
            Assert.Equal(1, deactivated.Count);
        }
    }
}
