// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Mocks;
using ReactiveUI.WinForms.Tests.Winforms.Mocks;
using Splat;
using Splat.Builder;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>Tests for resolving WinForms views registered for view models.</summary>
public sealed class WinFormsViewDependencyResolverTests : IDisposable
{
    /// <summary>The contract name used by contract-based view registrations.</summary>
    private const string Contract = "contract";

    /// <summary>The dependency resolver used for the tests.</summary>
    private readonly ModernDependencyResolver _resolver;

    /// <summary>Initializes a new instance of the <see cref="WinFormsViewDependencyResolverTests"/> class.</summary>
    public WinFormsViewDependencyResolverTests()
    {
        AppBuilder.ResetBuilderStateForTests();

        // Reset static counters to avoid cross-test interference when running entire suite
        SingleInstanceExampleView.ResetInstances();
        SingleInstanceWithContractExampleView.ResetInstances();
        NeverUsedView.ResetInstances();

        _resolver = new();
        _resolver.InitializeSplat();
        _ = _resolver.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();
        _resolver.RegisterViewsForViewModels(GetType().Assembly);
    }

    /// <summary>Tests that registering views for view models registers all expected views.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RegisterViewsForViewModelShouldRegisterAllViews()
    {
        using (_resolver.WithResolver())
        using (Assert.Multiple())
        {
            await Assert.That(_resolver.GetServices<IViewFor<ExampleViewModel>>()).Count().IsEqualTo(1);
            await Assert.That(_resolver.GetServices<IViewFor<AnotherViewModel>>()).Count().IsEqualTo(1);
            await Assert.That(_resolver.GetServices<IViewFor<ExampleWindowViewModel>>()).Count().IsEqualTo(1);
            await Assert.That(_resolver.GetServices<IViewFor<ViewModelWithWeirdName>>()).Count().IsEqualTo(1);
        }
    }

    /// <summary>Tests that non-contract view registrations resolve to the correct view type.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NonContractRegistrationsShouldResolveCorrectly()
    {
        using (_resolver.WithResolver())
        {
            await Assert.That(_resolver.GetService<IViewFor<AnotherViewModel>>()).IsTypeOf<AnotherView>();
        }
    }

    /// <inheritdoc/>
    public void Dispose() => _resolver?.Dispose();

    /// <summary>Tests that contract-based view registrations resolve to the correct view type.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ContractRegistrationsShouldResolveCorrectly()
    {
        using (_resolver.WithResolver())
        {
            await Assert.That(_resolver.GetService<IViewFor<ExampleViewModel>>(Contract)).IsTypeOf<ContractExampleView>();
        }
    }

    /// <summary>Tests that single-instance views are only instantiated once.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SingleInstanceViewsShouldOnlyBeInstantiatedOnce()
    {
        using (_resolver.WithResolver())
        {
            await Assert.That(SingleInstanceExampleView.Instances).IsEqualTo(0);

            var instance = _resolver.GetService<IViewFor<SingleInstanceExampleViewModel>>();
            await Assert.That(SingleInstanceExampleView.Instances).IsEqualTo(1);

            var instance2 = _resolver.GetService<IViewFor<SingleInstanceExampleViewModel>>();
            using (Assert.Multiple())
            {
                await Assert.That(SingleInstanceExampleView.Instances).IsEqualTo(1);

                await Assert.That(instance2).IsSameReferenceAs(instance);
            }
        }
    }

    /// <summary>Tests that single-instance views with a contract resolve correctly and are only instantiated once.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SingleInstanceViewsWithContractShouldResolveCorrectly()
    {
        using (_resolver.WithResolver())
        {
            await Assert.That(SingleInstanceWithContractExampleView.Instances).IsEqualTo(0);

            var instance = _resolver.GetService<IViewFor<SingleInstanceExampleViewModel>>(Contract);
            await Assert.That(SingleInstanceWithContractExampleView.Instances).IsEqualTo(1);

            var instance2 = _resolver.GetService<IViewFor<SingleInstanceExampleViewModel>>(Contract);
            using (Assert.Multiple())
            {
                await Assert.That(SingleInstanceWithContractExampleView.Instances).IsEqualTo(1);

                await Assert.That(instance2).IsSameReferenceAs(instance);
            }
        }
    }

    /// <summary>Tests that single-instance views are not instantiated until requested.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SingleInstanceViewsShouldOnlyBeInstantiatedWhenRequested()
    {
        using (_resolver.WithResolver())
        {
            await Assert.That(NeverUsedView.Instances).IsEqualTo(0);
        }
    }
}
