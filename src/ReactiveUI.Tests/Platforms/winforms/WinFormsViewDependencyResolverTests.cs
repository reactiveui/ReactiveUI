// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;
using Xunit;

using FactAttribute = Xunit.WpfFactAttribute;

namespace ReactiveUI.Tests.Winforms
{
    public sealed class WinFormsViewDependencyResolverTests : IDisposable
    {
        private readonly IDependencyResolver _resolver;

        public WinFormsViewDependencyResolverTests()
        {
            _resolver = new ModernDependencyResolver();
            _resolver.InitializeSplat();
            _resolver.InitializeReactiveUI();
            _resolver.RegisterViewsForViewModels(GetType().Assembly);
        }

        [Fact]
        public void RegisterViewsForViewModelShouldRegisterAllViews()
        {
            using (_resolver.WithResolver())
            {
                Assert.Single(_resolver.GetServices<IViewFor<ExampleViewModel>>());
                Assert.Single(_resolver.GetServices<IViewFor<AnotherViewModel>>());
                Assert.Single(_resolver.GetServices<IViewFor<ExampleWindowViewModel>>());
                Assert.Single(_resolver.GetServices<IViewFor<ViewModelWithWeirdName>>());
            }
        }

        [Fact]
        public void NonContractRegistrationsShouldResolveCorrectly()
        {
            using (_resolver.WithResolver())
            {
                Assert.IsType<AnotherView>(_resolver.GetService<IViewFor<AnotherViewModel>>());
            }
        }

        /// <inheritdoc/>
        public void Dispose() => _resolver?.Dispose();

        [Fact]
        public void ContractRegistrationsShouldResolveCorrectly()
        {
            using (_resolver.WithResolver())
            {
                Assert.IsType<ContractExampleView>(_resolver.GetService(typeof(IViewFor<ExampleViewModel>), "contract"));
            }
        }

        [Fact]
        public void SingleInstanceViewsShouldOnlyBeInstantiatedOnce()
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
        }

        [Fact]
        public void SingleInstanceViewsWithContractShouldResolveCorrectly()
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
        }

        [Fact]
        public void SingleInstanceViewsShouldOnlyBeInstantiatedWhenRequested()
        {
            using (_resolver.WithResolver())
            {
                Assert.Equal(0, NeverUsedView.Instances);
            }
        }
    }
}
