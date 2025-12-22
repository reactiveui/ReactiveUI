// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

using static TUnit.Assertions.Assert;

namespace ReactiveUI.Tests;
public class ActivatingViewTests
{
    /// <summary>
    /// Tests to make sure that views generally activate.
    /// </summary>
    [Test]
    public void ActivatingViewSmokeTest()
    {
        AppBuilder.ResetBuilderStateForTests();
        var locator = new ModernDependencyResolver();
        locator.InitializeSplat();
        locator.InitializeReactiveUI();
        locator.Register(static () => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

        using (locator.WithResolver())
        {
            var vm = new ActivatingViewModel();
            var fixture = new ActivatingView
            {
                ViewModel = vm
            };
            using (Assert.EnterMultipleScope())
            {
                Assert.That(vm.IsActiveCount, Is.Zero);
                Assert.That(fixture.IsActiveCount, Is.Zero);
            }

            fixture.Loaded.OnNext(Unit.Default);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(vm.IsActiveCount, Is.EqualTo(1));
                Assert.That(fixture.IsActiveCount, Is.EqualTo(1));
            }

            fixture.Unloaded.OnNext(Unit.Default);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(vm.IsActiveCount, Is.Zero);
                Assert.That(fixture.IsActiveCount, Is.Zero);
            }
        }
    }

    /// <summary>
    /// Tests for making sure nulling the view model deactivate it.
    /// </summary>
    [Test]
    public void NullingViewModelDeactivateIt()
    {
        AppBuilder.ResetBuilderStateForTests();
        var locator = new ModernDependencyResolver();
        locator.InitializeSplat();
        locator.InitializeReactiveUI();
        locator.Register(static () => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

        using (locator.WithResolver())
        {
            var vm = new ActivatingViewModel();
            var fixture = new ActivatingView
            {
                ViewModel = vm
            };
            using (Assert.EnterMultipleScope())
            {
                Assert.That(vm.IsActiveCount, Is.Zero);
                Assert.That(fixture.IsActiveCount, Is.Zero);
            }

            fixture.Loaded.OnNext(Unit.Default);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(vm.IsActiveCount, Is.EqualTo(1));
                Assert.That(fixture.IsActiveCount, Is.EqualTo(1));
            }

            fixture.ViewModel = null;
            Assert.That(vm.IsActiveCount, Is.Zero);
        }
    }

    /// <summary>
    /// Tests switching the view model deactivates it.
    /// </summary>
    [Test]
    public void SwitchingViewModelDeactivatesIt()
    {
        AppBuilder.ResetBuilderStateForTests();
        var locator = new ModernDependencyResolver();
        locator.InitializeSplat();
        locator.InitializeReactiveUI();
        locator.Register(static () => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

        using (locator.WithResolver())
        {
            var vm = new ActivatingViewModel();
            var fixture = new ActivatingView
            {
                ViewModel = vm
            };
            using (Assert.EnterMultipleScope())
            {
                Assert.That(vm.IsActiveCount, Is.Zero);
                Assert.That(fixture.IsActiveCount, Is.Zero);
            }

            fixture.Loaded.OnNext(Unit.Default);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(vm.IsActiveCount, Is.EqualTo(1));
                Assert.That(fixture.IsActiveCount, Is.EqualTo(1));
            }

            var newVm = new ActivatingViewModel();
            Assert.That(newVm.IsActiveCount, Is.Zero);

            fixture.ViewModel = newVm;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(vm.IsActiveCount, Is.Zero);
                Assert.That(newVm.IsActiveCount, Is.EqualTo(1));
            }
        }
    }

    /// <summary>
    /// Tests setting the view model after loaded loads it.
    /// </summary>
    [Test]
    public void SettingViewModelAfterLoadedLoadsIt()
    {
        AppBuilder.ResetBuilderStateForTests();
        var locator = new ModernDependencyResolver();
        locator.InitializeSplat();
        locator.InitializeReactiveUI();
        locator.Register(static () => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

        using (locator.WithResolver())
        {
            var vm = new ActivatingViewModel();
            var fixture = new ActivatingView();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(vm.IsActiveCount, Is.Zero);
                Assert.That(fixture.IsActiveCount, Is.Zero);
            }

            fixture.Loaded.OnNext(Unit.Default);
            Assert.That(fixture.IsActiveCount, Is.EqualTo(1));

            fixture.ViewModel = vm;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(fixture.IsActiveCount, Is.EqualTo(1));
                Assert.That(vm.IsActiveCount, Is.EqualTo(1));
            }

            fixture.Unloaded.OnNext(Unit.Default);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(fixture.IsActiveCount, Is.Zero);
                Assert.That(vm.IsActiveCount, Is.Zero);
            }
        }
    }

    /// <summary>
    /// Tests the can unload and load view again.
    /// </summary>
    [Test]
    public void CanUnloadAndLoadViewAgain()
    {
        AppBuilder.ResetBuilderStateForTests();
        var locator = new ModernDependencyResolver();
        locator.InitializeSplat();
        locator.InitializeReactiveUI();
        locator.Register(static () => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

        using (locator.WithResolver())
        {
            var vm = new ActivatingViewModel();
            var fixture = new ActivatingView
            {
                ViewModel = vm
            };
            using (Assert.EnterMultipleScope())
            {
                Assert.That(vm.IsActiveCount, Is.Zero);
                Assert.That(fixture.IsActiveCount, Is.Zero);
            }

            fixture.Loaded.OnNext(Unit.Default);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(vm.IsActiveCount, Is.EqualTo(1));
                Assert.That(fixture.IsActiveCount, Is.EqualTo(1));
            }

            fixture.Unloaded.OnNext(Unit.Default);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(vm.IsActiveCount, Is.Zero);
                Assert.That(fixture.IsActiveCount, Is.Zero);
            }

            fixture.Loaded.OnNext(Unit.Default);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(vm.IsActiveCount, Is.EqualTo(1));
                Assert.That(fixture.IsActiveCount, Is.EqualTo(1));
            }
        }
    }
}