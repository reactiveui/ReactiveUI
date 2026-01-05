// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Winforms;

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
        RxAppBuilder.CreateReactiveUIBuilder(_resolver)
            .WithCoreServices()
            .BuildApp();
        _resolver.RegisterViewsForViewModels(GetType().Assembly);
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
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

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task NonContractRegistrationsShouldResolveCorrectly()
    {
        using (_resolver.WithResolver())
        {
            await Assert.That(_resolver.GetService<IViewFor<AnotherViewModel>>()).IsTypeOf<AnotherView>();
        }
    }

    /// <inheritdoc/>
    public void Dispose() => _resolver?.Dispose();

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task ContractRegistrationsShouldResolveCorrectly()
    {
        using (_resolver.WithResolver())
        {
            await Assert.That(_resolver.GetService(typeof(IViewFor<ExampleViewModel>), "contract")).IsTypeOf<ContractExampleView>();
        }
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task SingleInstanceViewsShouldOnlyBeInstantiatedOnce()
    {
        using (_resolver.WithResolver())
        {
            await Assert.That(SingleInstanceExampleView.Instances).IsEqualTo(0);

            var instance = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>));
            await Assert.That(SingleInstanceExampleView.Instances).IsEqualTo(1);

            var instance2 = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>));
            using (Assert.Multiple())
            {
                await Assert.That(SingleInstanceExampleView.Instances).IsEqualTo(1);

                await Assert.That(instance2).IsSameReferenceAs(instance);
            }
        }
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task SingleInstanceViewsWithContractShouldResolveCorrectly()
    {
        using (_resolver.WithResolver())
        {
            await Assert.That(SingleInstanceWithContractExampleView.Instances).IsEqualTo(0);

            var instance = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>), "contract");
            await Assert.That(SingleInstanceWithContractExampleView.Instances).IsEqualTo(1);

            var instance2 = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>), "contract");
            using (Assert.Multiple())
            {
                await Assert.That(SingleInstanceWithContractExampleView.Instances).IsEqualTo(1);

                await Assert.That(instance2).IsSameReferenceAs(instance);
            }
        }
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task SingleInstanceViewsShouldOnlyBeInstantiatedWhenRequested()
    {
        using (_resolver.WithResolver())
        {
            await Assert.That(NeverUsedView.Instances).IsEqualTo(0);
        }
    }
}
