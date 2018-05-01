// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

namespace ReactiveUI.Tests
{
    using Splat;
    using TestViewModels;
    using TestViews;
    using Xunit;

    namespace TestViewModels
    {
        public class ExampleViewModel : ReactiveObject { }
        public class AnotherViewModel : ReactiveObject { }

        public class NeverUsedViewModel : ReactiveObject { }

        public class SingleInstanceExampleViewModel : ReactiveObject { }

        public class ViewModelWithWeirdName : ReactiveObject { }
    }

    namespace TestViews
    {
        using TestViewModels;

        public class ExampleView : ReactiveUserControl<ExampleViewModel> { }
        public class AnotherView : ReactiveUserControl<AnotherViewModel> { }

        [ViewContract("contract")]
        public class ContractExampleView : ReactiveUserControl<ExampleViewModel> { }

        [SingleInstanceView]
        public class NeverUsedView : ReactiveUserControl<NeverUsedViewModel>
        {
            public static int Instances = 0;

            public NeverUsedView()
            {
                Instances++;
            }
        }

        [SingleInstanceView]
        public class SingleInstanceExampleView : ReactiveUserControl<SingleInstanceExampleViewModel>
        {
            public static int Instances = 0;

            public SingleInstanceExampleView()
            {
                Instances++;
            }
        }

        [ViewContract("contract")]
        [SingleInstanceView]
        public class SingleInstanceWithContractExampleView : ReactiveUserControl<SingleInstanceExampleViewModel>
        {
            public static int Instances = 0;

            public SingleInstanceWithContractExampleView()
            {
                Instances++;
            }
        }

        public class ViewWithoutMatchingName : ReactiveUserControl<ViewModelWithWeirdName> { }
    }

    public class DependencyResolverTests
    {
        readonly IMutableDependencyResolver resolver;

        public DependencyResolverTests()
        {
            resolver = new ModernDependencyResolver();
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.RegisterViewsForViewModels(GetType().Assembly);
        }

        [WpfFact]
        public void RegisterViewsForViewModelShouldRegisterAllViews()
        {
            using (resolver.WithResolver()) {
                Assert.Single(resolver.GetServices<IViewFor<ExampleViewModel>>());
                Assert.Single(resolver.GetServices<IViewFor<AnotherViewModel>>());
                Assert.Single(resolver.GetServices<IViewFor<ViewModelWithWeirdName>>());
            }
        }

        [WpfFact]
        public void RegisterViewsForViewModelShouldIncludeContracts()
        {
            using (resolver.WithResolver()) {
                Assert.Single(resolver.GetServices(typeof(IViewFor<ExampleViewModel>), "contract"));
            }
        }

        [WpfFact]
        public void NonContractRegistrationsShouldResolveCorrectly()
        {
            using (resolver.WithResolver()) {
                Assert.IsType<AnotherView>(resolver.GetService<IViewFor<AnotherViewModel>>());
            }
        }

        [WpfFact]
        public void ContractRegistrationsShouldResolveCorrectly()
        {
            using (resolver.WithResolver()) {
                Assert.IsType<ContractExampleView>(resolver.GetService(typeof(IViewFor<ExampleViewModel>), "contract"));
            }
        }

        [Fact]
        public void SingleInstanceViewsShouldOnlyBeInstantiatedWhenRequested()
        {
            using (resolver.WithResolver()) {
                Assert.Equal(0, NeverUsedView.Instances);
            }
        }

        [WpfFact]
        public void SingleInstanceViewsShouldOnlyBeInstantiatedOnce()
        {
            using (resolver.WithResolver()) {
                Assert.Equal(0, SingleInstanceExampleView.Instances);

                var instance = resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>));
                Assert.Equal(1, SingleInstanceExampleView.Instances);

                var instance2 = resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>));
                Assert.Equal(1, SingleInstanceExampleView.Instances);

                Assert.Same(instance, instance2);
            }
        }

        [WpfFact]
        public void SingleInstanceViewsWithContractShouldResolveCorrectly()
        {
            using (resolver.WithResolver()) {
                Assert.Equal(0, SingleInstanceWithContractExampleView.Instances);

                var instance = resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>), "contract");
                Assert.Equal(1, SingleInstanceWithContractExampleView.Instances);

                var instance2 = resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>), "contract");
                Assert.Equal(1, SingleInstanceWithContractExampleView.Instances);

                Assert.Same(instance, instance2);
            }
        }
    }
}