// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#else
using System.Threading;
#endif

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests associated with UI and the <see cref="IDependencyResolver"/>.
/// </summary>
[TestFixture]
public sealed class XamlViewDependencyResolverTests : IDisposable
{
    private readonly IDependencyResolver _resolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="XamlViewDependencyResolverTests"/> class.
    /// </summary>
    public XamlViewDependencyResolverTests()
    {
        _resolver = new ModernDependencyResolver();
        _resolver.InitializeSplat();
        _resolver.InitializeReactiveUI();
        _resolver.RegisterViewsForViewModels(GetType().Assembly);
    }

    /// <summary>
    /// Test that register views for view model should register all views.
    /// </summary>
    [Test, Apartment(ApartmentState.STA)]
    public void RegisterViewsForViewModelShouldRegisterAllViews()
    {
        using (_resolver.WithResolver())
        {
            Assert.That(_resolver.GetServices<IViewFor<ExampleViewModel>>(, Has.Exactly(1).Items));
            Assert.That(_resolver.GetServices<IViewFor<AnotherViewModel>>(, Has.Exactly(1).Items));
            Assert.That(_resolver.GetServices<IViewFor<ExampleWindowViewModel>>(, Has.Exactly(1).Items));
            Assert.That(_resolver.GetServices<IViewFor<ViewModelWithWeirdName>>(, Has.Exactly(1).Items));
        }
    }

    /// <summary>
    /// Test that register views for view model should include contracts.
    /// </summary>
    [Test, Apartment(ApartmentState.STA)]
    public void RegisterViewsForViewModelShouldIncludeContracts()
    {
        using (_resolver.WithResolver())
        {
            Assert.That(_resolver.GetServices(typeof(IViewFor<ExampleViewModel>, Has.Exactly(1).Items), "contract"));
        }
    }

    /// <inheritdoc/>
    public void Dispose() => _resolver?.Dispose();
}
