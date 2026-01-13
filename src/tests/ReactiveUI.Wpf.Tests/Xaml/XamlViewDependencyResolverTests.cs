// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Mocks;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests associated with UI and the <see cref="IDependencyResolver"/>.
/// </summary>
[TestExecutor<WpfViewResolverTestExecutor>]
public sealed class XamlViewDependencyResolverTests
{
    /// <summary>
    /// Test that register views for view model should register all views.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RegisterViewsForViewModelShouldRegisterAllViews()
    {
        var resolver = AppLocator.Current as IDependencyResolver;
        await Assert.That(resolver).IsNotNull();

        using (resolver.WithResolver())
        using (Assert.Multiple())
        {
            await Assert.That(resolver.GetServices<IViewFor<ExampleViewModel>>()).Count().IsEqualTo(1);
            await Assert.That(resolver.GetServices<IViewFor<AnotherViewModel>>()).Count().IsEqualTo(1);
            await Assert.That(resolver.GetServices<IViewFor<ExampleWindowViewModel>>()).Count().IsEqualTo(1);
            await Assert.That(resolver.GetServices<IViewFor<ViewModelWithWeirdName>>()).Count().IsEqualTo(1);
        }
    }

    /// <summary>
    /// Test that register views for view model should include contracts.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RegisterViewsForViewModelShouldIncludeContracts()
    {
        var resolver = AppLocator.Current as IDependencyResolver;
        await Assert.That(resolver).IsNotNull();
        using (resolver.WithResolver())
        {
            await Assert.That(resolver.GetServices(typeof(IViewFor<ExampleViewModel>), "contract")).Count().IsEqualTo(1);
        }
    }
}
