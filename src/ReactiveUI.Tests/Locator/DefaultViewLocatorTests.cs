// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the default view locators.
/// </summary>
[TestFixture]
public class DefaultViewLocatorTests
{
    /// <summary>
    /// Tests that the default name of the view model is replaced with view when determining the service.
    /// </summary>
    [Test]
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
            Assert.That(result, Is.TypeOf<FooView>());
        }
    }

    /// <summary>
    /// Tests that the runtime type of the view model is used to resolve the view.
    /// </summary>
    [Test]
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
            Assert.That(result, Is.TypeOf<FooView>());
        }
    }

    /// <summary>
    /// Tests that the view model to view naming convention can be customized.
    /// </summary>
    [Test]
    public void ViewModelToViewNamingConventionCanBeCustomized()
    {
        var resolver = new ModernDependencyResolver();

        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();
        resolver.Register(() => new FooWithWeirdConvention(), typeof(FooWithWeirdConvention));

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator
            {
                ViewModelToViewFunc =
                viewModelName => viewModelName.Replace("ViewModel", "WithWeirdConvention")
            };
            var vm = new FooViewModel();

            var result = fixture.ResolveView(vm);
            Assert.That(result, Is.TypeOf<FooWithWeirdConvention>());
        }
    }

    /// <summary>
    /// Tests that makes sure that this instance [can resolve view from view model class using class registration].
    /// </summary>
    [Test]
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
            Assert.That(result, Is.TypeOf<FooView>());
        }
    }

    /// <summary>
    /// Tests that make sure this instance [can resolve view from view model class using interface registration].
    /// </summary>
    [Test]
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
            Assert.That(result, Is.TypeOf<FooView>());
        }
    }

    /// <summary>
    /// Test that makes sure that this instance [can resolve view from view model class using IView for registration].
    /// </summary>
    [Test]
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
            Assert.That(result, Is.TypeOf<FooView>());
        }
    }

    /// <summary>
    /// Tests that this instance [can resolve view from view model interface using class registration].
    /// </summary>
    [Test]
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
            Assert.That(result, Is.TypeOf<FooView>());
        }
    }

    /// <summary>
    /// Tests that this instance [can resolve view from view model interface using interface registration].
    /// </summary>
    [Test]
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
            Assert.That(result, Is.TypeOf<FooView>());
        }
    }

    /// <summary>
    /// Tests that this instance [can resolve view from view model interface using i view for registration].
    /// </summary>
    [Test]
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
            Assert.That(result, Is.TypeOf<FooView>());
        }
    }

    /// <summary>
    /// Tests that contracts is used when resolving view.
    /// </summary>
    [Test]
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
            Assert.That(result, Is.Null);

            result = fixture.ResolveView(vm, "first");
            Assert.That(result, Is.TypeOf<FooView>());

            result = fixture.ResolveView(vm, "second");
            Assert.That(result, Is.TypeOf<FooWithWeirdConvention>());
        }
    }

    /// <summary>
    /// Tests that no errors are raised if a type cannot be found.
    /// </summary>
    [Test]
    public void NoErrorIsRaisedIfATypeCannotBeFound()
    {
        var resolver = new ModernDependencyResolver();

        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator
            {
                ViewModelToViewFunc = viewModelName =>
                "DoesNotExist, " + typeof(DefaultViewLocatorTests).Assembly.FullName
            };
            var vm = new FooViewModel();

            var result = fixture.ResolveView(vm);
            Assert.That(result, Is.Null);
        }
    }

    /// <summary>
    /// Tests that no errors are raised if a service cannot be found.
    /// </summary>
    [Test]
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
            Assert.That(result, Is.Null);
        }
    }

    /// <summary>
    /// Tests that no errors are raised if the service does not implement IViewFor.
    /// </summary>
    [Test]
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
            Assert.That(result, Is.Null);
        }
    }

    /// <summary>
    /// Tests that no errors are raised if the creation of the view fails.
    /// </summary>
    [Test]
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
            Assert.That(ex.Message, Is.EqualTo("This is a test failure."));
        }
    }

    /// <summary>
    /// Tests that with odd interface name doesnt throw exception.
    /// </summary>
    [Test]
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
    [Test]
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
            Assert.That(result, Is.TypeOf<RoutableFooView>());
        }
    }

    /// <summary>
    /// Tests that make sure this instance [can override name resolution function].
    /// </summary>
    [Test]
    public void CanOverrideNameResolutionFunc()
    {
        var resolver = new ModernDependencyResolver();

        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();
        resolver.Register(() => new RoutableFooCustomView());

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator
            {
                ViewModelToViewFunc = x => x.Replace("ViewModel", "CustomView")
            };
            var vm = new RoutableFooViewModel();

            var result = fixture.ResolveView<IRoutableViewModel>(vm);
            Assert.That(result, Is.TypeOf<RoutableFooCustomView>());
        }
    }
}
