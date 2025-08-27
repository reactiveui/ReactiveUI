// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using FactAttribute = Xunit.WpfFactAttribute;

namespace ReactiveUI.Tests.Winforms;

public sealed class WinFormsViewDependencyResolverTests : AppBuilderTestBase, IDisposable
{
    private readonly IDependencyResolver _resolver;

    public WinFormsViewDependencyResolverTests()
    {
        // Reset static counters to avoid cross-test interference when running entire suite
        SingleInstanceExampleView.ResetInstances();
        SingleInstanceWithContractExampleView.ResetInstances();
        NeverUsedView.ResetInstances();

        _resolver = new ModernDependencyResolver();
        _resolver.InitializeSplat();
        _resolver.InitializeReactiveUI();
        _resolver.RegisterViewsForViewModels(GetType().Assembly);
    }

    [FactAttribute]
    public async Task RegisterViewsForViewModelShouldRegisterAllViews() =>
        await RunAppBuilderTestAsync(() =>
        {
            using (_resolver.WithResolver())
            {
                Assert.Single(_resolver.GetServices<IViewFor<ExampleViewModel>>());
                Assert.Single(_resolver.GetServices<IViewFor<AnotherViewModel>>());
                Assert.Single(_resolver.GetServices<IViewFor<ExampleWindowViewModel>>());
                Assert.Single(_resolver.GetServices<IViewFor<ViewModelWithWeirdName>>());
            }
        });

    [FactAttribute]
    public async Task NonContractRegistrationsShouldResolveCorrectly() =>
        await RunAppBuilderTestAsync(() =>
        {
            using (_resolver.WithResolver())
            {
                Assert.IsType<AnotherView>(_resolver.GetService<IViewFor<AnotherViewModel>>());
            }
        });

    /// <inheritdoc/>
    public void Dispose() => _resolver?.Dispose();

    [FactAttribute]
    public async Task ContractRegistrationsShouldResolveCorrectly() =>
        await RunAppBuilderTestAsync(() =>
        {
            using (_resolver.WithResolver())
            {
                Assert.IsType<ContractExampleView>(_resolver.GetService(typeof(IViewFor<ExampleViewModel>), "contract"));
            }
        });

    [FactAttribute]
    public async Task SingleInstanceViewsShouldOnlyBeInstantiatedOnce() =>
        await RunAppBuilderTestAsync(() =>
        {
            using (_resolver.WithResolver())
            {
                Assert.Equal(0, SingleInstanceExampleView.Instances);

                var instance = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>));
                Assert.Equal(1, SingleInstanceExampleView.Instances);

                var instance2 = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>));
                Assert.Equal(1, SingleInstanceExampleView.Instances);

                Assert.Same(instance, instance2);
            }
        });

    [FactAttribute]
    public async Task SingleInstanceViewsWithContractShouldResolveCorrectly() =>
        await RunAppBuilderTestAsync(() =>
        {
            using (_resolver.WithResolver())
            {
                Assert.Equal(0, SingleInstanceWithContractExampleView.Instances);

                var instance = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>), "contract");
                Assert.Equal(1, SingleInstanceWithContractExampleView.Instances);

                var instance2 = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>), "contract");
                Assert.Equal(1, SingleInstanceWithContractExampleView.Instances);

                Assert.Same(instance, instance2);
            }
        });

    [FactAttribute]
    public async Task SingleInstanceViewsShouldOnlyBeInstantiatedWhenRequested() =>
        await RunAppBuilderTestAsync(() =>
        {
            using (_resolver.WithResolver())
            {
                Assert.Equal(0, NeverUsedView.Instances);
            }
        });
}
