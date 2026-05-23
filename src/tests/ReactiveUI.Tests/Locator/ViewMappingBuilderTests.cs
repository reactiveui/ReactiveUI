// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Tests.Utilities.AppBuilder;
using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Locator;

/// <summary>
///     Tests for the ViewMappingBuilder which provides a fluent interface for registering
///     AOT-compatible view-to-viewmodel mappings.
/// </summary>
[NotInParallel]
[TestExecutor<AppBuilderTestExecutor>]
public class ViewMappingBuilderTests
{
    private const string Contract1 = "contract1";
    private const string Contract2 = "contract2";

    /// <summary>
    ///     Verifies that mapping via the parameterless constructor overload registers a view that can be resolved.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
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

    /// <summary>
    ///     Verifies that the parameterless constructor mapping overload supports contract-specific registrations.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Map_WithParameterlessConstructor_ShouldSupportContracts()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);

        // Act
        builder
            .Map<TestViewModel, TestView>(Contract1)
            .Map<AlternateViewModel, AlternateView>(Contract2);

        var view1 = locator.ResolveView<TestViewModel>(Contract1);
        var view2 = locator.ResolveView<AlternateViewModel>(Contract2);

        // Assert
        await Assert.That(view1).IsTypeOf<TestView>();
        await Assert.That(view2).IsTypeOf<AlternateView>();
    }

    /// <summary>
    ///     Verifies that the parameterless constructor mapping overload returns the builder for fluent chaining.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
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

    /// <summary>
    ///     Verifies that multiple parameterless constructor mappings can be chained and resolved.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
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

    /// <summary>
    ///     Verifies that the factory mapping overload throws when the supplied factory is null.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
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

    /// <summary>
    ///     Verifies that the factory mapping overload registers a view that can be resolved.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
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

    /// <summary>
    ///     Verifies that the factory mapping overload supports contract-specific registrations.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
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
            .Map<TestViewModel, TestView>(() => view1Instance, Contract1)
            .Map<TestViewModel, TestView>(() => view2Instance, Contract2);

        var view1 = locator.ResolveView<TestViewModel>(Contract1);
        var view2 = locator.ResolveView<TestViewModel>(Contract2);

        // Assert
        await Assert.That(view1).IsSameReferenceAs(view1Instance);
        await Assert.That(view2).IsSameReferenceAs(view2Instance);
    }

    /// <summary>
    ///     Verifies that the factory mapping overload invokes the factory on every resolve, producing new instances.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
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
            return new();
        });

        var view1 = locator.ResolveView<TestViewModel>();
        var view2 = locator.ResolveView<TestViewModel>();

        // Assert
        const int ExpectedCallCount = 2;
        await Assert.That(callCount).IsEqualTo(ExpectedCallCount);
        await Assert.That(view1).IsNotSameReferenceAs(view2);
    }

    /// <summary>
    ///     Verifies that the factory mapping overload returns the builder for fluent chaining.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Map_WithFactory_ShouldReturnBuilder()
    {
        // Arrange
        var locator = new DefaultViewLocator();
        var builder = new ViewMappingBuilder(locator);

        // Act
        var result = builder.Map<TestViewModel, TestView>(() => new());

        // Assert
        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    ///     Verifies that <c>MapFromServiceLocator</c> resolves the view from the service locator.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
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

    /// <summary>
    ///     Verifies that <c>MapFromServiceLocator</c> supports contract-specific registrations.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
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
            .MapFromServiceLocator<TestViewModel, TestView>(Contract1)
            .MapFromServiceLocator<TestViewModel, TestView>(Contract2);

        // Note: Contract is used for view locator registration
        var view1 = locator.ResolveView<TestViewModel>(Contract1);
        var view2 = locator.ResolveView<TestViewModel>(Contract2);

        // Assert
        await Assert.That(view1).IsNotNull();
        await Assert.That(view2).IsNotNull();
    }

    /// <summary>
    ///     Verifies that <c>MapFromServiceLocator</c> throws when the view is not registered in the service locator.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
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

    /// <summary>
    ///     Verifies that <c>MapFromServiceLocator</c> returns the builder for fluent chaining.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
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

    /// <summary>
    ///     Verifies that multiple <c>MapFromServiceLocator</c> calls can be chained and resolved.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
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

    /// <summary>
    ///     Verifies that the builder allows mixing parameterless, factory, and service locator registration types.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
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

    /// <summary>
    ///     Test view model used for testing view mapping builder functionality.
    /// </summary>
    [SuppressMessage(
        "Minor Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Empty type used as a test marker.")]
    private sealed class TestViewModel : ReactiveObject;

    /// <summary>
    ///     Alternate test view model used for testing multi-mapping scenarios.
    /// </summary>
    [SuppressMessage(
        "Minor Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Empty type used as a test marker.")]
    private sealed class AlternateViewModel : ReactiveObject;

    /// <summary>
    ///     Third test view model used for testing mixed registration scenarios.
    /// </summary>
    [SuppressMessage(
        "Minor Code Smell",
        "S2094:Classes should not be empty",
        Justification = "Empty type used as a test marker.")]
    private sealed class AnotherViewModel : ReactiveObject;

    /// <summary>
    ///     Test view implementing <see cref="IViewFor{TViewModel}" /> for <see cref="TestViewModel" />.
    /// </summary>
    private sealed class TestView : IViewFor<TestViewModel>
    {
        /// <summary>
        ///     Gets or sets the strongly-typed view model.
        /// </summary>
        public TestViewModel? ViewModel { get; set; }

        /// <summary>
        ///     Gets or sets the view model. Implements <see cref="IViewFor.ViewModel" />.
        /// </summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    /// <summary>
    ///     Alternate test view implementing <see cref="IViewFor{TViewModel}" /> for <see cref="AlternateViewModel" />.
    /// </summary>
    private sealed class AlternateView : IViewFor<AlternateViewModel>
    {
        /// <summary>
        ///     Gets or sets the strongly-typed view model.
        /// </summary>
        public AlternateViewModel? ViewModel { get; set; }

        /// <summary>
        ///     Gets or sets the view model. Implements <see cref="IViewFor.ViewModel" />.
        /// </summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (AlternateViewModel?)value;
        }
    }

    /// <summary>
    ///     Third test view implementing <see cref="IViewFor{TViewModel}" /> for <see cref="AnotherViewModel" />.
    /// </summary>
    private sealed class AnotherView : IViewFor<AnotherViewModel>
    {
        /// <summary>
        ///     Gets or sets the strongly-typed view model.
        /// </summary>
        public AnotherViewModel? ViewModel { get; set; }

        /// <summary>
        ///     Gets or sets the view model. Implements <see cref="IViewFor.ViewModel" />.
        /// </summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (AnotherViewModel?)value;
        }
    }

    /// <summary>
    ///     Test view that is intentionally not registered, used to verify failure when resolving unregistered views.
    /// </summary>
    private sealed class UnregisteredView : IViewFor<TestViewModel>
    {
        /// <summary>
        ///     Gets or sets the strongly-typed view model.
        /// </summary>
        public TestViewModel? ViewModel { get; set; }

        /// <summary>
        ///     Gets or sets the view model. Implements <see cref="IViewFor.ViewModel" />.
        /// </summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }
}
