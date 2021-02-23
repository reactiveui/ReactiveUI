// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using Splat;
using Xunit;

namespace ReactiveUI.Tests
{
    public class ActivatingViewTests
    {
        [Fact]
        public void ActivatingViewSmokeTest()
        {
            var locator = new ModernDependencyResolver();
            locator.InitializeSplat();
            locator.InitializeReactiveUI();
            locator.Register(() => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

            using (locator.WithResolver())
            {
                var vm = new ActivatingViewModel();
                var fixture = new ActivatingView();

                fixture.ViewModel = vm;
                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(0, fixture.IsActiveCount);

                fixture.Loaded.OnNext(Unit.Default);
                Assert.Equal(1, vm.IsActiveCount);
                Assert.Equal(1, fixture.IsActiveCount);

                fixture.Unloaded.OnNext(Unit.Default);
                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(0, fixture.IsActiveCount);
            }
        }

        [Fact]
        public void NullingViewModelShouldDeactivateIt()
        {
            var locator = new ModernDependencyResolver();
            locator.InitializeSplat();
            locator.InitializeReactiveUI();
            locator.Register(() => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

            using (locator.WithResolver())
            {
                var vm = new ActivatingViewModel();
                var fixture = new ActivatingView();

                fixture.ViewModel = vm;
                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(0, fixture.IsActiveCount);

                fixture.Loaded.OnNext(Unit.Default);
                Assert.Equal(1, vm.IsActiveCount);
                Assert.Equal(1, fixture.IsActiveCount);

                fixture.ViewModel = null;
                Assert.Equal(0, vm.IsActiveCount);
            }
        }

        [Fact]
        public void SwitchingViewModelShouldDeactivateIt()
        {
            var locator = new ModernDependencyResolver();
            locator.InitializeSplat();
            locator.InitializeReactiveUI();
            locator.Register(() => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

            using (locator.WithResolver())
            {
                var vm = new ActivatingViewModel();
                var fixture = new ActivatingView();

                fixture.ViewModel = vm;
                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(0, fixture.IsActiveCount);

                fixture.Loaded.OnNext(Unit.Default);
                Assert.Equal(1, vm.IsActiveCount);
                Assert.Equal(1, fixture.IsActiveCount);

                var newVm = new ActivatingViewModel();
                Assert.Equal(0, newVm.IsActiveCount);

                fixture.ViewModel = newVm;
                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(1, newVm.IsActiveCount);
            }
        }

        [Fact]
        public void SettingViewModelAfterLoadedShouldLoadIt()
        {
            var locator = new ModernDependencyResolver();
            locator.InitializeSplat();
            locator.InitializeReactiveUI();
            locator.Register(() => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

            using (locator.WithResolver())
            {
                var vm = new ActivatingViewModel();
                var fixture = new ActivatingView();

                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(0, fixture.IsActiveCount);

                fixture.Loaded.OnNext(Unit.Default);
                Assert.Equal(1, fixture.IsActiveCount);

                fixture.ViewModel = vm;
                Assert.Equal(1, fixture.IsActiveCount);
                Assert.Equal(1, vm.IsActiveCount);

                fixture.Unloaded.OnNext(Unit.Default);
                Assert.Equal(0, fixture.IsActiveCount);
                Assert.Equal(0, vm.IsActiveCount);
            }
        }

        [Fact]
        public void CanUnloadAndLoadViewAgain()
        {
            var locator = new ModernDependencyResolver();
            locator.InitializeSplat();
            locator.InitializeReactiveUI();
            locator.Register(() => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

            using (locator.WithResolver())
            {
                var vm = new ActivatingViewModel();
                var fixture = new ActivatingView();

                fixture.ViewModel = vm;
                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(0, fixture.IsActiveCount);

                fixture.Loaded.OnNext(Unit.Default);
                Assert.Equal(1, vm.IsActiveCount);
                Assert.Equal(1, fixture.IsActiveCount);

                fixture.Unloaded.OnNext(Unit.Default);
                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(0, fixture.IsActiveCount);

                fixture.Loaded.OnNext(Unit.Default);
                Assert.Equal(1, vm.IsActiveCount);
                Assert.Equal(1, fixture.IsActiveCount);
            }
        }
    }
}
