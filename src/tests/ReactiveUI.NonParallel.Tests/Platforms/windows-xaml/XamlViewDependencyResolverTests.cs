// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#else
#endif

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests associated with UI and the <see cref="IDependencyResolver"/>.
/// </summary>
public sealed class XamlViewDependencyResolverTests : IDisposable
{
    private readonly IDependencyResolver _resolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="XamlViewDependencyResolverTests"/> class.
    /// </summary>
    public XamlViewDependencyResolverTests()
    {
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();
        resolver.RegisterViewsForViewModels(GetType().Assembly);

        _resolver = resolver;
    }

    /// <summary>
    /// Test that register views for view model should register all views.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task RegisterViewsForViewModelShouldRegisterAllViews()
    {
        using (_resolver.WithResolver())
        using (Assert.Multiple())
        {
            await Assert.That(_resolver.GetServices<IViewFor<ExampleViewModel>>()).Count().IsEqualTo(1);
            await Assert.That(_resolver.GetServices<IViewFor<AnotherViewModel>>()).Count().IsEqualTo(1);
            await Assert.That(_resolver.GetServices<IViewFor<ExampleWindowViewModel>>()).Count().IsEqualTo(1);
            await Assert.That(_resolver.GetServices<IViewFor<ViewModelWithWeirdName>>()).Count().IsEqualTo(1);
        }
    }

    /// <summary>
    /// Test that register views for view model should include contracts.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task RegisterViewsForViewModelShouldIncludeContracts()
    {
        using (_resolver.WithResolver())
        {
            await Assert.That(_resolver.GetServices(typeof(IViewFor<ExampleViewModel>), "contract")).Count().IsEqualTo(1);
        }
    }

    /// <inheritdoc/>
    public void Dispose() => _resolver.Dispose();
}
