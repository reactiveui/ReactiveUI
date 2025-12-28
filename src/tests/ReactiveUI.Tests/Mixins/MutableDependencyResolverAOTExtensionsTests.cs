// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mixins;

/// <summary>
/// Tests for the <see cref="MutableDependencyResolverAOTExtensions"/> class.
/// These tests verify the AOT-friendly registration helpers.
/// </summary>
public class MutableDependencyResolverAOTExtensionsTests
{
    /// <summary>
    /// Verifies that RegisterViewForViewModelAOT registers a transient view.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterViewForViewModelAOT_RegistersTransientView()
    {
        using var resolver = new ModernDependencyResolver();

        MutableDependencyResolverAOTExtensions.RegisterViewForViewModelAOT<TestView, TestViewModel>(resolver);

        var view1 = resolver.GetService<IViewFor<TestViewModel>>();
        var view2 = resolver.GetService<IViewFor<TestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view1).IsNotNull();
            await Assert.That(view1).IsTypeOf<TestView>();
            await Assert.That(view2).IsNotNull();
            await Assert.That(view1).IsNotSameReferenceAs(view2);
        }
    }

    /// <summary>
    /// Verifies that RegisterViewForViewModelAOT registers view with contract.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterViewForViewModelAOT_WithContract_RegistersViewWithContract()
    {
        using var resolver = new ModernDependencyResolver();

        MutableDependencyResolverAOTExtensions.RegisterViewForViewModelAOT<TestView, TestViewModel>(resolver, "MyContract");

        var view = resolver.GetService<IViewFor<TestViewModel>>("MyContract");

        using (Assert.Multiple())
        {
            await Assert.That(view).IsNotNull();
            await Assert.That(view).IsTypeOf<TestView>();
        }
    }

    /// <summary>
    /// Verifies that RegisterViewForViewModelAOT throws ArgumentNullException when resolver is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterViewForViewModelAOT_ThrowsArgumentNullException_WhenResolverIsNull()
    {
        IMutableDependencyResolver? resolver = null;

        var exception = await Assert.That(() => MutableDependencyResolverAOTExtensions.RegisterViewForViewModelAOT<TestView, TestViewModel>(resolver!))
            .Throws<ArgumentNullException>();
        await Assert.That(exception).IsNotNull();
    }

    /// <summary>
    /// Verifies that RegisterSingletonViewForViewModelAOT registers a singleton view.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterSingletonViewForViewModelAOT_RegistersSingletonView()
    {
        using var resolver = new ModernDependencyResolver();

        MutableDependencyResolverAOTExtensions.RegisterSingletonViewForViewModelAOT<TestView, TestViewModel>(resolver);

        var view1 = resolver.GetService<IViewFor<TestViewModel>>();
        var view2 = resolver.GetService<IViewFor<TestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view1).IsNotNull();
            await Assert.That(view1).IsTypeOf<TestView>();
            await Assert.That(view1).IsSameReferenceAs(view2);
        }
    }

    /// <summary>
    /// Verifies that RegisterSingletonViewForViewModelAOT registers singleton view with contract.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterSingletonViewForViewModelAOT_WithContract_RegistersViewWithContract()
    {
        using var resolver = new ModernDependencyResolver();

        MutableDependencyResolverAOTExtensions.RegisterSingletonViewForViewModelAOT<TestView, TestViewModel>(resolver, "SingletonContract");

        var view1 = resolver.GetService<IViewFor<TestViewModel>>("SingletonContract");
        var view2 = resolver.GetService<IViewFor<TestViewModel>>("SingletonContract");

        using (Assert.Multiple())
        {
            await Assert.That(view1).IsNotNull();
            await Assert.That(view1).IsTypeOf<TestView>();
            await Assert.That(view1).IsSameReferenceAs(view2);
        }
    }

    /// <summary>
    /// Verifies that RegisterSingletonViewForViewModelAOT throws ArgumentNullException when resolver is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterSingletonViewForViewModelAOT_ThrowsArgumentNullException_WhenResolverIsNull()
    {
        IMutableDependencyResolver? resolver = null;

        var exception = await Assert.That(() => MutableDependencyResolverAOTExtensions.RegisterSingletonViewForViewModelAOT<TestView, TestViewModel>(resolver!))
            .Throws<ArgumentNullException>();
        await Assert.That(exception).IsNotNull();
    }

    /// <summary>
    /// Verifies that RegisterViewForViewModelAOT returns the resolver for chaining.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterViewForViewModelAOT_ReturnsResolverForChaining()
    {
        using var resolver = new ModernDependencyResolver();

        var result = MutableDependencyResolverAOTExtensions.RegisterViewForViewModelAOT<TestView, TestViewModel>(resolver);

        await Assert.That(result).IsSameReferenceAs(resolver);
    }

    /// <summary>
    /// Verifies that RegisterSingletonViewForViewModelAOT returns the resolver for chaining.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterSingletonViewForViewModelAOT_ReturnsResolverForChaining()
    {
        using var resolver = new ModernDependencyResolver();

        var result = MutableDependencyResolverAOTExtensions.RegisterSingletonViewForViewModelAOT<TestView, TestViewModel>(resolver);

        await Assert.That(result).IsSameReferenceAs(resolver);
    }

    private class TestViewModel : ReactiveObject
    {
    }

    private class TestView : IViewFor<TestViewModel>
    {
        public TestViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }
}
