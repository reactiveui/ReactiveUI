// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Splat;
using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Tests for the default view locators.
    /// </summary>
    public class DefaultViewLocatorTests
    {
        /// <summary>
        /// Tests that the the default name of the view model is replaced with view when determining the service.
        /// </summary>
        [Fact]
        public void ByDefaultViewModelIsReplacedWithViewWhenDeterminingTheServiceName()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(IViewFor<FooViewModel>));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                var vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        /// <summary>
        /// Tests that the runtime type of the view model is used to resolve the view.
        /// </summary>
        [Fact]
        public void TheRuntimeTypeOfTheViewModelIsUsedToResolveTheView()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(FooView));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                object vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        /// <summary>
        /// Tests that the view model to view naming convention can be customized.
        /// </summary>
        [Fact]
        public void ViewModelToViewNamingConventionCanBeCustomized()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooWithWeirdConvention(), typeof(FooWithWeirdConvention));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                fixture.ViewModelToViewFunc =
                    viewModelName => viewModelName.Replace("ViewModel", "WithWeirdConvention");
                var vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooWithWeirdConvention>(result);
            }
        }

        /// <summary>
        /// Tests that makes sure that this instance [can resolve view from view model class using class registration].
        /// </summary>
        [Fact]
        public void CanResolveViewFromViewModelClassUsingClassRegistration()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(FooView));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                var vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        /// <summary>
        /// Tests that make sure this instance [can resolve view from view model class using interface registration].
        /// </summary>
        [Fact]
        public void CanResolveViewFromViewModelClassUsingInterfaceRegistration()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(IFooView));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                var vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        /// <summary>
        /// Test that makes sure that this instance [can resolve view from view model class using IView for registration].
        /// </summary>
        [Fact]
        public void CanResolveViewFromViewModelClassUsingIViewForRegistration()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(IViewFor<FooViewModel>));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                var vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        /// <summary>
        /// Tests that this instance [can resolve view from view model interface using class registration].
        /// </summary>
        [Fact]
        public void CanResolveViewFromViewModelInterfaceUsingClassRegistration()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(FooView));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                IFooViewModel vm = new FooViewModelWithWeirdName();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        /// <summary>
        /// Tests that this instance [can resolve view from view model interface using interface registration].
        /// </summary>
        [Fact]
        public void CanResolveViewFromViewModelInterfaceUsingInterfaceRegistration()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(IFooView));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                IFooViewModel vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        /// <summary>
        /// Tests that this instance [can resolve view from view model interface using i view for registration].
        /// </summary>
        [Fact]
        public void CanResolveViewFromViewModelInterfaceUsingIViewForRegistration()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(IViewFor<IFooViewModel>));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                IFooViewModel vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        /// <summary>
        /// Tests that contracts is used when resolving view.
        /// </summary>
        [Fact]
        public void ContractIsUsedWhenResolvingView()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(IViewFor<IFooViewModel>), "first");
            resolver.Register(() => new FooWithWeirdConvention(), typeof(IViewFor<IFooViewModel>), "second");

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                var vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.Null(result);

                result = fixture.ResolveView(vm, "first");
                Assert.IsType<FooView>(result);

                result = fixture.ResolveView(vm, "second");
                Assert.IsType<FooWithWeirdConvention>(result);
            }
        }

        /// <summary>
        /// Tests that no errors are raised if a type cannot be found.
        /// </summary>
        [Fact]
        public void NoErrorIsRaisedIfATypeCannotBeFound()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                fixture.ViewModelToViewFunc = viewModelName =>
                    "DoesNotExist, " + typeof(DefaultViewLocatorTests).Assembly.FullName;
                var vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.Null(result);
            }
        }

        /// <summary>
        /// Tests that no errors are raised if a service cannot be found.
        /// </summary>
        [Fact]
        public void NoErrorIsRaisedIfAServiceCannotBeFound()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                var vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.Null(result);
            }
        }

        /// <summary>
        /// Tests that no errors are raised if the service does not implement IViewFor.
        /// </summary>
        [Fact]
        public void NoErrorIsRaisedIfTheServiceDoesNotImplementIViewFor()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => "this string does not implement IViewFor", typeof(IViewFor<IFooViewModel>));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                var vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.Null(result);
            }
        }

        /// <summary>
        /// Tests that no errors are raised if the creation of the view fails.
        /// </summary>
        [Fact]
        public void AnErrorIsRaisedIfTheCreationOfTheViewFails()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooThatThrowsView(), typeof(IViewFor<IFooViewModel>));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                var vm = new FooViewModel();

                var ex = Assert.Throws<InvalidOperationException>(() => fixture.ResolveView(vm));
                Assert.Equal("This is a test failure.", ex.Message);
            }
        }

        /// <summary>
        /// Tests that with odd interface name doesnt throw exception.
        /// </summary>
        [Fact]
        public void WithOddInterfaceNameDoesntThrowException()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();

                var vm = new StrangeClassNotFollowingConvention();

                fixture.ResolveView((IStrangeInterfaceNotFollowingConvention)vm);
            }
        }

        /// <summary>
        /// Tests that whether this instance [can resolve view from view model with IRoutableViewModel].
        /// </summary>
        [Fact]
        public void CanResolveViewFromViewModelWithIRoutableViewModelType()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new RoutableFooView(), typeof(IViewFor<IRoutableFooViewModel>));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                var vm = new RoutableFooViewModel();

                var result = fixture.ResolveView<IRoutableViewModel>(vm);
                Assert.IsType<RoutableFooView>(result);
            }
        }

        /// <summary>
        /// Tests that make sure this instance [can override name resolution function].
        /// </summary>
        [Fact]
        public void CanOverrideNameResolutionFunc()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new RoutableFooCustomView());

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                fixture.ViewModelToViewFunc = x => x.Replace("ViewModel", "CustomView");
                var vm = new RoutableFooViewModel();

                var result = fixture.ResolveView<IRoutableViewModel>(vm);
                Assert.IsType<RoutableFooCustomView>(result);
            }
        }
    }
}
