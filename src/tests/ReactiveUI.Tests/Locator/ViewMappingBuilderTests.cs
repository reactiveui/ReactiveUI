// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;
using Splat.Builder;

namespace ReactiveUI.Tests.Locator;

/// <summary>
///     Tests for the ViewMappingBuilder which provides a fluent interface for registering
///     AOT-compatible view-to-viewmodel mappings.
/// </summary>
[NotInParallel]
[TestExecutor<AppBuilderTestExecutor>]
public class ViewMappingBuilderTests
{
    [Test]
    public async Task Map_WithParameterlessConstructor_ShouldRegisterAndResolveView()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);

        // Act
        builder.Map<TestViewModel, TestView>();
        var view = locator.ResolveView<TestViewModel>();

        // Assert
        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<TestView>();
    }

    [Test]
    public async Task Map_WithParameterlessConstructor_ShouldSupportContracts()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);

        // Act
        builder
            .Map<TestViewModel, TestView>("contract1")
            .Map<AlternateViewModel, AlternateView>("contract2");

        var view1 = locator.ResolveView<TestViewModel>("contract1");
        var view2 = locator.ResolveView<AlternateViewModel>("contract2");

        // Assert
        await Assert.That(view1).IsTypeOf<TestView>();
        await Assert.That(view2).IsTypeOf<AlternateView>();
    }

    [Test]
    public async Task Map_WithParameterlessConstructor_ShouldReturnBuilder()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);

        // Act
        var result = builder.Map<TestViewModel, TestView>();

        // Assert
        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task Map_WithParameterlessConstructor_ShouldAllowChaining()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);

        // Act
        builder
            .Map<TestViewModel, TestView>()
            .Map<AlternateViewModel, AlternateView>();

        var view1 = locator.ResolveView<TestViewModel>();
        var view2 = locator.ResolveView<AlternateViewModel>();

        // Assert
        await Assert.That(view1).IsTypeOf<TestView>();
        await Assert.That(view2).IsTypeOf<AlternateView>();
    }

    [Test]
    public async Task Map_WithFactory_ShouldThrowArgumentNullException_WhenFactoryIsNull()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);

        // Act & Assert
        await Assert.That(() => builder.Map<TestViewModel, TestView>((Func<TestView>)null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task Map_WithFactory_ShouldRegisterAndResolveView()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);
        var createdView = new TestView();

        // Act
        builder.Map<TestViewModel, TestView>(() => createdView);
        var view = locator.ResolveView<TestViewModel>();

        // Assert
        await Assert.That(view).IsSameReferenceAs(createdView);
    }

    [Test]
    public async Task Map_WithFactory_ShouldSupportContracts()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);
        var view1Instance = new TestView();
        var view2Instance = new TestView();

        // Act
        builder
            .Map<TestViewModel, TestView>(() => view1Instance, "contract1")
            .Map<TestViewModel, TestView>(() => view2Instance, "contract2");

        var view1 = locator.ResolveView<TestViewModel>("contract1");
        var view2 = locator.ResolveView<TestViewModel>("contract2");

        // Assert
        await Assert.That(view1).IsSameReferenceAs(view1Instance);
        await Assert.That(view2).IsSameReferenceAs(view2Instance);
    }

    [Test]
    public async Task Map_WithFactory_ShouldCallFactoryOnEachResolve()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);
        var callCount = 0;

        // Act
        builder.Map<TestViewModel, TestView>(() =>
        {
            callCount++;
            return new TestView();
        });

        var view1 = locator.ResolveView<TestViewModel>();
        var view2 = locator.ResolveView<TestViewModel>();

        // Assert
        await Assert.That(callCount).IsEqualTo(2);
        await Assert.That(view1).IsNotSameReferenceAs(view2);
    }

    [Test]
    public async Task Map_WithFactory_ShouldReturnBuilder()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);

        // Act
        var result = builder.Map<TestViewModel, TestView>(() => new TestView());

        // Assert
        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task MapFromServiceLocator_ShouldResolveViewFromServiceLocator()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);
        var viewInstance = new TestView();

        // Register view in service locator
        var resolver = AppLocator.Current as IDependencyResolver;
        ArgumentNullException.ThrowIfNull(resolver);
        resolver.Register(() => viewInstance, typeof(TestView));

        // Act
        builder.MapFromServiceLocator<TestViewModel, TestView>();
        var view = locator.ResolveView<TestViewModel>();

        // Assert
        await Assert.That(view).IsSameReferenceAs(viewInstance);
    }

    [Test]
    public async Task MapFromServiceLocator_ShouldSupportContracts()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);
        var view1Instance = new TestView();
        var view2Instance = new TestView();

        // Register views in service locator
        var resolver = AppLocator.Current as IDependencyResolver;
        ArgumentNullException.ThrowIfNull(resolver);
        resolver.Register(() => view1Instance, typeof(TestView));
        resolver.Register(() => view2Instance, typeof(TestView));

        // Act
        builder
            .MapFromServiceLocator<TestViewModel, TestView>("contract1")
            .MapFromServiceLocator<TestViewModel, TestView>("contract2");

        // Note: Contract is used for view locator registration
        var view1 = locator.ResolveView<TestViewModel>("contract1");
        var view2 = locator.ResolveView<TestViewModel>("contract2");

        // Assert
        await Assert.That(view1).IsNotNull();
        await Assert.That(view2).IsNotNull();
    }

    [Test]
    public async Task MapFromServiceLocator_ShouldThrowInvalidOperationException_WhenViewNotRegistered()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);

        // Act
        builder.MapFromServiceLocator<TestViewModel, UnregisteredView>();

        // Assert - Should throw when trying to resolve
        await Assert.That(() => locator.ResolveView<TestViewModel>())
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task MapFromServiceLocator_ShouldReturnBuilder()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);

        // Register view in service locator
        var resolver = AppLocator.Current as IDependencyResolver;
        ArgumentNullException.ThrowIfNull(resolver);
        resolver.Register(() => new TestView(), typeof(TestView));

        // Act
        var result = builder.MapFromServiceLocator<TestViewModel, TestView>();

        // Assert
        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task MapFromServiceLocator_ShouldAllowChaining()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);

        // Register views in service locator
        var resolver = AppLocator.Current as IDependencyResolver;
        ArgumentNullException.ThrowIfNull(resolver);
        resolver.Register(() => new TestView(), typeof(TestView));
        resolver.Register(() => new AlternateView(), typeof(AlternateView));

        // Act
        builder
            .MapFromServiceLocator<TestViewModel, TestView>()
            .MapFromServiceLocator<AlternateViewModel, AlternateView>();

        var view1 = locator.ResolveView<TestViewModel>();
        var view2 = locator.ResolveView<AlternateViewModel>();

        // Assert
        await Assert.That(view1).IsTypeOf<TestView>();
        await Assert.That(view2).IsTypeOf<AlternateView>();
    }

    [Test]
    public async Task Builder_ShouldAllowMixedRegistrationTypes()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);
        var factoryView = new AlternateView();

        // Register one view in service locator
        var resolver = AppLocator.Current as IDependencyResolver;
        ArgumentNullException.ThrowIfNull(resolver);
        resolver.Register(() => new AnotherView(), typeof(AnotherView));

        // Act - Mix all three registration types
        builder
            .Map<TestViewModel, TestView>() // Parameterless constructor
            .Map<AlternateViewModel, AlternateView>(() => factoryView) // Factory
            .MapFromServiceLocator<AnotherViewModel, AnotherView>(); // Service locator

        var view1 = locator.ResolveView<TestViewModel>();
        var view2 = locator.ResolveView<AlternateViewModel>();
        var view3 = locator.ResolveView<AnotherViewModel>();

        // Assert
        await Assert.That(view1).IsTypeOf<TestView>();
        await Assert.That(view2).IsSameReferenceAs(factoryView);
        await Assert.That(view3).IsTypeOf<AnotherView>();
    }

    private sealed class TestViewModel : ReactiveObject
    {
    }

    private sealed class AlternateViewModel : ReactiveObject
    {
    }

    private sealed class AnotherViewModel : ReactiveObject
    {
    }

    private sealed class TestView : IViewFor<TestViewModel>
    {
        public TestViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    private sealed class AlternateView : IViewFor<AlternateViewModel>
    {
        public AlternateViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (AlternateViewModel?)value;
        }
    }

    private sealed class AnotherView : IViewFor<AnotherViewModel>
    {
        public AnotherViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (AnotherViewModel?)value;
        }
    }

    private sealed class UnregisteredView : IViewFor<TestViewModel>
    {
        public TestViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }
}
