// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;

using Splat;

using Xunit;

namespace ReactiveUI.Tests
{
    public class DefaultViewLocatorTests
    {
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

        [Fact]
        [SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Not in all platforms.")]
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

        [Fact]
        [SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Not in all frameworks.")]
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
