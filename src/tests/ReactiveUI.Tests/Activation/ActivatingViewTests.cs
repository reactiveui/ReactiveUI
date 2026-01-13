// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using Splat.Builder;

namespace ReactiveUI.Tests.Activation;

public class ActivatingViewTests
{
    /// <summary>
    ///     Tests to make sure that views generally activate.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ActivatingViewSmokeTest()
    {
        AppBuilder.ResetBuilderStateForTests();
        var locator = new ModernDependencyResolver();
        locator
            .CreateReactiveUIBuilder()
            .WithCoreServices()
            .WithCustomRegistration(builder =>
                builder.Register<IActivationForViewFetcher>(() => new ActivatingViewFetcher()))
            .BuildApp();

        using (locator.WithResolver())
        {
            var vm = new ActivatingViewModel();
            var fixture = new ActivatingView { ViewModel = vm };
            using (Assert.Multiple())
            {
                await Assert.That(vm.IsActiveCount).IsEqualTo(0);
                await Assert.That(fixture.IsActiveCount).IsEqualTo(0);
            }

            fixture.Loaded.OnNext(Unit.Default);
            using (Assert.Multiple())
            {
                await Assert.That(vm.IsActiveCount).IsEqualTo(1);
                await Assert.That(fixture.IsActiveCount).IsEqualTo(1);
            }

            fixture.Unloaded.OnNext(Unit.Default);
            using (Assert.Multiple())
            {
                await Assert.That(vm.IsActiveCount).IsEqualTo(0);
                await Assert.That(fixture.IsActiveCount).IsEqualTo(0);
            }
        }
    }

    /// <summary>
    ///     Tests the can unload and load view again.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanUnloadAndLoadViewAgain()
    {
        AppBuilder.ResetBuilderStateForTests();
        var locator = new ModernDependencyResolver();
        locator
            .CreateReactiveUIBuilder()
            .WithCoreServices()
            .WithCustomRegistration(builder =>
                builder.Register<IActivationForViewFetcher>(() => new ActivatingViewFetcher()))
            .BuildApp();

        using (locator.WithResolver())
        {
            var vm = new ActivatingViewModel();
            var fixture = new ActivatingView { ViewModel = vm };
            using (Assert.Multiple())
            {
                await Assert.That(vm.IsActiveCount).IsEqualTo(0);
                await Assert.That(fixture.IsActiveCount).IsEqualTo(0);
            }

            fixture.Loaded.OnNext(Unit.Default);
            using (Assert.Multiple())
            {
                await Assert.That(vm.IsActiveCount).IsEqualTo(1);
                await Assert.That(fixture.IsActiveCount).IsEqualTo(1);
            }

            fixture.Unloaded.OnNext(Unit.Default);
            using (Assert.Multiple())
            {
                await Assert.That(vm.IsActiveCount).IsEqualTo(0);
                await Assert.That(fixture.IsActiveCount).IsEqualTo(0);
            }

            fixture.Loaded.OnNext(Unit.Default);
            using (Assert.Multiple())
            {
                await Assert.That(vm.IsActiveCount).IsEqualTo(1);
                await Assert.That(fixture.IsActiveCount).IsEqualTo(1);
            }
        }
    }

    /// <summary>
    ///     Tests for making sure nulling the view model deactivate it.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullingViewModelDeactivateIt()
    {
        AppBuilder.ResetBuilderStateForTests();
        var locator = new ModernDependencyResolver();
        locator
            .CreateReactiveUIBuilder()
            .WithCoreServices()
            .WithCustomRegistration(builder =>
                builder.Register<IActivationForViewFetcher>(() => new ActivatingViewFetcher()))
            .BuildApp();

        using (locator.WithResolver())
        {
            var vm = new ActivatingViewModel();
            var fixture = new ActivatingView { ViewModel = vm };
            using (Assert.Multiple())
            {
                await Assert.That(vm.IsActiveCount).IsEqualTo(0);
                await Assert.That(fixture.IsActiveCount).IsEqualTo(0);
            }

            fixture.Loaded.OnNext(Unit.Default);
            using (Assert.Multiple())
            {
                await Assert.That(vm.IsActiveCount).IsEqualTo(1);
                await Assert.That(fixture.IsActiveCount).IsEqualTo(1);
            }

            fixture.ViewModel = null;
            await Assert.That(vm.IsActiveCount).IsEqualTo(0);
        }
    }

    /// <summary>
    ///     Tests setting the view model after loaded loads it.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SettingViewModelAfterLoadedLoadsIt()
    {
        AppBuilder.ResetBuilderStateForTests();
        var locator = new ModernDependencyResolver();
        locator
            .CreateReactiveUIBuilder()
            .WithCoreServices()
            .WithCustomRegistration(builder =>
                builder.Register<IActivationForViewFetcher>(() => new ActivatingViewFetcher()))
            .BuildApp();

        using (locator.WithResolver())
        {
            var vm = new ActivatingViewModel();
            var fixture = new ActivatingView();

            using (Assert.Multiple())
            {
                await Assert.That(vm.IsActiveCount).IsEqualTo(0);
                await Assert.That(fixture.IsActiveCount).IsEqualTo(0);
            }

            fixture.Loaded.OnNext(Unit.Default);
            await Assert.That(fixture.IsActiveCount).IsEqualTo(1);

            fixture.ViewModel = vm;
            using (Assert.Multiple())
            {
                await Assert.That(fixture.IsActiveCount).IsEqualTo(1);
                await Assert.That(vm.IsActiveCount).IsEqualTo(1);
            }

            fixture.Unloaded.OnNext(Unit.Default);
            using (Assert.Multiple())
            {
                await Assert.That(fixture.IsActiveCount).IsEqualTo(0);
                await Assert.That(vm.IsActiveCount).IsEqualTo(0);
            }
        }
    }

    /// <summary>
    ///     Tests switching the view model deactivates it.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SwitchingViewModelDeactivatesIt()
    {
        AppBuilder.ResetBuilderStateForTests();
        var locator = new ModernDependencyResolver();
        locator
            .CreateReactiveUIBuilder()
            .WithCoreServices()
            .WithCustomRegistration(builder =>
                builder.Register<IActivationForViewFetcher>(() => new ActivatingViewFetcher()))
            .BuildApp();

        using (locator.WithResolver())
        {
            var vm = new ActivatingViewModel();
            var fixture = new ActivatingView { ViewModel = vm };
            using (Assert.Multiple())
            {
                await Assert.That(vm.IsActiveCount).IsEqualTo(0);
                await Assert.That(fixture.IsActiveCount).IsEqualTo(0);
            }

            fixture.Loaded.OnNext(Unit.Default);
            using (Assert.Multiple())
            {
                await Assert.That(vm.IsActiveCount).IsEqualTo(1);
                await Assert.That(fixture.IsActiveCount).IsEqualTo(1);
            }

            var newVm = new ActivatingViewModel();
            await Assert.That(newVm.IsActiveCount).IsEqualTo(0);

            fixture.ViewModel = newVm;
            using (Assert.Multiple())
            {
                await Assert.That(vm.IsActiveCount).IsEqualTo(0);
                await Assert.That(newVm.IsActiveCount).IsEqualTo(1);
            }
        }
    }
}
