// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

using static TUnit.Assertions.Assert;

namespace ReactiveUI.Tests.Core;

/// <summary>
/// Tests for the <see cref="DefaultViewLocator"/> class.
/// </summary>
[NotInParallel]
public partial class DefaultViewLocatorTests
{
    /// <summary>
    /// Diagnostic test to verify registration and resolution.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DiagnosticTestForRegistrationAndResolution()
    {
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();

        // Register
        resolver.Register(() => new FooView(), typeof(IViewFor<FooViewModel>));

        // Verify registration
        var hasReg = resolver.HasRegistration(typeof(IViewFor<FooViewModel>));
        await Assert.That(hasReg).IsTrue();

        using (resolver.WithResolver())
        {
            // Test direct GetService
            var service = AppLocator.Current.GetService(typeof(IViewFor<FooViewModel>));
            await Assert.That(service).IsNotNull();
            await Assert.That(service).IsTypeOf<FooView>();

            // Test that the ViewLocator can find the view by manually checking the type
            var vmType = typeof(FooViewModel);
            var expectedViewForType = typeof(IViewFor<FooViewModel>);
            var manualService = AppLocator.Current.GetService(expectedViewForType);
            await Assert.That(manualService).IsNotNull();

            // Test through ViewLocator
            var fixture = new DefaultViewLocator();
            var vm = new FooViewModel();
            var result = fixture.ResolveView(vm);
            await Assert.That(result).IsNotNull();
            await Assert.That(result).IsTypeOf<FooView>();
        }
    }

    /// <summary>
    /// Tests that the default name of the view model is replaced with view when determining the service.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ByDefaultViewModelIsReplacedWithViewWhenDeterminingTheServiceName()
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
            await Assert.That(result).IsTypeOf<FooView>();
        }
    }

    /// <summary>
    /// Tests that the runtime type of the view model is used to resolve the view.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TheRuntimeTypeOfTheViewModelIsUsedToResolveTheView()
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
            await Assert.That(result).IsTypeOf<FooView>();
        }
    }

    /// <summary>
    /// Tests that the view model to view naming convention can be customized.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelToViewNamingConventionCanBeCustomized()
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
            await Assert.That(result).IsTypeOf<FooWithWeirdConvention>();
        }
    }

    /// <summary>
    /// Tests that makes sure that this instance [can resolve view from view model class using class registration].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanResolveViewFromViewModelClassUsingClassRegistration()
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
            await Assert.That(result).IsTypeOf<FooView>();
        }
    }

    /// <summary>
    /// Tests that make sure this instance [can resolve view from view model class using interface registration].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanResolveViewFromViewModelClassUsingInterfaceRegistration()
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
            await Assert.That(result).IsTypeOf<FooView>();
        }
    }

    /// <summary>
    /// Test that makes sure that this instance [can resolve view from view model class using IView for registration].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanResolveViewFromViewModelClassUsingIViewForRegistration()
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
            await Assert.That(result).IsTypeOf<FooView>();
        }
    }

    /// <summary>
    /// Tests that this instance [can resolve view from view model interface using class registration].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanResolveViewFromViewModelInterfaceUsingClassRegistration()
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
            await Assert.That(result).IsTypeOf<FooView>();
        }
    }

    /// <summary>
    /// Tests that this instance [can resolve view from view model interface using interface registration].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanResolveViewFromViewModelInterfaceUsingInterfaceRegistration()
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
            await Assert.That(result).IsTypeOf<FooView>();
        }
    }

    /// <summary>
    /// Tests that this instance [can resolve view from view model interface using i view for registration].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanResolveViewFromViewModelInterfaceUsingIViewForRegistration()
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
            await Assert.That(result).IsTypeOf<FooView>();
        }
    }

    /// <summary>
    /// Tests that contracts is used when resolving view.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ContractIsUsedWhenResolvingView()
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
            await Assert.That(result).IsNull();

            result = fixture.ResolveView(vm, "first");
            await Assert.That(result).IsTypeOf<FooView>();

            result = fixture.ResolveView(vm, "second");
            await Assert.That(result).IsTypeOf<FooWithWeirdConvention>();
        }
    }

    /// <summary>
    /// Tests that no errors are raised if a type cannot be found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NoErrorIsRaisedIfATypeCannotBeFound()
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
            await Assert.That(result).IsNull();
        }
    }

    /// <summary>
    /// Tests that no errors are raised if a service cannot be found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NoErrorIsRaisedIfAServiceCannotBeFound()
    {
        var resolver = new ModernDependencyResolver();

        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator();
            var vm = new FooViewModel();

            var result = fixture.ResolveView(vm);
            await Assert.That(result).IsNull();
        }
    }

    /// <summary>
    /// Tests that no errors are raised if the service does not implement IViewFor.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NoErrorIsRaisedIfTheServiceDoesNotImplementIViewFor()
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
            await Assert.That(result).IsNull();
        }
    }

    /// <summary>
    /// Tests that an exception is thrown if the creation of the view fails.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExceptionIsThrownIfTheCreationOfTheViewFails()
    {
        var resolver = new ModernDependencyResolver();

        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();
        resolver.Register(() => new FooThatThrowsView(), typeof(IViewFor<IFooViewModel>));

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator();
            var vm = new FooViewModel();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                fixture.ResolveView(vm);
                await Task.CompletedTask;
            });

            await Assert.That(ex!.Message).IsEqualTo("This is a test failure.");
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