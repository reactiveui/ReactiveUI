// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using ReactiveUI.Tests.Utilities.AppBuilder;
using Splat.Builder;

namespace ReactiveUI.Tests.Locator;

/// <summary>
///     Comprehensive test suite for <see cref="ViewLocator" /> static class.
///     Tests cover the static <see cref="ViewLocator.Current" /> property and initialization behavior.
///     Uses <see cref="AppBuilderTestExecutor" /> to ensure proper AppLocator isolation between tests.
/// </summary>
[NotInParallel]
[TestExecutor<AppBuilderTestExecutor>]
public class ViewLocatorTests
{
    /// <summary>
    ///     Verifies that <see cref="ViewLocator.Current" /> can be configured using
    ///     the builder's ConfigureViewLocator method to map and resolve views.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Current_CanBeConfigured_EndToEnd()
    {
        // Reset and configure the ViewLocator
        RxAppBuilder.ResetForTesting();
        AppBuilder.ResetBuilderStateForTests();

        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        builder.ConfigureViewLocator(locator => { locator.Map<TestViewModel, TestView>(() => new TestView()); });
        builder.WithCoreServices().BuildApp();

        var current = ViewLocator.Current;
        var view = current.ResolveView<TestViewModel>();

        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<TestView>();
    }

    /// <summary>
    ///     Verifies that <see cref="ViewLocator.Current" /> can resolve views
    ///     using the default view locator registration mechanism.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Current_CanResolveViews_ViaServiceLocator()
    {
        var resolver = AppLocator.Current as IDependencyResolver;
        ArgumentNullException.ThrowIfNull(resolver);

        // Register a view in the service locator
        resolver.Register(() => new TestView(), typeof(IViewFor<TestViewModel>));

        var locator = ViewLocator.Current;
        var view = locator.ResolveView<TestViewModel>();

        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<TestView>();
    }

    /// <summary>
    ///     Verifies that <see cref="ViewLocator.Current" /> returns a default <see cref="IViewLocator" />
    ///     when ReactiveUI is initialized with core services.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Current_ReturnsDefaultViewLocator()
    {
        // AppBuilderTestExecutor initializes ReactiveUI with core services
        var current = ViewLocator.Current;

        await Assert.That(current).IsNotNull();
        await Assert.That(current).IsTypeOf<DefaultViewLocator>();
    }

    /// <summary>
    ///     Verifies that <see cref="ViewLocator.Current" /> returns a new instance
    ///     after ReactiveUI is reset and re-initialized.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Current_ReturnsNewInstance_AfterReinitialization()
    {
        var current1 = ViewLocator.Current;

        // Simulate re-initialization
        RxAppBuilder.ResetForTesting();
        AppBuilder.ResetBuilderStateForTests();
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();

        var current2 = ViewLocator.Current;

        await Assert.That(ReferenceEquals(current1, current2)).IsFalse();
        await Assert.That(current2).IsNotNull();
        await Assert.That(current2).IsTypeOf<DefaultViewLocator>();
    }

    /// <summary>
    ///     Verifies that <see cref="ViewLocator.Current" /> returns the same instance
    ///     when called multiple times (singleton behavior).
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Current_ReturnsSameInstance_WhenCalledMultipleTimes()
    {
        var current1 = ViewLocator.Current;
        var current2 = ViewLocator.Current;

        await Assert.That(ReferenceEquals(current1, current2)).IsTrue();
    }

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
    ///     Test view model used for testing view locator functionality.
    /// </summary>
    private sealed class TestViewModel : ReactiveObject
    {
    }
}
