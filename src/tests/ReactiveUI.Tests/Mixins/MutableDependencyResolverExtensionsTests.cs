// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Tests.Utilities.AppBuilder;
using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Mixins;

/// <summary>Tests for MutableDependencyResolverExtensions.</summary>
[NotInParallel]
[TestExecutor<AppBuilderTestExecutor>]
public class MutableDependencyResolverExtensionsTests
{
    /// <summary>The contract name used when registering views in the tests.</summary>
    private const string TestContract = "TestContract";

    /// <summary>Verifies that RegisterSingletonViewForViewModel registers a singleton view.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterSingletonViewForViewModelRegistersSingleton()
    {
        var resolver = new ModernDependencyResolver();
        resolver.RegisterSingletonViewForViewModel<TestView, TestViewModel>();

        var view = resolver.GetService<IViewFor<TestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view).IsNotNull();
            await Assert.That(view).IsOfType(typeof(TestView));
        }
    }

    /// <summary>Verifies that RegisterSingletonViewForViewModel returns the resolver for chaining.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterSingletonViewForViewModelReturnsResolver()
    {
        var resolver = new ModernDependencyResolver();
        var result = resolver.RegisterSingletonViewForViewModel<TestView, TestViewModel>();

        await Assert.That(result).IsEqualTo(resolver);
    }

    /// <summary>Verifies that RegisterSingletonViewForViewModel resolves the same instance each time.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterSingletonViewForViewModelReturnsSameInstance()
    {
        var resolver = new ModernDependencyResolver();
        resolver.RegisterSingletonViewForViewModel<TestView, TestViewModel>();

        var view1 = resolver.GetService<IViewFor<TestViewModel>>();
        var view2 = resolver.GetService<IViewFor<TestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view1).IsNotNull();
            await Assert.That(view2).IsNotNull();
            await Assert.That(ReferenceEquals(view1, view2)).IsTrue();
        }
    }

    /// <summary>Verifies that RegisterSingletonViewForViewModel supports fluent chaining of registrations.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterSingletonViewForViewModelSupportsChaining()
    {
        var resolver = new ModernDependencyResolver();
        resolver.RegisterSingletonViewForViewModel<TestView, TestViewModel>().RegisterSingletonViewForViewModel<AlternateTestView, AlternateTestViewModel>();

        var view1 = resolver.GetService<IViewFor<TestViewModel>>();
        var view2 = resolver.GetService<IViewFor<AlternateTestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view1).IsNotNull();
            await Assert.That(view2).IsNotNull();
        }
    }

    /// <summary>Verifies that RegisterSingletonViewForViewModel throws when the resolver is null.</summary>
    [Test]
    public void RegisterSingletonViewForViewModelThrowsOnNullResolver()
    {
        IMutableDependencyResolver? resolver = null;
        Assert.Throws<ArgumentNullException>(() => resolver!.RegisterSingletonViewForViewModel<TestView, TestViewModel>());
    }

    /// <summary>Verifies that RegisterSingletonViewForViewModel registers a singleton view with a contract.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterSingletonViewForViewModelWithContractRegistersSingleton()
    {
        var resolver = new ModernDependencyResolver();
        resolver.RegisterSingletonViewForViewModel<TestView, TestViewModel>(TestContract);

        var view = resolver.GetService<IViewFor<TestViewModel>>(TestContract);

        using (Assert.Multiple())
        {
            await Assert.That(view).IsNotNull();
            await Assert.That(view).IsOfType(typeof(TestView));
        }
    }

    /// <summary>Verifies that RegisterViewForViewModel resolves a new instance each time.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterViewForViewModelCreatesNewInstanceEachTime()
    {
        var resolver = new ModernDependencyResolver();
        resolver.RegisterViewForViewModel<TestView, TestViewModel>();

        var view1 = resolver.GetService<IViewFor<TestViewModel>>();
        var view2 = resolver.GetService<IViewFor<TestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view1).IsNotNull();
            await Assert.That(view2).IsNotNull();
            await Assert.That(ReferenceEquals(view1, view2)).IsFalse();
        }
    }

    /// <summary>Verifies that RegisterViewForViewModel registers a view.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterViewForViewModelRegistersView()
    {
        var resolver = new ModernDependencyResolver();
        resolver.RegisterViewForViewModel<TestView, TestViewModel>();

        var view = resolver.GetService<IViewFor<TestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view).IsNotNull();
            await Assert.That(view).IsOfType(typeof(TestView));
        }
    }

    /// <summary>Verifies that RegisterViewForViewModel returns the resolver for chaining.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterViewForViewModelReturnsResolver()
    {
        var resolver = new ModernDependencyResolver();
        var result = resolver.RegisterViewForViewModel<TestView, TestViewModel>();

        await Assert.That(result).IsEqualTo(resolver);
    }

    /// <summary>Verifies that RegisterViewForViewModel supports fluent chaining of registrations.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterViewForViewModelSupportsChaining()
    {
        var resolver = new ModernDependencyResolver();
        resolver.RegisterViewForViewModel<TestView, TestViewModel>().RegisterViewForViewModel<AlternateTestView, AlternateTestViewModel>();

        var view1 = resolver.GetService<IViewFor<TestViewModel>>();
        var view2 = resolver.GetService<IViewFor<AlternateTestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view1).IsNotNull();
            await Assert.That(view2).IsNotNull();
        }
    }

    /// <summary>Verifies that RegisterViewForViewModel throws when the resolver is null.</summary>
    [Test]
    public void RegisterViewForViewModelThrowsOnNullResolver()
    {
        IMutableDependencyResolver? resolver = null;
        Assert.Throws<ArgumentNullException>(() => resolver!.RegisterViewForViewModel<TestView, TestViewModel>());
    }

    /// <summary>Verifies that RegisterViewForViewModel registers a view with a contract.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterViewForViewModelWithContractRegistersView()
    {
        var resolver = new ModernDependencyResolver();
        resolver.RegisterViewForViewModel<TestView, TestViewModel>(TestContract);

        var view = resolver.GetService<IViewFor<TestViewModel>>(TestContract);

        using (Assert.Multiple())
        {
            await Assert.That(view).IsNotNull();
            await Assert.That(view).IsOfType(typeof(TestView));
        }
    }

    /// <summary>Alternate test view.</summary>
    private sealed class AlternateTestView : IViewFor<AlternateTestViewModel>
    {
        /// <summary>Gets or sets the strongly typed view model.</summary>
        public AlternateTestViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as AlternateTestViewModel;
        }
    }

    /// <summary>Alternate test view model.</summary>
    [SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Empty type used as a test marker.")]
    private sealed class AlternateTestViewModel : ReactiveObject;

    /// <summary>Test view.</summary>
    private sealed class TestView : IViewFor<TestViewModel>
    {
        /// <summary>Gets or sets the strongly typed view model.</summary>
        public TestViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as TestViewModel;
        }
    }

    /// <summary>Test view model.</summary>
    [SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Empty type used as a test marker.")]
    private sealed class TestViewModel : ReactiveObject;
}
