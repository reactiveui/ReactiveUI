// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Splat;

namespace ReactiveUI.Tests.Mixins;

/// <summary>Tests for the <see cref="MutableDependencyResolverAOTExtensions" /> class. These tests verify the AOT-friendly registration helpers.</summary>
public class MutableDependencyResolverAotExtensionsTests
{
    /// <summary>The contract name used to verify singleton registration scoped to a contract.</summary>
    private const string SingletonContractName = "SingletonContract";

    /// <summary>Verifies that RegisterSingletonViewForViewModelAOT registers a singleton view.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterSingletonViewForViewModelAOT_RegistersSingletonView()
    {
        using var resolver = new ModernDependencyResolver();

        _ = resolver.RegisterSingletonViewForViewModelAOT<TestView, TestViewModel>();

        var view1 = resolver.GetService<IViewFor<TestViewModel>>();
        var view2 = resolver.GetService<IViewFor<TestViewModel>>();

        using (Assert.Multiple())
        {
            await Assert.That(view1).IsNotNull();
            await Assert.That(view1).IsTypeOf<TestView>();
            await Assert.That(view1).IsSameReferenceAs(view2);
        }
    }

    /// <summary>Verifies that RegisterSingletonViewForViewModelAOT returns the resolver for chaining.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterSingletonViewForViewModelAOT_ReturnsResolverForChaining()
    {
        using var resolver = new ModernDependencyResolver();

        var result = resolver.RegisterSingletonViewForViewModelAOT<TestView, TestViewModel>();

        await Assert.That(result).IsSameReferenceAs(resolver);
    }

    /// <summary>Verifies that RegisterSingletonViewForViewModelAOT throws ArgumentNullException when resolver is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterSingletonViewForViewModelAOT_ThrowsArgumentNullException_WhenResolverIsNull()
    {
        IMutableDependencyResolver? resolver = null;

        var exception = await Assert
            .That(() => resolver!.RegisterSingletonViewForViewModelAOT<TestView, TestViewModel>())
            .Throws<ArgumentNullException>();
        await Assert.That(exception).IsNotNull();
    }

    /// <summary>Verifies that RegisterSingletonViewForViewModelAOT registers singleton view with contract.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterSingletonViewForViewModelAOT_WithContract_RegistersViewWithContract()
    {
        using var resolver = new ModernDependencyResolver();

        _ = resolver.RegisterSingletonViewForViewModelAOT<TestView, TestViewModel>(SingletonContractName);

        var view1 = resolver.GetService<IViewFor<TestViewModel>>(SingletonContractName);
        var view2 = resolver.GetService<IViewFor<TestViewModel>>(SingletonContractName);

        using (Assert.Multiple())
        {
            await Assert.That(view1).IsNotNull();
            await Assert.That(view1).IsTypeOf<TestView>();
            await Assert.That(view1).IsSameReferenceAs(view2);
        }
    }

    /// <summary>Verifies that RegisterViewForViewModelAOT registers a transient view.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterViewForViewModelAOT_RegistersTransientView()
    {
        using var resolver = new ModernDependencyResolver();

        _ = resolver.RegisterViewForViewModelAOT<TestView, TestViewModel>();

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

    /// <summary>Verifies that RegisterViewForViewModelAOT returns the resolver for chaining.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterViewForViewModelAOT_ReturnsResolverForChaining()
    {
        using var resolver = new ModernDependencyResolver();

        var result = resolver.RegisterViewForViewModelAOT<TestView, TestViewModel>();

        await Assert.That(result).IsSameReferenceAs(resolver);
    }

    /// <summary>Verifies that RegisterViewForViewModelAOT throws ArgumentNullException when resolver is null.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterViewForViewModelAOT_ThrowsArgumentNullException_WhenResolverIsNull()
    {
        IMutableDependencyResolver? resolver = null;

        var exception = await Assert.That(() => resolver!.RegisterViewForViewModelAOT<TestView, TestViewModel>())
            .Throws<ArgumentNullException>();
        await Assert.That(exception).IsNotNull();
    }

    /// <summary>Verifies that RegisterViewForViewModelAOT registers view with contract.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task RegisterViewForViewModelAOT_WithContract_RegistersViewWithContract()
    {
        using var resolver = new ModernDependencyResolver();

        _ = resolver.RegisterViewForViewModelAOT<TestView, TestViewModel>("MyContract");

        var view = resolver.GetService<IViewFor<TestViewModel>>("MyContract");

        using (Assert.Multiple())
        {
            await Assert.That(view).IsNotNull();
            await Assert.That(view).IsTypeOf<TestView>();
        }
    }

    /// <summary>A test view used to verify AOT view registration.</summary>
    private sealed class TestView : IViewFor<TestViewModel>
    {
        /// <summary>Gets or sets the strongly typed view model.</summary>
        public TestViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    /// <summary>A test view model used to verify AOT view registration.</summary>
    [SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Empty type used as a test marker.")]
    private sealed class TestViewModel : ReactiveObject;
}
