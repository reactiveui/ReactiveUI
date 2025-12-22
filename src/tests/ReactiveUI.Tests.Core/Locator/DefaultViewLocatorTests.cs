using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

using static TUnit.Assertions.Assert;
// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Core;
[NonParallelizable]
public partial class DefaultViewLocatorTests
{
    /// <summary>
    /// Diagnostic test to verify registration and resolution.
    /// </summary>
    [Test]
    public void DiagnosticTestForRegistrationAndResolution()
    {
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();

        // Register
        resolver.Register(() => new FooView(), typeof(IViewFor<FooViewModel>));

        // Verify registration
        var hasReg = resolver.HasRegistration(typeof(IViewFor<FooViewModel>));
        Assert.That(hasReg, Is.True, "Registration should exist");

        using (resolver.WithResolver())
        {
            // Test direct GetService
            var service = AppLocator.Current.GetService(typeof(IViewFor<FooViewModel>));
            Assert.That(service, Is.Not.Null, "GetService should return a service");
            Assert.That(service, Is.TypeOf<FooView>(), "Service should be FooView");

            // Test that the ViewLocator can find the view by manually checking the type
            var vmType = typeof(FooViewModel);
            var expectedViewForType = typeof(IViewFor<FooViewModel>);
            var manualService = AppLocator.Current.GetService(expectedViewForType);
            Assert.That(manualService, Is.Not.Null, $"Manual GetService for {expectedViewForType} should work");

            // Test through ViewLocator
            var fixture = new DefaultViewLocator();
            var vm = new FooViewModel();
            var result = fixture.ResolveView(vm);
            Assert.That(result, Is.Not.Null, $"ResolveView should return a result. VM type: {vm.GetType().FullName}");
            Assert.That(result, Is.TypeOf<FooView>(), "Result should be FooView");
        }
    }

    /// <summary>
    /// Tests that the default name of the view model is replaced with view when determining the service.
    /// </summary>
    [Test]
    public void ByDefaultViewModelIsReplacedWithViewWhenDeterminingTheServiceName()
    {
        var resolver = new ModernDependencyResolver();

        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();
        resolver.Register(static () => new FooView(), typeof(IViewFor<FooViewModel>));

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
        resolver.Register(static () => new FooView(), typeof(FooView));

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
        resolver.Register(static () => new FooWithWeirdConvention(), typeof(FooWithWeirdConvention));

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator
            {
                ViewModelToViewFunc =
                static viewModelName => viewModelName.Replace("ViewModel", "WithWeirdConvention")
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
        resolver.Register(static () => new FooView(), typeof(FooView));

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
        resolver.Register(static () => new FooView(), typeof(IFooView));

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
        resolver.Register(static () => new FooView(), typeof(IViewFor<FooViewModel>));

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
        resolver.Register(static () => new FooView(), typeof(FooView));

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
        resolver.Register(static () => new FooView(), typeof(IFooView));

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
        resolver.Register(static () => new FooView(), typeof(IViewFor<IFooViewModel>));

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
        resolver.Register(static () => new FooView(), typeof(IViewFor<IFooViewModel>), "first");
        resolver.Register(static () => new FooWithWeirdConvention(), typeof(IViewFor<IFooViewModel>), "second");

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
                ViewModelToViewFunc = static viewModelName =>
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
        resolver.Register(static () => "this string does not implement IViewFor", typeof(IViewFor<IFooViewModel>));

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator();
            var vm = new FooViewModel();

            var result = fixture.ResolveView(vm);
            Assert.That(result, Is.Null);
        }
    }

    /// <summary>
    /// Tests that null is returned if the creation of the view fails.
    /// </summary>
    [Test]
    public void NoErrorIsRaisedIfTheCreationOfTheViewFails()
    {
        var resolver = new ModernDependencyResolver();

        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();
        resolver.Register(() => new FooThatThrowsView(), typeof(IViewFor<IFooViewModel>));

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator();
            var vm = new FooViewModel();

            var result = fixture.ResolveView(vm);
            Assert.That(result, Is.Null);
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
}