// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;
using FactAttribute = Xunit.WpfFactAttribute;

namespace ReactiveUI.Tests.Winforms;

[TestFixture]
public sealed class WinFormsViewDependencyResolverTests : IDisposable
{
    private readonly IDependencyResolver _resolver;

    public WinFormsViewDependencyResolverTests()
    {
        AppBuilder.ResetBuilderStateForTests();

        // Reset static counters to avoid cross-test interference when running entire suite
        SingleInstanceExampleView.ResetInstances();
        SingleInstanceWithContractExampleView.ResetInstances();
        NeverUsedView.ResetInstances();

        _resolver = new ModernDependencyResolver();
        _resolver.InitializeSplat();
        _resolver.InitializeReactiveUI();
        _resolver.RegisterViewsForViewModels(GetType().Assembly);
    }

    [Test]
    public void RegisterViewsForViewModelShouldRegisterAllViews()
    {
        using (_resolver.WithResolver())
        {
            Assert.That(_resolver.GetServices<IViewFor<ExampleViewModel>>(, Has.Exactly(1).Items));
            Assert.That(_resolver.GetServices<IViewFor<AnotherViewModel>>(, Has.Exactly(1).Items));
            Assert.That(_resolver.GetServices<IViewFor<ExampleWindowViewModel>>(, Has.Exactly(1).Items));
            Assert.That(_resolver.GetServices<IViewFor<ViewModelWithWeirdName>>(, Has.Exactly(1).Items));
        }
    }

    [Test]
    public void NonContractRegistrationsShouldResolveCorrectly()
    {
        using (_resolver.WithResolver())
        {
            Assert.That(_resolver.GetService<IViewFor<AnotherViewModel>>(, Is.TypeOf<AnotherView>()));
        }
    }

    /// <inheritdoc/>
    public void Dispose() => _resolver?.Dispose();

    [Test]
    public void ContractRegistrationsShouldResolveCorrectly()
    {
        using (_resolver.WithResolver())
        {
            Assert.That(_resolver.GetService(typeof(IViewFor<ExampleViewModel>, Is.TypeOf<ContractExampleView>()), "contract"));
        }
    }

    [Test]
    public void SingleInstanceViewsShouldOnlyBeInstantiatedOnce()
    {
        using (_resolver.WithResolver())
        {
            Assert.That(SingleInstanceExampleView.Instances, Is.EqualTo(0));

            var instance = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>));
            Assert.That(SingleInstanceExampleView.Instances, Is.EqualTo(1));

            var instance2 = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>));
            Assert.That(SingleInstanceExampleView.Instances, Is.EqualTo(1));

            Assert.Same(instance, instance2);
        }
    }

    [Test]
    public void SingleInstanceViewsWithContractShouldResolveCorrectly()
    {
        using (_resolver.WithResolver())
        {
            Assert.That(SingleInstanceWithContractExampleView.Instances, Is.EqualTo(0));

            var instance = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>), "contract");
            Assert.That(SingleInstanceWithContractExampleView.Instances, Is.EqualTo(1));

            var instance2 = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>), "contract");
            Assert.That(SingleInstanceWithContractExampleView.Instances, Is.EqualTo(1));

            Assert.Same(instance, instance2);
        }
    }

    [Test]
    public void SingleInstanceViewsShouldOnlyBeInstantiatedWhenRequested()
    {
        using (_resolver.WithResolver())
        {
            Assert.That(NeverUsedView.Instances, Is.EqualTo(0));
        }
    }
}
